package com.aura.bridge;

import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.fabric.api.client.networking.v1.ClientPlayConnectionEvents;

public class AuraNetworkMod implements ClientModInitializer {
    @Override
    public void onInitializeClient() {
        System.out.println("[AuraNetworkMod] Aura Network Mod yuklendi!"); AuraClientSettings.load();

        ClientPlayConnectionEvents.JOIN.register((handler, sender, client) -> {
            String token = System.getenv("AURA_TOKEN");
            if (token != null && !token.trim().isEmpty()) {
                System.out.println("[AuraNetworkMod] Token bulundu, sunucuya aktariliyor...");
                new Thread(() -> {
                    try {
                        Thread.sleep(2500); // 2.5 saniye bekle
                        client.execute(() -> {
                            if (client.getNetworkHandler() != null) {
                                client.getNetworkHandler().sendCommand("merdoauth " + token.trim());
                            }
                        });
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }).start();
            } else {
                System.out.println("[AuraNetworkMod] Token bulunamadi, normal giris yapiliyor.");
            }
        });
    }
}
