﻿using System;
using System.Collections.Generic;
using NCMS;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;
using HarmonyLib;
using ai;
using ai.behaviours;
using NCMS.Utils;
using Diplomacy_Army.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Diplomacy_Army
{
    [ModEntry]
    public class Main : MonoBehaviour
    {

        public Harmony harmony;
        public static Kingdom kingdom;
        public static City city;
        public static DateTime GCtime = DateTime.MinValue;





        public static string treatyTime = "0";

        public static Dictionary<Kingdom, Dictionary<string, DateTime>> KingdomMessages = new();
        public static List<string> messagesOutTime = new();
        public static List<Kingdom> kingdomsOutTime = new();
        public static string text = "";
        public static int index = 0;
        public static Rect rect = new(0, 0, 156f, 20f);
        public static Rect windowRect1 = new(0, 0, 250f, 400f);
        public static Rect windowRect2 = new(0, 0, 250f, 400f);
        public static Rect windowRect3 = new(0, 0, 120f, 240f);
        public static Rect windowRect4 = new(0, 0, 180f, 240f);
        public static Rect windowRect5 = new(0, 0, 180f, 160f);
        public static Rect windowRect6 = new(0, 0, 180f, 240f);
        public static Rect windowRect7 = new(0, 0, 180f, 280f);
        public static Rect scrollViewRect = new(0, 0, 0, 0);
        public static Vector3 vector3 = new();
        public static Vector3 position = new();
        public static Vector2 scrollPosition = Vector2.zero;
        public static Vector2 scrollPosition2 = Vector2.zero;


        public static ResourceAsset resourceAsset;
        public static int resTotalNum = 0;



        public static DateTime celebrationTime = DateTime.MinValue;

        public static Dictionary<string, Treaty> treaties = new();
        public static List<string> warMessage = new();

        public static Kingdom kingdom1;
        public static Kingdom kingdom2;
        public static Color kingdom1_color;
        public static Color kingdom2_color;


        public static bool showWorldLow = false;

        #region 急了和空星漫漫新增变量

        public static Color outlineColor = Color.black;
        public static float outlineWidth = 1.5f;
        public static List<PowersTab> PVZPowersTabs = new();
        public static int numofyears = 1;
        public static Dictionary<string, Text> SettingsText = new();
        public static Dictionary<string, int> moreSettings = new() { { "村庄人口上限", 0 }, { "村庄人口下限", 0 }, { "村庄领土上限", 0 }, { "村庄资源上限", 0 } };
        
        public static Dictionary<string, int> resourceSettings = new();
        public static Dictionary<string, Text> resourceText = new();
        public static Dictionary<string, NationalTraits> NationalTraits = new();
        public static Dictionary<string, bool> DASet = new()
        {
            { "ChooseKing", false },
            { "ChooseLeader", false },
            { "ChooseAllWarrior", true},
            { "ChooseCityWarrior", false},
            { "ChooseCityGeneral", false},
            { "ChooseAllGeneral", false},
            { "DA_自动内存清理", false},
            { "DA_显示外交消息", false},
            { "DA_显示交战", false},
            { "DA_显示条约", false},
            { "显示原版铭牌", false},
            { "封锁边境", false},
            { "异族占领", false},
            { "领土完整", false},
            { "禁止自主联盟", false},
            { "国王装备禁用", false},
            { "城市士兵装备禁用", false},
            { "将军装备禁用", false},
            { "领主装备禁用", false},
            { "DAdebug", true},
            { "DA_显示附庸", false}

        };


        #endregion

        public void Awake()
        {

            if (!System.IO.Directory.Exists(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army"))
            {
                System.IO.Directory.CreateDirectory(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army");
            }
            string text1 = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "Diplomacy_ArmySet.json");
            string text0 = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "numofyears.json");

            Dictionary<string, bool> NewSet = DA_save.LoadDictionaryFromFile(text1);
            foreach (var set in NewSet.Keys)
            {
                if (DASet.ContainsKey(set))
                {
                    DASet[set] = NewSet[set];
                }
                else
                {
                    DASet.Add(set, NewSet[set]);
                }
            }


            if (File.Exists(text0))
            {
                DAStorage pStorage = JsonConvert.DeserializeObject<DAStorage>(File.ReadAllText(text0));
                if (pStorage != null)
                {
                    numofyears = pStorage.nom;
                }
            }
            else
            {
                DAStorage NewStorage = new()
                {
                    Name = "numofyears",
                    nom = numofyears
                };
                File.WriteAllText(text0, JsonConvert.SerializeObject(NewStorage, Formatting.Indented));
            }
            string filePath = $".\\Mods\\KaiPanFuZhu-Mod-main\\NationalTraits\\NationalTraits.json";

            NationalTraits=DA_save.LoadFromFile(filePath);


            Windows.CreateNewWindow("DAHelper", "");
            GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Scroll View").SetActive(true);
            GameObject UIG = new("DA_UIG");
            var UIGRTF = UIG.AddComponent<RectTransform>();
            UIG.AddComponent<CanvasRenderer>();
            UIG.AddComponent<Image>().sprite = Resources.Load<Sprite>("ui/DAUIG");
            UIGRTF.sizeDelta = new Vector2(200, 30);

            Dictionary<string, ScrollWindow> allWindows = (Dictionary<string, ScrollWindow>)Reflection.GetField(typeof(ScrollWindow), null, "allWindows");
            Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "kingdom");
            Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", "debug");
            allWindows["debug"].gameObject.SetActive(false);
            allWindows["kingdom"].gameObject.SetActive(false);
            foreach (ResourceAsset resource in AssetManager.resources.list) { resourceSettings.Add(resource.id, 0); }
            harmony = new Harmony("10011011");
            translate.init();
            pvz_ui.NewTab("Diplomacy_Army", "icon", 150f);
            NationalTraitsWindow.init();
            NewWindow.init();
            DA_modder.init();
            DA_button.init();
            DiplomacyPowerWindow.init();



            MoreGodPower.init();
            KingdomPowerWindow.init();
            CityPowerWindow.init();
            ArmyPowerWindow.init();
            Patching(harmony);
            resourceAsset = AssetManager.resources.list[0];
            resTotalNum = AssetManager.resources.list.Count;
            NewFunction.localizedText = (Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText");
            Harmony.CreateAndPatchAll(typeof(Main));
            Harmony.CreateAndPatchAll(typeof(harmony_vassal));
            Harmony.CreateAndPatchAll(typeof(harmony_declare));
            Harmony.CreateAndPatchAll(typeof(harmony_NationalTraits));
            Personality();
            Plots();

            foreach (var set in DASet.Keys)
            {
                if (PowerButtons.ToggleValues.ContainsKey(set))
                {
                    if (PowerButtons.GetToggleValue(set) != DASet[set])
                        PowerButtons.ToggleButton(set);
                }
            }
            OpinionAsset opinionAsset = new()
            {
                id = "opinion_vassal",
                translation_key = "opinion_vassal",
                calc = delegate (Kingdom pMain, Kingdom pTarget)
                {
                    int result = 0;
                    if (MoreGodPower.Vassals.ContainsKey(pMain) && MoreGodPower.Vassals[pMain].Contains(pTarget))
                    {
                        result = 100;
                    }
                    return result;
                }
            };
            AssetManager.opinion_library.add(opinionAsset);
            Localization.addLocalization("opinion_vassal", "谁会跟自己的附庸过不去呢");
            opinionAsset = new()
            {
                id = "opinion_vassal",
                translation_key = "opinion_vassal",
                calc = delegate (Kingdom pMain, Kingdom pTarget)
                {
                    int result = 0;
                    if (MoreGodPower.Declares.ContainsKey(pMain))
                    {
                        foreach (var city in MoreGodPower.Declares[pMain])
                        {
                            if (city.kingdom != kingdom && city.kingdom == pTarget)
                            {
                                result -= 75;
                            }
                        }
                    }
                    return result;
                }
            };
            AssetManager.opinion_library.add(opinionAsset);
            Localization.addLocalization("opinion_vassal", "占着我宣称的城市");

            WarTypeAsset Declare = new()
            {
                id = "Declare",
                name_template = "war_conquest",
                localized_type = "war_type_Declare",
                path_icon = "wars/war_conquest",
                kingdom_for_name_attacker = true,
                forced_war = false,
                total_war = false,
                alliance_join = true
            };
            Localization.addLocalization("war_type_Declare", "宣称战争");

            AssetManager.war_types_library.add(Declare);
        }
        public static void Plots()
        {
            PlotAsset plotAsset = new()
            {
                id = "new_declare_war",
                translation_key = "plot_new_declare_war",
                path_icon = "plots/icons/plot_new_war",
                description = "plot_description_new_declare_war",
                check_initiator_actor = true,
                check_initiator_city = true,
                check_initiator_kingdom = true,
                check_target_kingdom = true,
                check_supporters = new PlotActorPlotDelegate(PlotsLibrary.checkMembersToRemoveDefault),
                check_launch = delegate (Actor pActor, Kingdom pKingdom)
                {
                    if (!DiplomacyPowerWindow.IsDeclareWarNeeded(pKingdom))
                    {
                        return false;
                    }
                    if (pKingdom.hasAlliance())
                    {
                        foreach (Kingdom kingdom in pKingdom.getAlliance().kingdoms_hashset)
                        {
                            if (kingdom != pKingdom && !(kingdom.king == null))
                            {
                                List<Plot> plotsFor = World.world.plots.getPlotsFor(kingdom.king, true);
                                if (plotsFor != null)
                                {
                                    using List<Plot>.Enumerator enumerator2 = plotsFor.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        if (enumerator2.Current.isSameType(AssetManager.plots_library.get("new_declare_war")))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }
                    return true;
                },
                check_should_continue = (Plot pPlot) => pPlot.initiator_actor.isUnitOk() && (!pPlot.initiator_kingdom.hasAlliance() || pPlot.initiator_kingdom.getAlliance() != pPlot.target_kingdom.getAlliance()) && DiplomacyPowerWindow.IsDeclareWarNeeded(pPlot.initiator_kingdom),
                plot_power = (Actor pActor) => (int)pActor.stats[S.warfare],
                action = delegate (Plot pPlot)
                {
                    World.world.diplomacy.startWar(pPlot.initiator_kingdom, pPlot.target_kingdom, AssetManager.war_types_library.get("Declare"), true);
                    return true;
                },
                cost = 200
            };
            AssetManager.plots_library.add(plotAsset);
        }
        public static void Personality()
        {
            var Personality = AssetManager.personalities.add(new PersonalityAsset
            {
                id = "hegemony"
            });
            Personality.base_stats[S.personality_diplomatic] = -0.5f;//外交
            Personality.base_stats[S.personality_administration] = 0.3f;//管理
            Personality.base_stats[S.personality_aggression] = 0.8f;//侵略
            Localization.addLocalization("personality_hegemony", "霸权主义");
            Personality = AssetManager.personalities.add(new PersonalityAsset
            {
                id = "Conservatism"
            });
            Personality.base_stats[S.personality_diplomatic] = 0.3f;//外交
            Personality.base_stats[S.personality_administration] = 0.3f;//管理
            Personality.base_stats[S.personality_aggression] = 0.2f;//侵略
            Localization.addLocalization("personality_Conservatism", "保守主义");
        }


        public void Update()
        {
            if (!Config.gameLoaded) { return; }
            Diplomacy_Army.Update.updateTreaty();
            Diplomacy_Army.Update.updateCities();
            Diplomacy_Army.Update.UpdateVassals();
            Diplomacy_Army.Update.UpdateDeclare();
            if (DateTime.Compare(GCtime, DateTime.Now.ToLocalTime()) < 0 && PowerButtons.GetToggleValue("DA_自动内存清理"))
            {
                GCGame();
            }
            if (Input.GetKey(KeyCode.Tab) && Input.GetKeyUp(KeyCode.K))
            {
                pvz_ui.Button_Powers_Click("Diplomacy_Army");
            }
            // if (Input.GetKey(KeyCode.K))
            // {
            //     pvz_ui.Button_Powers_Click("Diplomacy_Army");
            // }
        }


        public static void GCGame()
        {
            GC.Collect();
            GCtime = DateTime.Now.ToLocalTime().AddMinutes(3.0f);
        }

        public void OnGUI()
        {
            if (PowerButtons.GetToggleValue("DA_显示外交消息"))
            {
                GUI.skin.label.fontSize = 13;
                GUI.skin.label.fontStyle = FontStyle.Bold;
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                GUI.skin.label.normal.textColor = Color.black;
                GUI.skin.label.normal.background = Texture2D.whiteTexture;

                foreach (var item in KingdomMessages)
                {
                    Kingdom kingdom = item.Key;
                    if (!kingdom.data.alive)
                    {
                        KingdomMessages.Remove(item.Key);
                        break;
                    }
                    if (KingdomMessages[kingdom].Count > 0)
                    {
                        text = "";
                        index = 0;
                        foreach (var item2 in KingdomMessages[kingdom])
                        {
                            if (DateTime.Compare(item2.Value, DateTime.Now.ToLocalTime()) > 0)
                            {
                                if (index > 0)
                                {
                                    text += "\r\n";
                                }
                                index += (11 + item2.Key.Length) / 12;
                                text += item2.Key;
                            }
                            else
                            {
                                messagesOutTime.Add(item2.Key);
                            }
                        }
                        if (messagesOutTime.Count > 0)
                        {
                            foreach (string meg in messagesOutTime)
                            {
                                KingdomMessages[kingdom].Remove(meg);
                            }
                            messagesOutTime.Clear();
                        }
                        if (text != "" && kingdom.capital != null)
                        {
                            vector3 = kingdom.capital.cityCenter;
                            position = Camera.main.WorldToScreenPoint(vector3);
                            position.y = Screen.height - position.y;
                            rect.width = 156f;
                            rect.height = 20f * (index + 0.2f);
                            rect.y = position.y - rect.height - 14f;
                            rect.x = position.x - rect.width / 2;
                            GUI.Label(rect, text);
                        }
                    }
                    else
                    {
                        kingdomsOutTime.Add(kingdom);
                    }
                }
                if (kingdomsOutTime.Count > 0)
                {
                    foreach (Kingdom kingdom2 in kingdomsOutTime)
                    {
                        KingdomMessages.Remove(kingdom2);
                    }
                    kingdomsOutTime.Clear();
                }
            }
            if (!PowerButtons.GetToggleValue("DA_显示交战"))
            {
                warMessage.Clear();

                GUI.skin.box.fontSize = 13;
                GUI.skin.box.fontStyle = FontStyle.Bold;
                GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                GUI.skin.box.normal.textColor = Color.white;

                GUI.skin.scrollView.normal.background = Texture2D.blackTexture;

                windowRect1 = GUI.Window(13, windowRect1, window1_Function, "世界战争局势");
            }
            if (PowerButtons.GetToggleValue("DA_显示条约"))
            {
                GUI.skin.box.fontSize = 13;
                GUI.skin.box.fontStyle = FontStyle.Bold;
                GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                GUI.skin.box.normal.textColor = Color.white;

                GUI.skin.scrollView.normal.background = Texture2D.blackTexture;

                windowRect2 = GUI.Window(14, windowRect2, window2_Function, "世界外交局势");
            }
        }




        private static int warNum;
        public static void window1_Function(int windowID)//世界战争局势
        {
            GUI.skin.box.fontSize = 13;
            GUI.skin.box.fontStyle = FontStyle.Bold;
            GUI.skin.box.alignment = TextAnchor.MiddleLeft;
            GUI.skin.box.normal.textColor = Color.white;

            GUI.skin.scrollView.normal.background = Texture2D.blackTexture;

            rect.x = 10f;
            rect.y = 30f;
            rect.width = 230f;
            rect.height = 360f;
            scrollViewRect.width = 300f;
            scrollViewRect.height = 340f;
            if (warNum >= 17)
            {
                scrollViewRect.height = 20f * warNum;
            }

            scrollPosition = GUI.BeginScrollView(rect, scrollPosition, scrollViewRect);

            warNum = 0;
            if (MapBox.instance.wars.list.Count > 0)
            {
                rect.x = 0f;
                rect.y = 0f;
                rect.width = 300f;
                rect.height = 20f;
                foreach (War war in MapBox.instance.wars.list)
                {
                    foreach (Kingdom attacker in war.getAttackers())
                    {
                        foreach (Kingdom defender in war.getDefenders())
                        {
                            text = "<color=#" + ColorUtility.ToHtmlStringRGBA(attacker.getColor().getColorText()) + ">" + attacker.data.name + "</color>与<color=#" + ColorUtility.ToHtmlStringRGBA(defender.getColor().getColorText()) + ">" + defender.data.name + "</color>交战中";
                            GUI.Box(rect, text);
                            rect.y += 20f;
                            warNum++;
                        }
                    }
                }
            }

            GUI.EndScrollView();

            rect.x = 0f;
            rect.y = 0f;
            rect.width = 10000f;
            rect.height = 10000f;
            GUI.DragWindow();
        }
        public static void window2_Function(int windowID)//世界外交局势
        {
            rect.x = 10f;
            rect.y = 30f;
            rect.width = 230f;
            rect.height = 360f;
            scrollViewRect.width = 500f;
            scrollViewRect.height = 340f;
            if (treaties.Count > 17)
            {
                scrollViewRect.height = 20f * treaties.Count;
            }
            scrollPosition2 = GUI.BeginScrollView(rect, scrollPosition2, scrollViewRect);

            if (treaties.Count > 0)
            {
                rect.x = 0f;
                rect.y = 0f;
                rect.width = 500f;
                rect.height = 20f;
                foreach (var item in treaties)
                {
                    if (item.Value.endTime < MapBox.instance.mapStats.year)
                    {
                        treaties.Remove(item.Key);
                        break;
                    }
                    kingdom1 = item.Value.kingdom_1;
                    kingdom2 = item.Value.kingdom_2;
                    if (kingdom1 == null || kingdom2 == null || !MapBox.instance.kingdoms.list_civs.Contains(kingdom1) || !MapBox.instance.kingdoms.list_civs.Contains(kingdom2))
                    {
                        treaties.Remove(item.Key);
                        break;
                    }
                    kingdom1_color = kingdom1.kingdomColor.getColorText();
                    kingdom2_color = kingdom2.kingdomColor.getColorText();
                    if (kingdom1_color != item.Value.kingdom1_color || kingdom2_color != item.Value.kingdom2_color
                        || kingdom1.name != item.Value.kingdom1_name || kingdom2.name != item.Value.kingdom2_name)
                    {
                        item.Value.Change();
                    }
                    GUI.skin.box.normal.textColor = item.Value.color;
                    GUI.Box(rect, item.Value.message);
                    rect.y += 20f;
                }
            }

            GUI.EndScrollView();

            rect.x = 0f;
            rect.y = 0f;
            rect.width = 10000f;
            rect.height = 10000f;
            GUI.DragWindow();

        }



        #region patch
        public static void Patching(Harmony harmony)
        {
            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(PowerButton), "unselectActivePower"), AccessTools.Method(typeof(Main), "unselectActivePower_Prefix"));
            Debug.Log("Prefix: PowerButton.unselectActivePower");

            PVZTools.HarmonyPatching(harmony, "postfix", AccessTools.Method(typeof(ActorMove), "goTo"), AccessTools.Method(typeof(Main), "goTo_Postfix"));
            Debug.Log("Postfix: ActorMove.goto");

            PVZTools.HarmonyPatching(harmony, "postfix", AccessTools.Method(typeof(ScrollWindow), "setActive"), AccessTools.Method(typeof(Main), "setActive_Postfix"));
            Debug.Log("Postfix: ScrollWindow.setActive");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(Building), "clearCityZones"), AccessTools.Method(typeof(Main), "clearCityZones_Prefix"));
            Debug.Log("Prefix: Building.clearZoneBuilding");

            PVZTools.HarmonyPatching(harmony, "postfix", AccessTools.Method(typeof(BaseSimObject), "canAttackTarget"), AccessTools.Method(typeof(Main), "canAttackTarget_Postfix"));
            Debug.Log("Postfix: BaseSimObject.canAttackTarget");

            PVZTools.HarmonyPatching(harmony, "postfix", AccessTools.Method(typeof(UnitGroup), "addUnit"), AccessTools.Method(typeof(Main), "addUnit_Postfix"));
            Debug.Log("Postfix: UnitGroup.addUnit");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(City), "setKingdom"), AccessTools.Method(typeof(Main), "setKingdom_Prefix"));
            Debug.Log("Prefix: City.setKingdom");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(BehVerifierAttackZone), "execute"), AccessTools.Method(typeof(Main), "execute_BehVerifierAttackZone_Prefix"));
            Debug.Log("Prefix: BehVerifierAttackZone.execute");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(City), "updateConquest"), AccessTools.Method(typeof(Main), "updateConquest_Prefix"));
            Debug.Log("Prefix: City.updateConquest");

            // PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(CityBehProduceUnit), "tryToProduceUnit"), AccessTools.Method(typeof(Main), "tryToProduceUnit_Prefix"));
            // Debug.Log("Prefix: KingdomManager.tryToProduceUnit");

            // PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(CityBehProduceUnit), "execute"), AccessTools.Method(typeof(Main), "execute_CityBehProduceUnit_Prefix"));
            // Debug.Log("Prefix: CityBehProduceUnit.execute");

            //PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(CityBehBorderGrowth), "newGrowth"), AccessTools.Method(typeof(Main), "newGrowth_Prefix"));
            //Debug.Log("Prefix: CityBehBorderGrowth.newGrowth");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(CityBehBorderGrowth), "execute"), AccessTools.Method(typeof(Main), "execute_CityBehBorderGrowth_Prefix"));
            Debug.Log("Prefix: CityBehBorderGrowth.execute");

            PVZTools.HarmonyPatching(harmony, "prefix", AccessTools.Method(typeof(CityStorage), "change"), AccessTools.Method(typeof(Main), "change_Prefix"));
            Debug.Log("Prefix: Citystorage.change");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Actor), "setProfession")]
        public static bool setProfession(Actor __instance, UnitProfession pType, bool pCancelBeh = true)
        {
            if (__instance == null || __instance.data == null) { return false; }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "updateCitizens")]
        public static bool updateCitizens(City __instance)
        {
            __instance._dirty_units = false;
            if (__instance.professionsDict.Count == 0)
            {
                for (int i = 0; i < City.unitProfessions.Length; i++)
                {
                    UnitProfession key = City.unitProfessions[i];
                    __instance.professionsDict.Add(key, new List<Actor>());
                }
            }
            foreach (List<Actor> list in __instance.professionsDict.Values)
            {
                list.Clear();
            }
            List<Actor> simpleList = __instance.units.getSimpleList();
            for (int j = 0; j < simpleList.Count; j++)
            {
                Actor actor = simpleList[j];
                if (actor == null) { __instance.units.Remove(actor); continue; }
                if (actor.asset.baby)
                {
                    __instance.professionsDict[UnitProfession.Baby].Add(actor);
                }
                else if (actor == null || !actor.isAlive())
                {
                    __instance.units.Remove(actor);
                }
                else
                {
                    __instance.professionsDict[actor.data.profession].Add(actor);
                }
            }
            return false;
        }
        public static void unselectActivePower_Prefix()
        {
            if (MoreGodPower.selected_kingdom != null || MoreGodPower.selected_city != null || MoreGodPower.selected_culture != null)
            {
                MoreGodPower.selected_kingdom = null;
                MoreGodPower.selected_city = null;
                MoreGodPower.selected_culture = null;
                NewFunction.AddNewText("已取消操作", Toolbox.color_log_neutral);
            }
        }
        public static void goTo_Postfix(Actor actor, ref ExecuteEvent __result, WorldTile target, bool pPathOnLiquid = false, bool pWalkOnBlocks = false)
        {
            if (__result == ExecuteEvent.True && actor.current_path.Count > 2 && actor.city != null && target.zone.city != null
                && PowerButtons.GetToggleValue("封锁边境") && !actor.asset.isBoat)
            {
                kingdom = actor.city.kingdom;
                Kingdom kingdom2 = target.zone.city.kingdom;
                if (kingdom != kingdom2 && kingdom.isCiv() && kingdom2.isCiv())
                {
                    foreach (WorldTile worldTile in actor.current_path)
                    {
                        if (worldTile.zone.city != null)
                        {
                            kingdom2 = worldTile.zone.city.kingdom;

                            if (kingdom2 != null && kingdom != kingdom2 && !kingdom.isEnemy(kingdom2) && !MoreGodPower.KingdomsOwnedByPlayer.Contains(kingdom2)
                                && !(MoreGodPower.ArmyKingdoms.ContainsKey(kingdom) && MoreGodPower.ArmyKingdoms[kingdom].ContainsKey(kingdom2))
                                && (kingdom.data.allianceID == null || kingdom.data.allianceID != kingdom2.data.allianceID))
                            {
                                actor.CallMethod("setTileTarget", actor.city.getTile());
                                actor.current_path.Clear();
                                __result = ExecuteEvent.False;
                                if (!MoreGodPower.ArmyKingdoms.ContainsKey(kingdom))
                                {
                                    if (!MoreGodPower.KingdomsTryArmy.ContainsKey(kingdom))
                                    {
                                        MoreGodPower.KingdomsTryArmy.Add(kingdom, new List<Kingdom>());
                                    }
                                    MoreGodPower.KingdomsTryArmy[kingdom].Add(kingdom2);
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
        public static void setActive_Postfix(bool pActive, ScrollWindow __instance)
        {
            if (!pActive && __instance.screen_id.Equals("WorldLowControlWindow"))
            {
                showWorldLow = false;
            }
        }
        public static bool clearCityZones_Prefix()
        {
            if (PowerButtons.GetToggleValue("领土完整"))
            {
                return false;
            }
            return true;
        }
        private static MapObjectType objectType;
        public static void canAttackTarget_Postfix(BaseSimObject pTarget, BaseSimObject __instance, ref bool __result)
        {
            if (PowerButtons.GetToggleValue("异族统治") && PowerButtons.GetToggleValue("异族占领"))
            {
                objectType = __instance.objectType;
                if (!MapBox.instance.worldLaws.world_law_angry_civilians.boolVal && __result && objectType == MapObjectType.Actor)
                {
                    objectType = pTarget.objectType;
                    if (objectType == MapObjectType.Actor)
                    {
                        ProfessionAsset professionAsset = __instance.a.professionAsset;
                        ProfessionAsset professionAsset2 = pTarget.a.professionAsset;
                        if (professionAsset.is_civilian || professionAsset2.is_civilian)
                        {
                            __result = false;
                        }
                    }
                    else if (objectType == MapObjectType.Building)
                    {
                        Building building2 = pTarget.b;
                        BuildingAsset asset = building2.asset;
                        if (asset.cityBuilding && !asset.tower && pTarget.kingdom != null && pTarget.kingdom.asset.civ)//&& !stats2.isRuin 
                        {
                            __result = false;
                        }
                    }
                }
            }
        }


        public static UnitGroup unitGroup;
        public static void addUnit_Postfix(Actor pActor, UnitGroup __instance)
        {
            pActor.city = __instance.city;
        }
        public static void setKingdom_Prefix(City __instance)
        {
            if (__instance.army != null)
            {
                unitGroup = __instance.army;
                if (unitGroup.city != __instance)
                {
                    unitGroup.disband();
                    Reflection.SetField<bool>(unitGroup, "alive", true);
                    unitGroup.city = __instance;
                }
            }
        }
        public static bool execute_BehVerifierAttackZone_Prefix(Actor pActor, ref BehResult __result)
        {
            if (pActor.city != null && pActor.city.army != null)
            {
                unitGroup = pActor.unit_group;
                if (pActor.city.army != unitGroup)
                {
                    __result = BehResult.Stop;
                    return false;
                }
            }
            return true;
        }
        public static bool updateConquest_Prefix(Actor pActor, City __instance)
        {
            if (PowerButtons.GetToggleValue("异族占领"))
            {
                kingdom = (Kingdom)Traverse.Create(__instance).Field("kingdom").GetValue();
                if (pActor.kingdom.isCiv() && (pActor.kingdom == kingdom || pActor.kingdom.isEnemy(kingdom)))
                {
                    __instance.addCapturePoints(pActor, 1);
                }
                return false;
            }
            return true;
        }


        public static List<TileZone> cityZones = new();
        public static bool execute_CityBehBorderGrowth_Prefix(City pCity, ref BehResult __result)//领土限制
        {
            cityZones = pCity.zones;
            if (Main.moreSettings["村庄领土上限"] != 0 && pCity.getPopulationTotal() > Main.moreSettings["村庄领土上限"])
            {
                __result = BehResult.Stop;
                return false;
            }
            return true;
        }
        public static bool change_Prefix(string pRes, CityStorage __instance, ref int __result, int pAmount = 1)//资源限制
        {
            if (Main.moreSettings["村庄资源上限"] != 0)
            {
                if (DebugConfig.isOn(DebugOption.CityInfiniteResources))
                {
                    pAmount = Main.moreSettings["村庄资源上限"];
                }
                if (!__instance.resources.ContainsKey(pRes))
                {
                    __result = (int)__instance.CallMethod("addNew", pRes, pAmount);
                }
                else
                {
                    CityStorageSlot cityStorageSlot = __instance.resources[pRes];
                    cityStorageSlot.amount += pAmount;
                    if (cityStorageSlot.amount > Main.moreSettings["村庄资源上限"])
                    {
                        __instance.resources[pRes].amount = Main.moreSettings["村庄资源上限"];
                    }
                    __result = cityStorageSlot.amount;
                }
                return false;
            }
            return true;
        }
        #region 禁止获取装备
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "giveItem")]
        public static bool giveItem(Actor pActor, List<ItemData> pItems, City pCity, ref bool __result)
        {
            if (pItems.Count == 0)
            {
                __result = false;
                return false;
            }
            if (pActor == null || !pActor.Any()) { __result = false; return false; }
            if (pCity == null) { __result = false; return false; }

            pCity.kingdom.data.get("ProhibitgiveItemKing", out bool flag, false);
            if (flag && pActor.isKing())
            {
                __result = false;
                return false;
            }
            pCity.kingdom.data.get("ProhibitgiveItemLeader", out flag, false);
            if (flag && pActor.isCityLeader())
            {
                __result = false;
                return false;
            }
            pCity.kingdom.data.get("ProhibitgiveItemWarrior", out flag, false);
            pCity.kingdom.data.get("ProhibitgiveItemGroupLeader", out bool flag2, false);
            if (flag && pActor.unit_group != null)
            {
                __result = false;
                return false;
            }
            else if (flag2 && pActor.is_group_leader)
            {
                __result = false;
                return false;
            }
            return true;


        }
        #endregion
        #region 禁止zone增长
        [HarmonyPrefix]
        [HarmonyPatch(typeof(City), "canGrowZones")]
        public static bool canGrowZones(City __instance, ref bool __result)
        {
            __instance.data.get("ZoneGrowth", out bool flag, true);
            __instance.kingdom.data.get("ZoneGrowth", out bool flag2, true);
            if (!flag || !flag2)
            {
                __result = false;
                return false;
            }

            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BehCheckBuildCity), "execute")]
        public static bool execute(BehCheckBuildCity __instance, Actor pActor, ref BehResult __result)
        {
            if (pActor.Any() && pActor.city != null)
            {
                pActor.city.data.get("ZoneGrowth", out bool flag, true);
                pActor.city.kingdom.data.get("ZoneGrowth", out bool flag2, true);
                if (!flag || !flag2)
                {

                    __result = BehResult.Stop;
                    return false;
                }

            }
            return true;
        }


        #endregion



        #endregion

    }
}

