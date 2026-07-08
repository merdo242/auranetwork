package com.aura.bridge;

import net.fabricmc.api.ClientModInitializer;
import net.fabricmc.fabric.api.client.networking.v1.ClientPlayConnectionEvents;
import net.minecraft.text.Text;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class AuraNetworkMod implements ClientModInitializer {
    public static final Logger LOGGER = LoggerFactory.getLogger("auranetworkmod");

    @Override
    public void onInitializeClient() {
        System.out.println("[AuraNetworkMod] Aura Network Mod yuklendi!");
        AuraClientSettings.load();

        ClientPlayConnectionEvents.JOIN.register((handler, sender, client) -> {
            String token = System.getenv("AURA_TOKEN");
            if (token != null && !token.trim().isEmpty()) {
                new Thread(() -> {
                    try {
                        Thread.sleep(2500);
                        client.execute(() -> {
                            if (client.getNetworkHandler() != null) {
                                // LimboAuth ve standart auth pluginleri icin sirayla kayit ve giris komutlarini yolla
                                client.getNetworkHandler().sendChatCommand("register " + token.trim() + " " + token.trim());
                                
                                // Biraz bekleyip login komutunu gonder (register'in islenmesi icin kucuk bir gecikme)
                                new Thread(() -> {
                                    try {
                                        Thread.sleep(300);
                                        client.execute(() -> {
                                            if (client.getNetworkHandler() != null) {
                                                client.getNetworkHandler().sendChatCommand("login " + token.trim());
                                            }
                                        });
                                        
                                        // Ana sunucuya gectikten sonra giris mesajini attirmak icin aurajoin komutu gonderilir
                                        Thread.sleep(2500);
                                        client.execute(() -> {
                                            if (client.getNetworkHandler() != null) {
                                                client.getNetworkHandler().sendChatCommand("aurajoin");
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
                System.out.println("[AuraNetworkMod] Token bulunamadi, dogrulama atlandi.");
            }
        });
    }
}
