using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HighDeadlineSpawningFix.Hooks
{
    public static class RoundManagerHook
    {
        public static void PatchDaysUntilDeadlineAccess(ILContext il)
        {
            ILCursor c = new(il);
            int patched = 0;

            while (c.TryGotoNext(
                i => i.MatchLdfld<TimeOfDay>("daysUntilDeadline")
                ))
            {
                c.Remove();
                c.EmitDelegate<Func<TimeOfDay, int>>(ClampedDaysUntilDeadline);

                patched++;
            }

            HighDeadlineSpawningFix.Logger.LogInfo($"Patched {patched} occurence(s).");
        }

        private static int ClampedDaysUntilDeadline(TimeOfDay tod)
        {
            return HighDeadlineSpawningFix.ClampType.Value switch
            {
                DeadlineClampType.Clamp => Mathf.Min(HighDeadlineSpawningFix.ClampValue.Value, tod.daysUntilDeadline),
                DeadlineClampType.Modulo => tod.daysUntilDeadline % 4,
                _ => tod.daysUntilDeadline,
            };
        }
    }
}
