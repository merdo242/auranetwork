package com.merdo.bridge.gui;

import com.merdo.bridge.MerdoClientSettings;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

public class MerdoSettingsScreen extends Screen {
    private final Screen parent;

    public MerdoSettingsScreen(Screen parent) {
        super(Text.literal("Client Ayarları"));
        this.parent = parent;
    }

    @Override
    protected void init() {
        int centerX = this.width / 2;
        int startY = this.height / 2 - 40;

        // FPS Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("FPS Gostergesi: " + (MerdoClientSettings.showFPS ? "Acik" : "Kapali")), button -> {
            MerdoClientSettings.showFPS = !MerdoClientSettings.showFPS;
            button.setMessage(Text.literal("FPS Gostergesi: " + (MerdoClientSettings.showFPS ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY, 200, 20).build());

        // Ping Göstergesi
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Ping Gostergesi: " + (MerdoClientSettings.showPing ? "Acik" : "Kapali")), button -> {
            MerdoClientSettings.showPing = !MerdoClientSettings.showPing;
            button.setMessage(Text.literal("Ping Gostergesi: " + (MerdoClientSettings.showPing ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 25, 200, 20).build());

        // Keystrokes
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Keystrokes: " + (MerdoClientSettings.showKeystrokes ? "Acik" : "Kapali")), button -> {
            MerdoClientSettings.showKeystrokes = !MerdoClientSettings.showKeystrokes;
            button.setMessage(Text.literal("Keystrokes: " + (MerdoClientSettings.showKeystrokes ? "Acik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 50, 200, 20).build());

        // Geri Dön
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Geri Don"), button -> {
            this.client.setScreen(this.parent);
        }).dimensions(centerX - 100, startY + 90, 200, 20).build());
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        this.renderBackground(context, mouseX, mouseY, delta);
        context.drawCenteredTextWithShadow(this.textRenderer, this.title, this.width / 2, 20, 0xFFFFFF);
        super.render(context, mouseX, mouseY, delta);
    }
}
