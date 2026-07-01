package com.merdo.bridge.gui;

import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.screen.multiplayer.MultiplayerScreen;
import net.minecraft.client.gui.screen.option.OptionsScreen;
import net.minecraft.client.gui.screen.world.SelectWorldScreen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;
import net.minecraft.util.Identifier;
import net.minecraft.client.texture.NativeImage;
import net.minecraft.client.texture.NativeImageBackedTexture;
import net.minecraft.util.Util;
import java.io.InputStream;
import java.net.URL;
import java.util.concurrent.CompletableFuture;

public class MerdoTitleScreen extends Screen {
    private static final Identifier BACKGROUND_TEXTURE = Identifier.of("merdobridge", "textures/gui/background.png");
    private static final Identifier LOGO_TEXTURE = Identifier.of("merdobridge", "textures/gui/logo.png");
    
    private Identifier playerSkinId = null;
    private boolean skinLoading = false;

    public MerdoTitleScreen() {
        super(Text.literal("Merdo Launcher"));
    }

    @Override
    protected void init() {
        int leftX = 40;
        int startY = this.height / 2 - 70;

        // Tekli Oyuncu
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Tekli Oyuncu"), button -> {
            this.client.setScreen(new SelectWorldScreen(this));
        }).dimensions(leftX, startY, 160, 20).build());

        // Çoklu Oyuncu
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Coklu Oyuncu"), button -> {
            this.client.setScreen(new MultiplayerScreen(this));
        }).dimensions(leftX, startY + 25, 160, 20).build());

        // Ayarlar
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Oyun Ayarlari"), button -> {
            this.client.setScreen(new OptionsScreen(this, this.client.options));
        }).dimensions(leftX, startY + 50, 160, 20).build());

        // Client Ayarları
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Client Ayarlari"), button -> {
            this.client.setScreen(new MerdoSettingsScreen(this));
        }).dimensions(leftX, startY + 75, 160, 20).build());

        // Destek
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Destek Olustur"), button -> {
            Util.getOperatingSystem().open("https://merdonetwork.com/destek");
        }).dimensions(leftX, startY + 100, 160, 20).build());

        // Wiki
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Wiki"), button -> {
            Util.getOperatingSystem().open("https://merdonetwork.com/wiki");
        }).dimensions(leftX, startY + 125, 160, 20).build());

        // Oyundan Çık
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Oyundan Cik"), button -> {
            this.client.scheduleStop();
        }).dimensions(leftX, startY + 150, 160, 20).build());

        if (this.playerSkinId == null && !skinLoading) {
            loadPlayerSkin();
        }
    }

    private void loadPlayerSkin() {
        skinLoading = true;
        String username = this.client.getSession().getUsername();
        CompletableFuture.runAsync(() -> {
            try {
                URL url = new URL("https://minotar.net/armor/body/" + username + "/150.png");
                InputStream is = url.openStream();
                NativeImage image = NativeImage.read(is);
                is.close();
                
                this.client.execute(() -> {
                    NativeImageBackedTexture texture = new NativeImageBackedTexture(image);
                    this.playerSkinId = this.client.getTextureManager().registerDynamicTexture("player_skin", texture);
                });
            } catch (Exception e) {
                e.printStackTrace();
            }
        });
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        // Draw background
        context.drawTexture(BACKGROUND_TEXTURE, 0, 0, 0, 0, this.width, this.height, this.width, this.height);

        // Draw Left Panel semi-transparent background
        context.fill(20, 0, 220, this.height, 0x66000000);

        super.render(context, mouseX, mouseY, delta);

        // Draw Logo (1024x1024 file, drawn as 80x80 to keep square aspect ratio)
        context.drawTexture(LOGO_TEXTURE, 80, 20, 0, 0, 80, 80, 80, 80);

        // Draw Right Panel
        int rightPanelWidth = 260;
        int rightPanelHeight = 220;
        int rightPanelX = this.width - rightPanelWidth - 40;
        int rightPanelY = (this.height - rightPanelHeight) / 2;
        
        context.fill(rightPanelX, rightPanelY, rightPanelX + rightPanelWidth, rightPanelY + rightPanelHeight, 0xAA000000); // Darker black

        // Welcome Header
        context.drawCenteredTextWithShadow(this.textRenderer, Text.literal("MERDOCLIENT'A HOSGELDIN"), rightPanelX + rightPanelWidth / 2, rightPanelY + 15, 0xFFAA00);
        context.fill(rightPanelX + 20, rightPanelY + 30, rightPanelX + rightPanelWidth - 20, rightPanelY + 31, 0x55FFFFFF);

        // Username
        String username = this.client.getSession().getUsername();
        context.drawText(this.textRenderer, Text.literal("Oyuncu: " + username), rightPanelX + 110, rightPanelY + 60, 0xFFFFFF, true);
        context.drawText(this.textRenderer, Text.literal("Surum: v0.0.1"), rightPanelX + 110, rightPanelY + 80, 0xAAAAAA, true);
        context.drawText(this.textRenderer, Text.literal("Durum: Baglandi"), rightPanelX + 110, rightPanelY + 100, 0x55FF55, true);

        // Draw Skin (150x300 file, drawn as 70x140 to keep 1:2 aspect ratio)
        if (this.playerSkinId != null) {
            context.drawTexture(this.playerSkinId, rightPanelX + 20, rightPanelY + 50, 0, 0, 70, 140, 70, 140);
        }
    }
}
