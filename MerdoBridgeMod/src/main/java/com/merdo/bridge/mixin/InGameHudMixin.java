package com.merdo.bridge.mixin;

import com.merdo.bridge.MerdoClientSettings;
import net.minecraft.client.MinecraftClient;
import net.minecraft.client.gui.DrawContext;
import net.minecraft.client.gui.hud.InGameHud;
import net.minecraft.client.network.PlayerListEntry;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.Inject;
import org.spongepowered.asm.mixin.injection.callback.CallbackInfo;

import net.minecraft.client.render.RenderTickCounter;

@Mixin(InGameHud.class)
public class InGameHudMixin {

    @Inject(method = "render", at = @At("RETURN"))
    private void onRender(DrawContext context, RenderTickCounter tickCounter, CallbackInfo ci) {
        MinecraftClient client = MinecraftClient.getInstance();
        if (client.options.hudHidden || client.player == null) return;

        int yOffset = 5;

        // Draw FPS
        if (MerdoClientSettings.showFPS) {
            String fpsStr = "FPS: " + client.getCurrentFps();
            context.drawTextWithShadow(client.textRenderer, fpsStr, 5, yOffset, 0x00FF00);
            yOffset += 10;
        }

        // Draw Ping
        if (MerdoClientSettings.showPing) {
            int ping = 0;
            if (client.getNetworkHandler() != null) {
                PlayerListEntry entry = client.getNetworkHandler().getPlayerListEntry(client.player.getUuid());
                if (entry != null) {
                    ping = entry.getLatency();
                }
            }
            String pingStr = "Ping: " + ping + " ms";
            context.drawTextWithShadow(client.textRenderer, pingStr, 5, yOffset, 0x00FFFF);
            yOffset += 10;
        }

        // Draw Keystrokes
        if (MerdoClientSettings.showKeystrokes) {
            int startX = 5;
            int startY = yOffset + 10;
            
            // W
            drawKey(context, client.options.forwardKey.isPressed(), "W", startX + 22, startY);
            // A, S, D
            drawKey(context, client.options.leftKey.isPressed(), "A", startX, startY + 22);
            drawKey(context, client.options.backKey.isPressed(), "S", startX + 22, startY + 22);
            drawKey(context, client.options.rightKey.isPressed(), "D", startX + 44, startY + 22);
            
            // LMB, RMB
            drawKeyRect(context, client.options.attackKey.isPressed(), "LMB", startX, startY + 44, 31, 20);
            drawKeyRect(context, client.options.useKey.isPressed(), "RMB", startX + 33, startY + 44, 31, 20);
        }
    }

    private void drawKey(DrawContext context, boolean pressed, String name, int x, int y) {
        drawKeyRect(context, pressed, name, x, y, 20, 20);
    }

    private void drawKeyRect(DrawContext context, boolean pressed, String name, int x, int y, int width, int height) {
        int color = pressed ? 0x99FFFFFF : 0x66000000;
        int textColor = pressed ? 0x000000 : 0xFFFFFF;
        
        context.fill(x, y, x + width, y + height, color);
        MinecraftClient client = MinecraftClient.getInstance();
        
        int textWidth = client.textRenderer.getWidth(name);
        context.drawText(client.textRenderer, name, x + (width - textWidth) / 2, y + (height - 8) / 2, textColor, false);
    }
}
