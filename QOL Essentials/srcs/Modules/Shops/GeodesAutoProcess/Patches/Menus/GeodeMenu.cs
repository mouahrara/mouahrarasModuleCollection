using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using QOLEssentials.Shops.GeodesAutoProcess.Utilities;

namespace QOLEssentials.Shops.GeodesAutoProcess.Patches
{
	internal class GeodeMenuPatch
	{
		private const int							region_stopButton = 4321;
		internal static ClickableTextureComponent	stopButton;
		private static int							i;

		internal static void Apply(Harmony harmony)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
					postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(DrawPostfixDescriptionText))
				);
			}
			harmony.Patch(
				original: AccessTools.Constructor(typeof(GeodeMenu)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(GeodeMenuPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(DrawPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.performHoverAction), new Type[] { typeof(int), typeof(int) }),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(PerformHoverActionPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReceiveLeftClickPrefix)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReceiveLeftClickPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.update), new Type[] { typeof(GameTime) }),
				prefix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(Updateprefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.gameWindowSizeChanged)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(GameWindowSizeChangedPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.readyToClose)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(ReadyToClosePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.emergencyShutDown)),
				postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(EmergencyShutDownPostfix))
			);
		}

		private static void DrawPostfixDescriptionText(GeodeMenu __instance, SpriteBatch b)
		{
			if (__instance.alertTimer > 0)
			{
				Rectangle infoBox = (Rectangle)typeof(GeodeMenu).GetField("infoBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

				typeof(Utility).GetMethod("drawMultiLineTextWithShadow", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { b, __instance.descriptionText, Game1.smallFont, new Vector2(infoBox.X + 32, infoBox.Y + 32), infoBox.Width - 64, infoBox.Height - 64, Game1.textColor, true, true, true, true, false, false, 1f });
			}
		}

		private static void GeodeMenuPostfix(GeodeMenu __instance)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			GeodesAutoProcessUtility.InitializeAfterOpeningGeodeMenu(__instance);

			int width = 30 + StopButtonUtility.GetStopButtonWidthOffset();
			int height = 13;
			Vector2 position = StopButtonUtility.GetStopButtonPosition(__instance, width, height);

			__instance.geodeSpot.myID = GeodeMenu.region_geodeSpot;
			stopButton = new ClickableTextureComponent(null, new Rectangle((int)position.X, (int)position.Y, width * 4, height * 4), null, null, Game1.mouseCursors, new Rectangle(441, 411, width, height), 4f)
			{
				myID = region_stopButton,
				rightNeighborID = -1,
				leftNeighborID = -1,
				visible = true
			};
			__instance.trashCan.myID = GeodeMenu.region_trashCan;
			__instance.okButton.myID = GeodeMenu.region_okButton;
			__instance.geodeSpot.leftNeighborID = -1;
			__instance.geodeSpot.rightNeighborID = stopButton.myID;
			stopButton.leftNeighborID = __instance.geodeSpot.myID;
			stopButton.rightNeighborID = __instance.trashCan.myID;
			__instance.trashCan.leftNeighborID = stopButton.myID;
			__instance.trashCan.rightNeighborID = -1;
			__instance.trashCan.upNeighborID = -1;
			__instance.trashCan.downNeighborID = __instance.okButton.myID;
			__instance.okButton.upNeighborID = __instance.trashCan.myID;
			__instance.okButton.downNeighborID = -1;
			if (__instance.inventory.inventory is not null && __instance.inventory.inventory.Count >= 12)
			{
				stopButton.downNeighborID = 0;
				for (int i = 9; i < 12; i++)
				{
					if (__instance.inventory.inventory[i] is not null)
					{
						__instance.inventory.inventory[i].upNeighborID = stopButton.myID;
					}
				}
			}
		}

		private static void DrawPostfix(GeodeMenu __instance, SpriteBatch b)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			b.Draw(stopButton.texture, stopButton.getVector2(), stopButton.sourceRect, Color.White * (GeodesAutoProcessUtility.IsProcessing() ? 1f : 0.5f), 0.0f, Vector2.Zero, stopButton.scale, SpriteEffects.None, (float)(0.860000014305115 + stopButton.bounds.Y / 20000.0));
			__instance.drawMouse(b);
		}

		private static void PerformHoverActionPostfix(int x, int y)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			stopButton.tryHover(x, y);
		}

		private static bool ReceiveLeftClickPrefix(GeodeMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess || __instance.waitingForServerResponse || !__instance.geodeSpot.containsPoint(x, y))
				return true;

			Item heldItem = Constants.TargetPlatform == GamePlatform.Android ? (Item)typeof(MenuWithInventory).GetField("heldItem").GetValue(__instance) : __instance.heldItem;

			if (heldItem is not null && Utility.IsGeode(heldItem) && Game1.player.Money >= 25 && __instance.geodeAnimationTimer <= 0)
			{
				if (!GeodesAutoProcessUtility.IsInventoryFullForGeodeProcessing(Game1.player, heldItem))
				{
					GeodesAutoProcessUtility.StartGeodeProcessing();
					return false;
				}
			}
			return true;
		}

		private static void ReceiveLeftClickPostfix(GeodeMenu __instance, int x, int y)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess || __instance.waitingForServerResponse || !GeodesAutoProcessUtility.IsProcessing())
				return;

			if (stopButton.containsPoint(x, y))
			{
				GeodesAutoProcessUtility.EndGeodeProcessing();
			}
		}

		private static bool Updateprefix(GeodeMenu __instance, GameTime time)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess || __instance.geodeAnimationTimer <= 0)
				return true;

			if (i < ModEntry.Config.ShopsGeodesAutoProcessSpeedMultiplier)
			{
				i++;
				__instance.update(time);
				return true;
			}
			i = 0;
			return false;
		}

		private static void GameWindowSizeChangedPostfix(GeodeMenu __instance)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			int width = 30 + StopButtonUtility.GetStopButtonWidthOffset();
			int height = 13;

			stopButton.setPosition(StopButtonUtility.GetStopButtonPosition(__instance, width, height));
		}

		private static void ReadyToClosePostfix(ref bool __result)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			if (GeodesAutoProcessUtility.IsProcessing())
			{
				__result = false;
			}
		}

		private static void EmergencyShutDownPostfix()
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			GeodesAutoProcessUtility.CleanBeforeClosingGeodeMenu();
		}
	}
}
