﻿using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Patches
{
	internal class OptionsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.PropertyGetter(typeof(Options), nameof(Options.zoomLevel)),
				postfix: new HarmonyMethod(typeof(OptionsPatch), nameof(ZoomLevelPostfix))
			);
		}

		private static void ZoomLevelPostfix(ref float __result)
		{
			if (!Context.IsWorldReady || !ModEntry.Config.UserInterfaceZoom)
				return;
			if (Game1.activeClickableMenu is not CarpenterMenu && Game1.activeClickableMenu is not PurchaseAnimalsMenu && Game1.activeClickableMenu is not AnimalQueryMenu)
				return;
			if (Game1.activeClickableMenu is CarpenterMenu && (Game1.activeClickableMenu as CarpenterMenu).freeze)
				return;
			if (Game1.activeClickableMenu is PurchaseAnimalsMenu && (Game1.activeClickableMenu as PurchaseAnimalsMenu).freeze)
				return;
			if (!Game1.activeClickableMenu.shouldClampGamePadCursor())
				return;

			float nextZoomLevel = __result + ZoomUtility.ZoomLevel * ModEntry.Config.UserInterfaceZoomMultiplier / 8000f;
			bool viewportWidthHasOrWillOverflowMapWidth;
			bool viewportHeightHasOrWillOverflowMapHeight;

			if (nextZoomLevel <= 0)
			{
				viewportWidthHasOrWillOverflowMapWidth = true;
				viewportHeightHasOrWillOverflowMapHeight = true;
			}
			else
			{
				int nextViewportWidth = (int) Math.Ceiling(Game1.game1.localMultiplayerWindow.Width * (1.0 / (double) nextZoomLevel));
				int nextViewportHeight = (int) Math.Ceiling(Game1.game1.localMultiplayerWindow.Height * (1.0 / (double) nextZoomLevel));

				viewportWidthHasOrWillOverflowMapWidth = Game1.viewport.Size.Width > Game1.currentLocation.Map.DisplayWidth || nextViewportWidth > Game1.currentLocation.Map.DisplayWidth;
				viewportHeightHasOrWillOverflowMapHeight = Game1.viewport.Size.Height > Game1.currentLocation.Map.DisplayHeight || nextViewportHeight > Game1.currentLocation.Map.DisplayHeight;
			}

			if (viewportWidthHasOrWillOverflowMapWidth || viewportHeightHasOrWillOverflowMapHeight)
			{
				float zoomLevelBasedOnMaxViewportWidth = (float) Game1.game1.localMultiplayerWindow.Width / Game1.currentLocation.Map.DisplayWidth;
				float zoomLevelBasedOnMaxViewportHeight = (float) Game1.game1.localMultiplayerWindow.Height / Game1.currentLocation.Map.DisplayHeight;

				if (viewportWidthHasOrWillOverflowMapWidth && viewportHeightHasOrWillOverflowMapHeight)
				{
					__result = Math.Min(__result, Math.Max(zoomLevelBasedOnMaxViewportWidth, zoomLevelBasedOnMaxViewportHeight));
				}
				else
				{
					if (viewportWidthHasOrWillOverflowMapWidth)
						__result = Math.Min(__result, zoomLevelBasedOnMaxViewportWidth);
					else
						__result = Math.Min(__result, zoomLevelBasedOnMaxViewportHeight);
				}
				ZoomUtility.ZoomLevelMinReached = true;
			}
			else
			{
				__result = nextZoomLevel;
				ZoomUtility.ZoomLevelMinReached = false;
			}
			Game1.clampViewportToGameMap();
			Game1.game1.refreshWindowSettings();
		}
	}
}
