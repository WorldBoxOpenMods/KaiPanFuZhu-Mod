using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Diplomacy_Army;
using HarmonyLib;
using UnityEngine;

namespace Diplomacy_Army
{
    public class harmony_vassal
    {
        #region 附庸机制
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClanManager), "checkActionKing")]
        public static bool checkActionKing(ClanManager __instance, Actor pActor)
        {
            if (pActor.isFighting())
            {
                return false;
            }


            if (CheckVassal(pActor.kingdom))
            {
                return false;
            }
            if (harmony_declare.tryPlotDeclareWar(pActor, AssetManager.plots_library.get("new_declare_war")))
            {
                harmony_declare._timestamp_last_plot(__instance);
                return false;
            }
            return true;
        }

        public static bool CheckVassal(Kingdom kingdom)
        {
            kingdom.data.get("Vassal", out bool flag, false);

            if (flag)
            {
                kingdom.data.get("suzerainID", out string str, "");
                Kingdom suzerain = World.world.kingdoms.getKingdomByID(str);

                if (suzerain != null)
                {
                    if (!MoreGodPower.Vassals.ContainsKey(suzerain))
                    {
                        MoreGodPower.Vassals.Add(suzerain, new List<Kingdom> { kingdom });
                    }
                    else if (!MoreGodPower.Vassals[suzerain].Contains(kingdom))
                    {
                        MoreGodPower.Vassals[suzerain].Add(kingdom);
                    }
                    return true;
                }
            }

            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(DiplomacyManager), "startWar")]
        public static bool startWar_Prefix(Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pAsset, bool pLog = true)
        {
            if (CheckVassal(pAttacker)) return false;
            if (MoreGodPower.Vassals.ContainsKey(pAttacker) && MoreGodPower.Vassals[pAttacker].Contains(pDefender))
            {
                // if (Toolbox.randomChance(0.99f))
                // {
                MoreGodPower.Vassals[pAttacker].Remove(pDefender);
                pDefender.data.set("Vassal", false);
                pDefender.data.set("suzerainID", "");
                NewFunction.LogNewMessage(pAttacker, pDefender, "", "发动", "对附庸的吞并战争");
                return false;
                // }

                // MoreGodPower.DefenceKingdoms[pDefender].Remove(pAttacker);

            }
            if (MoreGodPower.AllianceKingdoms.ContainsKey(pAttacker) && MoreGodPower.AllianceKingdoms[pAttacker].ContainsKey(pDefender))
            {
                if (Toolbox.randomChance(0.2f))
                {
                    return false;
                }
                NewFunction.LogNewMessage(pAttacker, pDefender, "", "撕毁了与", "签订了互不侵犯条约");
                return false;
            }
            if (MoreGodPower.DefenceKingdoms.ContainsKey(pAttacker) && MoreGodPower.DefenceKingdoms[pAttacker].ContainsKey(pDefender))
            {
                MoreGodPower.DefenceKingdoms[pAttacker].Remove(pDefender);
                MoreGodPower.DefenceKingdoms[pDefender].Remove(pAttacker);
                NewFunction.LogNewMessage(pAttacker, pDefender, "", "撕毁了与", "签订的共同防御条约");
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "findKingdomToJoinAfterCapture")]
        public static bool findKingdomToJoinAfterCapture(City __instance, Kingdom pKingdom, ListPool<War> pWars, ref Kingdom __result)
        {
            Kingdom kingdom = null;
            War war1 = null;
            for (int i = 0; i < pWars.Count; i++)
            {
                War war = pWars[i];
                if (!war.getAsset().total_war && war.hasKingdom(__instance.kingdom) && war.isInWarWith(pKingdom, __instance.kingdom))
                {
                    if (war.main_attacker == pKingdom || war.main_defender == pKingdom)
                    {
                        war1 = war;
                        break;
                    }
                    if (war.isAttacker(__instance.kingdom) && war.main_defender != null)
                    {
                        if (__instance.neighbours_kingdoms.Contains(war.main_defender))
                        {
                            kingdom = war.main_defender;
                            war1 = war;
                            break;
                        }
                        if (__instance.neighbours_kingdoms.Contains(pKingdom))
                        {
                            kingdom = pKingdom;
                            war1 = war;
                            break;
                        }
                        kingdom = war.main_defender;
                        break;
                    }
                    else if (war.isDefender(__instance.kingdom) && war.main_attacker != null)
                    {
                        if (__instance.neighbours_kingdoms.Contains(war.main_attacker))
                        {
                            kingdom = war.main_attacker;
                            war1 = war;
                            break;
                        }
                        if (__instance.neighbours_kingdoms.Contains(pKingdom))
                        {
                            kingdom = pKingdom;
                            war1 = war;
                            break;
                        }
                        kingdom = war.main_attacker;
                        war1 = war;
                        break;
                    }
                }
            }
            if (kingdom == null)
            {
                kingdom = pKingdom;
            }
            else if (kingdom.race != __instance.kingdom.race)
            {
                kingdom = pKingdom;
            }
            if (CheckVassal(kingdom))
            {
                kingdom.data.get("suzerainID", out string str, "");
                Kingdom pKingdom2 = null;
                if (str != "")
                { pKingdom2 = World.world.kingdoms.getKingdomByID(str); }
                if (pKingdom2 != null)
                {
                    kingdom = pKingdom2;
                }
            }
            if(war1._asset==AssetManager.war_types_library.get("Declare"))
            {
                if(war1.isInWarWith(pKingdom, __instance.kingdom)&&MoreGodPower.Declares.ContainsKey(pKingdom))
                {
                    int num=0;
                    foreach(var city in MoreGodPower.Declares[pKingdom])
                    {
                        if(city.kingdom!=pKingdom&&city.kingdom==__instance.kingdom)
                        {
                            num++;
                        }
                    }
                    if(num<=1)
                    {
                        MoreGodPower.endWar(pKingdom, __instance.kingdom);
                    }
                }
            }
            __result = kingdom;
            return false;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiplomacyManager), "startWar")]
        public static void startWar_Postfix(Kingdom pAttacker, Kingdom pDefender, WarTypeAsset pAsset, bool pLog = true)
        {
            if (pAttacker == null || pDefender == null)
            {
                return;
            }
            War war;
            if (World.world.wars.getWar(pAttacker, pDefender, true) != null)
            {
                war = World.world.wars.getWar(pAttacker, pDefender, true);
            }
            else { war = World.world.wars.newWar(pAttacker, pDefender, pAsset); }
            if (MoreGodPower.DefenceKingdoms.ContainsKey(pDefender) && MoreGodPower.DefenceKingdoms[pDefender].Count > 0)
            {
                foreach (var item in MoreGodPower.DefenceKingdoms[pDefender])
                {
                    if (item.Key != null && item.Key.data != null)
                    {
                        war.joinDefenders(item.Key);
                    }
                }
            }
            if (MoreGodPower.Vassals.ContainsKey(pAttacker) && MoreGodPower.Vassals[pAttacker].Count > 0)
            {
                foreach (var item in MoreGodPower.Vassals[pAttacker])
                {
                    if (item != null && item.data != null)
                    {
                        war.joinAttackers(item);
                    }
                }
            }
            else if (MoreGodPower.Vassals.ContainsKey(pDefender) && MoreGodPower.Vassals[pDefender].Count > 0)
            {
                foreach (var item in MoreGodPower.Vassals[pDefender])
                {
                    if (item != null && item.data != null)
                    {
                        war.joinDefenders(item);
                    }
                }
            }
            pDefender.data.get("Vassal", out bool flag, false);
            if (CheckVassal(pDefender))
            {
                pDefender.data.get("suzerainID", out string str, "");
                Kingdom pKingdom = World.world.kingdoms.getKingdomByID(str);
                float num = pKingdom.king.stats[S.personality_aggression];
                if (pKingdom.power > pAttacker.power)
                {
                    num = 1f;
                }
                if (pKingdom.power < pAttacker.power)
                {
                    num = 0.8f;
                }
                if (Toolbox.randomChance(num))
                {
                    war.joinDefenders(pDefender);
                }
            }

        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiplomacyManager), "findSupremeKingdom")]
        public static void findSupremeKingdom_Postfix(DiplomacyManager __instance)
        {
            DiplomacyManager.kingdom_supreme = null;
            if (World.world.kingdoms.list_civs.Count == 0)
            {
                return;
            }
            List<Kingdom> list_civs = World.world.kingdoms.list_civs;
            for (int i = 0; i < list_civs.Count; i++)
            {
                Kingdom kingdom = list_civs[i];
                kingdom.power = kingdom.getArmy() * 2 + kingdom.cities.Count * 5 + 1;

                kingdom.data.get("suzerain", out bool flag, false);
                if (flag && MoreGodPower.Vassals.ContainsKey(kingdom) && MoreGodPower.Vassals[kingdom].Count > 0)
                {
                    foreach (var item in MoreGodPower.Vassals[kingdom])
                    {
                        if (item != null && item.data != null)
                        {
                            kingdom.power += (int)(item.power * 0.2);
                            item.power -= (int)(item.power * 0.3);
                        }
                    }
                }
            }
            list_civs.Sort(new Comparison<Kingdom>(__instance.sortByPower));
            DiplomacyManager.kingdom_supreme = list_civs[0];
            if (list_civs.Count > 1)
            {
                DiplomacyManager.kingdom_second = list_civs[1];
                return;
            }
            DiplomacyManager.kingdom_second = null;
        }
        #endregion
    }
}