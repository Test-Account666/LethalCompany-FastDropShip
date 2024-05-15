using HarmonyLib;

namespace FastDropShip.Patches;

[HarmonyPatch(typeof(Terminal))]
public static class TerminalPatch {
    [HarmonyPatch(nameof(Terminal.SyncGroupCreditsClientRpc))]
    [HarmonyPrefix]
    private static void SyncGroupCreditsClientRpcPrefix() {
        FastDropShip.Logger.LogInfo("Buying something!");
        ItemDropshipPatch.reset = true;
    }
}