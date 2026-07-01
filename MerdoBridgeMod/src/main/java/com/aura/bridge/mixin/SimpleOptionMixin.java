package com.aura.bridge.mixin;

import com.aura.bridge.settings.ChickenSettings;
import net.minecraft.client.option.SimpleOption;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.Inject;
import org.spongepowered.asm.mixin.injection.callback.CallbackInfoReturnable;

@Mixin(SimpleOption.class)
public class SimpleOptionMixin<T> {
    @Inject(method = "getValue", at = @At("HEAD"), cancellable = true)
    private void onGetValue(CallbackInfoReturnable<T> cir) {
        if (ChickenSettings.fullbright) {
            // "options.gamma" is used for brightness
            if (this.toString().contains("options.gamma")) {
                cir.setReturnValue((T) (Double) 100.0);
            }
        }
    }
}
