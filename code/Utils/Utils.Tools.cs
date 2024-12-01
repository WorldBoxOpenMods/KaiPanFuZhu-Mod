using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Events;
using ai;
using UnityEngine.UI;
using HarmonyLib;
using System.Reflection;


namespace Diplomacy_Army.Utils
{
    //感谢寒海赠送的tools，真的谢谢寒海
    public static class PVZTools
    {
        public static void HarmonyPatching(Harmony harmony, string type, MethodInfo original, MethodInfo patch)
        {
            switch (type)
            {
                case "prefix":
                    harmony.Patch(original, prefix: new HarmonyMethod(patch));
                    break;
                case "postfix":
                    harmony.Patch(original, postfix: new HarmonyMethod(patch));
                    break;
            }
        }
        public static bool IsID(this Actor a, string id)
        {
            return a.asset != null && a.asset.id == id;
        }
        public static bool IsID(this ActorBase a, string id)
        {
            return a.asset != null && a.asset.id == id;
        }
        public static bool IsID(this Building b, string id)
        {
            return b.asset != null && b.asset.id == id;
        }
        public static bool IsID(this Projectile p, string id)
        {
            return p.asset != null && p.asset.id == id;
        }
        public static bool Any(this Actor a)
        {
            return a != null && a.isAlive() && a.data != null && a.data.alive;
        }
        public static bool Any(this City c)
        {
            return c != null && c.isAlive() && c.data != null && c.data.alive;
        }
        public static Vector2 AddAngle(this Vector2 pos, Vector2 pos2, float angle)
        {
            return new Vector2((pos.x - pos2.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (pos.y - pos2.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + pos2.x,
            (pos.y - pos2.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + (pos.x - pos2.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + pos2.y);
        }
        public static Vector3 AddAngle(this Vector3 pos, Vector3 pos2, float angle)
        {
            return new Vector3((pos.x - pos2.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (pos.y - pos2.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + pos2.x,
            (pos.y - pos2.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + (pos.x - pos2.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + pos2.y, pos.z);
        }
        public static void AddPos(this Actor a, float x, float y, bool setTile = true)
        {
            a.currentPosition.x = a.currentPosition.x + x;
            a.currentPosition.y = a.currentPosition.y + y;
            a.curShadowPosition.x = a.curShadowPosition.x + x;
            a.curShadowPosition.y = a.curShadowPosition.y + y;
            a.curTransformPosition.x = a.curTransformPosition.x + x;
            a.curTransformPosition.y = a.curTransformPosition.y + y;
            a.transform.position = new Vector3(a.currentPosition.x, a.currentPosition.y, a.transform.position.z);
            if (!setTile) { return; }
            WorldTile currentTile2 = World.world.GetTile((int)a.currentPosition.x, (int)a.currentPosition.y);
            if (currentTile2 != null) { a.setCurrentTile(currentTile2); }
        }
        public static void SetPos(this Actor a, float x, float y, bool setTile = true)
        {
            a.currentPosition.x = x;
            a.currentPosition.y = y;
            a.curShadowPosition.x = x;
            a.curShadowPosition.y = y;
            a.curTransformPosition.x = x;
            a.curTransformPosition.y = y;
            a.transform.position = new Vector3(x, y, a.transform.position.z);
            if (!setTile) { return; }
            WorldTile currentTile2 = World.world.GetTile((int)a.currentPosition.x, (int)a.currentPosition.y);
            if (currentTile2 != null) { a.setCurrentTile(currentTile2); }
        }
        public static bool ActorCollision(this Actor a, Actor actor)
        {
            float size = a.GetSize() + actor.GetSize();
            float dist = Toolbox.Dist(a.currentPosition.x, a.currentPosition.y, actor.currentPosition.x, actor.currentPosition.y);
            float distZ = Toolbox.Dist(0f, a.zPosition.y, 0f, actor.zPosition.y);
            if (size > dist && size > distZ)
            {
                float chcd = size - dist;
                float m1 = a.data.health;
                float m2 = actor.data.health;
                float chcd1 = chcd * (m2 / (m1 + m2));
                float chcd2 = chcd * (m1 / (m1 + m2));
                float zdx = (a.currentPosition.x + actor.currentPosition.x) / 2;
                float zdy = (a.currentPosition.y + actor.currentPosition.y) / 2;
                float angle2 = Toolbox.getAngle(zdx, zdy, a.currentPosition.x, a.currentPosition.y);
                float angle3 = Toolbox.getAngle(zdx, zdy, actor.currentPosition.x, actor.currentPosition.y);
                a.AddPos(Mathf.Cos(angle2) * chcd1, Mathf.Sin(angle2) * chcd1);
                actor.AddPos(Mathf.Cos(angle3) * chcd2, Mathf.Sin(angle3) * chcd2);
                return true;
            }
            return false;
        }
        public static List<Actor> hasStatusUnits(this List<Actor> list, string id, bool has = true)
        {
            List<Actor> list2 = new();
            for (int i = 0; i < list.Count; i++)
            {
                Actor actor = list[i];
                if ((has && actor.hasStatus(id)) || (!has && !actor.hasStatus(id))) { list2.Add(actor); }
            }
            return list2;
        }
        public static float GetSize(this Actor a)
        {
            return a.currentScale.xysd() * 1.8f;
        }
        public static float xysd(this Vector3 pos)
        {
            return Mathf.Abs(Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y));
        }
        public static float xysd(this Vector2 pos)
        {
            return Mathf.Abs(Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y));
        }
        public static Sprite getItemSprite(this Actor a, string icon)
        {
            Sprite sprite = Resources.Load<Sprite>($"items/{icon}/walk_0");
            if (a._last_main_sprite == null)
            {
                return sprite;
            }
            string icon2 = a._last_main_sprite.name;
            if (a.asset.oceanCreature && icon2.Contains("swim_"))
            {
                return sprite;
            }
            Sprite sprite2 = Resources.Load<Sprite>($"items/{icon}/{icon2}");
            if (sprite2 != null) { return sprite2; }
            return sprite;
        }

        public static int Min(this int num, int num2)
        {
            if (num < num2) { return num2; }
            return num;
        }
        public static bool CPHealth(this Actor a, float num)
        {
            return a.data.health < a.stats[S.health] * num;
        }

    
        public static string dataId(this Actor a)
        {
            if (a.Any())
            {
                return a.data.id;
            }
            return "null";
        }
        public static List<string> dataId(this List<Actor> actors)
        {
            List<string> list = new();
            foreach (Actor a in actors)
            {
                if (a.Any()) { list.Add(a.data.id); }
            }
            return list;
        }


        public static void RTF(this string id)
        {
            if (pvz_ui.CustomWindowIds.Contains(id))
            {
                float pHeight = pvz_ui.CustomWindowsHeight[id];
                if (pvz_ui.CustomTextWindowIds.Contains(id))
                {
                    pHeight = pvz_ui.CustomWindowTexts[id].preferredHeight;
                    if (id == "Window_PVZachievements")
                    {
                        MonoBehaviour.print(pHeight.ToString());
                    }
                }
                GameObject contentComponent = pvz_ui.CustomWindowObjects[id];
                GameObject Content = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{id}/Background/Scroll View/Viewport/Content");
                RectTransform rect = contentComponent.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 1);
                rect.anchorMax = new Vector2(0.5f, 1);
                rect.offsetMin = new Vector2(-90f, pHeight * -1);
                rect.offsetMax = new Vector2(90f, -17);
                rect.sizeDelta = new Vector2(180, pHeight + 50);
                Content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, pHeight + 50);
                contentComponent.transform.localPosition = new Vector2(contentComponent.transform.localPosition.x, ((pHeight / 2) + 30) * -1);
            }
        }
        public static GameObject GetIcon(this string id)
        {
            GameObject obj = null;
            if (pvz_ui.CustomWindowIcons.ContainsKey(id)) { obj = pvz_ui.CustomWindowIcons[id]; }
            return obj;
        }
        public static GameObject GetObj(this string id)
        {
            GameObject obj = null;
            if (pvz_ui.CustomWindowObjects.ContainsKey(id)) { obj = pvz_ui.CustomWindowObjects[id]; }
            return obj;
        }
        public static Text GetText(this string id)
        {
            Text text = null;
            if (pvz_ui.CustomWindowTexts.ContainsKey(id)) { text = pvz_ui.CustomWindowTexts[id]; }
            return text;
        }

        public static void removeStatusEffect(this BaseSimObject obj, string pID)
        {
            if (obj == null) { return; }
            if (!obj.base_data.alive) { return; }
            if (obj.activeStatus_dict == null) { return; }
            if (!obj.activeStatus_dict.ContainsKey(pID)) { return; }
            obj.setStatsDirty();
            obj.activeStatus_dict.Remove(pID);
        }

    }
}

