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
import net.minecraft.client.render.entity.model.PlayerEntityModel;
import net.minecraft.client.render.entity.model.EntityModelLayers;
import net.minecraft.entity.LivingEntity;
import net.minecraft.client.render.RenderLayer;
import net.minecraft.client.render.OverlayTexture;

import java.io.InputStream;
import java.net.URL;
import java.util.concurrent.CompletableFuture;

public class MerdoTitleScreen extends Screen {
    private static final Identifier BACKGROUND_TEXTURE = Identifier.of("merdobridge", "textures/gui/background.png");
    private static final Identifier LOGO_TEXTURE = Identifier.of("merdobridge", "textures/gui/logo.png");
    
    private Identifier playerSkinId = null;
    private boolean skinLoading = false;
    private PlayerEntityModel<LivingEntity> playerModel;

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

        if (this.width >= 540) {
            int rightPanelWidth = 340;
            int rightPanelHeight = 260;
            int rightPanelX = this.width - rightPanelWidth - 40;
            int rightPanelY = (this.height - rightPanelHeight) / 2;
            
            this.addDrawableChild(ButtonWidget.builder(Text.literal("Discord"), button -> {
                Util.getOperatingSystem().open("https://discord.gg/merdonetwork");
            }).dimensions(rightPanelX + 130, rightPanelY + 210, 190, 20).build());
        }

        if (this.playerSkinId == null && !skinLoading) {
            loadPlayerSkin();
        }

        if (this.playerModel == null) {
            this.playerModel = new PlayerEntityModel<>(this.client.getEntityModelLoader().getModelPart(EntityModelLayers.PLAYER), false);
        }
    }

    private void loadPlayerSkin() {
        skinLoading = true;
        String username = this.client.getSession().getUsername();
        CompletableFuture.runAsync(() -> {
            try {
                // Skini raw 64x64 texture olarak al
                URL url = new URL("https://minotar.net/skin/" + username);
                InputStream is = url.openStream();
                NativeImage image = NativeImage.read(is);
                is.close();
                
                this.client.execute(() -> {
                    NativeImageBackedTexture texture = new NativeImageBackedTexture(image);
                    this.playerSkinId = this.client.getTextureManager().registerDynamicTexture("player_skin_raw", texture);
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

        com.mojang.blaze3d.systems.RenderSystem.enableBlend();
        com.mojang.blaze3d.systems.RenderSystem.defaultBlendFunc();

        // Draw Logo (Hide or scale down if screen is too small to prevent overlap)
        if (this.width >= 540) {
            context.drawTexture(LOGO_TEXTURE, 80, 20, 0, 0, 80, 80, 80, 80);
        } else {
            context.drawTexture(LOGO_TEXTURE, 40, 10, 0, 0, 40, 40, 80, 80);
        }

        super.render(context, mouseX, mouseY, delta);

        if (this.width >= 540) {
            int rightPanelWidth = 340;
            int rightPanelHeight = 260;
            int rightPanelX = this.width - rightPanelWidth - 40;
            int rightPanelY = (this.height - rightPanelHeight) / 2;
            
            context.fill(rightPanelX, rightPanelY, rightPanelX + rightPanelWidth, rightPanelY + rightPanelHeight, 0xAA000000);

            // Title
            context.drawTextWithShadow(this.textRenderer, Text.literal("Yenilenmis Haliyle MerdoClient v0.0.1"), rightPanelX + 130, rightPanelY + 20, 0xFFFFFF);
            context.fill(rightPanelX + 130, rightPanelY + 35, rightPanelX + rightPanelWidth - 20, rightPanelY + 36, 0x55FFFFFF);
            
            // Text
            String[] lines = {
                ">> Selam!",
                "",
                "MerdoClient'in yenilenmis halinin ilk surumune hos",
                "geldiniz.",
                "Bu yeni surumle birlikte yeni ve kristal pvp'ye uygun",
                "modlar, tamamen yenilenmis bir arayuz",
                "ve daha hizli bir oyun zevki sizlere sunmayi hedefliyoruz.",
                "",
                "Client hakkindaki geri donusleriniz icin sitemiz uzerinden",
                "destek talebi olusturabilirsiniz."
            };
            
            int textY = rightPanelY + 50;
            for(String line : lines) {
                context.drawText(this.textRenderer, Text.literal(line), rightPanelX + 130, textY, 0xAAAAAA, false);
                textY += 12;
            }
            
            // Draw 3D Skin
            if (this.playerSkinId != null && this.playerModel != null) {
                int skinX = rightPanelX + 65;
                int skinY = rightPanelY + 190;
                int scale = 55;
                
                float pitch = (float)Math.atan((double)((skinY - 90 - mouseY) / 40.0F));
                float yaw = (float)Math.atan((double)((skinX - mouseX) / 40.0F));
                
                net.minecraft.client.util.math.MatrixStack matrices = context.getMatrices();
                matrices.push();
                matrices.translate((float)skinX, (float)skinY, 50.0F);
                matrices.scale((float)(-scale), (float)scale, (float)scale);
                
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_Z.rotationDegrees(180.0F));
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_X.rotationDegrees(pitch * 20.0F));
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_Y.rotationDegrees(yaw * 20.0F));

                this.playerModel.setVisible(true);
                this.playerModel.head.pitch = pitch;
                this.playerModel.head.yaw = yaw;
                this.playerModel.rightArm.pitch = -0.1f;
                this.playerModel.leftArm.pitch = -0.1f;
                this.playerModel.rightLeg.pitch = 0.1f;
                this.playerModel.leftLeg.pitch = -0.1f;
                
                this.playerModel.render(matrices, context.getVertexConsumers().getBuffer(net.minecraft.client.render.RenderLayer.getEntityCutoutNoCull(this.playerSkinId)), 15728880, net.minecraft.client.render.OverlayTexture.DEFAULT_UV);
                context.draw(); 
                matrices.pop();
                
                // Draw Username above skin
                String username = this.client.getSession().getUsername();
                matrices.push();
                matrices.translate(skinX, skinY - 150, 0);
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_Z.rotationDegrees(-15.0F));
                context.drawCenteredTextWithShadow(this.textRenderer, Text.literal(username), 0, 0, 0xFFFFFF);
                matrices.pop();
            }
        }
    }
}
