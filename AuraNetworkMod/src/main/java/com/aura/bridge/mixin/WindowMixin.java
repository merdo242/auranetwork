package com.aura.bridge.mixin;

import net.minecraft.client.util.Window;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.ModifyVariable;

@Mixin(Window.class)
public class WindowMixin {
    @ModifyVariable(method = "setTitle", at = @At("HEAD"), argsOnly = true)
    private String modifyTitle(String original) {
        return "Aura Launcher";
    }
}
