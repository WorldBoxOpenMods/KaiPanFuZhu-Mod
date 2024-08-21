using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCMS.Utils;
using ReflectionUtility;
using UnityEngine;
using Diplomacy_Army.Utils;

namespace Diplomacy_Army
{
    public class DA_modder
    {
        public static int Buttons = 0;
        public static string wid = "DAWindow_modder";
        public static void init()
        {
            pvz_ui.NewWindow(wid, 0, "DA_MContentText", true);
            pvz_ui.CustomTextColors.Add(wid, "#FFFFFF");
            NewLJTZ("bilibili", "https://space.bilibili.com/3493140006701370");
            NewLJTZ("qq", "https://qm.qq.com/q/qtnN1CyApO");
            wid.RTF();
        }
        public static void NewLJTZ(string id, string wz)
        {
            GodPower NewL = new()
            {
                id = "DA" + id,
                name = "DA" + id,
                unselectWhenWindow = true,
                toggle_action = new PowerToggleAction((pPower) => Application.OpenURL(@$"{wz}"))
            };
            AssetManager.powers.add(NewL);
            var NewB = PowerButtons.CreateButton(
            "DA" + id,
            Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.other.{id}.png"),
            LocalizedTextManager.getText("DA" + id,null),
            LocalizedTextManager.getText("DA" + id,null),
            new Vector2(120, 110 - (Buttons * 32)),
            ButtonType.Click, pvz_ui.CustomWindows[wid].transform,
            null
            );
            Reflection.SetField<GodPower>(NewB, "godPower", NewL);
            NewB.type = PowerButtonType.Special;
            Buttons++;
        }
        
    }
}