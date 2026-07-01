package com.aura.bridge;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.util.Properties;

public class AuraClientSettings {
    public static boolean showFPS = true;
    public static boolean showPing = true;
    public static boolean showKeystrokes = true;
    public static boolean showCoords = true;
    public static boolean showBiome = true;

    private static final File configFile = new File("config/auranetwork_settings.properties");

    public static void load() {
        if (!configFile.exists()) {
            save();
            return;
        }
        try (FileReader reader = new FileReader(configFile)) {
            Properties props = new Properties();
            props.load(reader);
            
            showFPS = Boolean.parseBoolean(props.getProperty("showFPS", "true"));
            showPing = Boolean.parseBoolean(props.getProperty("showPing", "true"));
            showKeystrokes = Boolean.parseBoolean(props.getProperty("showKeystrokes", "true"));
            showCoords = Boolean.parseBoolean(props.getProperty("showCoords", "true"));
            showBiome = Boolean.parseBoolean(props.getProperty("showBiome", "true"));
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void save() {
        try {
            if (!configFile.getParentFile().exists()) {
                configFile.getParentFile().mkdirs();
            }
            Properties props = new Properties();
            props.setProperty("showFPS", String.valueOf(showFPS));
            props.setProperty("showPing", String.valueOf(showPing));
            props.setProperty("showKeystrokes", String.valueOf(showKeystrokes));
            props.setProperty("showCoords", String.valueOf(showCoords));
            props.setProperty("showBiome", String.valueOf(showBiome));
            
            try (FileWriter writer = new FileWriter(configFile)) {
                props.store(writer, "Aura Network Client Settings");
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
