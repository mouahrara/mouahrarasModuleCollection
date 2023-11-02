﻿using StardewModdingAPI.Events;
using mouahrarasModuleCollection.Utilities;

namespace mouahrarasModuleCollection.Hooks
{
	internal static class GameLaunchedHook
	{
		/// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, GameLaunchedEventArgs e)
		{
			// Initialize GMCM
			GMCMUtility.Initialize();

			// Load console commands
			ConsoleCommandsUtility.Load();
		}
	}
}
