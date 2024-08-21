using System;
using NCMS;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReflectionUtility;
using Diplomacy_Diplomacy;

namespace Diplomacy_Army
{
	[ModEntry]
	public class MoreGodPower
	{
		public static Kingdom selected_kingdom;
		public static City selected_city;
		public static Culture selected_culture;
		public static Dictionary<string, bool> toggles = new()
		{
		{ "更多法则", false },
		{ "更多外交", false },
		{ "更多设置", false },
		{ "外交操作", false },
		{ "村庄操作", false },
		{ "国家操作", false },
		{ "显示条约", false },
		{ "显示交战", false } ,
		{ "显示外交消息", false },
		{ "内存清理", false },
		{ "策略模式", false }};



		public static Dictionary<Kingdom, Dictionary<Kingdom, int>> AllianceKingdoms = new();
		public static Dictionary<Kingdom, Dictionary<Kingdom, int>> DefenceKingdoms = new();
		public static Dictionary<Kingdom, Dictionary<Kingdom, int>> ArmyKingdoms = new();
		public static Dictionary<Kingdom, List<Kingdom>> Vassals = new();
		public static Dictionary<Kingdom, List<Kingdom>> KingdomsTryArmy = new();
		public static List<City> citiesCelebrate = new();
		public static List<Kingdom> KingdomsOwnedByPlayer = new();
		public static Dictionary<string, GameObject> newGameObjects = new();

		public static void init()
		{
			#region godPower
			Transform pParent = DiplomacyPowerWindow.content.transform;
			int num = 2;


			pParent = pvz_ui.CustomTabObjs["Diplomacy_Army"].transform;
			num = 2;
			CreateNewActiveGodpower(num++, pParent, "指定宣战", "指定宣战", "强制国家之间开战");
			CreateNewActiveGodpower(num++, pParent, "指定和平", "指定和平", "强制国家之间和平");
			CreateNewSpecialGodpower(num++, pParent, "全屏烟花", "全屏烟花", "所有城市放一次烟花");
			CreateNewActiveGodpower(num++, pParent, "合并文化", "合并文化", "合并两个文明");
			CreateNewActiveGodpower(num++, pParent, "国家操作", "国家操作", "国家相关的操作", new UnityAction(tryToOpenWindow));
			CreateNewActiveGodpower(num++, pParent, "村庄操作", "村庄操作", "村庄相关的操作", new UnityAction(tryToOpenWindow2));
			CreateNewActiveGodpower(num++, pParent, "军事操作", "军事操作", "军事相关的操作", new UnityAction(tryToOpenWindow3));
			CreateNewActiveGodpower(num++, pParent, "外交操作", "外交操作", "外交相关的操作", new UnityAction(tryToOpenWindow4));
			#endregion
		}

