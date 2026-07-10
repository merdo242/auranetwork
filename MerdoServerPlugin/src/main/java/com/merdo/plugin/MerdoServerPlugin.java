package com.merdo.plugin;

import org.bukkit.Bukkit;
import org.bukkit.command.Command;
import org.bukkit.command.CommandSender;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.EventPriority;
import org.bukkit.event.Listener;
import org.bukkit.event.player.PlayerJoinEvent;
import org.bukkit.plugin.java.JavaPlugin;

public class MerdoServerPlugin extends JavaPlugin implements Listener {

    @Override
    public void onEnable() {
        getServer().getPluginManager().registerEvents(this, this);
        getLogger().info("MerdoServerPlugin enabled! (LimboAuth Edition - Command Blocker)");
        getLogger().info("VIP synchronizer is DISABLED. Command Blocker is active.");
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
        // PlayerJoinEvent su anlik baska bir islem yapmiyor
        // Ileride giris islemleri eklemek icin kullanilabilir
    }
}
