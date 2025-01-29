using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using NCMS.Utils;
using UnityEngine;

namespace Diplomacy_Army.HarmonySpace
{
    public class diplomacy
    {
                //禁止自主联盟功能
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClanManager), "tryPlotJoinAlliance")]
        public static bool tryPlotJoinAlliance_Prefix(Actor pActor, PlotAsset pPlotAsset, ref bool __result)
        {
            if (PowerButtons.GetToggleValue("禁止自主联盟"))
            {
                __result = false;
                return false;
            }
            //保守主义者的效果，30%概率不向他人联盟
            string personality = pActor.kingdom.king.s_personality.id;
            if (personality == "Conservatism")
            {
                if (Toolbox.randomChance(0.3f))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClanManager), "tryPlotNewAlliance")]
        public static bool tryPlotNewAlliance_Prefix(Actor pActor, PlotAsset pPlotAsset)
        {
            if (PowerButtons.GetToggleValue("禁止自主联盟"))
            {
                return false;
            }
            return true;
        }
    }
}