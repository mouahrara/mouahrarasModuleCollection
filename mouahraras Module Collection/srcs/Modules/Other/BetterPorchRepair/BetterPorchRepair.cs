﻿using System;
using HarmonyLib;
using StardewModdingAPI;
using mouahrarasModuleCollection.Other.BetterPorchRepair.Handlers;
using mouahrarasModuleCollection.Other.BetterPorchRepair.Patches;

namespace mouahrarasModuleCollection.Modules
{
	internal class BetterPorchRepairModule
	{
		internal static void Apply(Harmony harmony)
		{
			// Load Harmony patches
			try
			{
				// Apply locations patches
				GameLocationPatch.Apply(harmony);
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"Issue with Harmony patching of the {typeof(PayToPlayModule)} module: {e}", LogLevel.Error);
				return;
			}

			// Subscribe to events
			ModEntry.Helper.Events.GameLoop.DayStarted += DayStartedHandler.Apply;
			ModEntry.Helper.Events.Content.AssetRequested += AssetRequestedHandler.Apply;
		}
	}
}
