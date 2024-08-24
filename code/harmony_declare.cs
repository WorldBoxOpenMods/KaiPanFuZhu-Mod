using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace Diplomacy_Army
{
    public class harmony_declare
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WarManager), "newWar")]
        public static void warstart_Postfix(WarManager __instance, Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pType, ref War __result)
        {
            if (pAttacker != null && pDefender != null && pType != null)
            {
                if (pType.id == "reclaim")
                {

                    __result.data.name = $"{pAttacker}进攻在{pDefender}的宣称之战";
                }

            }
        }

        public static Kingdom getWarTarget(Kingdom pKingdom)
        {
            Kingdom kingdom = null;
            float num = 0f;
            pKingdom.data.get("DeclareCity", out bool flag);
            if (flag)
            {
                int num2 = pKingdom.getArmy();
                if (pKingdom.hasAlliance())
                {
                    num2 = pKingdom.getAlliance().countWarriors();
                }
                List<Kingdom> DeclareKingdoms = new();
                if (MoreGodPower.Declares.ContainsKey(pKingdom))
                {
                    foreach (var city in MoreGodPower.Declares[pKingdom])
                    {
                        if (city.kingdom != kingdom)
                        {
                            DeclareKingdoms.Add(city.kingdom);
                        }
                    }
                }
                if (DeclareKingdoms.Count > 0)
                {
                    foreach (Kingdom kingdom2 in DeclareKingdoms)
                    {
                        if (kingdom2.cities.Count != 0 && kingdom2.capital != null && kingdom2.getAge() >= SimGlobals.m.minimum_kingdom_age_for_attack)
                        {
                            int num3;
                            if (kingdom2.hasAlliance())
                            {
                                num3 = kingdom2.getAlliance().countWarriors();
                            }
                            else
                            {
                                num3 = kingdom2.getArmy();
                            }
                            if (num2 >= num3 && pKingdom.capital.reachableFrom(kingdom2.capital))
                            {
                                DiplomacyRelation relation = DiplomacyHelpers.diplomacy.getRelation(pKingdom, kingdom2);
                                if (World.world.getYearsSince(relation.data.timestamp_last_war_ended) >= (float)SimGlobals.m.minimum_years_between_wars && !pKingdom.isOpinionTowardsKingdomGood(kingdom2))
                                {
                                    float num4 = Kingdom.distanceBetweenKingdom(pKingdom, kingdom2);
                                    if (kingdom == null || num4 < num)
                                    {
                                        num = num4;
                                        kingdom = kingdom2;
                                    }
                                }
                            }
                        }
                    }

                    return kingdom;
                }
            }
            return null;
        }
        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
        // public static void checkActionKing(ClanManager __instance, Actor pActor)
        // {
        //     if (pActor.isFighting())
        //     {
        //         return;
        //     }
        //     if (tryPlotDeclareWar(pActor, AssetManager.plots_library.get("new_declare_war")))
        //     {
        //         _timestamp_last_plot(__instance);
        //         return;
        //     }

        //     return;
        // }
        public static void _timestamp_last_plot(ClanManager __instance)
        {
            Traverse.Create(__instance).Field("_timestamp_last_plot").SetValue(World.world.getCurWorldTime());
        }
        public static bool tryPlotDeclareWar(Actor pActor, PlotAsset pPlotAsset)
        {
            if (!basePlotChecks(pActor, pPlotAsset))
            {
                return false;
            }

            Kingdom warTarget = getWarTarget(pActor.kingdom);
            if (warTarget == null)
            {
                return false;
            }
            // Debug.Log("tryPlotDeclareWar!");
            Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
            plot.rememberInitiators(pActor);
            plot.target_kingdom = warTarget;

            if (!plot.checkInitiatorAndTargets())
            {
                Debug.Log("tryPlotDeclareWar is missing start requirements");
                return false;
            }

            return true;
        }
        private static bool basePlotChecks(Actor pActor, PlotAsset pPlotAsset)
        {
            if (pActor == null || pPlotAsset == null)
            {
                Debug.Log("null!");
                return false;
            }
            return pActor.getInfluence() >= pPlotAsset.cost && pPlotAsset.checkInitiatorPossible(pActor) && pPlotAsset.check_launch(pActor, pActor.kingdom);

        }

        public static bool CheckDeclare(City city)
        {
            city.data.get("Declare", out bool flag);

            if (flag)
            {
                city.data.get("DeclareKingdomID", out string str, "");
                Kingdom kingdom = World.world.kingdoms.getKingdomByID(str);

                if (kingdom != null)
                {
                    if (!MoreGodPower.Declares.ContainsKey(kingdom))
                    {
                        MoreGodPower.Declares.Add(kingdom, new List<City> { city });
                    }
                    else if (!MoreGodPower.Declares[kingdom].Contains(city))
                    {
                        MoreGodPower.Declares[kingdom].Add(city);
                    }
                    return true;
                }
            }

            return false;
        }
    }
}