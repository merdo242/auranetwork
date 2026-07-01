package com.aura.bridge.gui;

import com.aura.bridge.AuraClientSettings;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

public class AuraSettingsScreen extends Screen {
    private final Screen parent;

    public AuraSettingsScreen(Screen parent) {
        super(Text.literal("Client Ayarlari"));
        this.parent = parent;
    }

    @Override
    protected void init() {
        int centerX = this.width / 2;
        int startY = this.height / 2 - 40;

        // FPS Gostergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("FPS Gostergesi: " + (AuraClientSettings.showFPS ? "Acik" : "Kapali")), button -> {
            AuraClientSettings.showFPS = !AuraClientSettings.showFPS;
            AuraClientSettings.save();
            button.setMessage(Text.literal("FPS Gostergesi: " + (AuraClientSettings.showFPS ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY, 200, 20).build());

        // Ping Gostergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Ping Gostergesi: " + (AuraClientSettings.showPing ? "Acik" : "Kapali")), button -> {
            AuraClientSettings.showPing = !AuraClientSettings.showPing;
            AuraClientSettings.save();
            button.setMessage(Text.literal("Ping Gostergesi: " + (AuraClientSettings.showPing ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 25, 200, 20).build());

        // Keystrokes
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Keystrokes: " + (AuraClientSettings.showKeystrokes ? "Acik" : "Kapali")), button -> {
            AuraClientSettings.showKeystrokes = !AuraClientSettings.showKeystrokes;
            AuraClientSettings.save();
            button.setMessage(Text.literal("Keystrokes: " + (AuraClientSettings.showKeystrokes ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 50, 200, 20).build());

        // Koordinat Gostergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Koordinat (XYZ): " + (AuraClientSettings.showCoords ? "Acik" : "Kapali")), button -> {
            AuraClientSettings.showCoords = !AuraClientSettings.showCoords;
            AuraClientSettings.save();
            button.setMessage(Text.literal("Koordinat (XYZ): " + (AuraClientSettings.showCoords ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 75, 200, 20).build());

        // Biyom Gostergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Biyom (Biome): " + (AuraClientSettings.showBiome ? "Acik" : "Kapali")), button -> {
            AuraClientSettings.showBiome = !AuraClientSettings.showBiome;
            AuraClientSettings.save();
            button.setMessage(Text.literal("Biyom (Biome): " + (AuraClientSettings.showBiome ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 100, 200, 20).build());

        // Geri Don
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
