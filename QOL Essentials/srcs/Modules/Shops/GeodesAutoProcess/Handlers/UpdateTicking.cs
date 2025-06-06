﻿using StardewModdingAPI.Events;
using QOLEssentials.Shops.GeodesAutoProcess.Utilities;

namespace QOLEssentials.Shops.GeodesAutoProcess.Handlers
{
	internal static class UpdateTickingHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickingEventArgs e)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			if (GeodesAutoProcessUtility.IsProcessing() && GeodesAutoProcessUtility.GeodeMenu.geodeAnimationTimer <= 0)
			{
				GeodesAutoProcessUtility.CrackGeodeSecure();
			}
		}
	}
}
