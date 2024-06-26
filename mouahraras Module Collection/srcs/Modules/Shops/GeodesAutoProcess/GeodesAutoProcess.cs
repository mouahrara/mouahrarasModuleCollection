﻿using System;
using HarmonyLib;
using StardewModdingAPI;
using mouahrarasModuleCollection.Shops.GeodesAutoProcess.Patches;

namespace mouahrarasModuleCollection.Modules
{
	internal class GeodesAutoProcessModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply menus patches
				IClickableMenuPatch.Apply(harmony);
				MenuWithInventoryPatch.Apply(harmony);
				GeodeMenuPatch.Apply(harmony);

				// Apply objects patches
				FarmerPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(GeodesAutoProcessModule)} module: {e}", LogLevel.Error);
				return;
			}
		}
	}
}
