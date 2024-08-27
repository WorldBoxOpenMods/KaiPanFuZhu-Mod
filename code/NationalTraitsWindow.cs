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
        private static Dictionary<string,NameInput> NameInputs=new();

        private static readonly string wid = "NationalTraitsWindow";
        public static void init()
        {
            int index = 0;

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
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1000f) + originalSize;

            // 设置初始位置
            content.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0);

            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();

            NameInputs.Add("id",CreateInputOption("id", "id", "设置国家特质的id,用于储存,尽量用英文", 0, content, ""));
            NameInputs.Add("name",CreateInputOption("name", "名字", "设置国家特质的名字", -70, content, ""));
            NameInputs.Add("Description",CreateInputOption("Description", "说明", "设置国家特质的说明", -140, content, "").);

            PowerButton button = PowerButtons.CreateButton(
                "NationalTraitsCreate",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.UI.default.png"),
                "创造国家特质",
                "创造国家特质,重启游戏就会生效",
                new Vector3(-118, -80),
                ButtonType.Click,
                content.transform,
                CreateNationalTraits
            );
        }
        public static void CreateNationalTraits()
        {
            NationalTraits traits = new()
            {
                id = NameInputs["id"].inputField.text,
                name = NameInputs["name"].inputField.text,
                Description = NameInputs["description"].inputField.text,
            };
            Main.NationalTraits.Add(traits.id, traits);
            string filePath = $".\\Mods\\KaiPanFuZhu-Mod-main\\NationalTraits\\NationalTraits.json";
            DA_save.SaveToFile(filePath,Main.NationalTraits);
        }

        // public static void SetNT(string id, int i, int l = 1)
        // {
        //     if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { l = 10; }
        //     if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) { l = 100; }
        //     if (Main.moreSettings[id] + i * l < 1) { Main.moreSettings[id] = 0; }
        //     else { Main.moreSettings[id] += i * l; }
        //     translate.init();
        //     Main.SettingsText[id].text = LocalizedTextManager.getText($"{id}NT", null);
        // }
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