package com.merdo.bridge.gui;

import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.screen.multiplayer.MultiplayerScreen;
import net.minecraft.client.gui.screen.option.OptionsScreen;
import net.minecraft.client.gui.screen.world.SelectWorldScreen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import net.minecraft.client.gui.DrawContext;
import net.minecraft.util.Identifier;
import net.minecraft.util.Util;
import net.minecraft.client.render.entity.model.PlayerEntityModel;
import net.minecraft.client.render.entity.model.EntityModelLayers;
import net.minecraft.entity.LivingEntity;
import net.minecraft.client.util.SkinTextures;

public class MerdoTitleScreen extends Screen {
    private static final Identifier BACKGROUND_TEXTURE = Identifier.of("merdobridge", "textures/gui/background.png");
    private static final Identifier LOGO_TEXTURE = Identifier.of("merdobridge", "textures/gui/logo.png");
    private static final Identifier TEXT_LOGO_TEXTURE = Identifier.of("merdobridge", "textures/gui/text_logo.png");
    
    private PlayerEntityModel<LivingEntity> playerModelDefault;
    private PlayerEntityModel<LivingEntity> playerModelSlim;

    public MerdoTitleScreen() {
        super(Text.literal("Merdo Launcher"));
    }

    private Text getCustomFontText(String string) {
        return Text.literal(string);
    }

