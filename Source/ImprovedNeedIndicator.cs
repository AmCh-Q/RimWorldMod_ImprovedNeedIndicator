using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace Improved_Need_Indicator
{
    public class ImprovedNeedIndicator : Mod
    {
        private static readonly FieldInfo
            f_pawn = typeof(Need).GetField("pawn", Utility.flags);

        public static readonly Harmony
            harmony = new Harmony(id: "AmCh.ImprovedNeedIndicator");

        private delegate void ActionRef_r2<T1, T2>(T1 t1, ref T2 t2);

        private static readonly MethodBase
            original = typeof(Need).GetMethod(nameof(Need.GetTipString), Utility.flags);

        private static readonly HarmonyMethod
            postfix = new HarmonyMethod(((ActionRef_r2<Need, string>)Postfix).Method);

        public ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack) 
        {
            harmony.Patch(original, postfix: postfix);
        }

        private static void Postfix(Need __instance, ref string __result)
        {
            Pawn pawn = (Pawn)f_pawn.GetValue(__instance);
            int tickNow = Find.TickManager.TicksGame;

            // Skip if need type is not supported yet
            if (__instance is Need_Food)
                __result += Food.ProcessNeed(pawn, (Need_Food) __instance, tickNow);
            else if (__instance is Need_Rest)
                __result += Rest.ProcessNeed(pawn, (Need_Rest) __instance, tickNow);
        }
    }
}
