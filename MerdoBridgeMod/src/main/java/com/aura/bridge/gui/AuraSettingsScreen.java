package com.aura.bridge.gui;

import com.aura.bridge.AuraNetworkSettings;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

public class AuraSettingsScreen extends Screen {
    private final Screen parent;

    public AuraSettingsScreen(Screen parent) {
        super(Text.literal("Client Ayarları"));
        this.parent = parent;
    }

    @Override
    protected void init() {
        int centerX = this.width / 2;
        int startY = this.height / 2 - 40;

        // FPS Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("FPS Gostergesi: " + (AuraNetworkSettings.showFPS ? "Acik" : "Kapali")), button -> {
            AuraNetworkSettings.showFPS = !AuraNetworkSettings.showFPS;
            button.setMessage(Text.literal("FPS Gostergesi: " + (AuraNetworkSettings.showFPS ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY, 200, 20).build());

        // Ping Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Ping Gostergesi: " + (AuraNetworkSettings.showPing ? "Acik" : "Kapali")), button -> {
            AuraNetworkSettings.showPing = !AuraNetworkSettings.showPing;
            button.setMessage(Text.literal("Ping Gostergesi: " + (AuraNetworkSettings.showPing ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 25, 200, 20).build());

        // Keystrokes
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Keystrokes: " + (AuraNetworkSettings.showKeystrokes ? "Acik" : "Kapali")), button -> {
            AuraNetworkSettings.showKeystrokes = !AuraNetworkSettings.showKeystrokes;
            button.setMessage(Text.literal("Keystrokes: " + (AuraNetworkSettings.showKeystrokes ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 50, 200, 20).build());

        // Koordinat Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Koordinat (XYZ): " + (AuraNetworkSettings.showCoords ? "Acik" : "Kapali")), button -> {
            AuraNetworkSettings.showCoords = !AuraNetworkSettings.showCoords;
            button.setMessage(Text.literal("Koordinat (XYZ): " + (AuraNetworkSettings.showCoords ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 75, 200, 20).build());

        // Biyom Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Biyom (Biome): " + (AuraNetworkSettings.showBiome ? "Acik" : "Kapali")), button -> {
            AuraNetworkSettings.showBiome = !AuraNetworkSettings.showBiome;
            button.setMessage(Text.literal("Biyom (Biome): " + (AuraNetworkSettings.showBiome ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 100, 200, 20).build());

        // Geri Dön
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Geri Don"), button -> {
            this.client.setScreen(this.parent);
        }).dimensions(centerX - 100, startY + 140, 200, 20).build());
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        this.renderBackground(context, mouseX, mouseY, delta);
        context.drawCenteredTextWithShadow(this.textRenderer, this.title, this.width / 2, 20, 0xFFFFFF);
        super.render(context, mouseX, mouseY, delta);
    }
}
