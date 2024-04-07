using System.Reflection;
using HarmonyLib;
using RimWorld;
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
        private static readonly MethodBase
            original_Need_Food = typeof(Need_Food).GetMethod(nameof(Need_Food.GetTipString), Utility.flags);

        private static readonly HarmonyMethod
            postfix = new HarmonyMethod(((ActionRef_r2<Need, string>)Postfix).Method);
        private static readonly HarmonyMethod
            postfix_Need_Food = new HarmonyMethod(((ActionRef_r2<Need_Food, string>)Postfix_Need_Food).Method);

        public ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack) 
        {
            harmony.Patch(original, postfix: postfix);
            harmony.Patch(original_Need_Food, postfix: postfix_Need_Food);
        }

        private static void Postfix(Need __instance, ref string __result)
        {
            Pawn pawn = (Pawn)f_pawn.GetValue(__instance);
            int tickNow = Find.TickManager.TicksGame;

            // Skip if need type is not supported yet
            if (__instance is Need_Rest need_rest)
                __result += Rest.ProcessNeed(pawn, need_rest, tickNow);
            else if (__instance is Need_Joy need_joy)
                __result += Joy.ProcessNeed(pawn, need_joy, tickNow);
        }

        private static void Postfix_Need_Food(Need_Food __instance, ref string __result)
        {
            Pawn pawn = (Pawn)f_pawn.GetValue(__instance);
            int tickNow = Find.TickManager.TicksGame;
            __result += "\nThis is to test that the postfix worked, remove this.\n";
            __result += Food.ProcessNeed(pawn, __instance, tickNow);
        }
    }
}
