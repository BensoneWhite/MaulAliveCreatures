using BepInEx;
using BepInEx.Logging;
using System;

namespace MaulAliveCreatures;

[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "MaulAliveCreatures";
    public const string MOD_NAME = "MaulAliveCreatures";
    public const string AUTHORS = "BensoneWhite";
    public const string VERSION = "1.3.3";

    //BepInEx Logger for easy console logs
    public new static ManualLogSource Logger;
    public static void DebugLog(object ex) => Logger.LogInfo(ex);
    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);
    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    private bool IsInit;

    public static OptionsMenu optionsMenuInstance;

    // Called when the plugin is loaded.
    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        try
        {
            foreach (var mod in newlyDisabledMods)
            {
                if (mod.id == MOD_ID)
                {
                    Enums.UnregisterValues();
                    Hooks.Unhook();
                    DebugLog($"Unregistering.... {MOD_NAME} creatures and items");
                }
            }

        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        MachineConnector.SetRegisteredOI("MaulAliveCreatures", (OptionInterface)(object)(optionsMenuInstance = new OptionsMenu()));

        Hooks.Init();
        Enums.RegisterValues();
    }
}