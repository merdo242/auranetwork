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

import java.util.HashSet;
import java.util.Set;
import java.util.UUID;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    private final Set<UUID> merdoClients = new HashSet<>();
    private AuthMeApi authMeApi;

    @Override
    public void onEnable() {
        this.getServer().getPluginManager().registerEvents(this, this);
        this.authMeApi = AuthMeApi.getInstance();
        getLogger().info("MerdoServerPlugin aktif edildi!");
    }

    @Override
    public void onDisable() {
        getLogger().info("MerdoServerPlugin devre disi!");
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
