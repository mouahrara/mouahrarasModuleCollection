﻿using HarmonyLib;
using StardewValley.Menus;
using QOLEssentials.UserInterface.Zoom.Utilities;

namespace QOLEssentials.UserInterface.Zoom.Patches
{
	internal class AnimalQueryMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForAnimalPlacement)),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.EnterFarmViewPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForReturnFromPlacement)),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.LeaveFarmViewPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.finishedPlacingAnimal)),
				postfix: new HarmonyMethod(typeof(MenusPatchUtility), nameof(MenusPatchUtility.LeaveFarmViewPostfix))
			);
		}
	}
}
