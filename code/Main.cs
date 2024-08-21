using System;
using System.Reflection;
using System.Collections.Generic;
using NCMS;
using UnityEngine;
using UnityEngine.UI;
using ReflectionUtility;
using HarmonyLib;
using ai;
using ai.behaviours;
using Newtonsoft.Json.Linq;
using HarmonyLib;
using System.ComponentModel;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;
using Diplomacy_Army.Utils;
using Diplomacy_Diplomacy;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
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

        [JsonObject(MemberSerialization.OptIn)]
        public class DAStorage
        {
            public Guid Id { get; set; }
            [JsonProperty]
            public string Name = "NewStorage";
            public int nom = 1;
            public int num = 0;
            public int RS = 0;
        }
        #endregion

        public void Awake()
        {
            if (!System.IO.Directory.Exists(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Diplomacy"))
            {
                System.IO.Directory.CreateDirectory(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Diplomacy");
            }
            string text0 = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Diplomacy", "numofyears.json");
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
            Personality();


            OpinionAsset opinionAsset2 = new()
            {
                id = "opinion_vassal",
                translation_key = "opinion_vassal",
                calc = delegate (Kingdom pMain, Kingdom pTarget)
                {
                    int result = 0;
                    if (MoreGodPower.Vassals.ContainsKey(pMain) && MoreGodPower.Vassals[pMain].Contains(pTarget))
                    {
                        result = 500;
                    }
                    return result;
                }
            };
            AssetManager.opinion_library.add(opinionAsset2);
            Localization.addLocalization("opinion_vassal", "谁会跟自己的附庸过不去呢");
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
            update.updateTreaty();
            update.updateCities();
            update.updateVassal();
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
        #region 政治意识形态及国家特性
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ClanManager), "tryPlotJoinAlliance")]
        public static bool tryPlotJoinAlliance(Actor pActor, PlotAsset pPlotAsset)
        {
            string personality = pActor.kingdom.king.s_personality.id;
            if (personality == "Conservatism")
            {
                if (Toolbox.randomChance(0.3f))
                {
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
                __result = 0.6f;
                return false;
            }
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag)
            {
                __result = 0.4f;
                return false;
            }

            // float num = 0f + __instance.race.civ_base_army_mod;
            // float armyMaxLeaderPercentage = __instance.getArmyMaxLeaderPercentage();
            // float num2 = num + armyMaxLeaderPercentage;
            // float num3 = 0f;
            // if (__instance.getCulture() != null)
            // {
            //     num3 = __instance.getCulture().stats.bonus_max_army.value;
            // }
            // __result = num2 + num3 + (int)__instance.kingdom.stats.bonus_max_army.value;
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

            return true;
        }
        #endregion
        #region 全国军队腐败
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
                __instance.stats[S.damage] -= (float)(__instance.stats[S.damage] * 0.5);
                __instance.stats[S.armor] -= (float)(__instance.stats[S.armor] * 0.4);
                __instance.stats[S.speed] -= (float)(__instance.stats[S.speed] * 0.5);
            }
            __instance.kingdom.data.get("SteelFortress", out flag, false);
            if (flag)
            {
                __instance.stats[S.damage] -= (float)(__instance.stats[S.damage] * 0.8);
                __instance.stats[S.armor] += (float)(__instance.stats[S.armor] * 2);
                __instance.stats[S.knockback_reduction] += (float)(__instance.stats[S.knockback_reduction] * 2);
                __instance.stats[S.speed] -= (float)(__instance.stats[S.speed] * 0.2);
                __instance.stats[S.health] += (float)(__instance.stats[S.health] * 0.3);
                __instance.data.health += (int)(__instance.stats[S.health] * 0.3);
            }
            __instance.kingdom.data.get("EliteLegion", out flag, false);
            if (flag)
            {
                __instance.stats[S.damage] += (float)(__instance.stats[S.damage] * 0.5);
                __instance.stats[S.armor] += (float)(__instance.stats[S.armor] * 0.5);
                __instance.stats[S.health] += (float)(__instance.stats[S.health] * 0.3);
                __instance.data.health += (int)(__instance.stats[S.health] * 0.3);
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
            return true;
        }
        #endregion
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapText), "showTextKingdom")]
        public static bool showTextKingdom(MapText __instance, Kingdom pKingdom)
        {

            // __instance.text = Instantiate(__instance.text, Vector3.zero, Quaternion.identity);
            // __instance.text.transform.SetParent(canvasParent);
            if (PowerButtons.GetToggleValue("显示原版铭牌"))
            {
                return true;
            }
            if (__instance.text.gameObject.GetComponent<Outline>() == null)
            {
                Outline outline = __instance.text.gameObject.AddComponent<Outline>();
                outline.effectColor = outlineColor;
                outline.effectDistance = new Vector2(outlineWidth, -outlineWidth);
            }

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
            __instance.text.fontStyle = FontStyle.Bold;
            __instance.text.fontSize = 15;
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
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityBehProduceUnit), "isCityCanProduceUnits")]
        public static bool isCityCanProduceUnits(City pCity, ref bool __result)
        {
            if (Main.moreSettings["村庄人口上限"] != 0 && pCity.getPopulationTotal() > Main.moreSettings["村庄人口上限"])
            {
                __result = false;
                return false;
            }
            if (Main.moreSettings["村庄人口下限"] != 0 && pCity.getPopulationTotal() < Main.moreSettings["村庄人口下限"])
            {
                __result = true;
                return false;
            }
            return true;

        }


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
            else if (flag2&&pActor.is_group_leader)
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

