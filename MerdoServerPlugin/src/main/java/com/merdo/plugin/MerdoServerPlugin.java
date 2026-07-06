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
import java.util.*;
import java.util.concurrent.ConcurrentLinkedQueue;

import net.luckperms.api.LuckPerms;
import net.luckperms.api.LuckPermsProvider;
import net.luckperms.api.model.user.User;

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;
import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.URLDecoder;
import java.nio.charset.StandardCharsets;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    private final Set<UUID> merdoClients = new HashSet<>();
    private AuthMeApi authMeApi;
    private HttpServer httpServer;

    // Chat sistemi icin
    private final Queue<ChatMessage> chatMessages = new ConcurrentLinkedQueue<>();
    private long lastMessageId = 0;
    private static final int MAX_CHAT_MESSAGES = 200;

    public static class ChatMessage {
        public final long id;
        public final String username;
        public final String role;
        public final String message;
        public final long timestamp;

        public ChatMessage(long id, String username, String role, String message) {
            this.id = id;
            this.username = username;
            this.role = role;
            this.message = message;
            this.timestamp = System.currentTimeMillis() / 1000;
        }
    }

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
            
            // /check endpoint (mevcut)
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
                                        
                                        if (prefix == null || prefix.trim().isEmpty()) {
                                            net.luckperms.api.model.group.Group group = api.getGroupManager().getGroup(user.getPrimaryGroup());
                                            if (group != null) {
                                                prefix = group.getCachedData().getMetaData().getPrefix();
                                                if (prefix == null || prefix.trim().isEmpty()) {
                                                    prefix = group.getDisplayName();
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

            // /chat/send endpoint - mesaj gonderme
            httpServer.createContext("/chat/send", new HttpHandler() {
                @Override
                public void handle(HttpExchange exchange) throws IOException {
                    try {
                        String query = exchange.getRequestURI().getQuery();
                        String username = null;
                        String password = null;
                        String message = null;
                        
                        if (query != null) {
                            for (String param : query.split("&")) {
                                String[] pair = param.split("=");
                                if (pair.length > 1) {
                                    String key = URLDecoder.decode(pair[0], "UTF-8");
                                    String val = URLDecoder.decode(pair[1], "UTF-8");
                                    if (key.equalsIgnoreCase("username")) username = val;
                                    else if (key.equalsIgnoreCase("password")) password = val;
                                    else if (key.equalsIgnoreCase("message")) message = val;
                                }
                            }
                        }

                        String response;
                        if (username == null || password == null || message == null || message.trim().isEmpty()) {
                            response = "{\"success\":false,\"error\":\"Eksik parametreler\"}";
                        } else if (!authMeApi.isRegistered(username)) {
                            response = "{\"success\":false,\"error\":\"Kullanici kayitli degil\"}";
                        } else if (!authMeApi.checkPassword(username, password)) {
                            response = "{\"success\":false,\"error\":\"Sifre yanlis\"}";
                        } else {
                            // Rolu al
                            String role = "OYUNCU";
                            try {
                                LuckPerms api = LuckPermsProvider.get();
                                java.util.UUID uuid = api.getUserManager().lookupUniqueId(username).join();
                                if (uuid != null) {
                                    User user = api.getUserManager().loadUser(uuid).join();
                                    if (user != null) {
                                        String prefix = user.getCachedData().getMetaData().getPrefix();
                                        if (prefix == null || prefix.trim().isEmpty()) {
                                            net.luckperms.api.model.group.Group group = api.getGroupManager().getGroup(user.getPrimaryGroup());
                                            if (group != null) {
                                                prefix = group.getCachedData().getMetaData().getPrefix();
                                                if (prefix == null || prefix.trim().isEmpty()) {
                                                    prefix = group.getDisplayName();
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

                            // Mesaji kuyruga ekle
                            long msgId;
                            synchronized (MerdoServerPlugin.this) {
                                lastMessageId++;
                                msgId = lastMessageId;
                            }
                            chatMessages.add(new ChatMessage(msgId, username, role.toUpperCase(), message.trim()));
                            
                            // Fazla mesajlari temizle
                            while (chatMessages.size() > MAX_CHAT_MESSAGES) {
                                chatMessages.poll();
                            }

                            // Oyun ici sohbette de goster
                            Bukkit.broadcastMessage("§7[§6AuraChat§7] §" + getRoleColorCode(role) + username + "§7: " + message);

                            response = "{\"success\":true,\"id\":" + msgId + "}";
                        }

                        exchange.getResponseHeaders().set("Content-Type", "application/json; charset=UTF-8");
                        byte[] responseBytes = response.getBytes("UTF-8");
                        exchange.sendResponseHeaders(200, responseBytes.length);
                        OutputStream os = exchange.getResponseBody();
                        os.write(responseBytes);
                        os.close();
                    } catch (Exception e) {
                        String error = "{\"success\":false,\"error\":\"" + e.getMessage() + "\"}";
                        exchange.getResponseHeaders().set("Content-Type", "application/json; charset=UTF-8");
                        byte[] responseBytes = error.getBytes("UTF-8");
                        exchange.sendResponseHeaders(200, responseBytes.length);
                        OutputStream os = exchange.getResponseBody();
                        os.write(responseBytes);
                        os.close();
                    }
                }
            });

            // /chat/messages endpoint - mesajlari cekme (polling)
            httpServer.createContext("/chat/messages", new HttpHandler() {
                @Override
                public void handle(HttpExchange exchange) throws IOException {
                    String query = exchange.getRequestURI().getQuery();
                    long sinceId = 0;
                    if (query != null) {
                        for (String param : query.split("&")) {
                            String[] pair = param.split("=");
                            if (pair.length > 1 && pair[0].equalsIgnoreCase("since")) {
                                try { sinceId = Long.parseLong(pair[1]); } catch (Exception ignored) { }
                            }
                        }
                    }

                    StringBuilder json = new StringBuilder("{\"messages\":[");
                    boolean first = true;
                    for (ChatMessage msg : chatMessages) {
                        if (msg.id > sinceId) {
                            if (!first) json.append(",");
                            json.append("{\"id\":").append(msg.id)
                                .append(",\"username\":\"").append(escapeJson(msg.username))
                                .append("\",\"role\":\"").append(escapeJson(msg.role))
                                .append("\",\"message\":\"").append(escapeJson(msg.message))
                                .append("\",\"timestamp\":").append(msg.timestamp)
                                .append("}");
                            first = false;
                        }
                    }
                    json.append("]}");

                    exchange.getResponseHeaders().set("Content-Type", "application/json; charset=UTF-8");
                    exchange.getResponseHeaders().set("Access-Control-Allow-Origin", "*");
                    byte[] responseBytes = json.toString().getBytes("UTF-8");
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

    private String getRoleColorCode(String role) {
        String r = role.toLowerCase();
        if (r.contains("kurucu") || r.contains("founder")) return "6";
        if (r.contains("admin") || r.contains("owner")) return "c";
        if (r.contains("mod")) return "5";
        if (r.contains("vip")) return "b";
        return "f";
    }

    private String escapeJson(String s) {
        return s.replace("\\", "\\\\")
                .replace("\"", "\\\"")
                .replace("\n", "\\n")
                .replace("\r", "\\r")
                .replace("\t", "\\t");
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

        Bukkit.broadcastMessage("§e§l✨ §b" + player.getName() + " §eAuraNW Client ayrıcalığıyla sunucuya katıldı!");
    }
}