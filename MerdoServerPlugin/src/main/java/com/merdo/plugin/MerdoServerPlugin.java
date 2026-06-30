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
        getLogger().info("MerdoServerPlugin aktif edildi!");
        
        startHttpServer();
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
                    if (username != null && !username.trim().isEmpty()) {
                        registered = authMeApi.isRegistered(username.trim());
                    }

                    String response = "{\"registered\":" + registered + "}";
                    exchange.getResponseHeaders().set("Content-Type", "application/json");
                    exchange.sendResponseHeaders(200, response.getBytes().length);
                    OutputStream os = exchange.getResponseBody();
                    os.write(response.getBytes());
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
                        player.kickPlayer("§cMerdo Client şifreniz sunucudaki şifrenizle eşleşmiyor!");
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

        Bukkit.dispatchCommand(Bukkit.getConsoleSender(), "lp user " + player.getName() + " parent addtemp vip 30d");
        Bukkit.broadcastMessage("§e§l✨ §b" + player.getName() + " §eMerdo Client ayrıcalığıyla sunucuya katıldı!");
    }
}
