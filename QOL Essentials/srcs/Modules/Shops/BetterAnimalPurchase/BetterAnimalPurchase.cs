﻿using System;
using HarmonyLib;
using StardewModdingAPI;
using QOLEssentials.Shops.BetterAnimalPurchase.Handlers;
using QOLEssentials.Shops.BetterAnimalPurchase.Patches;

namespace QOLEssentials.Modules
{
	internal class BetterAnimalPurchaseModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply menus patches
				IClickableMenuPatch.Apply(harmony);
				PurchaseAnimalsMenuPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(BetterAnimalPurchaseModule)} module: {e}", LogLevel.Error);
				return;
			}

			// Subscribe to events
			ModEntry.Helper.Events.Input.ButtonPressed += ButtonPressedHandler.Apply;
		}
	}
}
