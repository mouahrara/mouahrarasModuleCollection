﻿using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using QOLEssentials.UserInterface.Zoom.Utilities;

namespace QOLEssentials.UserInterface.Zoom.Patches
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
			if (!Context.IsWorldReady || !ModEntry.Config.UserInterfaceZoom || Game1.currentLocation is null || ZoomUtility.AdditionalZoom == 0)
				return;

			if (Game1.activeClickableMenu is CarpenterMenu or PurchaseAnimalsMenu or AnimalQueryMenu && Game1.activeClickableMenu.shouldClampGamePadCursor())
			{
				ClickableTextureComponent cancelButton = (Game1.activeClickableMenu as CarpenterMenu)?.cancelButton ?? (Game1.activeClickableMenu as PurchaseAnimalsMenu)?.okButton ?? (Game1.activeClickableMenu as AnimalQueryMenu)?.okButton;
				bool viewportWidthHasOrWillOverflowMapWidth;
				bool viewportHeightHasOrWillOverflowMapHeight;

				if (ZoomUtility.ZoomLevel <= 0)
				{
					viewportWidthHasOrWillOverflowMapWidth = true;
					viewportHeightHasOrWillOverflowMapHeight = true;
				}
				else
				{
					int nextViewportWidth = (int) Math.Ceiling(Game1.game1.localMultiplayerWindow.Width * (1.0 / (double) ZoomUtility.ZoomLevel));
					int nextViewportHeight = (int) Math.Ceiling(Game1.game1.localMultiplayerWindow.Height * (1.0 / (double) ZoomUtility.ZoomLevel));

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
						{
							__result = Math.Min(__result, zoomLevelBasedOnMaxViewportWidth);
						}
						else
						{
							__result = Math.Min(__result, zoomLevelBasedOnMaxViewportHeight);
						}
					}
					ZoomUtility.ZoomLevelMinReached = true;
				}
				else
				{
					__result = ZoomUtility.ZoomLevel;
					ZoomUtility.ZoomLevelMinReached = false;
				}
				cancelButton?.setPosition(new Vector2(Game1.uiViewport.Width - cancelButton.bounds.Width - 64, Game1.uiViewport.Height - cancelButton.bounds.Height - 64));
				Game1.clampViewportToGameMap();
				Game1.game1.refreshWindowSettings();
			}
		}
	}
}
