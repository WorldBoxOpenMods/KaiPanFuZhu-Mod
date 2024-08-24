using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMS;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ReflectionUtility;
using Diplomacy_Army;
using Diplomacy_Army.Utils;

namespace Diplomacy_Army
{
    class ItemModWindow : MonoBehaviour
    {
        private static GameObject content;
        private static GameObject scrollView;
        private static Vector2 originalSize;
        public static int currentButtonID;

        public static void init()
        {
            string wid = "ItemMod";
            pvz_ui.NewWindow(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["ItemMod"];
            originalSize = content.GetComponent<RectTransform>().sizeDelta;
            // // 设置 RectTransform 的大小
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, AssetManager.items.list.Count / 16 * originalSize.y + 240f) + originalSize;
            // 添加 GridLayoutGroup 并配置
            GridLayoutGroup layoutGroup = content.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            // 设置 RectTransform 的大小
            GameObject scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{wid}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);
            // 设置初始位置
            content.GetComponent<RectTransform>().localPosition = new Vector3(80f, -20f, 0);
            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();

            // PowerButton submitButton = PowerButtons.CreateButton(
            //     "重置",
            //     Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            //     "重置",
            //     "重置装备修饰词条",
            //     new Vector3(-118, -50),
            //     ButtonType.Toggle,
            //     content.transform,
            //     clearToggleButtons
            // );
            LoadItemMods();
        }

        private static void LoadItemMods()
        {
            // foreach(Transform child in content.transform)
            // {
            //     Destroy(child.gameObject);
            // }
            // content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, AssetManager.items_modifiers.list.Count/16*originalSize.y) + originalSize;

            // int index = 0;
            // int indexY = 0;
            foreach (ItemAsset mod in AssetManager.items_modifiers.list)
            {
                if (PowerButtons.CustomButtons.ContainsKey($"{mod.id}_modifier_DA"))
                {
                    PowerButtons.CustomButtons.Remove($"{mod.id}_modifier_DA");
                }
                Sprite iconSprite = Sprites.LoadSprite($"{Mod.Info.Path}/icon.png");
                if (mod.mod_type != "mastery")
                {
                    iconSprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.Icons.ItemIcons.{mod.id}.png");
                }
                PowerButtons.CreateButton(
                    $"{mod.id}_modifier_DA",
                    iconSprite,
                    mod.id,
                    mod.id,
                    new Vector2(0, 0),
                    ButtonType.Toggle,
                    content.transform
                // () => onModClick(mod)
                );
                // increaseIndex(ref index, ref indexY);
            }
        }

        private static void increaseIndex(ref int index, ref int indexY)
        {
            index++;
            if (index > 4)
            {
                index = 0;
                indexY++;
            }
        }

        public static void openWindow()
        {
            // currentButtonID = buttonID;
            // checkToggleButtons();
            Windows.ShowWindow("ItemMod");
        }

        private static void onModClick(ItemAsset mod)
        {
            if (!PowerButtons.GetToggleValue($"{mod.id}_modifier_DA") && NewWindow.itemModifiers.ContainsKey(currentButtonID.ToString()))
            {
                NewWindow.itemModifiers[currentButtonID.ToString()].Remove(mod);
            }
            else if (NewWindow.itemModifiers.ContainsKey(currentButtonID.ToString()))
            {
                NewWindow.itemModifiers[currentButtonID.ToString()].Add(mod);
            }
            else
            {
                NewWindow.itemModifiers.Add(currentButtonID.ToString(), new List<ItemAsset> { mod });
            }
        }

        // private static void checkToggleButtons()
        // {
        //     foreach(KeyValuePair<string, PowerButton> kv in PowerButtons.CustomButtons)
        //     {
        //         if (!kv.Key.Contains("_modifier_DA"))
        //         {
        //             continue;
        //         }
        //         string itemID = kv.Key.Remove(kv.Key.IndexOf("_modifier_DA"));
        //         ItemAsset asset = AssetManager.items_modifiers.get(itemID);
        //         if (!PowerButtons.GetToggleValue(kv.Key) && !NewWindow.itemModifiers[currentButtonID.ToString()].Contains(asset))
        //         {
        //             continue;
        //         }
        //         if (PowerButtons.GetToggleValue(kv.Key) && NewWindow.itemModifiers[currentButtonID.ToString()].Contains(asset))
        //         {
        //             continue;
        //         }
        //         PowerButtons.ToggleButton(kv.Key);
        //     }
        // }

        private static void clearToggleButtons()
        {
            foreach (KeyValuePair<string, PowerButton> kv in PowerButtons.CustomButtons)
            {
                if (!kv.Key.Contains("_modifier_DA"))
                {
                    continue;
                }
                if (!PowerButtons.GetToggleValue(kv.Key))
                {
                    continue;
                }
                string itemID = kv.Key.Remove(kv.Key.IndexOf("_modifier_DA"));
                ItemAsset asset = AssetManager.items_modifiers.get(itemID);
                NewWindow.itemModifiers[currentButtonID.ToString()].Remove(asset);
                PowerButtons.ToggleButton(kv.Key);
            }
        }
    }
}