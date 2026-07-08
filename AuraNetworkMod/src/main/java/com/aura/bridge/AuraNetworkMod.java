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
                                // client.player.sendMessage(Text.literal("§a[AuraNetwork] §7Sunucuya dogrulama anahtari gonderildi!"), false);
                            }
                            if (client.getNetworkHandler() != null) {
                                // LimboAuth ve standart auth pluginleri icin sirayla kayit ve giris komutlarini yolla
                                client.getNetworkHandler().sendCommand("register " + token.trim() + " " + token.trim());
                                
                                // Biraz bekleyip login komutunu gonder (register'in islenmesi icin kucuk bir gecikme)
                                new Thread(() -> {
                                    try {
                                        Thread.sleep(300);
                                        client.execute(() -> {
                                            if (client.getNetworkHandler() != null) {
                                                client.getNetworkHandler().sendCommand("login " + token.trim());
                                            }
                                        });
                                        
                                        // Ana sunucuya gectikten sonra giris mesajini attirmak icin aurajoin komutu gonderilir
                                        Thread.sleep(2500);
                                        client.execute(() -> {
                                            if (client.getNetworkHandler() != null) {
                                                client.getNetworkHandler().sendCommand("aurajoin");
                                            }
                                        });
                                    } catch (Exception ignored) {}
                                }).start();
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