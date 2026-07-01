package com.aura.bridge.gui;

import com.aura.bridge.settings.ChickenSettings;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;

public class ChickenSettingsScreen extends Screen {
    private final Screen parent;

    public ChickenSettingsScreen(Screen parent) {
        super(Text.literal("Chicken Client Ayarlari"));
        this.parent = parent;
    }

    @Override
    protected void init() {
        int centerX = this.width / 2;
        int startY = this.height / 4;

        this.addDrawableChild(ButtonWidget.builder(Text.literal("Fullbright: " + (ChickenSettings.fullbright ? "Açik" : "Kapali")), button -> {
            ChickenSettings.fullbright = !ChickenSettings.fullbright;
            button.setMessage(Text.literal("Fullbright: " + (ChickenSettings.fullbright ? "Açik" : "Kapali")));
        }).dimensions(centerX - 100, startY, 200, 20).build());

        this.addDrawableChild(ButtonWidget.builder(Text.literal("Düsük Ates (Low Fire): " + (ChickenSettings.lowFire ? "Açik" : "Kapali")), button -> {
            ChickenSettings.lowFire = !ChickenSettings.lowFire;
            button.setMessage(Text.literal("Düsük Ates (Low Fire): " + (ChickenSettings.lowFire ? "Açik" : "Kapali")));
        }).dimensions(centerX - 100, startY + 30, 200, 20).build());

        this.addDrawableChild(ButtonWidget.builder(Text.literal("Geri"), button -> {
            this.client.setScreen(this.parent);
        }).dimensions(centerX - 100, startY + 80, 200, 20).build());
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        super.render(context, mouseX, mouseY, delta);
        context.drawCenteredTextWithShadow(this.textRenderer, this.title, this.width / 2, 20, 0xFFFFFF);
    }
}
