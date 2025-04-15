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
    public const string VERSION = "1.3.4";

    public new static ManualLogSource Logger;
    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);

    private bool IsInit;

    public static OptionsMenu optionsMenuInstance;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            DebugWarning($"{MOD_NAME} is loading.... {VERSION}");

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
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

        MachineConnector.SetRegisteredOI("MaulAliveCreatures", optionsMenuInstance = new OptionsMenu());

        Hooks.Init();
        Enums.RegisterValues();
    }
}