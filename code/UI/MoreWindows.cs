using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCMS.Utils;
using NCMS;
using UnityEngine;
using UnityEngine.UI;

using static Diplomacy_Army.Main;
using Newtonsoft.Json;
using System.IO;
using Diplomacy_Army;
using UnityEngine.Events;
using ReflectionUtility;

namespace Diplomacy_Army
{
    public class MoreWindows
    {
        public static GameObject content;
        private static Vector2 originalSize;
        public static Kingdom kingdom;
        public static GodPower power;
        public static PowerButton powerButton;
        public static int button = 0;
        public static int Settingsbutton = 0;

        public static Text MSText;
        public static float NYJG;
        public static Dictionary<string, List<ItemAsset>> itemModifiers = new();
        internal static PowerButtonSelector pbsInstance;

        public static void init()
        {
            KingdomWindow();
            MoreRules();
            MoreSettings();
            ProhibitgiveItem();

        }




        public static void MoreSettings()
        {
            int index = 0;
            string wid = "MoreSettings";
            pvz_ui.NewWindows(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["MoreSettings"];
            // Getting power button selector using reflections with ReflectionUtility
            pbsInstance = Reflection.GetField(typeof(PowerButtonSelector), null, "instance") as PowerButtonSelector;
            foreach (string setting in Main.moreSettings.Keys) { MSSetB(setting, content); }
            // power = new()
            // {
            //     id = "修改指向单独村庄开关",
            //     name = "修改指向单独村庄开关",
            //     unselectWhenWindow = true
            // };
            // AssetManager.powers.add(power);
            // powerButton = PowerButtons.CreateButton(
            //     "修改指向单独村庄开关",
            //     Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            //     "修改指向单独村庄",
            //     "修改指向单独村庄,修改等级高于指向全体村庄(修改村庄资源上限无效果)",
            //     new Vector2(-118, -40),
            //     ButtonType.GodPower,
            //     content.transform,
            //     new UnityAction(tryToHideWindow)
            // );

        }
        public static void tryToHideWindow()
        {
            power.click_action = null;
            power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(CityPowerWindow.TryToChangeDirectionToCity));
            ScrollWindow.get("MoreSettings").clickHide();
            pbsInstance.clickPowerButton(powerButton);
        }
        public static void MoreRules()
        {
            int index = 0;
            string wid = "MoreRules";
            pvz_ui.NewWindows(wid, 0, "null", true);
            wid.RTF();
            content = pvz_ui.CustomWindowObjects["MoreRules"];
            PowerButtons.CreateButton("封锁边境", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "封锁边境", "封锁边境", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("异族占领", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "异族占领", "异族可以相互", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("异族统治", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "异族统治", "异族占领时不会伤害平民或者破坏房屋", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("领土完整", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "领土完整", "暂时没有效果", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("禁止自主联盟", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "禁止自主联盟", "禁止自主联盟", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("动员无装备", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "动员无装备", "动员无装备", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            PowerButtons.CreateButton("调遣军队前往目的地", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            "调遣军队前往目的地", "军队直接前往点击的坐标而不是前往国家或者村庄,但驻守无此作用", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
            // PowerButtons.CreateButton("种族寿命统一70岁", Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
            // "种族寿命统一70岁", "种族寿命统一70岁", getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;
        }
        public static void ProhibitgiveItem()
        {
            int index = 0;
            string wid = "ProhibitgiveItem";
            pvz_ui.NewWindows(wid, 0, "null", true);
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
            pvz_ui.NewWindows(wid, 0, "null", true);
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
                () => openWindow("NewKingdomWindow")
            );
            button.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            button.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            foreach (var key in Main.NationalTraits.Keys)
            {
                button = PowerButtons.CreateButton(
                    key,
                    Mod.EmbededResources.LoadSprite(Main.NationalTraits[key].path),
                    Main.NationalTraits[key].name,
                    Main.NationalTraits[key].Description,
                    getPositionByIndex(index),
                    ButtonType.Toggle,
                    content.transform,
                    () => ToggleButtonActive(key)
                );
                index++;
            }
            button = PowerButtons.CreateButton(
                "NationalTraitsWindow",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "添加国家特质",
                "添加国家特质,重启游戏就会生效",
                new Vector3(-118, -80),
                ButtonType.Click,
                content.transform,
                () => openWindow("NationalTraitsWindow")
            );
            button.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            button.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);



        }
        public static void openWindow(string wid)
        {
            Windows.ShowWindow(wid);


        }
        public static void ToggleButtonActive(string str)
        {
            kingdom.data.get(str, out bool flag, false);
            kingdom.data.set(str, !flag);
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
            RSObjRTF.localPosition = new Vector3(30f, NYJG * -0.5431562f - (Settingsbutton * 40f));
            Main.SettingsText.Add(id, MSText);
            NCMS.Utils.PowerButtons.CreateButton(id + "MSLeftButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
            "数值 - 1", "按住Ctrl - 10,按住Shift - 100", BVC, ButtonType.Click, CT.transform, () => SetRSZ(id, -1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            NCMS.Utils.PowerButtons.CreateButton(id + "MSRightButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
            "数值 + 1", "按住Ctrl + 10,按住Shift + 100", BVC2, ButtonType.Click, CT.transform, () => SetRSZ(id, 1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            Settingsbutton++;
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



    }

}