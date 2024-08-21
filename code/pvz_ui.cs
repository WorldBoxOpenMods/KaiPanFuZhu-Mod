using System.Collections.Generic;
using NCMS.Utils;
using UnityEngine;
using ReflectionUtility;
using UnityEngine.UI;
using UnityEngine.Events;
using NCMS;
using NCMS.Utils;
using ReflectionUtility;
using Diplomacy_Army.Utils;

namespace Diplomacy_Army
{
  class pvz_ui//感谢寒海的技术支持！
  {
    public static List<string> CustomIconIds = new();
    public static List<string> CustomTextIds = new();
    public static List<string> CustomWindowIds = new();
    public static List<string> LocalizedTextIds = new();
    public static List<string> CustomTextWindowIds = new();
    public static Dictionary<string, Text> CustomTexts = new();
    public static Dictionary<string, string> CustomText = new();
    public static Dictionary<string, Button> CustomButtons = new();
    public static Dictionary<string, string> CustomTextColors = new();
    public static Dictionary<string, float> CustomWindowsHeight = new();
    public static Dictionary<string, GameObject> CustomIcons = new();
    public static Dictionary<string, GameObject> CustomTextObjects = new();
    public static Dictionary<string, ScrollWindow> CustomWindows = new();
    public static Dictionary<string, GameObject> CustomWindowObjects = new();
    public static Dictionary<string, Text> CustomWindowTexts = new();
    public static Dictionary<string, PowersTab> CustomTabs = new();
    public static Dictionary<string, GameObject> CustomTabObjs = new();
    public static Dictionary<string, GameObject> CustomWindowIcons = new();
    public static StatBar NewBar(string id, string icon, string color, string Ending, float Val, int Max, GameObject CO, Vector3 Pos, Vector3 Scale1, Vector3 Scale2)
    {
      StatBar Bar = new();
      Transform icons = NCMS.Utils.GameObjects.FindEvenInactive("SpectateUnit").transform.Find("Icons");
      GameObject New = GameObject.Instantiate(icons.Find("HealthBar").gameObject, icons);
      New.transform.Find("Mask/Bar").GetComponent<Image>().color = Toolbox.makeColor(color, -1f);
      New.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(icon);
      New.transform.Find("Icon").GetComponent<Image>().transform.localScale = Scale1;
      New.transform.SetParent(CO.transform);
      New.transform.localPosition = Pos;
      New.transform.localScale = Scale2;
      New.name = id;
      Bar = New.GetComponent<StatBar>();
      Bar.lEnding = Ending;
      Bar.lVal = Val;
      Bar.lMax = Max;
      Bar.setBar(Bar.lVal, Bar.lMax, Bar.lEnding, true, Bar.lFloat, true, false);
      return Bar;
    }
    public static Text NewText(string id, string text, string color, bool localized, Transform parentr, Vector3 position, Vector3 size, GameObject MObj = null)
    {
      if (MObj == null)
      {
        GameObject MRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/MWHelper/Background/Name");
        MObj = GameObject.Instantiate(MRef, parentr);
      }
      MObj.SetActive(true);
      Text MText = MObj.GetComponent<Text>();
      CustomText.Add(id, text);
      if (localized)
      {
        LocalizedTextIds.Add(id);
        text = LocalizedTextManager.getText(text, null);
      }
      if (!string.IsNullOrEmpty(color))
      {
        text = $"<color={color}><b>{text}</b></color>";
        CustomTextColors.Add(id, color);
      }
      MText.text = text;
      MText.supportRichText = true;
      MText.transform.SetParent(parentr);
      var MObjRTF = MObj.GetComponent<RectTransform>();
      if (size != new Vector3(-1, -1)) { MObjRTF.sizeDelta = size; }
      MObjRTF.position = new Vector3(0f, 0f, 0f);
      MObjRTF.localPosition = position;
      CustomTextObjects.Add(id, MObj);
      CustomTexts.Add(id, MText);
      CustomTextIds.Add(id);
      return MText;
    }
    public static Text NewText(string id, string color, Transform parentr, Vector3 position)
    {
      GameObject MRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/MWHelper/Background/Name");
      GameObject MObj = GameObject.Instantiate(MRef, parentr);
      MObj.SetActive(true);
      Text MText = MObj.GetComponent<Text>();
      // string text = LocalizedTextManager.getText("mw_" + id, null);
      MText.text = $"<color={color}><b>{id}</b></color>";
      MText.supportRichText = true;
      MText.transform.SetParent(parentr);
      var MObjRTF = MObj.GetComponent<RectTransform>();
      MObjRTF.position = new Vector3(0f, 0f, 0f);
      MObjRTF.localPosition = position;
      // CustomTextColors.Add(id, color);
      // CustomTexts.Add(id, MText);
      // CustomTextIds.Add(id);
      return MText;
    }
    public static GameObject NewIcon(string id, string icon, Transform parentr, Vector3 position, Vector3 size)
    {
      GameObject Icon = new(id);
      var IconRTF = Icon.AddComponent<RectTransform>();
      Icon.AddComponent<CanvasRenderer>();
      Icon.AddComponent<Image>().sprite = Resources.Load<Sprite>(icon);
      Icon.transform.SetParent(parentr);
      Icon.transform.localScale = Vector3.one;
      IconRTF.localPosition = position;
      IconRTF.sizeDelta = size;
      CustomIcons.Add(id, Icon);
      CustomIconIds.Add(id);
      return Icon;
    }
    public static ScrollWindow NewWindow(string id, float height = 0f, string text = "null", bool textHeight = false, string iconId = "null", string icon = "null", float pos3Dx = 0f, float pos3Dy = 0f, float sizex = 0f, float sizey = 0f)
    {
      CustomWindowsHeight.Add(id, height);
      ScrollWindow Window = Windows.CreateNewWindow(id, id);
      if (iconId != "null") { AddIconToWindow(id, iconId, icon, Vector3.one, new Vector3(pos3Dx, pos3Dy, 0f), new Vector2(sizex, sizey)); }
      Window.transform.Find("Background").Find("Scroll View").gameObject.SetActive(true);
      GameObject contentComponent = Window.transform.Find("Background").Find("Name").gameObject;
      Text contentText = contentComponent.GetComponent<Text>();
      string text2 = "";
      if (text != "null"&&text!=null)
      {
        CustomTextObjects.Add(id, contentComponent);
        CustomTexts.Add(id, contentText);
        CustomText.Add(id, text);
        CustomTextIds.Add(id);
        LocalizedTextIds.Add(id);
        text2 = LocalizedTextManager.getText(text, null);

      }
      contentText.text = text2;
      contentText.supportRichText = true;
      Transform pContent = Window.transform.Find("Background").Find("Scroll View").Find("Viewport").Find("Content");
      if (textHeight)
      {
        contentText.transform.SetParent(pContent);
        CustomTextWindowIds.Add(id);
      }
      else { contentComponent.transform.SetParent(pContent); }
      contentComponent.SetActive(true);
      CustomWindowObjects.Add(id, contentComponent);
      CustomWindowTexts.Add(id, contentText);
      CustomWindows.Add(id, Window);
      CustomWindowIds.Add(id);
      id.RTF();
      return Window;
    }
    public static WorldLogMessage NewWorldLog(string text, string icon, string color, Vector3 location, Actor a = null)
    {
      WorldLogMessage log = new(text);
      if (color != "null") { log.color_special1 = Toolbox.makeColor(color, -1f); }
      log.location = location;
      log.icon = icon;
      log.unit = a;
      log.add();
      return log;
    }
    public static GameObject AddIconToWindow(string wid, string iconId, string icon, Vector3 pos, Vector3 pos3D, Vector2 size)
    {
      Transform pContent = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{wid}/Background/Scroll View/Viewport/Content").transform;
      var pObj = NewIcon(iconId, icon, pContent.transform, pos, Vector3.one);
      RectTransform pRect = pObj.GetComponent<RectTransform>();
      if (pos == Vector3.one) { pRect.anchoredPosition3D = pos3D; }
      pRect.sizeDelta = size;
      CustomWindowIcons.Add(wid, pObj);
      return pObj;
    }
    public static Button NewButton(string id, Vector3 pos, Transform parent = null, Sprite sprite1 = null, Sprite sprite2 = null, UnityAction call = null)
    {
      Button pB = null;
      var pB2 = GameObjects.FindEvenInactive("Button_Other");
      if (pB2 != null)
      {
        GameObject pObj = GameObject.Instantiate(pB2);
        pObj.name = id;
        if (parent != null) { pObj.transform.SetParent(parent); }
        else { pObj.transform.SetParent(pB2.transform.parent); }
        pObj.transform.localPosition = pos;
        if (sprite1 != null) { pObj.GetComponent<Image>().sprite = sprite1; }
        if (sprite2 != null) { pObj.transform.Find("Icon").GetComponent<Image>().sprite = sprite2; }
        pB = pObj.GetComponent<Button>();
        pB.onClick = new Button.ButtonClickedEvent();
        pB.onClick.AddListener(call);
        TipButton pTip = pB.gameObject.GetComponent<TipButton>();
        pTip.textOnClick = id;
        pTip.textOnClickDescription = id + " Description";
        pTip.text_description_2 = id + " Description2";
        pB.gameObject.SetActive(true);
        CustomButtons.Add(id, pB);
      }
      return pB;
    }
    public static PowersTab NewTab(string id, string icon, float x)
    {
      PowersTab pTab = null;
      var OtherTab = GameObjects.FindEvenInactive("Tab_Other");
      if (OtherTab != null)
      {
        foreach (Transform child in OtherTab.transform) { child.gameObject.SetActive(false); }
        GameObject pObj = GameObject.Instantiate(OtherTab);
        foreach (Transform child in pObj.transform)
        {
          if (child.gameObject.name == "tabBackButton" || child.gameObject.name == "-space") { child.gameObject.SetActive(true); continue; }
          GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in OtherTab.transform) { child.gameObject.SetActive(true); }
        pObj.transform.SetParent(OtherTab.transform.parent);
        pObj.name = $"Tab_Additional_{id}";
        pTab = pObj.GetComponent<PowersTab>();
        pTab.powerButton = NewButton($"tab_{id}", new Vector3(x, 49.615f), null, null, NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/Sprites/{icon}.png"), () => Button_Powers_Click(id));
        Reflection.SetField<GameObject>(pTab, "parentObj", OtherTab.transform.parent.parent.gameObject);
        pObj.SetActive(true);
        CustomTabs.Add(id, pTab);
        CustomTabObjs.Add(id, pObj);
        if (!Main.PVZPowersTabs.Contains(pTab)) { Main.PVZPowersTabs.Add(pTab); }
      }
      return pTab;
    }
    public static void Button_Powers_Click(string id)
    {
      var AdditionalTab = GameObjects.FindEvenInactive($"Tab_Additional_{id}");
      var AdditionalPowersTab = AdditionalTab.GetComponent<PowersTab>();
      AdditionalPowersTab.showTab(AdditionalPowersTab.powerButton);
    }


  }
}