using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomacy_Army
{

    public class NationalTraits
    {
        public string id="";
        public string name="";
        public string Description="";
        public string path=$"{Mod.Info.Name}.Resources.UI.default.png";
        //动员率
        public float MobilizationRate=0f;
        //属性
        public float mod_damage=0f;
        public float armor=0f;
        public float mod_speed=0f;
        public float mod_health=0f;
        public float mod_knockback_reduction=0f;
        public float mod_gold_out_army=0f;
        public int gold=0;
        public int bread=0;
        public int housing=0;
    }
}