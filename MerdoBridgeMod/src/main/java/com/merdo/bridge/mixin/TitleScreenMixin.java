package com.merdo.bridge.mixin;

import com.merdo.bridge.gui.ChickenSettingsScreen;
import net.minecraft.client.gui.screen.Screen;
import net.minecraft.client.gui.screen.TitleScreen;
import net.minecraft.client.gui.widget.ButtonWidget;
import net.minecraft.text.Text;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.Inject;
import org.spongepowered.asm.mixin.injection.callback.CallbackInfo;

@Mixin(TitleScreen.class)
public class TitleScreenMixin extends Screen {
    protected TitleScreenMixin(Text title) {
        super(title);
    }

    @Inject(method = "init", at = @At("RETURN"))
    private void addChickenSettingsButton(CallbackInfo ci) {
        int y = this.height / 4 + 48;
        this.addDrawableChild(ButtonWidget.builder(Text.literal("Chicken Client Ayarlari"), button -> {
            this.client.setScreen(new ChickenSettingsScreen(this));
        }).dimensions(this.width / 2 - 100, y + 72 + 24, 200, 20).build());
    }
}