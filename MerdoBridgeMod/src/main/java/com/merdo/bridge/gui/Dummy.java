package com.merdo.bridge.gui;
import net.minecraft.client.MinecraftClient;
import net.minecraft.client.gui.DrawContext;
import net.minecraft.client.render.entity.model.PlayerEntityModel;
import net.minecraft.client.render.entity.model.EntityModelLayers;
import net.minecraft.client.render.RenderLayer;
import net.minecraft.client.render.OverlayTexture;
import net.minecraft.entity.LivingEntity;
import net.minecraft.util.Identifier;

public class Dummy {
    public static void test(DrawContext context, Identifier texture) {
        MinecraftClient client = MinecraftClient.getInstance();
        PlayerEntityModel<LivingEntity> model = new PlayerEntityModel<>(client.getEntityModelLoader().getModelPart(EntityModelLayers.PLAYER), false);
        model.render(context.getMatrices(), context.getVertexConsumers().getBuffer(RenderLayer.getEntityCutoutNoCull(texture)), 15728880, OverlayTexture.DEFAULT_UV);
    }
}
