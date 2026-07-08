package com.aura.velocity;

import com.google.inject.Inject;
import com.velocitypowered.api.command.SimpleCommand;
import com.velocitypowered.api.event.Subscribe;
import com.velocitypowered.api.event.proxy.ProxyInitializeEvent;
import com.velocitypowered.api.plugin.Plugin;
import com.velocitypowered.api.proxy.ProxyServer;
import com.velocitypowered.api.proxy.Player;
import net.kyori.adventure.text.Component;

@Plugin(id = "auravelocity", name = "AuraVelocityPlugin", version = "1.0", authors = {"AuraNetwork"})
public class AuraVelocityPlugin {

    private final ProxyServer server;

    @Inject
    public AuraVelocityPlugin(ProxyServer server) {
        this.server = server;
    }

    @Subscribe
    public void onProxyInitialization(ProxyInitializeEvent event) {
        server.getCommandManager().register("aurajoin", new SimpleCommand() {
            @Override
            public void execute(Invocation invocation) {
                if (invocation.source() instanceof Player) {
                    Player player = (Player) invocation.source();
                    server.sendMessage(Component.text("§e§l✨ §b" + player.getUsername() + " §eAura network client ile katıldı!"));
                }
            }
        });
    }
}
