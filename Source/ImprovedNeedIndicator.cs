using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class ImprovedNeedIndicator : Mod
    {
        public static readonly Harmony
            harmony = new Harmony(id: "AmCh.ImprovedNeedIndicator");

        private delegate void ActionRef_r2<T1, T2>(T1 t1, ref T2 t2);


        // Need postfix
        // The majority of needs fall into this category.
        private static readonly MethodBase
            originalNeed = typeof(Need).GetMethod(nameof(Need.GetTipString), Utility.flags);
        private static readonly HarmonyMethod
            postfixNeed = new HarmonyMethod(((ActionRef_r2<Need, string>)Postfix_Need).Method);


        // Need_Food postfix
        // Need_Food overrides GetTipString, so we have to patch it separately.
        private static readonly MethodBase
            originalNeedFood = typeof(Need_Food).GetMethod(nameof(Need_Food.GetTipString), Utility.flags);
        private static readonly HarmonyMethod
            postfixNeedFood = new HarmonyMethod(((ActionRef_r2<Need_Food, string>)Postfix_Need_Food).Method);


        public ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack) 
        {
            harmony.Patch(originalNeed, postfix: postfixNeed);
            harmony.Patch(originalNeedFood, postfix: postfixNeedFood);
        }

        private static void Postfix_Need(Need __instance, ref string __result)
        {
            // Skip if need type is not supported yet
            if (__instance is Need_Rest)
                __result += AddendumProcessor.GetTipAddendum(__instance);
            else if (__instance is Need_Joy)
                __result += AddendumProcessor.GetTipAddendum(__instance);
        }

        private static void Postfix_Need_Food(Need_Food __instance, ref string __result)
        {
            __result += AddendumProcessor.GetTipAddendum(__instance);
        }
    }
}
