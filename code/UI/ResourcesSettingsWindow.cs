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
    public class ResourcesSettingsWindow
    {
        public static GameObject content;
        private static Vector2 originalSize;
        public static float NYJG;
        public static int RSbutton = 0;
        public static Text MSText;
        public static void init()
        {

            ResourcesSettings();


        }
        public static void ResourcesSettings()
        {
            string wid = "Window_ResourcesSettings";
            ScrollWindow Window = pvz_ui.NewWindows(wid, 0, "null", true);
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
            // _ = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/Window_ResourcesSettings/Background/Scroll View/Viewport/Content");
            // 确保 ScrollRect 正确配置
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();


            // float MinY = ((pvz_ui.CustomWindowTexts[wid].preferredHeight / 2) + 30) * -1;
            foreach (ResourceAsset resource in AssetManager.resources.list) { RSSetB(resource, content); }
        }
        public static void RSSetB(ResourceAsset resource, GameObject CT)
        {
            RSbutton++;
            GameObject UIG = GameObjects.FindEvenInactive("DA_UIG");

            var neDAsmdmyIG1 = UnityEngine.Object.Instantiate(UIG, CT.transform);
            neDAsmdmyIG1.transform.localPosition = new Vector2(0f, NYJG * -54.31562f - (RSbutton * 40f));
            var BVC = new Vector3(-20f, NYJG * -0.5431562f - (RSbutton * 40f));
            var BVC2 = new Vector3(100f, NYJG * -0.5431562f - (RSbutton * 40f));
            GameObject RSRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Name");
            GameObject RSObj = UnityEngine.Object.Instantiate(RSRef, CT.transform);
            RSObj.SetActive(true);
            MSText = RSObj.GetComponent<Text>();
            MSText.text = LocalizedTextManager.getText($"{resource.id}RS", null);
            MSText.supportRichText = true;
            MSText.transform.SetParent(CT.transform);
            var RSObjRTF = RSObj.GetComponent<RectTransform>();
            RSObjRTF.position = new Vector3(0, 0, 0);
            RSObjRTF.localPosition = new Vector3(20f, NYJG * -0.5431562f - (RSbutton * 40f));
            resourceText.Add(resource.id, MSText);
            PowerButtons.CreateButton(resource.id + "RSTLeftButton", Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
            "数值 - 1", "按住Ctrl - 10,按住Shift - 100", BVC, ButtonType.Click, CT.transform, () => SetRST(resource.id, -1)).button.GetComponent<Image>().sprite = Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            PowerButtons.CreateButton(resource.id + "RSTRightButton", Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
            "数值 + 1", "按住Ctrl + 10,按住Shift + 100", BVC2, ButtonType.Click, CT.transform, () => SetRST(resource.id, 1)).button.GetComponent<Image>().sprite = Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
            var UIGRTF = UIG.GetComponent<RectTransform>();
            UIGRTF.position = new Vector3(0, 0, 0);
            UIGRTF.localPosition = new Vector3(0, NYJG * -0.5431562f - (RSbutton * 40f));

        }
        public static void SetRST(string id, int i, int l = 1)
        {
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { l = 10; }
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) { l = 100; }
            resourceSettings[id] += i * l;
            string text = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "ResourcesSettings" + ".json");
            if (Application.platform == RuntimePlatform.WindowsPlayer) { text = text.Replace("\\", "/"); }
            DAStorage NewStorage = new()
            {
                Name = "ResourcesSettings" + id,
                RS = resourceSettings[id]
            };
            File.WriteAllText(text, JsonConvert.SerializeObject(NewStorage, Formatting.Indented));
            translate.init();
            resourceText[id].text = LocalizedTextManager.getText($"{id}RS", null);
        }
    }
}