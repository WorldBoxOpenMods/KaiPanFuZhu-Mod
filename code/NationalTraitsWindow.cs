using System.Collections.Generic;
using System.IO;
using Diplomacy_Army.Utils;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Diplomacy_Army
{
    public class NationalTraitsWindow
    {
        public static GameObject content;
        public static float NYJG;
        private static Vector2 originalSize;
        public static int NTbutton = 0;
        public static Text contentText;
        public static Text MSText;

        private static readonly Dictionary<string, NameInput> NameInputs = new();
        public static Dictionary<string, Text> NTText = new();
        public static Dictionary<string, int> NTSet = new()
        {
            { "MobilizationRate", 0 },
            { "mod_damage", 0 },
            { "armor", 0 },
            { "mod_speed", 0 },
            { "mod_health", 0 },
            { "mod_knockback_reduction", 0 },
            { "mod_gold_out_army", 0 },
            { "gold", 0 },
            { "bread", 0 },
            { "housing", 0 },

        };

        private static readonly string wid = "NationalTraitsWindow";
        public static void init()
        {
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
            content.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0);

            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();



            NameInputs.Add("id", CreateInputOption("id", "id", "设置国家特质的id,用于储存,尽量用英文", 0, content, ""));
            NameInputs.Add("name", CreateInputOption("name", "名字", "设置国家特质的名字", -70, content, ""));
            NameInputs.Add("Description", CreateInputOption("Description", "说明", "设置国家特质的说明", -140, content, ""));

            PowerButton button = PowerButtons.CreateButton(
                "NationalTraitsCreate",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "创造国家特质",
                "创造国家特质,重启游戏就会生效",
                new Vector3(-60, -60),
                ButtonType.Click,
                content.transform,
                ()=>CreateNationalTraits()
            );
            foreach (var trait in NTSet.Keys) { NTSetB(trait, content); }
        }
        public static void CreateNationalTraits()
        {
            if(NameInputs["id"].inputField.text==""||NameInputs["name"].inputField.text=="")
            {
                WorldTip.showNow("id或名字未填写", true, "top", 5f);
                return;
            }
            NationalTraits traits = new()
            {
                id = NameInputs["id"].inputField.text,
                name = NameInputs["name"].inputField.text,
                Description = NameInputs["Description"].inputField.text,
                MobilizationRate = NTSet["MobilizationRate"] / 100,
                mod_damage = NTSet["mod_damage"] / 100,
                mod_speed = NTSet["mod_speed"] / 100,
                mod_health = NTSet["mod_health"] / 100,
                mod_gold_out_army = NTSet["mod_gold_out_army"] / 100,
                mod_knockback_reduction = NTSet["mod_knockback_reduction"] / 100,
                armor = NTSet["armor"] / 100,
                gold = NTSet["gold"],
                bread = NTSet["bread"],
                housing = NTSet["housing"],
            };

            Main.NationalTraits.Add(traits.id, traits);
            string filePath = $".\\Mods\\KaiPanFuZhu-Mod-main\\NationalTraits\\NationalTraits.json";
            DA_save.SaveToFile(filePath, Main.NationalTraits);
            WorldTip.showNow("创建成功", true, "top", 5f);
        }
        public static void NTSetB(string id, GameObject CT)
        {
            NTbutton++;
            GameObject UIG = NCMS.Utils.GameObjects.FindEvenInactive("DA_UIG");

            var neDAsmdmyIG1 = GameObject.Instantiate(UIG, CT.transform);
            neDAsmdmyIG1.transform.localPosition = new Vector2(0f, NYJG * -54.31562f - (NTbutton * 40f) - 140f);
            var BVC = new Vector3(-40f, NYJG * -0.5431562f - (NTbutton * 40f) - 140f);
            var BVC2 = new Vector3(100f, NYJG * -0.5431562f - (NTbutton * 40f) - 140f);
            GameObject RSRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Name");
            GameObject RSObj = GameObject.Instantiate(RSRef, CT.transform);
            RSObj.SetActive(true);
            MSText = RSObj.GetComponent<Text>();
            MSText.text = LocalizedTextManager.getText($"{id}NT", null);
            MSText.supportRichText = true;
            MSText.transform.SetParent(CT.transform);
            var RSObjRTF = RSObj.GetComponent<RectTransform>();
            RSObjRTF.position = new Vector3(0, 0, 0);
            RSObjRTF.localPosition = new Vector3(0f, NYJG * -0.5431562f - (NTbutton * 40f) - 140f);
            NTText.Add(id, MSText);
            NCMS.Utils.PowerButtons.CreateButton(id + "NTTLeftButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
            null, null, BVC, ButtonType.Click, CT.transform, () => SetNT(id, -1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            NCMS.Utils.PowerButtons.CreateButton(id + "NTTRightButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
            null, null, BVC2, ButtonType.Click, CT.transform, () => SetNT(id, 1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            var UIGRTF = UIG.GetComponent<RectTransform>();
            UIGRTF.position = new Vector3(0, 0, 0);
            UIGRTF.localPosition = new Vector3(0, NYJG * -0.5431562f - (NTbutton * 40f) - 140f);

        }
        public static void SetNT(string id, int i, int l = 1)
        {
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { l = 10; }
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) { l = 100; }
            if (NTSet[id] + i * l < 1) { NTSet[id] = 0; }
            else { NTSet[id] += i * l; }
            translate.init();
            NTText[id].text = LocalizedTextManager.getText($"{id}NT", null);
        }
        public static NameInput CreateInputOption(string objName, string title, string desc, int posY, GameObject parent, string textValue = "-1")
        {
            GameObject statHolder = new("OptionHolder");
            statHolder.transform.SetParent(parent.transform);
            Image statImage = statHolder.AddComponent<Image>();
            statImage.sprite = Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.windowInnerSliced.png");
            RectTransform statHolderRect = statHolder.GetComponent<RectTransform>();
            statHolderRect.localPosition = new Vector3(10, posY, 0);
            statHolderRect.sizeDelta = new Vector2(400, 150);

            Text statText = addText(title, statHolder, 20, new Vector3(0, 110, 0), new Vector2(100, 0));
            RectTransform statTextRect = statText.gameObject.GetComponent<RectTransform>();
            statTextRect.sizeDelta = new Vector2(statTextRect.sizeDelta.x + 50, 80);

            Text descText = addText(desc, statHolder, 20, new Vector3(0, 60, 0), new Vector2(300, 0));
            RectTransform descTextRect = descText.gameObject.GetComponent<RectTransform>();
            descTextRect.sizeDelta = new Vector2(descTextRect.sizeDelta.x, 80);

            GameObject inputRef = NCMS.Utils.GameObjects.FindEvenInactive("NameInputElement");

            GameObject inputField = GameObject.Instantiate(inputRef, statHolder.transform);
            NameInput nameInputComp = inputField.GetComponent<NameInput>();
            nameInputComp.setText(textValue);
            RectTransform inputRect = inputField.GetComponent<RectTransform>();
            inputRect.localPosition = new Vector3(0, -40, 0);
            inputRect.sizeDelta += new Vector2(120, 40);

            GameObject inputChild = inputField.transform.Find("InputField").gameObject;
            RectTransform inputChildRect = inputChild.GetComponent<RectTransform>();
            inputChildRect.sizeDelta *= 2;
            Text inputChildText = inputChild.GetComponent<Text>();
            inputChildText.resizeTextMaxSize = 20;
            return nameInputComp;
        }


        public static Text addText(string textString, GameObject parent, int sizeFont, Vector3 pos, Vector2 addSize = default(Vector2))
        {
            GameObject textRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{wid}/Background/Title");
            GameObject textGo = GameObject.Instantiate(textRef, parent.transform);
            textGo.SetActive(true);

            var textComp = textGo.GetComponent<Text>();
            textComp.fontSize = sizeFont;
            textComp.resizeTextMaxSize = sizeFont;
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.position = new Vector3(0, 0, 0);
            textRect.localPosition = pos + new Vector3(0, -50, 0);
            textRect.sizeDelta = new Vector2(100, 100) + addSize;
            textGo.AddComponent<GraphicRaycaster>();
            textComp.text = textString;

            return textComp;
        }

    }

    public class InputOption
    {
        public bool active = true;
        public string value;
    }

}