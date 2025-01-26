using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCMS.Utils;
using NCMS;
using UnityEngine;
using UnityEngine.UI;
using Diplomacy_Army.Utils;
using static Diplomacy_Army.Main;
using Newtonsoft.Json;
using System.IO;
using Diplomacy_Army;

namespace Diplomacy_Army
{
    public class ItemSettingsWindow
    {
        public static GameObject content;
        private static Vector2 originalSize;
        public static List<string> wrongItems = new()
        {
            "base",
            "claws",
            "hands",
            "fire_hands",
            "jaws",
            "bite",
            "rocks",
            "snowball","Ballista_Arrows","stones"
        };
        public static void init()
        {
            ItemModWindow.init();
            ItemSettings();

        }

        public static void ItemSettings()
        {
            int index = 0;
            string wid = "ItemSettings";
            pvz_ui.NewWindows(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["ItemSettings"];
            originalSize = content.GetComponent<RectTransform>().sizeDelta;

            // 添加 GridLayoutGroup 并配置
            GridLayoutGroup layoutGroup = content.AddComponent<GridLayoutGroup>();
            layoutGroup.cellSize = new Vector2(30, 30);
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = 4;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.spacing = new Vector2(15, 5);
            GameObject scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{wid}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);
            // 设置 RectTransform 的大小
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, AssetManager.items.list.Count / 16 * originalSize.y + 860f) + originalSize;

            // 设置初始位置
            content.GetComponent<RectTransform>().localPosition = new Vector3(80f, -1000f, 0);

            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();
            PowerButton button = PowerButtons.CreateButton(
            "ChooseKing",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择国王",
            "装备将分配到国王对象",
            new Vector3(-118, 30),
            ButtonType.Toggle,
            content.transform,
            () => Toggle("ChooseKing")
        ); index++;
            button = PowerButtons.CreateButton(
                "ChooseLeader",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "选择领主",
                "装备将分配到领主对象",
                new Vector3(-118, 30),
                ButtonType.Toggle,
                content.transform,
                () => Toggle("ChooseLeader")
            ); index++;
            button = PowerButtons.CreateButton(
            "ChooseAllWarrior",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择全国士兵",
            "装备将分配到全国士兵",
            new Vector3(-118, -6),
            ButtonType.Toggle,
            content.transform,
            () => Toggle("ChooseAllWarrior")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseCityWarrior",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择城市中的士兵",
            "装备将分配到城市中的士兵",
            new Vector3(-118, -42),
            ButtonType.Toggle,
            content.transform,
            () => Toggle("ChooseCityWarrior")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseCityGeneral",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择城市中的旗手",
            "装备将分配到城市中的旗手",
            new Vector3(-118, -78),
            ButtonType.Toggle,
            content.transform,
            () => Toggle("ChooseCityGeneral")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseAllGeneral",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择全国的旗手",
            "装备将分配到全国旗手",
            new Vector3(-118, -108),
            ButtonType.Toggle,
            content.transform,
            () => Toggle("ChooseAllGeneral")
        ); index++;
            button = PowerButtons.CreateButton(
            "DA_itemEdit",
            Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/" + "装备编辑" + ".jpg"),
            "装备编辑",
            "编辑装备的词条",
            new Vector3(-118, -108),
            ButtonType.GodPower,
            content.transform,
            () => ItemModWindow.openWindow()
        ); index++;
            // 创建按钮
            foreach (ItemAsset item in AssetManager.items.list)
            {
                if (item.id[0] == '_' || wrongItems.Contains(item.id))
                {
                    continue;
                }
                if (item.materials.Count <= 0)
                {
                    if (PowerButtons.CustomButtons.ContainsKey($"{item.id}_DA"))
                    {
                        PowerButtons.CustomButtons.Remove($"{item.id}_DA");
                    }
                    index++;
                    PowerButtons.CreateButton(
                        $"{item.id}_DA",
                        Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}"),
                        item.id,
                        item.id,
                        new Vector2(0, 0),
                        ButtonType.Toggle,
                        content.transform
                    );

                    continue;
                }
                foreach (string material in item.materials)
                {
                    if (PowerButtons.CustomButtons.ContainsKey($"{item.id}_DA_{material}"))
                    {
                        PowerButtons.CustomButtons.Remove($"{item.id}_DA_{material}");
                    }
                    Sprite pSprite;
                    if (material != "base")
                    {
                        pSprite = Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}_{material}");
                    }
                    else
                    {
                        pSprite = Resources.Load<Sprite>($"ui/Icons/items/icon_{item.id}");
                    }
                    index++;
                    PowerButtons.CreateButton(
                        $"{item.id}_DA_{material}",
                        pSprite,
                        $"{item.id}_{material}",
                        $"{item.id}_{material}",
                        new Vector2(0, 0),
                        ButtonType.Toggle,
                        content.transform
                    );
                }
            }

        }
        public static void Toggle(string choice)
        {
            switch (choice)
            {
                case "ChooseCityGeneral":
                    if (PowerButtons.GetToggleValue("ChooseAllGeneral"))
                        PowerButtons.ToggleButton("ChooseAllGeneral");
                    break;
                case "ChooseAllGeneral":
                    if (PowerButtons.GetToggleValue("ChooseCityGeneral"))
                        PowerButtons.ToggleButton("ChooseCityGeneral");
                    break;
                case "ChooseAllWarrior":
                    if (PowerButtons.GetToggleValue("ChooseCityWarrior"))
                        PowerButtons.ToggleButton("ChooseCityWarrior");
                    break;
                case "ChooseCityWarrior":
                    if (PowerButtons.GetToggleValue("ChooseAllWarrior"))
                        PowerButtons.ToggleButton("ChooseAllWarrior");
                    break;
                case "ChooseKing":
                    break;
                default:
                    break;

            }
        }
    }
}