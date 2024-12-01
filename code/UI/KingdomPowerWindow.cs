using System;
using NCMS.Utils;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using ReflectionUtility;

namespace Diplomacy_Army
{
    class KingdomPowerWindow
    {
        public static string name = "KingdomControlWindow";
        public static PowerButton powerButton;

        private static ScrollWindow window;
        private static GameObject content;
        private static GodPower power;
        private static int index = 0;

        // Initializing Tiles Window
        internal static PowerButtonSelector pbsInstance;
        public static void init()
        {
            // Creating new window
            window = Windows.CreateNewWindow(name, "Select Tile");

            // Activating Scroll View object
            var scrollView = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View");
            scrollView.gameObject.SetActive(true);


            // Fixing size to fit
            var viewport = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View/Viewport");
            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.sizeDelta = new Vector2(0, 17);

            // Getting Content object
            content = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/{window.name}/Background/Scroll View/Viewport/Content");

            // Getting power button selector using reflections with ReflectionUtility
            pbsInstance = Reflection.GetField(typeof(PowerButtonSelector), null, "instance") as PowerButtonSelector;

            initKingdomControl();
        }
        private static void initKingdomControl()
        {
            createTileButton(index++, content.transform, "合并国家", "合并国家", "将两个国家合并", new UnityAction(tryToHideWindow));
            createTileButton(index++, content.transform, "迁都", "迁都", "改变国家首都", new UnityAction(tryToHideWindow2));
            createTileButton(index++, content.transform, "控制外交", "控制外交", "控制指定国家的外交随机性", new UnityAction(tryToHideWindow3));

        }
        private static void createTileButton(int index, Transform pParent, string powerID, string pSprite, string pDescription, UnityAction pCall = null)
        {
            GodPower godPower = new()
            {
                id = powerID,
                name = powerID,
                unselectWhenWindow = true
            };
            AssetManager.powers.add(godPower);
            NewFunction.CreateNewButtonOnWindow(NewFunction.getPositionByIndex(index), pParent, pSprite, godPower, pDescription, pCall, PowerButtonType.Active);
        }

        public static bool tryToCombineKingdom(WorldTile pTile, string pPower)
        {
            if (pTile.zone.city == null)
            {
                return false;
            }
            var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
            if (MoreGodPower.selected_kingdom == null)
            {
                MoreGodPower.selected_kingdom = kingdom;
                NewFunction.LogNewMessage(kingdom, "国家", "想要与某个国家合并......");
            }
            else
            {
                if (kingdom == MoreGodPower.selected_kingdom)
                {
                    return false;
                }
                List<City> cities = new();
                foreach (City city in MoreGodPower.selected_kingdom.cities)
                {
                    cities.Add(city);
                }
                foreach (City city1 in cities)
                {
                    city1.joinAnotherKingdom(kingdom);
                }
                NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "与国家", "合并了。");
                MoreGodPower.selected_kingdom = null;
                cities.Clear();
            }
            return true;
        }

