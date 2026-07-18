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
        logger.info("AuraVelocityPlugin aktif! Launcher uzerinden kayitsiz oyunculari engelleme (Firebase) devrede.");
    }

    @Subscribe
    public void onLogin(LoginEvent event) {
        String username = event.getPlayer().getUsername();

        // Oyuncunun veritabanında (Firebase) kayıtlı olup olmadığını kontrol et
        if (!isRegistered(username)) {
            // Kayıtlı değil — bağlantıyı kes ve açıklama yap
            Component kickMessage = Component.text()
                .append(Component.text("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n", NamedTextColor.YELLOW))
                .append(Component.text("  ☆ AURA NETWORK ☆\n\n", NamedTextColor.GOLD, TextDecoration.BOLD))
                .append(Component.text("  Bu sunucuya giriş yapabilmek için\n", NamedTextColor.WHITE))
                .append(Component.text("  önce kayıt olmanız gerekmektedir!\n\n", NamedTextColor.WHITE))
                .append(Component.text("  ● Kayıt Ol:\n", NamedTextColor.YELLOW, TextDecoration.BOLD))
                .append(Component.text("  Lütfen sunucumuza ", NamedTextColor.GRAY))
                .append(Component.text("AuraNW Client\n", NamedTextColor.GREEN, TextDecoration.BOLD))
                .append(Component.text("  uygulaması üzerinden kayıt olup giriş yapın.\n\n", NamedTextColor.GRAY))
                .append(Component.text("  Kaçak istemciler (TLauncher vb.) desteklenmez.\n", NamedTextColor.RED))
                .append(Component.text("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━", NamedTextColor.YELLOW))
                .build();

            event.setResult(LoginEvent.ComponentResult.denied(kickMessage));
            logger.info("[Engellendi] " + username + " Launcher'a kayitsiz oldugu icin baglanamadi.");
        }
    }

    private boolean isRegistered(String username) {
        try {
            // Firebase uzerinden kullanicinin kayitli olup olmadigini kontrol et
            java.net.URL url = new java.net.URL("https://auranw-c3bf4-default-rtdb.firebaseio.com/accounts/" + username + ".json?shallow=true");
            java.net.HttpURLConnection conn = (java.net.HttpURLConnection) url.openConnection();
            conn.setRequestMethod("GET");
            conn.setConnectTimeout(3000);
            conn.setReadTimeout(3000);

            if (conn.getResponseCode() == 200) {
                java.io.BufferedReader in = new java.io.BufferedReader(new java.io.InputStreamReader(conn.getInputStream()));
                String response = in.readLine();
                in.close();
                // Eger veri "null" donuyorsa kullanici yok demektir
                if (response != null && !response.trim().equals("null")) {
                    return true;
                }
            }
        } catch (Exception e) {
            // Firebase'e baglanilamazsa (hata vb.) sunucuyu kilitlememek icin gecir
            logger.warning("[AuraVelocity] Firebase baglanti hatasi: " + e.getMessage());
            return true;
        }
        return false;
    }
}
