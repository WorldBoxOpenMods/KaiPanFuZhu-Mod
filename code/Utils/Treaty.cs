using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ReflectionUtility;

namespace Diplomacy_Army
{
    public class Treaty
    {
        public static Dictionary<Kingdom, Dictionary<Kingdom, TreatyType>> kingdomsTryNewTreaty = new();

        public TreatyType treatyType;
        public string type;
        public Kingdom kingdom_1;
        public Kingdom kingdom_2;
        public int startTime;
        public int endTime;
        public string message;
        public Color color;

        public string kingdom1_name;
        public string kingdom2_name;

        public Color kingdom1_color;
        public string kingdom1_color_string;
        public Color kingdom2_color;
        public string kingdom2_color_string;

        public Treaty(Kingdom pKingdom, Kingdom pKingdom2, int pTime, TreatyType pType,string type)
        {
            this.kingdom_1 = pKingdom;
            this.kingdom_2 = pKingdom2;
            this.startTime = MapBox.instance.mapStats.year;
            this.endTime = this.startTime + pTime;
            this.treatyType = pType;
            this.type = type;
            switch (this.treatyType)
            {
                case TreatyType.Alliance:
                    color = Color.white;
                    break;
                case TreatyType.Defence:
                    color = Color.green;
                    break;
                case TreatyType.Army:
                    color = Color.red;
                    break;
            }
            this.kingdom1_name = this.kingdom_1.name;
            this.kingdom2_name = this.kingdom_2.name;
            this.kingdom1_color = ((ColorAsset)Reflection.GetField(this.kingdom_1.GetType(), this.kingdom_1, "kingdomColor")).getColorText();
            this.kingdom2_color = ((ColorAsset)Reflection.GetField(this.kingdom_2.GetType(), this.kingdom_2, "kingdomColor")).getColorText();
            this.kingdom1_color_string = "#" + ColorUtility.ToHtmlStringRGBA(this.kingdom1_color);
            this.kingdom2_color_string = "#" + ColorUtility.ToHtmlStringRGBA(this.kingdom2_color);
            this.message = "<color=" + this.kingdom1_color_string + ">" + this.kingdom1_name + "</color> 和 <color=" + this.kingdom2_color_string
                            + ">" + this.kingdom2_name + "</color> 的【" + this.type + "】条约将于世界历" + this.endTime.ToString() + "年结束";
        }

        public void Change()
        {
            this.kingdom1_name = this.kingdom_1.name;
            this.kingdom2_name = this.kingdom_2.name;
            this.kingdom1_color = ((ColorAsset)Reflection.GetField(this.kingdom_1.GetType(), this.kingdom_1, "kingdomColor")).getColorText();
            this.kingdom2_color = ((ColorAsset)Reflection.GetField(this.kingdom_2.GetType(), this.kingdom_2, "kingdomColor")).getColorText();
            this.kingdom1_color_string = "#" + ColorUtility.ToHtmlStringRGBA(this.kingdom1_color);
            this.kingdom2_color_string = "#" + ColorUtility.ToHtmlStringRGBA(this.kingdom2_color);
            this.message = "<color=" + this.kingdom1_color_string + ">" + this.kingdom1_name + "</color> 和 <color=" + this.kingdom2_color_string
                            + ">" + this.kingdom2_name + "</color> 的【" + this.type + "】条约将于世界历" + this.endTime.ToString() + "年结束";
        }
    } 

    public enum TreatyType
    {
        Alliance,

        Defence,

        Army
    }
}