        public static bool tryToChangeCapital(WorldTile pTile, string pPower)
        {
            if (pTile.zone.city == null || pTile.zone.city.isCapitalCity())
            {
                return false;
            }
            City city = pTile.zone.city;
            MoreGodPower.selected_city = city;
            MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
            var data = MoreGodPower.selected_city.data;
            Kingdom kingdom = (Kingdom)Reflection.GetField(city.GetType(), city, "kingdom");
            kingdom.capital = city;
            kingdom.data.capitalID = city.data.id;
            kingdom.location = (Vector3)Reflection.GetField(city.GetType(), city, "cityCenter");
            NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "迁都完毕");
            return true;
        }

        public static bool tryToControlDiplomacy(WorldTile pTile, string pPower)
        {
            if (pTile.zone.city == null)
            {
                return false;
            }
            var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
            if (!MoreGodPower.KingdomsOwnedByPlayer.Contains(kingdom))
            {
                if (MoreGodPower.selected_kingdom == null)
                {
                    MoreGodPower.selected_kingdom = kingdom;
                    NewFunction.LogNewMessage(kingdom, "国家", "将要取消随机外交");
                    NewFunction.LogNewMessage(kingdom, "将要取消随机外交");
                }
                else
                {
                    if (kingdom != MoreGodPower.selected_kingdom)
                    {
                        MoreGodPower.selected_kingdom = null;
                        return false;
                    }
                    foreach (Kingdom kingdom2 in MapBox.instance.kingdoms.list_civs)
                    {
                        if (kingdom.isEnemy(kingdom2))
                        {
                            MapBox.instance.diplomacy.CallMethod("startPeace", kingdom, kingdom2, false);
                        }
                    }
                    MoreGodPower.KingdomsOwnedByPlayer.Add(kingdom);
                    NewFunction.LogNewMessage(kingdom, "国家", "取消了随机外交");
                    NewFunction.LogNewMessage(kingdom, "取消了随机外交");
                    MoreGodPower.selected_kingdom = null;
                }

            }
            else
            {
                if (MoreGodPower.selected_kingdom == null || kingdom != MoreGodPower.selected_kingdom)
                {
                    MoreGodPower.selected_kingdom = kingdom;
                    NewFunction.LogNewMessage(kingdom, "国家", "将要开启随机外交");
                    NewFunction.LogNewMessage(kingdom, "将要开启随机外交");
                }
                else
                {
                    if (kingdom != MoreGodPower.selected_kingdom)
                    {
                        MoreGodPower.selected_kingdom = null;
                        return false;
                    }
                    MoreGodPower.KingdomsOwnedByPlayer.Remove(kingdom);
                    NewFunction.LogNewMessage(kingdom, "国家", "开启了随机外交");
                    NewFunction.LogNewMessage(kingdom, "开启了随机外交");
                    MoreGodPower.selected_kingdom = null;
                }
            }
            return true;
        }

        public static void tryToStartRebellion(City pCity)
        {
            Kingdom kingdom = Reflection.GetField(pCity.GetType(), pCity, "kingdom") as Kingdom;
            pCity.CallMethod("makeOwnKingdom");
            Kingdom kingdom2 = Reflection.GetField(pCity.GetType(), pCity, "kingdom") as Kingdom;
            War war = null;
            foreach (War war2 in World.world.wars.getWars(kingdom))
            {
                if (war2.main_attacker == kingdom && war2.getAsset() == WarTypeLibrary.rebellion)
                {
                    war = war2;
                    war.joinDefenders(kingdom2);
                    break;
                }
            }
            if (war == null)
            {
                war = World.world.diplomacy.CallMethod("startWar", kingdom, kingdom2, WarTypeLibrary.rebellion, true) as War;
                if (kingdom.hasAlliance())
                {
                    foreach (Kingdom kingdom3 in kingdom.getAlliance().kingdoms_hashset)
                    {
                        if (kingdom3 != kingdom && DiplomacyHelpers.diplomacy.getOpinion(kingdom3, kingdom).total >= 0)
                        {
                            war.joinAttackers(kingdom3);
                        }
                    }
                }
            }
            int count = kingdom.cities.Count - 1;
            int num = (int)(count / 3f);
            int num2 = 0;
            while (num2 < num && DiplomacyHelpers.checkMoreAlignedCities(kingdom2, kingdom))
            {
                num2++;
            }
            WorldLog.logCityRevolt(pCity);
        }

        public static void tryToHideWindow()
        {
            power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
            power.click_action = null;
            power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToCombineKingdom));
            ScrollWindow.get(name).clickHide();
            pbsInstance.clickPowerButton(powerButton);
        }

        public static void tryToHideWindow2()
        {
            power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
            power.click_action = null;
            power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToChangeCapital));
            ScrollWindow.get(name).clickHide();
            pbsInstance.clickPowerButton(powerButton);
        }

        public static void tryToHideWindow3()
        {
            power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
            power.click_action = null;
            power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToControlDiplomacy));
            ScrollWindow.get(name).clickHide();
            pbsInstance.clickPowerButton(powerButton);
        }

    }
}
