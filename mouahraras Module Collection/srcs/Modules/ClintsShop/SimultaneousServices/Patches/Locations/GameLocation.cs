﻿using HarmonyLib;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Menus;

namespace mouahrarasModuleCollection.ClintsShop.SimultaneousServices.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.blacksmith)),
				prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(BlacksmithPrefix))
			);
		}

		private static bool BlacksmithPrefix(GameLocation __instance, Location tileLocation, ref bool __result)
		{
			if (!ModEntry.Config.ClintsShopSimultaneousServices)
				return true;

			foreach (NPC character in __instance.characters)
			{
				if (!character.Name.Equals("Clint"))
					continue;
				if (!character.getTileLocation().Equals(new Vector2(tileLocation.X, tileLocation.Y - 1)))
					character.getTileLocation().Equals(new Vector2(tileLocation.X - 1, tileLocation.Y - 1));
				character.faceDirection(2);
				if (Game1.player.toolBeingUpgraded.Value != null && Game1.player.daysLeftForToolUpgrade.Value > 0)
				{
					if (!Game1.player.hasItemInInventory(535, 1) && !Game1.player.hasItemInInventory(536, 1) && !Game1.player.hasItemInInventory(537, 1) && !Game1.player.hasItemInInventory(749, 1) && !Game1.player.hasItemInInventory(275, 1) && !Game1.player.hasItemInInventory(791, 1))
					{
						Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithStock(), 0, "Clint");
					}
					else
					{
						Response[] answerChoices = new Response[3]
						{
							new("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
							new("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
							new("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
						};
						__instance.createQuestionDialogue("", answerChoices, "Blacksmith");
					};
					__result = true;
					return false;
				}
				__result = true;
				return true;
			}
			__result = false;
			return false;
		}
	}
}
