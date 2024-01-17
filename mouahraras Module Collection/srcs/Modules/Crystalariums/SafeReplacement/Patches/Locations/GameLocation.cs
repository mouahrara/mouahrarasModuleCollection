﻿using HarmonyLib;
using xTile.Dimensions;
using StardewValley;
using mouahrarasModuleCollection.Crystalariums.SafeReplacement.Utilities;

namespace mouahrarasModuleCollection.Crystalariums.SafeReplacement.Patches
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

		private static void CheckActionPostfix(GameLocation __instance)
		{
			if (!ModEntry.Config.CrystalariumsSafeReplacement)
				return;
			if (SafeReplacementUtility.GetObjectToRecover() == null)
				return;

			Game1.player.addItemToInventory(SafeReplacementUtility.GetObjectToRecover());
			SafeReplacementUtility.Reset();
		}
	}
}