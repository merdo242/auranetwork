package com.merdo.plugin;

import com.google.gson.JsonObject;
import com.google.gson.JsonParser;
import org.bukkit.Bukkit;
import org.bukkit.ChatColor;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.EventPriority;
import org.bukkit.event.Listener;
import org.bukkit.event.block.BlockBreakEvent;
import org.bukkit.event.block.BlockPlaceEvent;
import org.bukkit.event.entity.EntityDamageEvent;
import org.bukkit.event.player.*;
import org.bukkit.plugin.java.JavaPlugin;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    private final Set<UUID> unauthenticatedPlayers = new HashSet<>();
    private final HttpClient httpClient = HttpClient.newBuilder()
            .version(HttpClient.Version.HTTP_2)
            .connectTimeout(Duration.ofSeconds(10))
            .build();
    
    private final String FIREBASE_URL = "https://auranw-c3bf4-default-rtdb.firebaseio.com/accounts/";

    @Override
    public void onEnable() {
        getServer().getPluginManager().registerEvents(this, this);
        getLogger().info("MerdoServerPlugin enabled! (Firebase Auth Edition)");
        getLogger().info("All other auth plugins should be REMOVED to prevent conflicts.");
    }

    @Override
    public void onDisable() {
        getLogger().info("MerdoServerPlugin disabled!");
    }

    private void sendMessage(Player player, String message) {
        player.sendMessage(ChatColor.translateAlternateColorCodes('&', message));
    }

    @Override
    public boolean onCommand(CommandSender sender, Command command, String label, String[] args) {
        if (!(sender instanceof Player)) {
            sender.sendMessage("Only players can use this command.");
            return true;
        }

        Player player = (Player) sender;
        String cmd = command.getName().toLowerCase();

        if (cmd.equals("aurajoin")) {
            Bukkit.broadcastMessage("§e§l✨ §b" + player.getName() + " §eAura network client ile katıldı!");
            return true;
        }

        if (cmd.equals("login")) {
            if (!unauthenticatedPlayers.contains(player.getUniqueId())) {
                sendMessage(player, "&cZaten giriş yapmışsınız!");
                return true;
            }
            if (args.length != 1) {
                sendMessage(player, "&cKullanım: /login <şifre>");
                return true;
            }
            
            String password = args[0];
            String username = player.getName().toLowerCase();
            
            sendMessage(player, "&eGiriş yapılıyor, lütfen bekleyin...");
            
            CompletableFuture.runAsync(() -> {
                try {
                    HttpRequest request = HttpRequest.newBuilder()
                            .uri(URI.create(FIREBASE_URL + username + ".json"))
                            .GET()
                            .build();

                    HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
                    if (response.statusCode() == 200 && response.body() != null && !response.body().equals("null")) {
                        JsonObject json = JsonParser.parseString(response.body()).getAsJsonObject();
                        if (json.has("password") && json.get("password").getAsString().equals(password)) {
                            Bukkit.getScheduler().runTask(this, () -> {
                                unauthenticatedPlayers.remove(player.getUniqueId());
                                sendMessage(player, "&aBaşarıyla giriş yaptınız! İyi oyunlar.");
                            });
                        } else {
                            Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cŞifre yanlış!"));
                        }
                    } else {
                        Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cKayıtlı bir hesap bulunamadı. Lütfen /register <şifre> <şifre> komutunu veya web sitesini kullanarak kayıt olun."));
                    }
                } catch (Exception e) {
                    Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cVeritabanına bağlanılamadı. Lütfen yöneticiye bildirin."));
                    getLogger().severe("Firebase Login Error: " + e.getMessage());
                }
            });
            return true;
        }

        if (cmd.equals("register")) {
            if (!unauthenticatedPlayers.contains(player.getUniqueId())) {
                sendMessage(player, "&cZaten giriş yapmışsınız!");
                return true;
            }
            if (args.length != 2) {
                sendMessage(player, "&cKullanım: /register <şifre> <şifre_tekrar>");
                return true;
            }
            if (!args[0].equals(args[1])) {
                sendMessage(player, "&cŞifreler uyuşmuyor!");
                return true;
            }

            String password = args[0];
            String username = player.getName().toLowerCase();

            sendMessage(player, "&eKayıt işlemi başlatılıyor, lütfen bekleyin...");

            CompletableFuture.runAsync(() -> {
                try {
                    // Check if already registered
                    HttpRequest checkRequest = HttpRequest.newBuilder()
                            .uri(URI.create(FIREBASE_URL + username + ".json?shallow=true"))
                            .GET()
                            .build();
                    HttpResponse<String> checkResponse = httpClient.send(checkRequest, HttpResponse.BodyHandlers.ofString());
                    
                    if (checkResponse.statusCode() == 200 && checkResponse.body() != null && !checkResponse.body().equals("null")) {
                        Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cBu kullanıcı adı zaten kayıtlı! Eğer sizinse /login <şifre> yazın."));
                        return;
                    }

                    // Register new account
                    JsonObject newAccount = new JsonObject();
                    newAccount.addProperty("password", password);
                    String jsonBody = newAccount.toString();

                    HttpRequest putRequest = HttpRequest.newBuilder()
                            .uri(URI.create(FIREBASE_URL + username + ".json"))
                            .PUT(HttpRequest.BodyPublishers.ofString(jsonBody))
                            .header("Content-Type", "application/json")
                            .build();

                    HttpResponse<String> putResponse = httpClient.send(putRequest, HttpResponse.BodyHandlers.ofString());
                    if (putResponse.statusCode() == 200) {
                        Bukkit.getScheduler().runTask(this, () -> {
                            unauthenticatedPlayers.remove(player.getUniqueId());
                            sendMessage(player, "&aBaşarıyla kayıt oldunuz ve giriş yapıldı! İyi oyunlar.");
                        });
                    } else {
                        Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cKayıt başarısız oldu. Sunucu hatası."));
                    }
                } catch (Exception e) {
                    Bukkit.getScheduler().runTask(this, () -> sendMessage(player, "&cVeritabanına bağlanılamadı. Lütfen yöneticiye bildirin."));
                    getLogger().severe("Firebase Register Error: " + e.getMessage());
                }
            });
            return true;
        }

        return false;
    }

    @EventHandler(priority = EventPriority.HIGHEST)
    public void onPlayerJoin(PlayerJoinEvent event) {
        Player player = event.getPlayer();
        unauthenticatedPlayers.add(player.getUniqueId());
        
        // Let them know they need to login/register
        Bukkit.getScheduler().runTaskLater(this, () -> {
            if (unauthenticatedPlayers.contains(player.getUniqueId())) {
                sendMessage(player, "&b========================================");
                sendMessage(player, "&eAura Network'e Hoşgeldiniz!");
                sendMessage(player, "&eGiriş yapmak için: &a/login <şifre>");
                sendMessage(player, "&eKayıt olmak için: &a/register <şifre> <şifre_tekrar>");
                sendMessage(player, "&b========================================");
            }
        }, 20L); // Delay 1 second
    }

    @EventHandler
    public void onPlayerQuit(PlayerQuitEvent event) {
        unauthenticatedPlayers.remove(event.getPlayer().getUniqueId());
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onPlayerMove(PlayerMoveEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            if (event.getFrom().getX() != event.getTo().getX() || event.getFrom().getZ() != event.getTo().getZ()) {
                event.setTo(event.getFrom());
            }
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onPlayerChat(AsyncPlayerChatEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            event.setCancelled(true);
            sendMessage(event.getPlayer(), "&cLütfen önce giriş yapın veya kayıt olun.");
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onPlayerCommand(PlayerCommandPreprocessEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            String cmd = event.getMessage().toLowerCase();
            if (!cmd.startsWith("/login") && !cmd.startsWith("/register")) {
                event.setCancelled(true);
                sendMessage(event.getPlayer(), "&cLütfen önce giriş yapın veya kayıt olun.");
            }
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onEntityDamage(EntityDamageEvent event) {
        if (event.getEntity() instanceof Player) {
            Player player = (Player) event.getEntity();
            if (unauthenticatedPlayers.contains(player.getUniqueId())) {
                event.setCancelled(true);
            }
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onBlockBreak(BlockBreakEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            event.setCancelled(true);
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onBlockPlace(BlockPlaceEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            event.setCancelled(true);
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onPlayerInteract(PlayerInteractEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            event.setCancelled(true);
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onPlayerDropItem(PlayerDropItemEvent event) {
        if (unauthenticatedPlayers.contains(event.getPlayer().getUniqueId())) {
            event.setCancelled(true);
        }
    }
}