		public static void CreateNewActiveGodpower(int index, Transform pParent, string powerID, string pSprite, string pDescription, UnityAction pCall = null)
		{
			GodPower godPower = new()
			{
				id = powerID,
				name = powerID,
				unselectWhenWindow = true
			};
			switch (powerID)
			{
				case "指定宣战":
					godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToStartWar));
					break;
				case "指定和平":
					godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToEndWar));
					break;
				case "合并文化":
					godPower.click_action = (PowerActionWithID)Delegate.Combine(godPower.click_action, new PowerActionWithID(tryToCombineCulture));
					break;
				case "国家操作":
				case "村庄操作":
				case "军事操作":
				case "外交操作":
					break;
				default:
					return;
			}
			AssetManager.powers.add(godPower);
			GameObject gameObject = NewFunction.CreateNewButton(index, pParent, pSprite, godPower, pDescription, pCall, PowerButtonType.Active);
			switch (powerID)
			{
				case "国家操作":
					KingdomPowerWindow.powerButton = gameObject.GetComponent<PowerButton>();
					break;
				case "村庄操作":
					CityPowerWindow.powerButton = gameObject.GetComponent<PowerButton>();
					break;
				case "军事操作":
					ArmyPowerWindow.powerButton = gameObject.GetComponent<PowerButton>();
					break;
				case "外交操作":
					DiplomacyPowerWindow.powerButton = gameObject.GetComponent<PowerButton>();
					break;
				default:
					break;
			}
		}

		public static void CreateNewSpecialGodpower(int index, Transform pParent, string powerID, string pSprite, string pDescription, UnityAction pCall = null)
		{
			GodPower godPower = new()
			{
				id = powerID,
				name = powerID,
				unselectWhenWindow = true
			};
			switch (powerID)
			{
				case "全屏烟花":
					godPower.toggle_action = (PowerToggleAction)Delegate.Combine(godPower.toggle_action, new PowerToggleAction(tryToCelebrate));
					break;
				default:
					godPower.toggle_action = (PowerToggleAction)Delegate.Combine(godPower.toggle_action, new PowerToggleAction(toggleOption));
					break;
			}
			AssetManager.powers.add(godPower);
			GameObject gameObject = NewFunction.CreateNewButton(index, pParent, pSprite, godPower, pDescription, pCall, PowerButtonType.Special);
			newGameObjects.Add(powerID, gameObject);
		}

		public static void tryToOpenWindow()
		{
			ScrollWindow.showWindow("KingdomControlWindow");
		}
		public static void tryToOpenWindow2()
		{
			ScrollWindow.showWindow("CityControlWindow");
		}
		public static void tryToOpenWindow3()
		{
			ScrollWindow.showWindow("ArmyControlWindow");
		}
		public static void tryToOpenWindow4()
		{
			ScrollWindow.showWindow("DiplomacyControlWindow");
		}
		public static bool tryToStartWar(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (selected_kingdom == null)
			{
				selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要发动一场战争......");
			}
			else
			{
				if (kingdom == selected_kingdom)
				{
					return false;
				}
				War war = MapBox.instance.wars.getWar(selected_kingdom, kingdom, false);
				if (war != null)
				{
					NewFunction.LogNewMessage(selected_kingdom, kingdom, "国家", "和国家", "正在交战");
					return false;
				}
				MapBox.instance.diplomacy.CallMethod("startWar", selected_kingdom, kingdom, WarTypeLibrary.whisper_of_war, true);
				selected_kingdom = null;
			}
			return true;
		}

		public static bool tryToEndWar(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (selected_kingdom == null)
			{
				selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要结束一场战争......");
			}
			else
			{
				if (kingdom == selected_kingdom)
				{
					return false;
				}
				War war = MapBox.instance.wars.getWar(selected_kingdom, kingdom, false);
				if (war == null)
				{
					NewFunction.LogNewMessage(selected_kingdom, kingdom, "国家", "和国家", "之间不存在战争");
					return false;
				}
				while (war != null)
				{
					MapBox.instance.wars.endWar(war);
					war = MapBox.instance.wars.getWar(selected_kingdom, kingdom, false);
				}
				selected_kingdom = null;
			}

			return true;
		}

		public static bool tryToCombineCulture(WorldTile pTile, string pPower)
		{
			if (pTile.zone.culture == null)
			{
				return false;
			}
			var culture = pTile.zone.culture;
			if (selected_culture == null)
			{
				selected_culture = culture;
				NewFunction.AddNewText(culture.name + "文化即将被合并", Toolbox.color_log_good, null);
			}
			else
			{
				if (selected_culture == culture)
				{
					return false;
				}
				if (selected_culture.zones.Count == 0)
				{
					selected_culture = null;
					return false;
				}
				foreach (var item in selected_culture.zones)
				{
					culture.zones.Add(item);
					item.setCulture(culture);
				}
				foreach (Actor actor in MapBox.instance.units)
				{
					if (actor.getCulture() == selected_culture)
					{
						actor.CallMethod("setCulture", culture);
					}
				}
				selected_culture.zones.Clear();
				MapBox.instance.cultures.list.Remove(selected_culture);
				selected_culture = null;
			}
			return true;
		}

		public static void toggleOption(string pPower)
		{
			GodPower godPower = AssetManager.powers.get(pPower);
			WorldTip.instance.showToolbarText(godPower);
			toggles[pPower] = !toggles[pPower];
		}

		public static void tryToCelebrate(string pPower)
		{
			//float timeout = 0.05f;
			if (MapBox.instance.cities.list.Count == 0)
			{
				return;
			}
			foreach (City city in MapBox.instance.cities.list)
			{
				citiesCelebrate.Add(city);
			}
		}

		public static bool tryToCombineArmy(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_city == null)
			{
				MoreGodPower.selected_city = city;
				MoreGodPower.selected_kingdom = kingdom;
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "想要在城市 " + data.name + " 组建军团");
			}
			else
			{
				if (kingdom != MoreGodPower.selected_kingdom)
					return false;
				var units = Reflection.GetField(MoreGodPower.selected_city.army.GetType(), MoreGodPower.selected_city.army, "units") as ActorContainer;
				foreach (Actor actor in units.getSimpleList())
				{
					MoreGodPower.selected_city.army.removeUnit(actor);
					city.army.addUnit(actor);
				}
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "组建了 " + data.name + " 军团");
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}

		public static void startWar(Kingdom attacker, Kingdom defender)
		{
			War war = MapBox.instance.wars.getWar(attacker, defender, false);
			if (war != null)
			{
				return;
			}
			MapBox.instance.diplomacy.CallMethod("startWar", attacker, defender, WarTypeLibrary.whisper_of_war, true);
		}

		public static void endWar(Kingdom kingdom1, Kingdom kingdom2)
		{
			War war = MapBox.instance.wars.getWar(kingdom1, kingdom2, false);
			if (war == null)
			{
				return;
			}
			while (war != null)
			{
				MapBox.instance.wars.endWar(war);
				war = MapBox.instance.wars.getWar(kingdom1, kingdom2, false);
			}
		}

		public static void formAlliance(Kingdom kingdom1, Kingdom kingdom2, string allianceName)
		{
			if (kingdom1.hasAlliance() || kingdom2.hasAlliance())
			{
				return;
			}
			endWar(kingdom1, kingdom2);
			NewFunction.AddNewText("1", Color.black, null);
			Alliance alliance = MapBox.instance.alliances.newAlliance(kingdom1, kingdom2);
			alliance.data.name = allianceName;
		}
	}
}
