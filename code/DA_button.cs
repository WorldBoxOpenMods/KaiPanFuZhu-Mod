using System;
using System.Collections.Generic;
using System.IO;
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

            public static int index = 6;
            public static float x = 144f + 36 * (index / 2);
            public static float y = 18f - 36 * (index % 2);
            public static void init()
            {
                  GreateButton();
                  GreateLine();
            }
            public static void GreateLine()
            {
                  GameObject line = new("DALine");
                  var lineRTF = line.AddComponent<RectTransform>();
                  line.AddComponent<CanvasRenderer>();
                  line.AddComponent<Image>().sprite = Resources.Load<Sprite>("ui/DAline.png");
                  lineRTF.sizeDelta = new Vector2(6, 86);
                  var newLine1 = GameObject.Instantiate(line, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  newLine1.transform.localPosition = new Vector2(108f, newLine1.transform.localPosition.y - 6f);
                  var newLine2 = GameObject.Instantiate(line, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  newLine2.transform.localPosition = new Vector2(460f, newLine1.transform.localPosition.y - 6f);
            }
            public static void GreateButton()
            {
                  PowerButtons.CreateButton("DA_modder2", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.贝伦帝国.jpg"),
                  "原作者", "原作者：贝伦帝国", new Vector2(72, 18), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform);
                  PowerButtons.CreateButton("DA_modder1", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.other.modder.png"),
                  "作者简介", "作者:空星漫漫", new Vector2(72, -18), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow(DA_modder.wid));
                  update(); update();

                  PowerButtons.CreateButton("DA_显示外交消息", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "显示外交消息" + ".jpg"),
                  "显示外交消息", "显示外交消息", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_显示交战", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "关闭显示交战" + ".jpg"),
                  "关闭显示交战", "关闭显示交战", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_关闭显示附庸颜色", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "关闭显示附庸" + ".jpg"),
                  "关闭显示附庸颜色", "关闭显示附庸颜色", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_显示条约", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "显示条约" + ".jpg"),
                  "显示条约", "显示条约", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("显示原版铭牌", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "显示原版铭牌" + ".jpg"),
                  "显示原版铭牌", "显示原版铭牌", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DA_更多法则", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "更多法则" + ".jpg"),
                  "更多法则", "打开更多法则", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("MoreRules")); update();
                  PowerButtons.CreateButton("DA_更多设置", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "更多设置" + ".jpg"),
                  "全局设置", "打开全局设置", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("MoreSettings")); update();
                  PowerButtons.CreateButton("DA_自动内存清理", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.CleanMemorytoSave.png"),
                  "自动内存清理", "", new Vector2(x, y), ButtonType.Toggle, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform); update();
                  PowerButtons.CreateButton("DAdebug", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.iconDebug.png"),
                  "debug", "debug", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, () => Windows.ShowWindow("debug")); update();
                  PowerButtons.CreateButton("保存", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "保存设置" + ".jpg"),
                  "保存设置", "保存所有的设置", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => MoreGodPower.AddToDASet()); update();

                  update(); update();

                  PowerButtons.CreateButton("资源设置", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "资源设置" + ".jpg"),
                  "资源设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("Window_ResourcesSettings")); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "资源添加", "资源添加", "资源添加到全国每个城市"); update();
                  PowerButtons.CreateButton("特质设置", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.iconRainGammaEdit.png"),
                  "特质设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, () => MoreGodPower.clickTraitEditorRainButton(PowerLibrary.traits_gamma_rain_edit.id)); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "特质添加", "特质添加", "特质添加到全国每个军人身上"); update();
                  PowerButtons.CreateButton("装备设置", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.DAweapons.png"),
                  "装备设置", "", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("ItemSettings")); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备添加", "装备添加", "装备添加"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "特质删除", "特质删除", "特质从全国每个军人身上去除"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备删除", "装备删除", "装备删除"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "城市地块拓展开关", "城市地块拓展", "城市地块拓展开关"); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "国家地块拓展开关", "国家地块拓展", "国家地块拓展开关"); update();
                  PowerButtons.CreateButton("DA_装备禁止获取", Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "装备禁止获取" + ".jpg"),
                  "装备禁止获取", "打开装备禁止获取参数设置窗口", new Vector2(x, y), ButtonType.Click, pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, static () => Windows.ShowWindow("ProhibitgiveItem")); update();
                  CreateNewActiveGodpower(pvz_ui.CustomTabObjs["Diplomacy_Army"].transform, "装备禁止获取", "装备禁止获取", "点击城市或国家设置装备禁止获取的参数"); update();

            }
            public static void update()
            {
                  index++;
                  x = 144f + 36 * (index / 2);
                  y = 18f - 36 * (index % 2);
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
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.tryToAddResources));
                              break;
                        case "特质添加":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.tryToAddTraits));
                              break;
                        case "特质删除":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.tryToRemoveTraits));
                              break;
                        case "装备添加":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.tryToAddItems));
                              break;
                        case "装备删除":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.tryToRemoveItem));
                              break;
                        case "城市地块拓展开关":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.CityZoneGrowth));
                              break;
                        case "国家地块拓展开关":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.KingdomZoneGrowth));
                              break;
                        case "装备禁止获取":
                              godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(MoreGodPower.GetItemSwitch));
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
                  Sprite sprite = Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + pSprite + ".jpg");
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

      }
}