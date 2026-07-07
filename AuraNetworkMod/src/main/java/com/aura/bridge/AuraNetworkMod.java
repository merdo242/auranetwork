package com.aura.bridge;

import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.fabric.api.client.networking.v1.ClientPlayConnectionEvents;
import net.minecraft.text.Text;

public class AuraNetworkMod implements ClientModInitializer {
    @Override
    public void onInitializeClient() {
        System.out.println("[AuraNetworkMod] Aura Network Mod yuklendi!"); AuraClientSettings.load();

        ClientPlayConnectionEvents.JOIN.register((handler, sender, client) -> {
            String token = System.getenv("AURA_TOKEN");
            if (token != null && !token.trim().isEmpty()) {
                new Thread(() -> {
                    try {
                        Thread.sleep(2500);
                        client.execute(() -> {
                            if (client.player != null) {
                                client.player.sendMessage(Text.literal("§a[AuraNetwork] §7Sunucuya dogrulama anahtari gonderildi!"), false);
                            }
                            if (client.getNetworkHandler() != null) {
                                client.getNetworkHandler().sendCommand("merdoauth " + token.trim());
                            }
                        });
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }).start();
            } else {
                new Thread(() -> {
                    try {
                        Thread.sleep(2500);
                        client.execute(() -> {
                            if (client.player != null) {
                                client.player.sendMessage(Text.literal("§c[AuraNetwork] §7Launcher'dan sifre (token) girmediginiz icin dogrulama yapilamadi!"), false);
                            }
                        });
                    } catch (Exception e) {}
                }).start();
            }
        });
    }
}