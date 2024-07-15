using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FastDropShip.Dependencies;
using HarmonyLib;

namespace FastDropShip;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class FastDropShip : BaseUnityPlugin {
    public static FastDropShip Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    internal static ConfigEntry<float> shipLeaveWait = null!;
    internal static ConfigEntry<float> shipLandTimer = null!;

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        if (DependencyChecker.IsLobbyCompatibilityInstalled()) {
            Logger.LogInfo("Found LobbyCompatibility, enabling support for it :)");
            LobbyCompatibilitySupport.Initialize();
        }

        shipLeaveWait = Config.Bind("General", "Ship leave wait seconds", 0F,
                                    new ConfigDescription("Defines how long the dropship waits before leaving after items were fetched",
                                                          new AcceptableValueRange<float>(0F, 4F)));

        shipLandTimer = Config.Bind("General", "Ship land timer", 37F,
                                    new ConfigDescription("Defines how fast the ship lands. The higher, the faster",
                                                          new AcceptableValueRange<float>(0F, 40F)));

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch() {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch() {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}