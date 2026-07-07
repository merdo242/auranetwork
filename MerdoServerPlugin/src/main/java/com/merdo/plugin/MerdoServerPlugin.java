package com.merdo.plugin;

import fr.xephi.authme.api.v3.AuthMeApi;
import fr.xephi.authme.events.LoginEvent;
import fr.xephi.authme.events.RegisterEvent;
import org.bukkit.Bukkit;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.EventPriority;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerCommandPreprocessEvent;
import org.bukkit.event.player.PlayerQuitEvent;
import org.bukkit.plugin.java.JavaPlugin;
import org.bukkit.configuration.file.FileConfiguration;
import org.bukkit.configuration.file.YamlConfiguration;
import java.io.File;

import net.luckperms.api.LuckPerms;
import net.luckperms.api.LuckPermsProvider;
import net.luckperms.api.model.user.User;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.util.HashSet;
import java.util.Set;
import java.util.UUID;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    private final Set<UUID> merdoClients = new HashSet<>();
    private AuthMeApi authMeApi;
    private HttpServer httpServer;

    @Override
    public void onEnable() {
        saveDefaultConfig();
        this.getServer().getPluginManager().registerEvents(this, this);
        this.authMeApi = AuthMeApi.getInstance();
        setupEconomy();
        getLogger().info("MerdoServerPlugin aktif edildi!");
        
        startHttpServer();
    }

    private net.milkbowl.vault.economy.Economy econ = null;
    private boolean setupEconomy() {
        if (getServer().getPluginManager().getPlugin("Vault") == null) {
            return false;
        }
        org.bukkit.plugin.RegisteredServiceProvider<net.milkbowl.vault.economy.Economy> rsp = getServer().getServicesManager().getRegistration(net.milkbowl.vault.economy.Economy.class);
        if (rsp == null) {
            return false;
        }
        econ = rsp.getProvider();
        return econ != null;
    }

    @Override
    public void onDisable() {
        stopHttpServer();
        getLogger().info("MerdoServerPlugin devre disi!");
    }

    private void startHttpServer() {
        try {
            int port = getConfig().getInt("http-port", 8880);
            httpServer = HttpServer.create(new InetSocketAddress(port), 0);
            httpServer.createContext("/check", new HttpHandler() {
                @Override
                public void handle(HttpExchange exchange) throws IOException {
                    String query = exchange.getRequestURI().getQuery();
                    String username = null;
                    if (query != null) {
                        for (String param : query.split("&")) {
                            String[] pair = param.split("=");
                            if (pair.length > 1 && pair[0].equalsIgnoreCase("username")) {
                                username = pair[1];
                                break;
                            }
                        }
                    }

                    boolean registered = false;
                    String role = "OYUNCU";
                    if (username != null && !username.trim().isEmpty()) {
                        String cleanUser = username.trim();
                        registered = authMeApi.isRegistered(cleanUser);
                        if (registered) {
                            try {
                                LuckPerms api = LuckPermsProvider.get();
                                java.util.UUID uuid = api.getUserManager().lookupUniqueId(cleanUser).join();
                                if (uuid != null) {
                                    User user = api.getUserManager().loadUser(uuid).join();
                                    if (user != null) {
                                        String prefix = user.getCachedData().getMetaData().getPrefix();
                                        
                                        // If prefix is null (e.g. offline user or inherited), try from their primary group
                                        if (prefix == null || prefix.trim().isEmpty()) {
                                            net.luckperms.api.model.group.Group group = api.getGroupManager().getGroup(user.getPrimaryGroup());
                                            if (group != null) {
                                                prefix = group.getCachedData().getMetaData().getPrefix();
                                                if (prefix == null || prefix.trim().isEmpty()) {
                                                    prefix = group.getDisplayName(); // fallback to display name if prefix doesn't exist
                                                }
                                            }
                                        }

                                        if (prefix != null && !prefix.trim().isEmpty()) {
                                            role = prefix.replaceAll("(?i)[&§][0-9a-fk-or]", "")
                                                         .replaceAll("(?i)[&§]#[0-9a-f]{6}", "")
                                                         .replaceAll("(?i)<#[0-9a-f]{6}>", "")
                                                         .replaceAll("[\\[\\]]", "")
                                                         .trim();
                                        } else {
                                            role = user.getPrimaryGroup();
                                        }
                                        if (role.equalsIgnoreCase("default")) role = "OYUNCU";
                                    }
                                }
                            } catch (Exception ignored) { }
                        }
                    }

                    double balance = 0.0;
                    if (registered && econ != null && username != null) {
                        org.bukkit.OfflinePlayer offlinePlayer = Bukkit.getOfflinePlayer(username.trim());
                        if (offlinePlayer != null) {
                            balance = econ.getBalance(offlinePlayer);
                        }
                    }

                    String response = "{\"registered\":" + registered + ", \"role\":\"" + role.toUpperCase() + "\", \"balance\":" + balance + "}";
                    exchange.getResponseHeaders().set("Content-Type", "application/json; charset=UTF-8");
                    byte[] responseBytes = response.getBytes("UTF-8");
                    exchange.sendResponseHeaders(200, responseBytes.length);
                    OutputStream os = exchange.getResponseBody();
                    os.write(responseBytes);
                    os.close();
                }
            });
            httpServer.setExecutor(null);
            httpServer.start();
            getLogger().info("HTTP Sunucusu port " + port + " uzerinde baslatildi.");
        } catch (Exception e) {
            getLogger().severe("HTTP Sunucusu baslatilamadi: " + e.getMessage());
        }
    }

    private void stopHttpServer() {
        if (httpServer != null) {
            try {
                httpServer.stop(0);
                getLogger().info("HTTP Sunucusu durduruldu.");
            } catch (Exception e) {
                getLogger().severe("HTTP Sunucusu durdurulurken hata: " + e.getMessage());
            }
        }
    }

    @EventHandler(priority = EventPriority.LOWEST)
    public void onCommand(PlayerCommandPreprocessEvent event) {
        if (event.getMessage().startsWith("/merdoauth ")) {
            event.setCancelled(true);
            String password = event.getMessage().substring(11).trim();
            Player player = event.getPlayer();
            
            merdoClients.add(player.getUniqueId());

            if (!authMeApi.isAuthenticated(player)) {
                if (authMeApi.isRegistered(player.getName())) {
                    if (authMeApi.checkPassword(player.getName(), password)) {
                        authMeApi.forceLogin(player);
                        handleMerdoLogin(player);
                    } else {
                        player.kickPlayer("§cAuraNW Client şifreniz sunucudaki şifrenizle eşleşmiyor!");
                    }
                } else {
                    player.performCommand("register " + password + " " + password);
                }
            } else {
                handleMerdoLogin(player);
            }
        }
    }

    @EventHandler
    public void onLogin(LoginEvent event) {
        if (merdoClients.contains(event.getPlayer().getUniqueId())) {
            handleMerdoLogin(event.getPlayer());
        }
    }

    @EventHandler
    public void onRegister(RegisterEvent event) {
        if (merdoClients.contains(event.getPlayer().getUniqueId())) {
            handleMerdoLogin(event.getPlayer());
        }
    }

    @EventHandler
    public void onQuit(PlayerQuitEvent event) {
        merdoClients.remove(event.getPlayer().getUniqueId());
    }

    private void handleMerdoLogin(Player player) {
        if (!merdoClients.contains(player.getUniqueId())) return;
        merdoClients.remove(player.getUniqueId());

        File dataFile = new File(getDataFolder(), "data.yml");
        FileConfiguration data = YamlConfiguration.loadConfiguration(dataFile);
        String uuidStr = player.getUniqueId().toString();

        if (!data.getBoolean("rewards." + uuidStr, false)) {
            Bukkit.dispatchCommand(Bukkit.getConsoleSender(), "lp user " + player.getName() + " parent addtemp vip 30d");
            data.set("rewards." + uuidStr, true);
            try {
                data.save(dataFile);
            } catch (IOException e) {
                getLogger().severe("Could not save data.yml");
            }
        }

        Bukkit.broadcastMessage("§e§l✨ §b" + player.getName() + " §eAura network client ile katıldı!");
    }
}
