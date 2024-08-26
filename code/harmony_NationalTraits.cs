using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ai.behaviours;
using Diplomacy_Army;
using HarmonyLib;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Diplomacy_Army
{
    public class harmony_NationalTraits
    {
        #region 政治意识形态及国家特性
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClanManager), "tryPlotJoinAlliance")]
        public static bool tryPlotJoinAlliance(Actor pActor, PlotAsset pPlotAsset, ref bool __result)
        {
            if (PowerButtons.GetToggleValue("禁止自主联盟"))
            {
                __result = false;
                return false;
            }
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
        [HarmonyPatch(typeof(City), "getArmyMaxTotalPercentage")]
        public static bool getArmyMaxTotalPercentage(City __instance, ref float __result)
        {
            __instance.kingdom.data.get("CorruptArmy", out bool flag, false);

            if (flag)
            {
                __result = 0.75f;
                return false;
            }
            __instance.kingdom.data.get("SteelFortress", out flag, false);
            if (flag)
            {
                __result = 0.65f;
                return false;
            }
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag)
            {
                __result = 0.4f;
                return false;
            }
            __instance.kingdom.data.get("CivilianConscripts", out flag, false);
            if (flag)
            {
                __result = 0.95f;
                return false;
            }

            float num = 0f + __instance.race.civ_base_army_mod;
            float armyMaxLeaderPercentage = __instance.getArmyMaxLeaderPercentage();
            float num2 = num + armyMaxLeaderPercentage;
            float num3 = 0f;
            if (__instance.getCulture() != null)
            {
                num3 = __instance.getCulture().stats.bonus_max_army.value;
            }
            __result = num2 + num3 + (int)__instance.kingdom.stats.bonus_max_army.value;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "updateAge")]
        public static bool updateAge_Postfix(City __instance)
        {
            if (__instance.kingdom.king != null)
            {
                string personality = __instance.kingdom.king.s_personality.id;
                if (personality == "militarist")
                {
                    __instance.gold_out_army += (int)(__instance.gold_out_army * 0.1);
                    __instance.gold_change -= (int)(__instance.gold_out_army * 0.1);
                    __instance.data.storage.change("gold", -(int)(__instance.gold_out_army * 0.1));
                }
            }

            __instance.kingdom.data.get("CorruptArmy", out bool flag, false);

            if (flag)
            {
                __instance.gold_out_army += __instance.gold_out_army * 2;
                __instance.gold_change -= __instance.gold_out_army * 2;
                __instance.data.storage.change("gold", -(int)(__instance.gold_out_army * 2));
            }
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag)
            {
                __instance.gold_out_army += (int)(__instance.gold_out_army * 0.1);
                __instance.gold_change -= (int)(__instance.gold_out_army * 0.1);
                __instance.data.storage.change("gold", -(int)(__instance.gold_out_army * 0.1));
            }
            __instance.kingdom.data.get("IndustrialKingdom", out flag, false);
            if (flag)
            {
                __instance.gold_change += 100;
                __instance.data.storage.change("gold", 100);
                __instance.data.storage.change("bread", 20);
            }

            return true;
        }
        #endregion
        #region 国家特性
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorBase), "updateStats")]
        public static void UpdateStats_Postfix(ActorBase __instance)
        {
            if (__instance == null || __instance.asset == null || __instance.data == null || __instance.kingdom == null || !__instance.isAlive())
            {
                return;
            }

            __instance.kingdom.data.get("CorruptArmy", out bool flag, false);

            if (flag)
            {
                __instance.stats[S.damage] -= (float)(__instance.stats[S.damage] * 0.4);
                __instance.stats[S.armor] -= (float)(__instance.stats[S.armor] * 0.25);
                __instance.stats[S.speed] -= (float)(__instance.stats[S.speed] * 0.3);
            }
            __instance.kingdom.data.get("SteelFortress", out flag, false);
            if (flag)
            {
                __instance.stats[S.damage] -= (float)(__instance.stats[S.damage] * 0.6);
                __instance.stats[S.armor] += (float)(__instance.stats[S.armor] * 2);
                __instance.stats[S.knockback_reduction] += (float)(__instance.stats[S.knockback_reduction] * 2);
                __instance.stats[S.speed] -= (float)(__instance.stats[S.speed] * 0.2);
            }
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag)
            {
                __instance.stats[S.damage] += (float)(__instance.stats[S.damage] * 0.5);
                __instance.stats[S.armor] += (float)(__instance.stats[S.armor] * 0.5);
                __instance.stats[S.health] += (float)(__instance.stats[S.health] * 0.3);
                __instance.data.health += (int)(__instance.stats[S.health] * 0.3);
            }
            __instance.kingdom.data.get("CivilianConscripts", out flag, false);
            if (flag)
            {
                __instance.stats[S.damage] -= (float)(__instance.stats[S.damage] * 0.1);
                __instance.stats[S.armor] -= (float)(__instance.stats[S.armor] * 0.5);
            }
            if (__instance.asset.unit)
            {
                __instance.s_personality = null;
                if ((__instance.kingdom != null && __instance.kingdom.isCiv() && __instance.isKing()) || (__instance.city != null && __instance.city.leader == __instance))
                {
                    string pID2 = "balanced";
                    float num = __instance.stats[S.diplomacy];
                    if (__instance.stats[S.diplomacy] > __instance.stats[S.stewardship])
                    {
                        pID2 = "diplomat";
                        num = __instance.stats[S.diplomacy];
                    }
                    else if (__instance.stats[S.diplomacy] < __instance.stats[S.stewardship])
                    {
                        pID2 = "administrator";
                        num = __instance.stats[S.stewardship];
                    }
                    if (__instance.stats[S.warfare] > num)
                    {
                        pID2 = "militarist";
                    }
                    if (__instance.kingdom == DiplomacyManager.kingdom_supreme)
                    {
                        pID2 = "hegemony";
                    }
                    if (__instance.hasTrait("content"))
                    {
                        pID2 = "Conservatism";
                    }
                    __instance.s_personality = AssetManager.personalities.get(pID2);
                    __instance.stats.mergeStats(__instance.s_personality.base_stats);
                }
            }

            // 规范化统计数据并确保健康状况不超过最大值
            __instance.stats.normalize();

            // 可能需要进一步处理健康值超过最大值的情况
            if (__instance.data.health > __instance.getMaxHealth())
            {
                __instance.data.health = __instance.getMaxHealth();
            }

        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KingdomWindow), "OnEnable")]
        public static bool KingdomOnEnable_Prefix(KingdomWindow __instance)
        {
            NewWindow.kingdom = __instance.kingdom;
            __instance.kingdom.data.get("CorruptArmy", out bool flag, false);
            if (flag != PowerButtons.GetToggleValue("CorruptArmy"))
                PowerButtons.ToggleButton("CorruptArmy");
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag != PowerButtons.GetToggleValue("EliteLegion"))
                PowerButtons.ToggleButton("EliteLegion");
            __instance.kingdom.data.get("SteelFortress", out flag, false);
            if (flag != PowerButtons.GetToggleValue("SteelFortress"))
                PowerButtons.ToggleButton("SteelFortress");
            __instance.kingdom.data.get("CivilianConscripts", out flag, false);
            if (flag != PowerButtons.GetToggleValue("CivilianConscripts"))
                PowerButtons.ToggleButton("CivilianConscripts");
            __instance.kingdom.data.get("IndustrialKingdom", out flag, false);
            if (flag != PowerButtons.GetToggleValue("IndustrialKingdom"))
                PowerButtons.ToggleButton("IndustrialKingdom");
            return true;
        }
        #endregion
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapText), "showTextKingdom")]
        public static bool showTextKingdom(MapText __instance, Kingdom pKingdom)
        {

            if (PowerButtons.GetToggleValue("显示原版铭牌"))
            {
                return true;
            }
            // if (__instance.text.gameObject.GetComponent<Outline>() == null)
            // {
            //     Outline outline = __instance.text.gameObject.AddComponent<Outline>();
            //     outline.effectColor = outlineColor;
            //     outline.effectDistance = new Vector2(outlineWidth, -outlineWidth);
            // }

            __instance.text.fontStyle = FontStyle.Bold;
            __instance.text.color = Color.white;
            __instance.text.fontSize = 15;

            string text = pKingdom.name + "  " + pKingdom.getPopulationTotal().ToString();
            pKingdom.data.get("Vassal", out bool flag, false);
            if (harmony_vassal.CheckVassal(pKingdom))
            {
                pKingdom.data.get("suzerainID", out string str, "");
                Kingdom pKingdom2 = World.world.kingdoms.getKingdomByID(str);
                if (pKingdom2 != null)
                {
                    text = $"  {pKingdom2.name}  " + "\r\n" + pKingdom.name + "  " + pKingdom.getPopulationTotal().ToString();
                }
            }

            // if (DebugConfig.isOn(DebugOption.ShowWarriorsCityText))
            // {
            text = string.Concat(new string[]
            {
                text,
                " | ",
                pKingdom.getArmy().ToString(),
                "/",
                pKingdom.countArmyMax().ToString()
            });
            // }
            // int ArmyMaxTotalPercentage = 0;
            List<TileZone> zones = new();
            foreach (City city in pKingdom.cities)
            {
                zones.AddRange(city.zones);
                // ArmyMaxTotalPercentage += (int)(city.getArmyMaxTotalPercentage() * 100f);
            }
            // text += " | " + (ArmyMaxTotalPercentage / pKingdom.cities.Count).ToString() + "%";
            if (DebugConfig.isOn(DebugOption.ShowCityWeaponsText))
            {
                text = text + " | w" + pKingdom.countWeapons().ToString();
            }
            if (pKingdom.king != null)
            {
                text += "\r\n" + pKingdom.king.getName() + "国王";
                int yearsSince = World.world.getYearsSince(pKingdom.data.timestamp_king_rule);
                if (yearsSince == 0) { text += "元年"; }
                else if (yearsSince > 0) { text += yearsSince.ToString() + "年"; }
                text += "\r\n" + LocalizedTextManager.getText("personality_" + pKingdom.king.s_personality.id, null);
            }





            __instance.setText(text, CalculateCityCenter(zones));
            __instance.priority_population = pKingdom.units.Count;
            __instance.base_icon.sprite = pKingdom.race.getRaceIconSprite();
            __instance._show_base_icon = true;
            __instance._show_banner_kingdom = false;
            // __instance.banner_kingdoms.load(pKingdom);
            // BannerContainer bannerContainer = BannerGenerator.dict[pKingdom.race.banner_id];
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapText), "showTextCity")]
        public static bool showTextCity(MapText __instance, City pCity)
        {
            if (PowerButtons.GetToggleValue("显示原版铭牌"))
            {
                return true;
            }
            string text = pCity.getCityName() + "  " + pCity.getPopulationTotal(true).ToString();
            // if (DebugConfig.isOn(DebugOption.ShowWarriorsCityText))
            // {
            text = string.Concat(new string[]
            {
                text,
                " | ",
                pCity.getArmy().ToString(),
                "/",
                pCity.getArmyMaxCity().ToString()
            });
            // if (Config.isEditor)
            // {
            string str = "  :  " + ((int)(pCity.getArmyMaxTotalPercentage() * 100f)).ToString() + "%";
            text += str;
            // }
            // }
            if (DebugConfig.isOn(DebugOption.ShowCityWeaponsText))
            {
                text = text + " | w" + pCity.countWeapons().ToString();
            }
            if (DebugConfig.isOn(DebugOption.ShowFoodCityText))
            {
                text = text + " | F" + pCity.getTotalFood().ToString();
            }
            if (__instance.text.color != pCity.kingdom.kingdomColor.getColorText())
            {
                __instance.text.color = pCity.kingdom.kingdomColor.getColorText();
            }
            if (harmony_declare.CheckDeclare(pCity))
            {
                pCity.data.get("DeclareKingdomID", out str, "");
                Kingdom kingdom = World.world.kingdoms.getKingdomByID(str);
                text += "\r\n宣称国家:" + kingdom.name;
            }

            __instance.setText(text, pCity.cityCenter);
            __instance.base_icon.sprite = SpriteTextureLoader.getSprite("ui/Icons/iconKingdom");
            if (pCity.kingdom.capital == pCity)
            {
                __instance._show_base_icon = true;
            }
            if (pCity.last_ticks != 0)
            {
                __instance._show_conquest = true;
                if (pCity.being_captured_by != null)
                {
                    __instance.conquerText.color = pCity.being_captured_by.kingdomColor.getColorText();
                }
                Text text2 = __instance.conquerText;
                int last_ticks = pCity.last_ticks;
                text2.text = last_ticks.ToString() + "%";
            }
            __instance.priority_capital = pCity.kingdom != null && pCity.kingdom.capital == pCity;
            __instance.priority_population = pCity.status.population;
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapNamesManager), "prepareNext")]
        public static bool prepareNext(MapNamesManager __instance, NameplateAsset pAsset, ref MapText __result)
        {
            if (PowerButtons.GetToggleValue("显示原版铭牌"))
            {
                return true;
            }
            if (pAsset.id == "plate_city" || pAsset.id == "plate_kingdom" || pAsset.id == "nameplate_city_capital")
            {
                MapText mapText;
                if (__instance.active.Count > __instance._usedIndex)
                {
                    mapText = __instance.active[__instance._usedIndex];
                }
                else
                {
                    if (__instance.pool.Count == 0)
                    {
                        mapText = __instance.createNew();
                    }
                    else
                    {
                        mapText = __instance.pool.Pop();
                    }
                    __instance.active.Add(mapText);
                }
                mapText.reset();
                mapText.setShowing(true);
                mapText.background_image.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.nothing.png");
                mapText.layout_group.padding.left = pAsset.padding_left;
                mapText.layout_group.padding.right = pAsset.padding_right;
                mapText.layout_group.padding.top = pAsset.padding_top;
                __instance._usedIndex++;
                __result = mapText;
                return false;
            }
            return true;
        }
        private static Vector3 CalculateCityCenter(List<TileZone> zones)
        {
            if (zones.Count == 0)
            {
                return Globals.POINT_IN_VOID;
            }

            float totalX = 0f;
            float totalY = 0f;
            Vector3 cityCenter = Vector3.zero;

            foreach (TileZone zone in zones)
            {
                totalX += zone.centerTile.posV3.x;
                totalY += zone.centerTile.posV3.y;
            }

            cityCenter.x = totalX / zones.Count;
            cityCenter.y = totalY / zones.Count;

            TileZone closestZone = null;
            float closestDist = 0f;

            foreach (TileZone zone in zones)
            {
                float dist = Toolbox.Dist(zone.centerTile.posV3.x, zone.centerTile.posV3.y, cityCenter.x, cityCenter.y);
                if (closestZone == null || dist < closestDist)
                {
                    closestZone = zone;
                    closestDist = dist;
                }
            }

            cityCenter = closestZone.centerTile.posV3 + new Vector3(0f, 2f, 0f);

            return cityCenter;
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(CityBehProduceUnit), "isCityCanProduceUnits")]
        // public static bool IsCityCanProduceUnits(City pCity, ref bool __result)
        // {
        //     if (Main.moreSettings["村庄人口上限"] != 0 && pCity.getPopulationTotal() > Main.moreSettings["村庄人口上限"])
        //     {
        //         __result = false;
        //         return false;
        //     }
        //     // if (Main.moreSettings["村庄人口下限"] != 0 && pCity.getPopulationTotal() < Main.moreSettings["村庄人口下限"])
        //     // {
        //     //     __result = true;
        //     //     return false;
        //     // }
        //     return true;

        // }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehCheckCulture), "recalcMainCulture")]
        public static bool recalcMainCulture(CityBehCheckCulture __instance, City pCity)
        {
            __instance._counters_dict.Clear();
            __instance._counters_list.Clear();
            CultureMainCounter cultureMainCounter = null;
            foreach (TileZone tileZone in pCity.zones)
            {
                if (tileZone.culture != null)
                {
                    if (!__instance._counters_dict.TryGetValue(tileZone.culture, out cultureMainCounter))
                    {
                        cultureMainCounter = new CultureMainCounter(tileZone.culture);
                        __instance._counters_dict.Add(tileZone.culture, cultureMainCounter);
                        __instance._counters_list.Add(cultureMainCounter);
                    }
                    __instance._counters_dict[tileZone.culture].amount++;
                }
            }
            foreach (Actor actor in pCity.units)
            {
                if (actor == null) { pCity.units.Remove(actor); continue; }
                Culture culture = actor.getCulture();
                if (culture != null)
                {
                    if (!__instance._counters_dict.TryGetValue(culture, out cultureMainCounter))
                    {
                        cultureMainCounter = new CultureMainCounter(culture);
                        __instance._counters_dict.Add(culture, cultureMainCounter);
                        __instance._counters_list.Add(cultureMainCounter);
                    }
                    __instance._counters_dict[culture].amount++;
                }
            }
            if (!__instance._counters_list.Any<CultureMainCounter>())
            {
                return false;
            }
            __instance._counters_list.Sort(new Comparison<CultureMainCounter>(__instance.sortByPower));
            Culture culture2 = __instance._counters_list[0].culture;
            pCity.setCulture(culture2);
            __instance._counters_dict.Clear();
            __instance._counters_list.Clear();
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "updateCityStatus")]
        public static bool updateCityStatus(City __instance)
        {
            __instance._dirty_city_status = false;
            __instance.setAbandonedZonesDirty();
            __instance.recalculateCityTile();
            __instance.recalculateNeighbourZones();
            __instance.recalculateNeighbourCities();
            Culture culture = __instance.getCulture();
            __instance.status.zoneRange = __instance.race.civ_base_zone_range;
            foreach (Building building in __instance.buildings)
            {
                if (!building.isUnderConstruction())
                {
                    __instance.status.zoneRange += building.asset.max_zone_range;
                }
            }
            if (__instance.leader != null)
            {
                __instance.status.zoneRange += (int)__instance.leader.stats[S.zone_range];
            }
            if (culture != null)
            {
                __instance.status.zoneRange += (int)culture.stats.city_zone_range.value;
            }
            int num = __instance.countProfession(UnitProfession.Baby);
            __instance.status.population = __instance.getPopulationTotal(false);
            __instance.status.populationAdults = __instance.status.population - num;
            __instance.status.hungry = 0;
            __instance.status.housingTotal = 0;
            __instance.status.housingFree = 0;
            __instance.status.housingOccupied = 0;
            __instance.blacksmith = null;
            List<Actor> simpleList = __instance.units.getSimpleList();
            for (int i = 0; i < simpleList.Count; i++)
            {
                Actor actor = simpleList[i];
                if (actor == null || actor.data == null) { __instance.units.Remove(actor); continue; };
                if (actor.data.hunger <= 10)
                {
                    __instance.status.hungry++;
                }
                if (actor.isCitizenJob(S.blacksmith))
                {
                    __instance.blacksmith = actor;
                }
            }
            List<Building> simpleList2 = __instance.buildings.getSimpleList();
            for (int j = 0; j < simpleList2.Count; j++)
            {
                Building building2 = simpleList2[j];
                if (!building2.isUnderConstruction() && building2.asset.housing > 0)
                {
                    __instance.status.housingTotal += building2.asset.housing;
                    if (culture != null)
                    {
                        __instance.status.housingTotal += (int)culture.stats.housing.value;
                    }
                }
            }
            if (Main.moreSettings["村庄人口上限"] != 0 && __instance.status.housingTotal > Main.moreSettings["村庄人口上限"])
            {
                __instance.status.housingTotal = Main.moreSettings["村庄人口上限"];
            }
            if (Main.moreSettings["村庄人口下限"] != 0 && __instance.status.housingTotal < Main.moreSettings["村庄人口下限"])
            {
                __instance.status.housingTotal = Main.moreSettings["村庄人口下限"];
            }
            __instance.kingdom.data.get("IndustrialKingdom", out bool flag, false);
            if (flag)
            {
                __instance.status.housingTotal += 50;
            }

            if (__instance.status.population > __instance.status.housingTotal)
            {
                __instance.status.housingOccupied = __instance.status.housingTotal;
            }
            else
            {
                __instance.status.housingOccupied = __instance.status.population;
            }
            __instance.status.housingFree = __instance.status.housingTotal - __instance.status.housingOccupied;
            __instance.status.maximumItems = 2;
            if (culture != null)
            {
                __instance.status.maximumItems += (int)culture.stats.max_city_items.value;
            }
            CityBehCheckFarms.check(__instance);
            return false;
        }

    }
}