﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace QOLEssentials.Other.FestivalEndTime.Patches
{
	internal class EventPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
				transpiler: new HarmonyMethod(typeof(EventPatch), nameof(ExitEventTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> ExitEventTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldc_I4) && (list[i].operand.Equals(2200) || list[i].operand.Equals(2400)))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(list[i].opcode, list[i].operand),
							new(OpCodes.Call, typeof(EventPatch).GetMethod("GetFestivalEndTime", BindingFlags.NonPublic | BindingFlags.Static))
						};
						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveAt(i);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(Event)}.{nameof(Event.exitEvent)}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static int GetFestivalEndTime(Event __instance, int vanillaEndTime)
		{
			if (!ModEntry.Config.OtherFestivalEndTime || !__instance.isFestival)
				return vanillaEndTime;

			return Math.Min(2500, Convert.ToInt32(((Dictionary<string,string>)AccessTools.Field(typeof(Event), "festivalData").GetValue(__instance))["conditions"].Split('/')[1].Split(' ')[1]) + ModEntry.Config.OtherFestivalEndTimeAdditionalTime);
		}
	}
}
