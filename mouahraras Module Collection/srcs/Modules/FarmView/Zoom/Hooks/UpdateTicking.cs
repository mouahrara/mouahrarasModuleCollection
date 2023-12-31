﻿using StardewModdingAPI.Events;
using mouahrarasModuleCollection.FarmView.Zoom.Utilities;

namespace mouahrarasModuleCollection.FarmView.Zoom.Hooks
{
	internal static class UpdateTickingHook
	{
		/// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickingEventArgs e)
		{
			if (!ModEntry.Config.FarmViewZoom)
				return;
			if (ModEntry.Helper.Input.IsDown(ModEntry.Config.FarmViewZoomInKey))
			{
				ZoomUtility.AddZoomLevel(120);
				return;
			}
			if (ModEntry.Helper.Input.IsDown(ModEntry.Config.FarmViewZoomOutKey))
			{
				ZoomUtility.AddZoomLevel(-120);
				return;
			}
		}
	}
}
