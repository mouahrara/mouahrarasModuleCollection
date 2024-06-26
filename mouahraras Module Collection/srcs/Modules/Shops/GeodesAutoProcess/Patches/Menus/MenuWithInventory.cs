using System;
using HarmonyLib;
using StardewValley.Menus;
using mouahrarasModuleCollection.Shops.GeodesAutoProcess.Utilities;

namespace mouahrarasModuleCollection.Shops.GeodesAutoProcess.Patches
{
	internal class MenuWithInventoryPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(MenuWithInventory), nameof(MenuWithInventory.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(MenuWithInventoryPatch), nameof(ReceiveLeftClickPrefix))
			);
		}

		private static bool ReceiveLeftClickPrefix(MenuWithInventory __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return true;

			if (__instance.GetType() == typeof(GeodeMenu))
			{
				if (__instance.okButton != null && __instance.okButton.containsPoint(x, y) && !(__instance as GeodeMenu).readyToClose())
				{
					GeodesAutoProcessUtility.EndGeodeProcessing();
					return false;
				}
			}
			return true;
		}
	}
}
