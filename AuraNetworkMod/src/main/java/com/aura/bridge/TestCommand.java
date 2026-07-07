package com.aura.bridge;

import net.fabricmc.api.ModInitializer;
import net.minecraft.client.network.ClientPlayNetworkHandler;
import java.lang.reflect.Method;

public class TestCommand {
    public static void check() {
        for (Method m : ClientPlayNetworkHandler.class.getMethods()) {
            if (m.getName().toLowerCase().contains("command")) {
                System.out.println(m.getName() + " - " + m.getParameterTypes()[0].getName());
            }
        }
    }
}
