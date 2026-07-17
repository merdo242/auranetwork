package com.merdo.plugin;

import com.google.gson.JsonObject;
import org.bukkit.BanEntry;
import org.bukkit.BanList;
import org.bukkit.Bukkit;
import org.bukkit.plugin.java.JavaPlugin;
import org.bukkit.scheduler.BukkitRunnable;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;

public class MerdoServerPlugin extends JavaPlugin {

    private final HttpClient httpClient = HttpClient.newBuilder()
            .version(HttpClient.Version.HTTP_2)
            .connectTimeout(Duration.ofSeconds(10))
            .build();

    private final String FIREBASE_BANS_URL = "https://auranw-c3bf4-default-rtdb.firebaseio.com/bans.json";
    private int lastBanCount = -1;

    @Override
    public void onEnable() {
        getLogger().info("MerdoServerPlugin enabled! (Ban Synchronizer Edition)");

        // Her 10 saniyede bir ban listesini kontrol et ve Firebase'e guncelle
        new BukkitRunnable() {
            @Override
            public void run() {
                syncBansToFirebase();
            }
        }.runTaskTimerAsynchronously(this, 100L, 200L); // 5 saniye sonra basla, her 10 saniyede tekrarla
    }

    @Override
    public void onDisable() {
        getLogger().info("MerdoServerPlugin disabled!");
    }

    private void syncBansToFirebase() {
        try {
            int currentBanCount = Bukkit.getBanList(BanList.Type.NAME).getBanEntries().size();
            
            // Eger ban sayisi degismediyse bosa Firebase'i yorma (performans optimizasyonu)
            if (currentBanCount == lastBanCount) {
                return;
            }

            JsonObject bansJson = new JsonObject();
            for (BanEntry entry : Bukkit.getBanList(BanList.Type.NAME).getBanEntries()) {
                String target = entry.getTarget();
                if (target != null && !target.isEmpty()) {
                    bansJson.addProperty(target.toLowerCase(), true);
                }
            }

            String jsonBody = bansJson.toString();

            HttpRequest putRequest = HttpRequest.newBuilder()
                    .uri(URI.create(FIREBASE_BANS_URL))
                    .PUT(HttpRequest.BodyPublishers.ofString(jsonBody))
                    .header("Content-Type", "application/json")
                    .build();

            HttpResponse<String> response = httpClient.send(putRequest, HttpResponse.BodyHandlers.ofString());
            if (response.statusCode() == 200) {
                lastBanCount = currentBanCount;
            }
        } catch (Exception e) {
            getLogger().warning("Ban listesi senkronize edilirken hata olustu: " + e.getMessage());
        }
    }
}