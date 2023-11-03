using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using StardewValley.Minigames;

namespace mouahrarasModuleCollection.ArcadeGames.NotRealisticLeaderboard.Patches
{
	internal class NetLeaderboardsPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(NetLeaderboards), nameof(NetLeaderboards.GetScores)),
				prefix: new HarmonyMethod(typeof(NetLeaderboardsPatch), nameof(GetScoresPrefix))
			);
		}

		private static bool IsCalledFromMineCart()
		{
			IEnumerable<System.Type> callingMethods = new System.Diagnostics.StackTrace().GetFrames()
				.Select(frame => frame.GetMethod())
				.Where(method => method != null)
				.Select(method => method.DeclaringType);

			return callingMethods.Any(type => type == typeof(MineCart));
		}

		private static bool GetScoresPrefix(NetLeaderboards __instance, ref List<KeyValuePair<string, int>> __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayNotRealisticLeaderboard || !IsCalledFromMineCart())
				return true;

			__result = new()
            {
                new KeyValuePair<string, int>("Lewis", 50000),
                new KeyValuePair<string, int>("Shane", 25000),
                new KeyValuePair<string, int>("Sam", 10000),
                new KeyValuePair<string, int>("Abigail", 5000),
                new KeyValuePair<string, int>("Vincent", 250)
            };

			foreach (NetLeaderboardsEntry entry in __instance.entries)
			{
				__result.Add(new KeyValuePair<string, int>(entry.name.Value, entry.score.Value));
			}

			__result.Sort((KeyValuePair<string, int> a, KeyValuePair<string, int> b) => a.Value.CompareTo(b.Value));
			__result.Reverse();

			for (int i = 0; i < __result.Count; i++)
			{
				bool isDuplicate = false;

				for (int j = 0; j < i; j++)
				{
					if (__result[i].Key == __result[j].Key)
					{
						isDuplicate = true;
						break;
					}
				}

				if (isDuplicate)
				{
					__result.RemoveAt(i);
					i--;
				}
			}
			return false;
		}
	}
}
