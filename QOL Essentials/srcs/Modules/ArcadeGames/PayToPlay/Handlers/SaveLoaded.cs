﻿using StardewModdingAPI.Events;
using QOLEssentials.ArcadeGames.PayToPlay.Managers;

namespace QOLEssentials.ArcadeGames.PayToPlay.Handlers
{
	internal static class SaveLoadedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, SaveLoadedEventArgs e)
		{
			// Load assets
			AssetManager.Apply();
		}
	}
}
