using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class ImprovedNeedIndicator: Mod
    {
        public ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack)
        {
            Harmony harmony = new Harmony("AmCh.ImprovedNeedIndicator");

            MethodInfo need_GetTipString = AccessTools.Method(typeof(Need), nameof(Need.GetTipString));
            harmony.Patch(
                need_GetTipString,
                postfix: new HarmonyMethod(typeof(ImprovedNeedIndicator), nameof(Need_Joy_Postfix))
            );
            harmony.Patch(
                need_GetTipString,
                postfix: new HarmonyMethod(typeof(ImprovedNeedIndicator), nameof(Need_Outdoors_Postfix))
            );

            harmony.Patch(
                need_GetTipString,
                postfix: new HarmonyMethod(typeof(ImprovedNeedIndicator), nameof(Need_Rest_Postfix))
            );

            MethodInfo need_Food_GetTipString = AccessTools.Method(typeof(Need_Food), nameof(Need_Food.GetTipString));
            harmony.Patch(
                need_Food_GetTipString,
                postfix: new HarmonyMethod(typeof(ImprovedNeedIndicator), nameof(Need_Food_Postfix))
            );
        }


        private static AddendumManager_Need cachedNeedManager;

        private static void Need_Food_Postfix(Need_Food __instance, ref string __result)
        {
            if (cachedNeedManager is null)
                cachedNeedManager = new AddendumManager_Need_Rate_Food(__instance);

            else if (cachedNeedManager.IsSameNeed(__instance) == false)
                cachedNeedManager = new AddendumManager_Need_Rate_Food(__instance);

            __result += cachedNeedManager.ToTip(
                Find.TickManager.TicksGame,
                INIKeyBindingDefOf.ShowDetails.IsDown
            );
        }

        private static void Need_Joy_Postfix(Need __instance, ref string __result)
        {
            if ((__instance is Need_Joy) == false)
                return;

            if (cachedNeedManager is null)
                cachedNeedManager = new AddendumManager_Need_Rate_Joy((Need_Joy)__instance);

            else if (cachedNeedManager.IsSameNeed(__instance) == false)
                cachedNeedManager = new AddendumManager_Need_Rate_Joy((Need_Joy)__instance);

            __result += cachedNeedManager.ToTip(
                Find.TickManager.TicksGame,
                INIKeyBindingDefOf.ShowDetails.IsDown
            );
        }

        private static void Need_Outdoors_Postfix(Need __instance, ref string __result)
        {
            if ((__instance is Need_Outdoors) == false)
                return;

            if (cachedNeedManager is null)
                cachedNeedManager = new AddendumManager_Need_Rate_Outdoors((Need_Outdoors)__instance);

            else if (cachedNeedManager.IsSameNeed(__instance) == false)
                cachedNeedManager = new AddendumManager_Need_Rate_Outdoors((Need_Outdoors)__instance);

            __result += cachedNeedManager.ToTip(
                Find.TickManager.TicksGame,
                INIKeyBindingDefOf.ShowDetails.IsDown
            );
        }

        private static void Need_Rest_Postfix(Need __instance, ref string __result)
        {
            if ((__instance is Need_Rest) == false)
                return;

            if (cachedNeedManager is null)
                cachedNeedManager = new AddendumManager_Need_Rate_Sleep((Need_Rest)__instance);

            else if (cachedNeedManager.IsSameNeed(__instance) == false)
                cachedNeedManager = new AddendumManager_Need_Rate_Sleep((Need_Rest)__instance);

            __result += cachedNeedManager.ToTip(
                Find.TickManager.TicksGame,
                INIKeyBindingDefOf.ShowDetails.IsDown
            );
        }
    }
}
