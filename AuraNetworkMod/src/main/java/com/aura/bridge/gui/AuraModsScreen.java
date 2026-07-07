package com.aura.bridge.gui;

import net.fabricmc.loader.api.FabricLoader;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

import java.io.File;

public class AuraModsScreen extends Screen {
    private final Screen parent;
    public boolean requiresRestart = false;

    public AuraModsScreen(Screen parent) {
        super(Text.literal("Mod Yoneticisi"));
        this.parent = parent;
    }

    @Override
    protected void init() {
        int centerX = this.width / 2;
        int startY = 40;

        File modsDir = FabricLoader.getInstance().getGameDir().resolve("mods").toFile();
        if (modsDir.exists() && modsDir.isDirectory()) {
            File[] files = modsDir.listFiles((dir, name) -> name.endsWith(".jar") || name.endsWith(".disabled"));
            if (files != null) {
                int yOffset = 0;
                for (File file : files) {
                    String fileName = file.getName();
                    // AuraNetwork core mods cannot be disabled to prevent breaking the client
                    if (fileName.contains("auranetwork") || fileName.contains("merdobridge") || fileName.contains("chickenclient") || fileName.contains("fabric-api")) {
                        continue;
                    }

                    boolean isEnabled = fileName.endsWith(".jar");
                    String displayName = fileName.replace(".jar", "").replace(".disabled", "");
                    if (displayName.length() > 25) {
                        displayName = displayName.substring(0, 25) + "...";
                    }

                    this.addDrawableChild(ButtonWidget.builder(Text.literal(displayName + ": " + (isEnabled ? "Acik" : "Kapali")), button -> {
                        boolean currentlyEnabled = file.getName().endsWith(".jar");
                        String newName = currentlyEnabled ? file.getName().replace(".jar", ".disabled") : file.getName().replace(".disabled", ".jar");
                        File newFile = new File(file.getParent(), newName);
                        if (file.renameTo(newFile)) {
                            this.requiresRestart = true;
                            // Re-init the screen to reflect the change
                            AuraModsScreen newScreen = new AuraModsScreen(this.parent);
                            newScreen.requiresRestart = true;
                            this.client.setScreen(newScreen);
                        }
                    }).dimensions(centerX - 100, startY + yOffset, 200, 20).build());

                    yOffset += 25;
                    // Prevent buttons from going off-screen (basic limit)
                    if (startY + yOffset > this.height - 60) break;
                }
            }
        }

        // Geri Don button
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Geri Don"), button -> {
            this.client.setScreen(this.parent);
        }).dimensions(centerX - 100, this.height - 30, 200, 20).build());
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        this.renderBackground(context, mouseX, mouseY, delta);
        context.drawCenteredTextWithShadow(this.textRenderer, this.title, this.width / 2, 20, 0xFFFFFF);
        
        if (requiresRestart) {
            context.drawCenteredTextWithShadow(this.textRenderer, Text.literal("§cDegisikliklerin uygulanmasi icin oyunu yeniden baslatin!"), this.width / 2, this.height - 50, 0xFF5555);
        }
        
        super.render(context, mouseX, mouseY, delta);
    }
}
