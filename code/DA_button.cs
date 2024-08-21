using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diplomacy_Army.Utils;
using NCMS.Utils;
using ReflectionUtility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Diplomacy_Army
{
      public class DA_button
      {
            public static ToggleIcon PVZPublicIcon;

            public static Dictionary<string, int> PlantSuns = new();
            public static bool action_Unit(WorldTile pTile, GodPower pPower) { AssetManager.powers.CallMethod("spawnUnit", pTile, pPower); return true; }
            public static bool action_Drop(WorldTile pTile, GodPower pPower) { AssetManager.powers.CallMethod("spawnDrops", pTile, pPower); return true; }
            public static bool action_1(WorldTile pTile, GodPower pPower) { AssetManager.powers.CallMethod("loopWithCurrentBrushPower", pTile, pPower); return true; }
            public static bool action_0(WorldTile pTile, GodPower pPower) { return true; }
            public static bool action_Unit(WorldTile pTile, string pPower) { AssetManager.powers.CallMethod("spawnUnit", pTile, pPower); return true; }
            public static bool action_Drop(WorldTile pTile, string pPower) { AssetManager.powers.CallMethod("spawnDrops", pTile, pPower); return true; }
            public static bool action_1(WorldTile pTile, string pPower) { AssetManager.powers.CallMethod("loopWithCurrentBrushPower", pTile, pPower); return true; }
            public static bool action_0(WorldTile pTile, string pPower) { return true; }
            public static PowerActionWithID PWspawnUnit = new(action_Unit);
            public static PowerActionWithID PWspawnDrops = new(action_Drop);
            public static PowerActionWithID PWloopWithCurrentBrushPower = new(action_1);
            public static PowerActionWithID PWfalse = new(action_0);
            public static PowerAction PspawnUnit = new(action_Unit);
            public static PowerAction PspawnDrops = new(action_Drop);
            public static PowerAction PloopWithCurrentBrushPower = new(action_1);
            public static PowerAction Pfalse = new(action_0);
            public static int index = 6;
            public static float x = 144f + 36 * (index / 2);
            public static float y = 18f - 36 * (index % 2);
            public static void init()
            {
                  GameObject line = new("DALine");
                  var lineRTF = line.AddComponent<RectTransform>();
                  line.AddComponent<CanvasRenderer>();
                  line.AddComponent<Image>().sprite = Resources.Load<Sprite>("ui/DAline.png");
                  lineRTF.sizeDelta = new Vector2(6, 86);
                  var newLine1 = GameObject.Instantiate(line, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  PowerButtons.CreateButton("DA_modder2", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.贝伦帝国.jpg"),
            "原作者", "原作者：贝伦帝国", new Vector2(72, 18), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  PowerButtons.CreateButton("DA_modder1", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.other.modder.png"),
            "作者简介", "作者:空星漫漫", new Vector2(72, -18), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow(DA_modder.wid));

                  newLine1.transform.localPosition = new Vector2(108f, newLine1.transform.localPosition.y - 6f);

                  PowerButtons.CreateButton("DA_显示外交消息", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "显示外交消息" + ".jpg"),
            "显示外交消息", "显示外交消息", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();

                  PowerButtons.CreateButton("DA_显示交战", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "关闭显示交战" + ".jpg"),
            "关闭显示交战", "关闭显示交战", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_显示条约", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "显示条约" + ".jpg"),
            "显示条约", "显示条约", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("显示原版铭牌", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "显示原版铭牌" + ".jpg"),
"显示原版铭牌", "显示原版铭牌", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_更多法则", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "更多法则" + ".jpg"),
            "更多法则", "打开更多法则", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("MoreRules")); update();
                  PowerButtons.CreateButton("DA_更多设置", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "更多设置" + ".jpg"),
            "全局设置", "打开全局设置", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("MoreSettings")); update();
                  PowerButtons.CreateButton("DA_自动内存清理", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.CleanMemorytoSave.png"),
            "自动内存清理", "", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DAdebug", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconDebug.png"),
"debug", "debug", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, () => Windows.ShowWindow("debug")); update(); update(); ; update();

                  var newLine2 = GameObject.Instantiate(line, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  newLine2.transform.localPosition = new Vector2(400f, newLine1.transform.localPosition.y - 6f);
                  PowerButtons.CreateButton("资源设置", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "资源设置" + ".jpg"),
"资源设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("Window_ResourcesSettings")); update();

                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "资源添加", "资源添加", "资源添加到全国每个城市"); update();

                  PowerButtons.CreateButton("特质设置", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.iconRainGammaEdit.png"),
"特质设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, () => clickTraitEditorRainButton(PowerLibrary.traits_gamma_rain_edit.id)); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "特质添加", "特质添加", "特质添加到全国每个军人身上"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "特质删除", "特质删除", "特质从全国每个军人身上去除"); update(); update();
                  PowerButtons.CreateButton("装备设置", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.DAweapons.png"),
"装备设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("ItemSettings")); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备添加", "装备添加", "装备添加"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备删除", "装备删除", "装备删除"); update(); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "城市地块拓展开关", "城市地块拓展", "城市地块拓展开关"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "国家地块拓展开关", "国家地块拓展", "国家地块拓展开关"); update();
                        PowerButtons.CreateButton("DA_装备禁止获取", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "装备禁止获取" + ".jpg"),
                  "装备禁止获取", "打开装备禁止获取参数设置窗口", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("ProhibitgiveItem")); update();
                        CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备禁止获取", "装备禁止获取", "点击城市或国家设置装备禁止获取的参数"); update();
            }
            public static void update()
            {
                  index++;
                  x = 144f + 36 * (index / 2);
                  y = 18f - 36 * (index % 2);
            }
            public static bool clickTraitEditorRainButton(string pPowerId)
            {
                  Config.selected_trait_editor = pPowerId;
                  ScrollWindow.showWindow("trait_rain_editor");
                  return true;
            }
            public static void CreateNewActiveGodpower(Transform pParent, string powerID, string pSprite, string pDescription, UnityAction pCall = null)
            {
                  GodPower godPower = new()
                  {
                        id = powerID,
                        name = powerID,
                        unselectWhenWindow = true
                  };
                  switch (powerID)
                  {
                        case "资源添加":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToAddResources));
                              break;
                        case "特质添加":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToAddTraits));
                              break;
                        case "特质删除":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToRemoveTraits));
                              break;
                        case "装备添加":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToAddItems));
                              break;
                        case "装备删除":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToRemoveItem));
                              break;
                        case "城市地块拓展开关":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(CityZoneGrowth));
                              break;
                        case "国家地块拓展开关":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(KingdomZoneGrowth));
                              break;
                        case "装备禁止获取":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(GetItemSwitch));
                              break;
                        default:
                              return;
                  }
                  AssetManager.powers.add(godPower);
                  CreateNewButton(index, pParent, pSprite, godPower, pDescription, pCall, PowerButtonType.Active);
            }
            public static GameObject CreateNewButton(int index, Transform pParent, string pSprite, GodPower pID, string pDescription, UnityAction pCall = null, PowerButtonType type = PowerButtonType.Active)
            {
                  ((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name, pID.name);
                  ((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name + " Description", pDescription);
                  // float x = 144f + 36 * (index / 2);
                  // float y = 18f - 36 * (index % 2);
                  Sprite sprite = Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + pSprite + ".jpg");
                  GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("SettingsButton"), pParent);
                  gameObject2.GetComponent<PowerButton>().type = type;
                  Reflection.SetField<GodPower>(gameObject2.GetComponent<PowerButton>(), "godPower", pID);
                  gameObject2.transform.name = pID.id;
                  gameObject2.transform.localScale = new Vector2(1f, 1f);
                  gameObject2.transform.localPosition = new Vector2(x, y);
                  gameObject2.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
                  gameObject2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                  if (pCall != null)
                  {
                        gameObject2.GetComponent<Button>().onClick.AddListener(pCall);
                  }
                  gameObject2.SetActive(true);
                  NewFunction.localizedText.Add(pID.name, pID.name);
                  NewFunction.localizedText.Add(pID.name + " Description", pDescription);
                  return gameObject2;
            }
            public static bool CityZoneGrowth(WorldTile pTile, string pPower)
            {
                  City city = pTile.zone.city;
                  if (city == null)
                  {
                        return true;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  city.data.get("ZoneGrowth", out bool flag, true);
                  city.data.set("ZoneGrowth", !flag);

                  string text;
                  if (!flag)
                  {
                        text = "开启";
                  }
                  else
                  {
                        text = "关闭";
                  }
                  NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}", "地块拓展开关状态：" + text);
                  return true;
            }
            public static bool GetItemSwitch(WorldTile pTile, string pPower)
            {
                  City city = pTile.zone.city;
                  if (city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  kingdom.data.set("ProhibitgiveItemKing", PowerButtons.GetToggleValue("国王装备禁用"));
                  kingdom.data.set("ProhibitgiveItemLeader", PowerButtons.GetToggleValue("领主装备禁用"));
                  kingdom.data.set("ProhibitgiveItemWarrior", PowerButtons.GetToggleValue("城市士兵装备禁用"));
                  kingdom.data.set("ProhibitgiveItemGroupLeader", PowerButtons.GetToggleValue("将军装备禁用"));
                  kingdom.data.set("ProhibitgiveItemGroupLeader", true);
                  NewFunction.LogNewMessage(kingdom, "国家", "装备禁止获取的参数已设置");
                  return true;
            }
            public static bool KingdomZoneGrowth(WorldTile pTile, string pPower)
            {
                  if (pTile.zone.city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  // foreach (City city in kingdom.cities)
                  // {
                  //       city.data.get("ZoneGrowth", out flag, true);
                  //       city.data.set("ZoneGrowth", !flag);
                  // }
                  kingdom.data.get("ZoneGrowth", out bool flag,true);
                  kingdom.data.set("ZoneGrowth", !flag);
                  string text;
                  if (!flag)
                  {
                        text = "开启";
                  }
                  else
                  {
                        text = "关闭";
                  }
                  NewFunction.LogNewMessage(kingdom, "国家", "地块拓展开关状态：" + text);
                  return true;
            }
            public static bool tryToAddResources(WorldTile pTile, string pPower)
            {
                  if (pTile.zone.city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  foreach (City city in kingdom.cities)
                  {
                        foreach (var resource in Main.resourceSettings.Keys)
                        {
                              city.data.storage.change(resource, Main.resourceSettings[resource]);
                        }
                  }
                  NewFunction.LogNewMessage(kingdom, "国家", "成功添加资源");
                  return true;
            }
            public static bool tryToAddTraits(WorldTile pTile, string pPower)
            {
                  if (pTile.zone.city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  List<string> pList = PlayerConfig.instance.data.trait_editor_gamma;
                  foreach (City city in kingdom.cities)
                  {
                        foreach (Actor act in city.professionsDict[UnitProfession.Warrior])
                        {
                              if (act.Any())
                              {
                                    if (pList.Count == 0) { return false; }
                                    int i = 0;
                                    while (i < pList.Count)
                                    {
                                          string pID = pList[i];
                                          if (AssetManager.traits.get(pID) == null) { pList.RemoveAt(i); }
                                          else { i++; }
                                    }
                                    if (act.asset.can_edit_traits)
                                    {
                                          foreach (string pTrait in pList)
                                          {
                                                act.addTrait(pTrait);
                                          }
                                          act.startShake(0.3f, 0.1f, true, true);
                                          act.startColorEffect(ActorColorEffect.White);
                                    }
                              }
                        }
                  }
                  NewFunction.LogNewMessage(kingdom, "国家军队", "成功添加特质");
                  return true;
            }
            public static bool tryToAddItems(WorldTile pTile, string pPower)
            {
                  City city = pTile.zone.city;
                  if (city == null)
                  {
                        return false;
                  }

                  Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;

                  List<Actor> actorsToAddItems = new List<Actor>();
                  if (PowerButtons.GetToggleValue("ChooseCityWarrior"))
                  {
                        actorsToAddItems.AddRange(city.professionsDict[UnitProfession.Warrior]);
                        NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "士兵成功添加装备！");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseAllWarrior"))
                  {
                        foreach (City c in kingdom.cities)
                        {
                              actorsToAddItems.AddRange(c.professionsDict[UnitProfession.Warrior]);
                        }
                        NewFunction.LogNewMessage(kingdom, "装备已到达...", "士兵成功添加装备");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseCityGeneral"))
                  {
                        City c = pTile.zone.city;
                        if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
                        {
                              actorsToAddItems.Add(c.army.groupLeader);
                              NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军成功添加装备");
                        }
                        else
                        {
                              NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军添加装备失败");
                        }

                  }
                  else if (PowerButtons.GetToggleValue("ChooseAllGeneral"))
                  {
                        foreach (City c in kingdom.cities)
                        {
                              if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
                              {
                                    actorsToAddItems.Add(c.army.groupLeader);
                              }
                        }
                        NewFunction.LogNewMessage(kingdom, "装备已到达... ", " 将军成功添加装备");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseKing"))
                  {
                        if (kingdom.king != null && kingdom.king.Any())
                        {
                              actorsToAddItems.Add(kingdom.king);
                              NewFunction.LogNewMessage(kingdom, "装备已到达...", " 国王成功添加装备");
                        }

                  }
                  else if (PowerButtons.GetToggleValue("ChooseLeader"))
                  {
                        if (city.leader != null && city.leader.Any())
                        {
                              actorsToAddItems.Add(city.leader);
                              NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "领主成功添加装备");
                        }

                  }

                  foreach (Actor act in actorsToAddItems)
                  {
                        if (act.Any())
                        {
                              foreach (ItemAsset item in AssetManager.items.list)
                              {
                                    if (item.id[0] == '_' || NewWindow.wrongItems.Contains(item.id) || item.materials.Count <= 0)
                                    {
                                          continue;
                                    }

                                    foreach (string material in item.materials)
                                    {
                                          string key = material.Length > 0 ? $"{item.id}_DA_{material}" : $"{item.id}_DA";
                                          if (PowerButtons.CustomButtons.ContainsKey(key) && PowerButtons.GetToggleValue(key))
                                          {
                                                ItemData data = ItemGenerator.generateItem(item, material, World.world.mapStats.year, act.kingdom, act.getName(), 1, act);
                                                data.modifiers.Clear();
                                                ActorEquipmentSlot slot = act.equipment.getSlot(item.equipmentType);
                                                slot.setItem(data);
                                                act.setStatsDirty();
                                                act.startShake(0.3f, 0.1f, true, true);
                                                act.startColorEffect(ActorColorEffect.White);
                                          }
                                    }
                              }
                        }
                  }

                  return true;
            }
            public static bool tryToRemoveTraits(WorldTile pTile, string pPower)
            {
                  if (pTile.zone.city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  List<string> pList = PlayerConfig.instance.data.trait_editor_gamma;
                  foreach (City city in kingdom.cities)
                  {
                        foreach (Actor act in city.professionsDict[UnitProfession.Warrior])
                        {
                              if (act.Any())
                              {
                                    if (pList.Count == 0) { return false; }
                                    int i = 0;
                                    while (i < pList.Count)
                                    {
                                          string pID = pList[i];
                                          if (AssetManager.traits.get(pID) == null) { pList.RemoveAt(i); }
                                          else { i++; }
                                    }
                                    if (act.asset.can_edit_traits)
                                    {
                                          foreach (string pTrait in pList)
                                          {
                                                act.removeTrait(pTrait);
                                          }
                                          act.startShake(0.3f, 0.1f, true, true);
                                          act.startColorEffect(ActorColorEffect.White);
                                    }
                              }
                        }
                  }
                  NewFunction.LogNewMessage(kingdom, "国家军队", "成功删除特质");
                  return true;
            }
            public static bool tryToRemoveItem(WorldTile pTile, string pPower)
            {
                  City city = pTile.zone.city;
                  if (city == null)
                  {
                        return false;
                  }
                  var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
                  List<Actor> actorsToAddItems = new List<Actor>();
                  if (PowerButtons.GetToggleValue("ChooseCityWarrior"))
                  {
                        actorsToAddItems.AddRange(city.professionsDict[UnitProfession.Warrior]);
                        NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "士兵成功添加装备！");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseAllWarrior"))
                  {
                        foreach (City c in kingdom.cities)
                        {
                              actorsToAddItems.AddRange(c.professionsDict[UnitProfession.Warrior]);
                        }
                        NewFunction.LogNewMessage(kingdom, "装备已到达...", "士兵成功添加装备");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseCityGeneral"))
                  {
                        City c = pTile.zone.city;
                        if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
                        {
                              actorsToAddItems.Add(c.army.groupLeader);
                              NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军成功添加装备");
                        }
                        else
                        {
                              NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军添加装备失败");
                        }

                  }
                  else if (PowerButtons.GetToggleValue("ChooseAllGeneral"))
                  {
                        foreach (City c in kingdom.cities)
                        {
                              if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
                              {
                                    actorsToAddItems.Add(c.army.groupLeader);
                              }
                        }
                        NewFunction.LogNewMessage(kingdom, "装备已到达... ", " 将军成功添加装备");
                  }
                  else if (PowerButtons.GetToggleValue("ChooseKing"))
                  {
                        if (kingdom.king != null && kingdom.king.Any())
                        {
                              actorsToAddItems.Add(kingdom.king);
                              NewFunction.LogNewMessage(kingdom, "装备已到达...", " 国王成功添加装备");
                        }

                  }
                  else if (PowerButtons.GetToggleValue("ChooseLeader"))
                  {
                        if (city.leader != null && city.leader.Any())
                        {
                              actorsToAddItems.Add(city.leader);
                              NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "领主成功添加装备");
                        }

                  }

                  foreach (Actor act in actorsToAddItems)
                  {
                        if (act.Any())
                        {
                              List<ActorEquipmentSlot> list = ActorEquipment.getList(act.equipment);
                              if (list == null)
                              {
                                    continue;
                              }
                              for (int i = 0; i < list.Count; i++)
                              {
                                    ActorEquipmentSlot actorEquipmentSlot = list[i];
                                    if (actorEquipmentSlot.data != null)
                                    {
                                          if (PowerButtons.CustomButtons.ContainsKey($"{actorEquipmentSlot.data.id}_DA") && PowerButtons.GetToggleValue($"{actorEquipmentSlot.data.id}_DA"))
                                          {
                                                actorEquipmentSlot.emptySlot();
                                                act.setStatsDirty();
                                                act.dirty_sprite_item = true;
                                                act.startShake(0.3f, 0.1f, true, true);
                                                act.startColorEffect(ActorColorEffect.White);
                                          }
                                          else if (PowerButtons.CustomButtons.ContainsKey($"{actorEquipmentSlot.data.id}_DA_{actorEquipmentSlot.data.material}"))
                                          {
                                                if (PowerButtons.GetToggleValue($"{actorEquipmentSlot.data.id}_DA_{actorEquipmentSlot.data.material}"))
                                                {
                                                      actorEquipmentSlot.emptySlot();
                                                      act.setStatsDirty();
                                                      act.dirty_sprite_item = true;
                                                      act.startShake(0.3f, 0.1f, true, true);
                                                      act.startColorEffect(ActorColorEffect.White);
                                                }

                                          }

                                    }
                              }
                        }



                  }


                  NewFunction.LogNewMessage(kingdom, "国家军队", "成功删除装备");
                  return true;
            }

      }
}