package com.aura.bridge.mixin;

import net.minecraft.client.gui.screen.multiplayer.MultiplayerScreen;
import net.minecraft.client.network.ServerInfo;
import net.minecraft.client.network.ServerInfo.ServerType;
import net.minecraft.client.option.ServerList;
import org.spongepowered.asm.mixin.Mixin;
import org.spongepowered.asm.mixin.Shadow;
import org.spongepowered.asm.mixin.injection.At;
import org.spongepowered.asm.mixin.injection.Inject;
import org.spongepowered.asm.mixin.injection.callback.CallbackInfo;

@Mixin(MultiplayerScreen.class)
public class MultiplayerScreenMixin {
    @Shadow private ServerList serverList;

    @Inject(method = "init", at = @At("RETURN"))
    private void pinOurServer(CallbackInfo ci) {
        if (this.serverList != null) {
            String targetIp = "91.132.49.16";
            int foundIndex = -1;
            for (int i = 0; i < this.serverList.size(); i++) {
                if (this.serverList.get(i).address.equalsIgnoreCase(targetIp)) {
                    foundIndex = i;
                    break;
                }
            }
            
            ServerInfo pinnedServer;
            if (foundIndex != -1) {
                pinnedServer = this.serverList.get(foundIndex);
                // Remove from current position to move to top
                if (foundIndex > 0) {
                    ServerList newList = new ServerList(net.minecraft.client.MinecraftClient.getInstance());
                    newList.add(pinnedServer, false);
                    for (int i = 0; i < this.serverList.size(); i++) {
                        if (i != foundIndex) {
                            newList.add(this.serverList.get(i), false);
                        }
                    }
                    newList.saveFile();
                    this.serverList.loadFile();
                }
            } else {
                pinnedServer = new ServerInfo("Chicken Network", targetIp, ServerType.OTHER);
                ServerList newList = new ServerList(net.minecraft.client.MinecraftClient.getInstance());
                newList.add(pinnedServer, false);
                for (int i = 0; i < this.serverList.size(); i++) {
                    newList.add(this.serverList.get(i), false);
                }
                newList.saveFile();
                this.serverList.loadFile();
            }
        }
    }
}
