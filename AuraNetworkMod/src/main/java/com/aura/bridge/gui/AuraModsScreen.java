package com.aura.bridge.gui;

import net.fabricmc.loader.api.FabricLoader;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

import java.io.File;
import java.nio.file.Files;
import java.util.HashSet;
import java.util.Set;
import java.util.List;

public class AuraModsScreen extends Screen {
    private final Screen parent;
    public boolean requiresRestart = false;
    private Set<String> disabledMods = new HashSet<>();
    private File disabledModsFile;

    public AuraModsScreen(Screen parent) {
        super(Text.literal("Mod Yoneticisi"));
        this.parent = parent;
        this.disabledModsFile = new File(FabricLoader.getInstance().getGameDir().toFile(), "disabled_mods.txt");
        loadDisabledMods();
    }

    private void loadDisabledMods() {
        if (disabledModsFile.exists()) {
            try {
                List<String> lines = Files.readAllLines(disabledModsFile.toPath());
                disabledMods.addAll(lines);
            } catch (Exception e) {}
        }
    }

    private void saveDisabledMods() {
        try {
            Files.write(disabledModsFile.toPath(), disabledMods);
        } catch (Exception e) {}
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
                    if (fileName.contains("auranetwork") || fileName.contains("merdobridge") || fileName.contains("chickenclient") || fileName.contains("fabric-api")) {
                        continue;
                    }

                    String modKey = fileName.replace(".jar", "").replace(".disabled", "");
                    
                    // Check if it's currently loaded as a jar, OR if it's marked as disabled in our txt
                    // But to be accurate, we just rely on our txt file if it exists, otherwise fallback to extension
                    boolean isEnabled = !disabledMods.contains(modKey);
                    
                    // Special case: if it ends with .disabled and it's not in the txt, we add it to the txt
                    if (fileName.endsWith(".disabled") && !disabledMods.contains(modKey)) {
                        disabledMods.add(modKey);
                        isEnabled = false;
                        saveDisabledMods();
                    }

                    String displayName = modKey;
                    if (displayName.length() > 25) {
                        displayName = displayName.substring(0, 25) + "...";
                    }
                    final String finalDisplayName = displayName;

                    this.addDrawableChild(ButtonWidget.builder(Text.literal(finalDisplayName + ": " + (isEnabled ? "Acik" : "Kapali")), button -> {
                        if (disabledMods.contains(modKey)) {
                            disabledMods.remove(modKey);
                            button.setMessage(Text.literal(finalDisplayName + ": Acik"));
                        } else {
                            disabledMods.add(modKey);
                            button.setMessage(Text.literal(finalDisplayName + ": Kapali"));
                        }
                        saveDisabledMods();
                        this.requiresRestart = true;
                    }).dimensions(centerX - 100, startY + yOffset, 200, 20).build());

                    yOffset += 25;
                    if (startY + yOffset > this.height - 60) break;
                }
            }
        }

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
