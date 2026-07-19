package com.aura.velocity;

import com.google.inject.Inject;
import com.velocitypowered.api.event.Subscribe;
import com.velocitypowered.api.event.connection.LoginEvent;
import com.velocitypowered.api.event.proxy.ProxyInitializeEvent;
import com.velocitypowered.api.plugin.Plugin;
import com.velocitypowered.api.proxy.ProxyServer;
import net.kyori.adventure.text.Component;
import net.kyori.adventure.text.format.NamedTextColor;
import net.kyori.adventure.text.format.TextDecoration;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.logging.Logger;

@Plugin(id = "auravelocity", name = "AuraVelocityPlugin", version = "1.1", authors = {"AuraNetwork"})
public class AuraVelocityPlugin {

    // === VERİTABANI AYARLARI ===
    // Bu bilgileri kendi MySQL sunucunuza göre güncelleyin!
    private static final String DB_HOST     = "127.0.0.1";
    private static final String DB_PORT     = "3306";
    private static final String DB_NAME     = "limboauth";
    private static final String DB_USER     = "root";
    private static final String DB_PASS     = "";
    private static final String DB_TABLE    = "limboauth";
    // ===========================

    private final ProxyServer server;
    private final Logger logger;

    @Inject
    public AuraVelocityPlugin(ProxyServer server, Logger logger) {
        this.server = server;
        this.logger = logger;
    }

    @Subscribe
    public void onProxyInitialization(ProxyInitializeEvent event) {
        logger.info("AuraVelocityPlugin aktif! Kayıt kontrol sistemi devre dışı bırakıldı (serbest giriş).");
    }
}
