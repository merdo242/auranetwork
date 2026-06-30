package com.merdo.bridge.mixin;

import net.minecraft.client.gui.screen.ingame.CreativeInventoryScreen;
import net.minecraft.screen.slot.Slot;
import net.minecraft.screen.slot.SlotActionType;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.Inject;
import org.spongepowered.asm.mixin.injection.callback.CallbackInfo;

@Mixin(CreativeInventoryScreen.class)
public class CreativeInventoryScreenMixin {
    @Inject(method = "onMouseClick", at = @At("HEAD"), cancellable = true)
    private void fixShiftClickClear(Slot slot, int slotId, int button, SlotActionType actionType, CallbackInfo ci) {
        if (slot != null && actionType == SlotActionType.QUICK_MOVE) {
            // Destroy item slot id in creative inventory is often a specific slot or handled by checking its inventory type
            if (slot.inventory instanceof net.minecraft.inventory.SimpleInventory && slot.inventory.size() == 1) {
                // This is the destroy item slot
                net.minecraft.client.MinecraftClient client = net.minecraft.client.MinecraftClient.getInstance();
                if (client != null && client.player != null) {
                    client.player.getInventory().clear();
                    client.player.playerScreenHandler.sendContentUpdates();
                    // Send to server that we cleared it
                    for (int i = 0; i < client.player.playerScreenHandler.slots.size(); i++) {
                        client.interactionManager.clickCreativeStack(net.minecraft.item.ItemStack.EMPTY, i);
                    }
                    ci.cancel();
                }
            }
        }
    }
}