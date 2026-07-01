package com.aura.bridge.gui;
import net.minecraft.client.MinecraftClient;
import net.minecraft.util.Identifier;
import net.minecraft.text.Text;

public class Dummy {
    public void test() {
        Text text = Text.literal("Test").styled(style -> style.withFont(Identifier.of("minecraft", "uniform")));
    }
}
