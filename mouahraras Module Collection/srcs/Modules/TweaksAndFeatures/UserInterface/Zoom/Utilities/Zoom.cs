using StardewModdingAPI.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Utilities
{
	internal class ZoomUtility
	{
		private static readonly PerScreen<int>	zoomLevel = new(() => 0);
		private static readonly PerScreen<bool>	zoomLevelMinReached = new(() => false);

		internal static void Reset()
		{
			zoomLevel.Value = 0;
			zoomLevelMinReached.Value = false;
		}

		internal static int ZoomLevel
		{
			get => zoomLevel.Value;
			set => zoomLevel.Value = value;
		}

		internal static bool ZoomLevelMinReached
		{
			get => zoomLevelMinReached.Value;
			set => zoomLevelMinReached.Value = value;
		}

		internal static void AddZoomLevel(int direction)
		{
			if (direction < 0 && zoomLevelMinReached.Value)
				return;
			if (direction > 0 && zoomLevel.Value + direction > 0)
				return;
			zoomLevel.Value += direction;
		}
	}
}
