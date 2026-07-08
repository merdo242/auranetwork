package com.merdo.plugin;

import net.luckperms.api.LuckPerms;
import net.luckperms.api.LuckPermsProvider;
import net.luckperms.api.model.user.User;
import net.luckperms.api.node.Node;
import org.bukkit.Bukkit;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.EventPriority;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerJoinEvent;
import org.bukkit.plugin.java.JavaPlugin;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    @Override
    public void onEnable() {
        getServer().getPluginManager().registerEvents(this, this);
        getLogger().info("MerdoServerPlugin enabled! (LimboAuth Edition)");
        getLogger().info("VIP synchronizer and Command Blocker is active.");
    }

    @Override
    public void onDisable() {
        getLogger().info("MerdoServerPlugin disabled!");
    }

    @Override
    public boolean onCommand(CommandSender sender, Command command, String label, String[] args) {
        if (command.getName().equalsIgnoreCase("aurajoin")) {
            if (sender instanceof Player) {
                Bukkit.broadcastMessage("§e§l✨ §b" + sender.getName() + " §eAura network client ile katıldı!");
            }
            return true;
        }
        if (command.getName().equalsIgnoreCase("login") || command.getName().equalsIgnoreCase("register")) {
            // Sessizce yut
            return true;
        }
        return false;
    }

    @EventHandler(priority = EventPriority.LOW)
    public void onPlayerJoin(PlayerJoinEvent event) {
        Player player = event.getPlayer();
        Bukkit.getScheduler().runTaskLaterAsynchronously(this, () -> {
            if (player.isOnline()) {
                handleMerdoLogin(player);
            }
        }, 40L); // 2 seconds delay
    }

    private void handleMerdoLogin(Player player) {
        String username = player.getName();
        String apiToken = "2b4b4566f103b44b82d477fcbb2123512e20b3327d6d371d34f07a4a2b217031"; 
        
        try {
            URL url = new URL("https://auranw.com/api/get_user_info.php?username=" + username + "&token=" + apiToken);
            HttpURLConnection conn = (HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            conn.setConnectTimeout(5000);
            conn.setReadTimeout(5000);

            int responseCode = conn.getResponseCode();
            if (responseCode == 200) {
                BufferedReader in = new BufferedReader(new InputStreamReader(conn.getInputStream()));
                String inputLine;
                StringBuilder response = new StringBuilder();

                while ((inputLine = in.readLine()) != null) {
                    response.append(inputLine);
                }
                in.close();

                String json = response.toString();
                boolean success = false;
                if (json.contains("\"success\":true") || json.contains("\"success\": true")) {
                    success = true;
                }
                
                if (success) {
                    String role = extractJsonValue(json, "role");
                    String expiryStr = extractJsonValue(json, "expiry");
                    
                    Bukkit.getScheduler().runTask(this, () -> {
                        updatePlayerRole(player, role, expiryStr);
                    });
                } else {
                    getLogger().warning("API returned failure for " + username + ": " + json);
                }
            } else {
                getLogger().warning("Failed to connect to web API for " + username + ". Response code: " + responseCode);
            }
        } catch (Exception e) {
            getLogger().warning("Error checking VIP status for " + username + ": " + e.getMessage());
        }
    }

    private String extractJsonValue(String json, String key) {
        String searchKey = "\"" + key + "\":\"";
        int startIndex = json.indexOf(searchKey);
        if (startIndex == -1) {
            searchKey = "\"" + key + "\":";
            startIndex = json.indexOf(searchKey);
            if (startIndex == -1) return null;
            startIndex += searchKey.length();
            int endIndex = json.indexOf(",", startIndex);
            if (endIndex == -1) endIndex = json.indexOf("}", startIndex);
            if (endIndex == -1) return null;
            return json.substring(startIndex, endIndex).trim();
        }
        startIndex += searchKey.length();
        int endIndex = json.indexOf("\"", startIndex);
        if (endIndex == -1) return null;
        return json.substring(startIndex, endIndex);
    }

    private void updatePlayerRole(Player player, String roleName, String expiryStr) {
        if (roleName == null || roleName.isEmpty() || roleName.equals("null")) {
            return;
        }

        long expiryUnix = 0;
        if (expiryStr != null && !expiryStr.isEmpty() && !expiryStr.equals("null")) {
            try {
                expiryUnix = Long.parseLong(expiryStr);
            } catch (NumberFormatException e) {
                getLogger().warning("Invalid expiry time format for " + player.getName() + ": " + expiryStr);
            }
        }

        try {
            LuckPerms luckPerms = LuckPermsProvider.get();
            User user = luckPerms.getUserManager().getUser(player.getUniqueId());
            if (user != null) {
                String[] allVipRoles = {"mvip+", "mvip", "vip+", "vip"};
                for (String vipRole : allVipRoles) {
                    user.data().remove(Node.builder("group." + vipRole).build());
                }

                Node roleNode;
                if (expiryUnix > 0) {
                    roleNode = Node.builder("group." + roleName).expiry(expiryUnix).build();
                } else {
                    roleNode = Node.builder("group." + roleName).build();
                }
                user.data().add(roleNode);
                luckPerms.getUserManager().saveUser(user);
                getLogger().info("Successfully applied role " + roleName + " to " + player.getName());
            }
        } catch (Exception e) {
            getLogger().warning("Error applying LuckPerms role to " + player.getName() + ": " + e.getMessage());
        }
    }
}
