using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace Improved_Need_Indicator
{
    public class ImprovedNeedIndicator : Mod
    {
        public static readonly Harmony harmony
            = new Harmony(id: "AmCh.ImprovedNeedIndicator");
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
            // Skip if need type is not rest
            if (__instance is Need_Rest need)
                __result += Rest.ProcessNeed(need);
        }
    }
}
