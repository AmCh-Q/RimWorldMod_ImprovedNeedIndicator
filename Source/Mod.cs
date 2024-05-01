using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Improved_Need_Indicator
{
    public class Mod_ImprovedNeedIndicator: Mod
    {
        public Mod_ImprovedNeedIndicator(ModContentPack contentPack) : base(contentPack)
        {
            Harmony harmony = new Harmony("AmCh.ImprovedNeedIndicator");

            harmony.Patch(
                AccessTools.Method(typeof(Need), nameof(Need.GetTipString)),
                postfix: new HarmonyMethod(typeof(Mod_ImprovedNeedIndicator), nameof(Need_Postfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Need_Food), nameof(Need_Food.GetTipString)),
                postfix: new HarmonyMethod(typeof(Mod_ImprovedNeedIndicator), nameof(Need_Food_Postfix))
            );
        }


        private static AddendumManager_Need cachedNeedManager;

        private static void Need_Postfix(Need __instance, ref string __result)
        {
            // In this case we want to keep the cachedNeedManager. So we do nothing.
            if (
                (cachedNeedManager is null) == false
                && cachedNeedManager.IsSameNeed(__instance)
            ) { }

            else if (__instance is Need_Joy needJoy)
                cachedNeedManager = new AddendumManager_Need_Rate_Joy(needJoy);

            else if (__instance is Need_Outdoors needOutdoors)
                cachedNeedManager = new AddendumManager_Need_Rate_Outdoors(needOutdoors);

            else if (__instance is Need_Rest needRest)
                cachedNeedManager = new AddendumManager_Need_Rate_Rest(needRest);


            __result += cachedNeedManager.ToTip(
                Find.TickManager.TicksGame,
                INIKeyBindingDefOf.ShowDetails.IsDown
            );
        }

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
    }
}
