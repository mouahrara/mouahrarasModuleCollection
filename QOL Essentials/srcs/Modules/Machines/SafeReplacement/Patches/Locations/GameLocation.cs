﻿using HarmonyLib;
using xTile.Dimensions;
using StardewValley;
using QOLEssentials.Machines.SafeReplacement.Utilities;

namespace QOLEssentials.Machines.SafeReplacement.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction), new System.Type[] { typeof(Location), typeof(Rectangle), typeof(Farmer) }),
				postfix: new HarmonyMethod(typeof(GameLocationPatch), nameof(CheckActionPostfix))
			);
		}

		private static void CheckActionPostfix()
		{
			if (!ModEntry.Config.MachinesSafeReplacement || SafeReplacementUtility.ObjectToRecover is null)
				return;

			Game1.player.addItemToInventory(SafeReplacementUtility.ObjectToRecover);
			SafeReplacementUtility.Reset();
		}
	}
}
