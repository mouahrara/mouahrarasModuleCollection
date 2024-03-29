using System.Reflection;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.FastScrolling.Utilities
{
	internal class MenusPatchUtility
	{
		internal static void ReceiveKeyPressPostfix(IClickableMenu __instance, Keys key)
		{
			if (!ModEntry.Config.UserInterfaceFastScrolling)
				return;
			if (__instance is CarpenterMenu && (__instance as CarpenterMenu).freeze)
				return;
			if (__instance is PurchaseAnimalsMenu && (__instance as PurchaseAnimalsMenu).freeze)
				return;
			if (!__instance.overrideSnappyMenuCursorMovementBan())
				return;
			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && __instance.readyToClose() && Game1.locationRequest == null)
				return;
			if (Game1.options.SnappyMenus)
				return;

			int offset = 2 * (int)(ModEntry.Config.UserInterfaceFastScrollingMultiplier - 1) * 4;

			if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
				Game1.panScreen(-offset, 0);
			else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
				Game1.panScreen(offset, 0);
			else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
				Game1.panScreen(0, -offset);
			else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
				Game1.panScreen(0, offset);
		}

		internal static void UpdatePostfix(IClickableMenu __instance)
		{
			if (!ModEntry.Config.UserInterfaceFastScrolling)
				return;
			if (!__instance.overrideSnappyMenuCursorMovementBan())
				return;
			if (Game1.IsFading())
				return;

			int x = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
			int y = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
			int offset = 2 * (int)(ModEntry.Config.UserInterfaceFastScrollingMultiplier - 1) * 4;

			if (x - Game1.viewport.X < 64)
				Game1.panScreen(-offset, 0);
			else if (x - (Game1.viewport.X + Game1.viewport.Width) >= -128)
				Game1.panScreen(offset, 0);
			if (y - Game1.viewport.Y < 64)
				Game1.panScreen(0, -offset);
			else if (y - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
				Game1.panScreen(0, offset);
		}
	}
}
