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
    public class NewWindow
    {
        public static GameObject content;
        public static Kingdom kingdom;
        public static int button = 0;
        public static int Settingsbutton = 0;
        public static int RSbutton = 0;
        public static Text contentText;
        public static Text MSText;
        public static float NYJG;
        public static void init()
        {
            ItemModWindow.init();
            KingdomWindow();
            MoreRules();
            MoreSettings();
            ResourcesSettings();
            ItemSettings();
            ProhibitgiveItem();

        }
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
        public static Dictionary<string, List<ItemAsset>> itemModifiers = new Dictionary<string, List<ItemAsset>>();
        private static Vector2 originalSize;
        public static void ItemSettings()
        {
            int index = 0;
            string wid = "ItemSettings";
            pvz_ui.NewWindow(wid, 0, "null", true);
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
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, AssetManager.items.list.Count / 16 * originalSize.y + 800f) + originalSize;

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
            () => toggle("ChooseKing")
        ); index++;
            button = PowerButtons.CreateButton(
                "ChooseLeader",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "选择领主",
                "装备将分配到领主对象",
                new Vector3(-118, 30),
                ButtonType.Toggle,
                content.transform,
                () => toggle("ChooseLeader")
            ); index++;
            button = PowerButtons.CreateButton(
            "ChooseAllWarrior",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择全国士兵",
            "装备将分配到全国士兵",
            new Vector3(-118, -6),
            ButtonType.Toggle,
            content.transform,
            () => toggle("ChooseAllWarrior")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseCityWarrior",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择城市中的士兵",
            "装备将分配到城市中的士兵",
            new Vector3(-118, -42),
            ButtonType.Toggle,
            content.transform,
            () => toggle("ChooseCityWarrior")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseCityGeneral",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择城市中的旗手",
            "装备将分配到城市中的旗手",
            new Vector3(-118, -78),
            ButtonType.Toggle,
            content.transform,
            () => toggle("ChooseCityGeneral")
        ); index++;
            button = PowerButtons.CreateButton(
            "ChooseAllGeneral",
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "选择全国的旗手",
            "装备将分配到全国旗手",
            new Vector3(-118, -108),
            ButtonType.Toggle,
            content.transform,
            () => toggle("ChooseAllGeneral")
        ); index++;
            button = PowerButtons.CreateButton(
            "DA_itemEdit",
            Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "装备编辑" + ".jpg"),
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
        public static void ResourcesSettings()
        {
            int index = 0;
            string wid = "Window_ResourcesSettings";
            ScrollWindow Window = pvz_ui.NewWindow(wid, 0, "null", true);
            Window.transform.Find("Background").Find("Scroll View").gameObject.SetActive(true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects[wid];
            originalSize = content.GetComponent<RectTransform>().sizeDelta;

            GameObject scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{wid}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);
            float ngjg = pvz_ui.CustomWindowTexts[wid].preferredHeight;
            NYJG = pvz_ui.CustomWindowTexts[wid].preferredHeight - ngjg;
            // 设置 RectTransform 的大小
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 3600f) + originalSize;

            // 设置初始位置
            content.GetComponent<RectTransform>().localPosition = new Vector3(100f, -40f, 0);

            GameObject DAInventoryContent = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/Window_ResourcesSettings/Background/Scroll View/Viewport/Content");
            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();


            float MinY = ((pvz_ui.CustomWindowTexts[wid].preferredHeight / 2) + 30) * -1;
            foreach (ResourceAsset resource in AssetManager.resources.list) { RSSetB(resource, content); }
        }
        public static void MoreSettings()
        {
            int index = 0;
            string wid = "MoreSettings";
            pvz_ui.NewWindow(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["MoreSettings"];
            foreach (string setting in Main.moreSettings.Keys) { MSSetB(setting, content); }
        }
        public static void MoreRules()
        {
            int index = 0;
            string wid = "MoreRules";
            pvz_ui.NewWindow(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["MoreRules"];
            PowerButtons.CreateButton("封锁边境", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "封锁边境", "封锁边境", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("异族占领", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "异族占领", "异族占领", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("异族统治", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "异族统治", "异族统治", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("领土完整", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "领土完整", "领土完整", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
        }
        public static void ProhibitgiveItem()
        {
            int index = 0;
            string wid = "ProhibitgiveItem";
            pvz_ui.NewWindow(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["ProhibitgiveItem"];
            PowerButtons.CreateButton("国王装备禁用", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "国王装备禁用", "点击一个王国，国王装备将无法从城市中获取", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("城市士兵装备禁用", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "城市士兵装备禁用", "点击一个城市，其的士兵装备将无法从城市中获取", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("将军装备禁用", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "城市将军装备禁用", "点击一个城市，其的将军装备将无法从城市中获取", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("领主装备禁用", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "领主装备禁用", "点击一个城市，其的领主装备将无法从城市中获取", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;

            // PowerButtons.CreateButton("强制改名", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            // "强制改名", "强制改名", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
        }
        public static void KingdomWindow()
        {
            int index = 0;
            string wid = "NewKingdomWindow";
            pvz_ui.NewWindow(wid, 0, "null", true);
            wid.RTF();

            content = pvz_ui.CustomWindowObjects["NewKingdomWindow"];
            PowerButton button = PowerButtons.CreateButton(
                "NewKingdomWindow",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "国家设置窗口",
                "国家设置窗口",
                new Vector3(-118, -80),
                ButtonType.Click,
                GameObject.Find($"Canvas Container Main/Canvas - Windows/windows/kingdom").transform,
                () => openWindow()
            );
            button.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            button.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            button = PowerButtons.CreateButton(
                "CorruptArmy",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.腐败的军队.png"),
                "全国军队腐败",
                "全国军队腐败",
                getPositionByIndex(index),
                ButtonType.Toggle,
                content.transform,
                () => toggleCorruptArmyActive(kingdom)
            );
            index++;
            button = PowerButtons.CreateButton(
                "SteelFortress",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.钢铁堡垒.png"),
                "钢铁堡垒",
                "增加200%防御和200%抗击退 减少80%伤害 20%移速 拥有60%动员率",
                getPositionByIndex(index),
                ButtonType.Toggle,
                content.transform,
                () => toggleSteelFortressActive(kingdom)
            );
            index++;
            button = PowerButtons.CreateButton(
                "EliteLegion",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.精锐军团.png"),
                "精锐军团",
                "增加50%伤害 50%防御 30%血量的军队 但仅有40%的动员率",
                getPositionByIndex(index),
                ButtonType.Toggle,
                content.transform,
                () => toggleEliteLegionActive(kingdom)
            );
            index++;
            // button.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            // button.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            // Image buttonBG = button.gameObject.GetComponent<Image>();
            // buttonBG.sprite = Mod.EmbededResources.LoadSprite(
            //     $"{Mod.Info.Name}.Resources.UI.backgroundTabButton.png"
            // );
            // Button buttonButton = button.gameObject.GetComponent<Button>();
            // buttonBG.rectTransform.localScale = Vector3.one;
        }
        public static void openWindow()
        {
            Windows.ShowWindow("NewKingdomWindow");


        }
        public static void toggleCorruptArmyActive(Kingdom kingdom)
        {
            kingdom.data.get("CorruptArmy", out bool flag, false);
            kingdom.data.set("CorruptArmy", !flag);
        }
        public static void toggleSteelFortressActive(Kingdom kingdom)
        {
            kingdom.data.get("SteelFortress", out bool flag, false);
            kingdom.data.set("SteelFortress", !flag);
        }
        public static void toggleEliteLegionActive(Kingdom kingdom)
        {
            kingdom.data.get("EliteLegion", out bool flag, false);
            kingdom.data.set("EliteLegion", !flag);
        }
        public static void toggle(string choice)
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
        public static Vector2 getPositionByIndex(int index)
        {
            // Starting position by x
            float startX = -80;

            // Starting position by y
            float startY = 10;

            // Buttons size + gap between
            float sizeWithGap = 40;

            // Buttons per row
            int buttonsPerRow = 5;

            // Calculating points
            float positionX = startX + (index * sizeWithGap) - (Mathf.Floor(index / buttonsPerRow) * sizeWithGap * buttonsPerRow);
            float positionY = startY - (Mathf.Floor(index / buttonsPerRow) * sizeWithGap);

            return new Vector2(positionX, positionY);

            // float x = 50f + 36 * index;
            // float y = 18f - 36 * (index % 2);
            // return new Vector2(x, y);
        }
        public static void MSSetB(string id, GameObject CT)
        {
            Settingsbutton++;
            GameObject UIG = NCMS.Utils.GameObjects.FindEvenInactive("DA_UIG");
            var neDAsmdmyIG1 = GameObject.Instantiate(UIG, CT.transform);
            neDAsmdmyIG1.transform.localPosition = new Vector2(0f, NYJG * 51.9125f - (Settingsbutton * 40f));
            var BVC = new Vector3(-20f, NYJG * -0.5431562f - (Settingsbutton * 40f));
            var BVC2 = new Vector3(100f, NYJG * -0.5431562f - (Settingsbutton * 40f));
            GameObject RSRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Name");
            GameObject RSObj = GameObject.Instantiate(RSRef, CT.transform);
            RSObj.SetActive(true);
            MSText = RSObj.GetComponent<Text>();
            MSText.text = LocalizedTextManager.getText($"{id}MS", null);
            MSText.supportRichText = true;
            MSText.transform.SetParent(CT.transform);
            var RSObjRTF = RSObj.GetComponent<RectTransform>();
            RSObjRTF.position = new Vector3(0, 0, 0);
            RSObjRTF.localPosition = new Vector3(20f, NYJG * -0.5431562f - (Settingsbutton * 40f));
            Main.SettingsText.Add(id, MSText);
            NCMS.Utils.PowerButtons.CreateButton(id + "MSLeftButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
            null, null, BVC, ButtonType.Click, CT.transform, () => SetRSZ(id, -1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            NCMS.Utils.PowerButtons.CreateButton(id + "MSRightButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
            null, null, BVC2, ButtonType.Click, CT.transform, () => SetRSZ(id, 1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
        }
        public static void SetRSZ(string id, int i, int l = 1)
        {
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { l = 10; }
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) { l = 100; }
            if (Main.moreSettings[id] + i * l < 1) { Main.moreSettings[id] = 0; }
            else { Main.moreSettings[id] += i * l; }
            string text = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "moreSeting" + ".json");
            if (Application.platform == RuntimePlatform.WindowsPlayer) { text = text.Replace("\\", "/"); }
            DAStorage NewStorage = new()
            {
                Name = "MoreSeting" + id,
                num = Main.moreSettings[id]
            };
            File.WriteAllText(text, JsonConvert.SerializeObject(NewStorage, Formatting.Indented));
            translate.init();
            Main.SettingsText[id].text = LocalizedTextManager.getText($"{id}MS", null);
        }
        public static void RSSetB(ResourceAsset resource, GameObject CT)
        {
            RSbutton++;
            GameObject UIG = NCMS.Utils.GameObjects.FindEvenInactive("DA_UIG");

            var neDAsmdmyIG1 = GameObject.Instantiate(UIG, CT.transform);
            neDAsmdmyIG1.transform.localPosition = new Vector2(0f, NYJG * -54.31562f - (RSbutton * 40f));
            var BVC = new Vector3(-20f, NYJG * -0.5431562f - (RSbutton * 40f));
            var BVC2 = new Vector3(100f, NYJG * -0.5431562f - (RSbutton * 40f));
            GameObject RSRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Name");
            GameObject RSObj = GameObject.Instantiate(RSRef, CT.transform);
            RSObj.SetActive(true);
            MSText = RSObj.GetComponent<Text>();
            MSText.text = LocalizedTextManager.getText($"{resource.id}RS", null);
            MSText.supportRichText = true;
            MSText.transform.SetParent(CT.transform);
            var RSObjRTF = RSObj.GetComponent<RectTransform>();
            RSObjRTF.position = new Vector3(0, 0, 0);
            RSObjRTF.localPosition = new Vector3(20f, NYJG * -0.5431562f - (RSbutton * 40f));
            Main.resourceText.Add(resource.id, MSText);
            NCMS.Utils.PowerButtons.CreateButton(resource.id + "RSTLeftButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
            null, null, BVC, ButtonType.Click, CT.transform, () => SetRST(resource.id, -1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            NCMS.Utils.PowerButtons.CreateButton(resource.id + "RSTRightButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
            null, null, BVC2, ButtonType.Click, CT.transform, () => SetRST(resource.id, 1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            var UIGRTF = UIG.GetComponent<RectTransform>();
            UIGRTF.position = new Vector3(0, 0, 0);
            UIGRTF.localPosition = new Vector3(0, NYJG * -0.5431562f - (RSbutton * 40f));

        }
        public static void SetRST(string id, int i, int l = 1)
        {
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { l = 10; }
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) { l = 100; }
            Main.resourceSettings[id] += i * l;
            string text = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "ResourcesSettings" + ".json");
            if (Application.platform == RuntimePlatform.WindowsPlayer) { text = text.Replace("\\", "/"); }
            DAStorage NewStorage = new()
            {
                Name = "ResourcesSettings" + id,
                RS = Main.resourceSettings[id]
            };
            File.WriteAllText(text, JsonConvert.SerializeObject(NewStorage, Formatting.Indented));
            translate.init();
            Main.resourceText[id].text = LocalizedTextManager.getText($"{id}RS", null);
        }

    }

}