    @Override
    protected void init() {
        int leftX = 40;
        int startY = this.height / 2 - 70;

        // Tekli Oyuncu
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Tekli Oyuncu"), button -> {
            this.client.setScreen(new SelectWorldScreen(this));
        }).dimensions(leftX, startY, 160, 20).build());

        // Çoklu Oyuncu
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Coklu Oyuncu"), button -> {
            this.client.setScreen(new MultiplayerScreen(this));
        }).dimensions(leftX, startY + 25, 160, 20).build());

        // Ayarlar
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Oyun Ayarlari"), button -> {
            this.client.setScreen(new OptionsScreen(this, this.client.options));
        }).dimensions(leftX, startY + 50, 160, 20).build());

        // Destek
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Destek Olustur"), button -> {
            Util.getOperatingSystem().open("https://merdonetwork.com/destek");
        }).dimensions(leftX, startY + 75, 160, 20).build());

        // Wiki
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Wiki"), button -> {
            Util.getOperatingSystem().open("https://merdonetwork.com/wiki");
        }).dimensions(leftX, startY + 100, 160, 20).build());

        // Oyundan Çık
        this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Oyundan Cik"), button -> {
            this.client.scheduleStop();
        }).dimensions(leftX, startY + 125, 160, 20).build());



        if (this.width >= 540) {
            int rightPanelWidth = 340;
            int rightPanelHeight = 260;
            int rightPanelX = this.width - rightPanelWidth - 40;
            int rightPanelY = (this.height - rightPanelHeight) / 2;
            
            this.addDrawableChild(ButtonWidget.builder(getCustomFontText("Discord"), button -> {
                Util.getOperatingSystem().open("https://discord.gg/merdonetwork");
            }).dimensions(rightPanelX + 130, rightPanelY + 210, 190, 20).build());
        }

        if (this.playerModelDefault == null) {
            this.playerModelDefault = new PlayerEntityModel<>(this.client.getEntityModelLoader().getModelPart(EntityModelLayers.PLAYER), false);
        }
        if (this.playerModelSlim == null) {
            this.playerModelSlim = new PlayerEntityModel<>(this.client.getEntityModelLoader().getModelPart(EntityModelLayers.PLAYER_SLIM), true);
        }
    }

    @Override
    public void render(DrawContext context, int mouseX, int mouseY, float delta) {
        // Draw background
        context.drawTexture(BACKGROUND_TEXTURE, 0, 0, 0, 0, this.width, this.height, this.width, this.height);

        com.mojang.blaze3d.systems.RenderSystem.enableBlend();
        com.mojang.blaze3d.systems.RenderSystem.defaultBlendFunc();
        com.mojang.blaze3d.systems.RenderSystem.setShaderColor(1.0f, 1.0f, 1.0f, 1.0f);

        super.render(context, mouseX, mouseY, delta);

        // Arka plandaki blurdan/karanliktan logoyu göstermek için siyah şeffaf arka plan çiziyoruz
        if (this.width >= 540) {
            context.drawTexture(LOGO_TEXTURE, 80, 20, 0, 0, 80, 80, 80, 80);
            context.drawTexture(TEXT_LOGO_TEXTURE, 30, 110, 0, 0, 180, 38, 180, 38);
        } else {
            context.drawTexture(LOGO_TEXTURE, 40, 10, 0, 0, 40, 40, 80, 80);
            context.drawTexture(TEXT_LOGO_TEXTURE, 15, 60, 0, 0, 90, 19, 90, 19);
        }

        if (this.width >= 540) {
            int rightPanelWidth = 340;
            int rightPanelHeight = 260;
            int rightPanelX = this.width - rightPanelWidth - 40;
            int rightPanelY = (this.height - rightPanelHeight) / 2;
            
            context.fill(rightPanelX, rightPanelY, rightPanelX + rightPanelWidth, rightPanelY + rightPanelHeight, 0xAA000000);

            // Title
            context.drawTextWithShadow(this.textRenderer, getCustomFontText("Yenilenmis Haliyle MerdoClient v0.0.1"), rightPanelX + 130, rightPanelY + 20, 0xFFFFFF);
            context.fill(rightPanelX + 130, rightPanelY + 35, rightPanelX + rightPanelWidth - 20, rightPanelY + 36, 0x55FFFFFF);
            
            // Text
            String[] lines = {
                ">> Selam!",
                "",
                "MerdoClient'in yenilenmis halinin",
                "ilk surumune hos geldiniz.",
                "",
                "Bu surumle birlikte yeni ve",
                "kristal pvp'ye uygun modlar,",
                "tamamen yenilenmis bir arayuz",
                "ve daha hizli bir oyun zevki",
                "sizlere sunmayi hedefliyoruz.",
                "",
                "Client hakkindaki geri donusleriniz",
                "icin destek talebi olusturabilirsiniz."
            };
            
            int textY = rightPanelY + 50;
            for(String line : lines) {
                context.drawText(this.textRenderer, getCustomFontText(line), rightPanelX + 130, textY, 0xAAAAAA, false);
                textY += 12;
            }
            
            // Draw 3D Skin using Minecraft's SkinProvider (Fixes the broken textures)
            SkinTextures skinTextures = this.client.getSkinProvider().getSkinTextures(this.client.getGameProfile());
            Identifier playerSkinId = skinTextures.texture();
            boolean isSlim = skinTextures.model() == SkinTextures.Model.SLIM;
            
            PlayerEntityModel<LivingEntity> modelToRender = isSlim ? this.playerModelSlim : this.playerModelDefault;
            
            if (playerSkinId != null && modelToRender != null) {
                int skinX = rightPanelX + 65;
                int skinY = rightPanelY + 235;
                int scale = 125;
                
                float pitch = (float)Math.atan((double)((skinY - 90 - mouseY) / 40.0F));
                float yaw = (float)Math.atan((double)((skinX - mouseX) / 40.0F));
                
                net.minecraft.client.util.math.MatrixStack matrices = context.getMatrices();
                matrices.push();
                matrices.translate((float)skinX, (float)skinY, 50.0F);
                matrices.scale((float)(-scale), (float)scale, (float)scale);
                
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_X.rotationDegrees(pitch * 20.0F));
                matrices.multiply(net.minecraft.util.math.RotationAxis.POSITIVE_Y.rotationDegrees(yaw * 20.0F + 180.0F));

                modelToRender.setVisible(true);
                modelToRender.head.pitch = pitch;
                modelToRender.head.yaw = yaw;
                modelToRender.rightArm.pitch = -0.1f;
                modelToRender.leftArm.pitch = -0.1f;
                modelToRender.rightLeg.pitch = 0.1f;
                modelToRender.leftLeg.pitch = -0.1f;
                
                modelToRender.render(matrices, context.getVertexConsumers().getBuffer(net.minecraft.client.render.RenderLayer.getEntityCutoutNoCull(playerSkinId)), 15728880, net.minecraft.client.render.OverlayTexture.DEFAULT_UV);
                context.draw(); 
                matrices.pop();
                
                // Draw Username above skin
                String username = this.client.getSession().getUsername();
                matrices.push();
                matrices.translate(skinX, skinY - 250, 0);
                context.drawCenteredTextWithShadow(this.textRenderer, getCustomFontText(username), 0, 0, 0xFFFFFF);
                matrices.pop();
            }
        }
    }
}
