﻿using HarmonyLib;
using StardewModdingAPI;
using QOLEssentials.Sections;
using QOLEssentials.Handlers;
using QOLEssentials.Utilities;

namespace QOLEssentials
{
	/// <summary>The mod entry point.</summary>
	internal sealed class ModEntry : Mod
	{
		// Shared static helpers
		internal static new IModHelper	Helper { get; private set; }
		internal static new IMonitor	Monitor { get; private set; }
		internal static new IManifest	ModManifest { get; private set; }
		internal static ModConfig		Config;

		public override void Entry(IModHelper helper)
		{
			Helper = base.Helper;
			Monitor = base.Monitor;
			ModManifest = base.ModManifest;

			Harmony harmony = new(ModManifest.UniqueID);

			// Apply sections
			ArcadeGamesSection.Apply(harmony);
			MachinesSection.Apply(harmony);
			ShopsSection.Apply(harmony);
			UserInterfaceSection.Apply(harmony);
			OtherSection.Apply(harmony);

			// Subscribe to events
			Helper.Events.GameLoop.GameLaunched += GameLaunchedHandler.Apply;
		}
	}
}
