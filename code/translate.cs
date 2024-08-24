using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCMS.Utils;
using ReflectionUtility;
using UnityEngine;

namespace Diplomacy_Army
{
    public class translate
    {
        public static void init()
        {
            int SCGL = Main.numofyears;
            easyTranslate("cz", "tab_Diplomacy_Army", "Diplomacy_Army");
            easyTranslate("en", "tab_Diplomacy_Army", "Diplomacy_Army");
            TipTranslate("tab_Diplomacy_Army", "Diplomacy_Army", "mod by 空星漫漫", "热键 : [Tab + K]".Replace("Tab + K", "<color=#00ff00><b>Tab + K</b></color>"));
            easyTranslate("cz", "modder", "modder");
            easyTranslate("en", "modder", "modder");
            easyTranslate("cz", "DAbilibili", "bilibili");
            easyTranslate("en", "DAbilibili", "作者的bilibili账号");
            easyTranslate("cz", "DAqq", "加入qq群聊");
            easyTranslate("en", "DAqq", "加入qq群聊");
            easyTranslate("cz", "DAmodderDescription", "modder");
            easyTranslate("en", "DAmodderDescription", "modder");
            easyTranslate("cz", "plot_description_new_declare_war","准备进攻宣称城市");
            easyTranslate("en", "plot_description_new_declare_war","准备进攻宣称城市");
            easyTranslate("cz", "plot_new_declare_war","准备进攻宣称城市");
            easyTranslate("en", "plot_new_declare_war","准备进攻宣称城市");
            // easyTranslate("cz", "tab_Diplomacy_Army Description", "Diplomacy_Army");
            // easyTranslate("en", "tab_Diplomacy_Army Description", "Diplomacy_Army");
            // easyTranslate("cz", "tab_Diplomacy_Army Description2", "mod by 空星漫漫");
            // easyTranslate("en", "tab_Diplomacy_Army Description2", "mod by 空星漫漫");
            easyTranslate("cz", "DA_MContentText", "代码by空星漫漫\n\n特别感谢寒海为本Mod中提供的技术支持!!!\n\n感谢急了为本mod提供大量建议\n\n感谢原作者贝伦帝国\n\nqq群781471990");
            easyTranslate("en", "DA_MContentText", "代码by空星漫漫\n\n特别感谢寒海为本Mod中提供的技术支持!!!\n\n感谢急了为本mod提供大量建议\n\n感谢原作者贝伦帝国\n\nqq群781471990");
            ButtonTranslate("numofyearsXSLeftButton", "概率 - 1%", "按住Ctrl - 10,按住Shift ÷ 10");
            ButtonTranslate("numofyearsXSRightButton", "概率 + 1%", "按住Ctrl + 10,按住Shift × 10");
            easyTranslate("en", "numofyears", $"签订期限{SCGL}");
            easyTranslate("cz", "numofyears", $"签订期限{SCGL}");
            foreach (string setting in Main.moreSettings.Keys)
            {
                easyTranslate($"{setting}MS", $"{setting} #" + Main.moreSettings[setting]);
                ButtonTranslate(setting + "RSLeftButton", "数值 - 1", "按住Ctrl - 10,按住Shift - 100");
                ButtonTranslate(setting + "RSRightButton", "数值 + 1", "按住Ctrl + 10,按住Shift + 100");
                // if (RaceText.ContainsKey(race) && RaceText[race] != null) { RaceText[race].text = LocalizedTextManager.getText($"#{race}BLSZ", null); }
            }
            foreach (ResourceAsset resource in AssetManager.resources.list)
            {
                easyTranslate($"{resource.id}RS", $"{LocalizedTextManager.getText(resource.id, null)} #" + Main.resourceSettings[resource.id]);
                ButtonTranslate(resource.id + "RSTLeftButton", "数值 - 1", "按住Ctrl - 10,按住Shift - 100");
                ButtonTranslate(resource.id + "RSTRightButton", "数值 + 1", "按住Ctrl + 10,按住Shift + 100");
            }


        }
        public static void TipTranslate(string name, string localName, string localDescription, string localDescription2)
        {
            Localization.AddOrSet(name, localName);
            Localization.AddOrSet(name + " Description", localDescription);
            Localization.AddOrSet(name + " Description2", localDescription2);
        }
        public static void ButtonTranslate(string name, string localName, string localDescription)
        {
            Localization.AddOrSet(name, localName);
            Localization.AddOrSet(name + " Description", localDescription);
        }
        public static void easyTranslate(string id, string name)
        {
            Localization.AddOrSet(id, name);
        }
        public static void easyTranslate(string pLanguage, string id, string name)
        {
            string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
            if (language != "en" && language != "cz")
            {
                language = "en";
            }
            if (pLanguage != language)
            {
                return;
            }
            Localization.addLocalization(id, name);
        }
    }
}