using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;

namespace Diplomacy_Army
{
    public class harmony_saves
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveManager), "loadWorld", new Type[] { typeof(string), typeof(bool) })]
        public static void LoadWorld(string pPath)
        {
            MoreGodPower.Declares = new Dictionary<Kingdom, List<City>>();
            MoreGodPower.Vassals = new Dictionary<Kingdom, List<Kingdom>>();
            if (World.world.kingdoms.list_civs.Count == 0)
            {
                return;
            }
            List<Kingdom> list_civs = World.world.kingdoms.list_civs;
            for (int i = 0; i < list_civs.Count; i++)
            {
                Kingdom kingdom = list_civs[i];


                if (!MoreGodPower.Vassals.ContainsKey(kingdom))
                {
                    kingdom.data.get("suzerain", out bool flag, false);
                    if (flag)
                    {
                        MoreGodPower.Vassals.Add(kingdom, null);
                    }
                    harmony_vassal.CheckVassal(kingdom);
                }
                foreach(City city in kingdom.cities)
                {
                    harmony_declare.CheckDeclare(city);
                }
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveManager), "loadMapFromResources")]
        public static void LoadMapFromResources(string pPath)
        {
            MoreGodPower.Declares = new Dictionary<Kingdom, List<City>>();
            MoreGodPower.Vassals = new Dictionary<Kingdom, List<Kingdom>>();
            if (World.world.kingdoms.list_civs.Count == 0)
            {
                return;
            }
            List<Kingdom> list_civs = World.world.kingdoms.list_civs;
            for (int i = 0; i < list_civs.Count; i++)
            {
                Kingdom kingdom = list_civs[i];


                if (!MoreGodPower.Vassals.ContainsKey(kingdom))
                {
                    kingdom.data.get("suzerain", out bool flag, false);
                    if (flag)
                    {
                        MoreGodPower.Vassals.Add(kingdom, null);
                    }
                    harmony_vassal.CheckVassal(kingdom);
                }
                foreach(City city in kingdom.cities)
                {
                    harmony_declare.CheckDeclare(city);
                }
            }
        }
    }
}