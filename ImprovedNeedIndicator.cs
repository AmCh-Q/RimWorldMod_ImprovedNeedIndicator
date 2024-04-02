using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class ImprovedNeedIndicator : Mod
    {
        const BindingFlags flags 
            = BindingFlags.Instance 
            | BindingFlags.Static 
            | BindingFlags.NonPublic 
            | BindingFlags.Public;
        private static readonly Harmony harmony 
            = new Harmony(id: "AmCh.ImprovedNeedIndicator");
        private static readonly FieldInfo
            f_lastRestEffectiveness 
            = typeof(Need_Rest).GetField("lastRestEffectiveness", flags),
            f_pawn
            = typeof(Need).GetField("pawn", flags);
        private static void Postfix(Need __instance, ref string __result)
        {
            if (!(__instance is Need_Rest need))
                return;
            float needChange, secondsTo;
            if (need.Resting)
            {
                needChange = 0.0022857144f
                    * (float)f_lastRestEffectiveness.GetValue(need)
                    * ((Pawn)f_pawn.GetValue(need)).GetStatValue(StatDefOf.RestRateMultiplier);
                secondsTo = (1f - need.CurLevel) / needChange;
                if (secondsTo > 0f)
                    __result = string.Concat(__result, "\n", 
                        secondsTo, " seconds to 100% Rest");
            }
            else
            {
                needChange = 60f
                    * need.RestFallPerTick
                    * ((Pawn)f_pawn.GetValue(need)).GetStatValue(StatDefOf.RestFallRateFactor);
                secondsTo = need.CurLevel / needChange;
                if (secondsTo > 0f)
                    __result = string.Concat(__result, "\n", 
                        secondsTo, " seconds to Exhaustion");
            }
        }
        private delegate void ActionRef_r2<T1, T2>(T1 t1, ref T2 t2);
        private static readonly MethodBase
            original = typeof(Need).GetMethod(nameof(Need.GetTipString), flags);
        private static readonly HarmonyMethod
            postfix = new HarmonyMethod(((ActionRef_r2<Need, string>)Postfix).Method);
        public ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack) 
        {
            harmony.Patch(original, postfix: postfix);
        }
    }
}
