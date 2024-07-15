using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace FastDropShip.Patches;

[HarmonyPatch(typeof(ItemDropship))]
public static class ItemDropshipPatch {
    internal static bool reset;

    [HarmonyPatch(nameof(ItemDropship.Update))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void UpdatePostfix(ItemDropship __instance) {
        if (__instance is {
                deliveringOrder: true,
            } or {
                itemsToDeliver.Count: > 0,
            }) return;

        if (__instance.terminalScript is not {
                orderedItemsFromTerminal.Count: > 0,
            } and not {
                vehicleInDropship: true,
            }) return;

        if (__instance.shipTimer >= FastDropShip.shipLandTimer.Value && !reset) return;

        __instance.shipTimer = FastDropShip.shipLandTimer.Value;
        reset = false;
    }

    [HarmonyPatch(nameof(ItemDropship.OpenShipClientRpc))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void OpenShipClientRpcPostfix(ItemDropship __instance) =>
        __instance.StartCoroutine(ShipLeaveLater(__instance, FastDropShip.shipLeaveWait.Value));

    private static IEnumerator ShipLeaveLater(ItemDropship itemDropship, float secondsToWait) {
        yield return new WaitForSeconds(secondsToWait);
        yield return new WaitForEndOfFrame();

        itemDropship.ShipLeaveClientRpc();
    }
}