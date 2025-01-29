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
using Diplomacy_Army;
using NCMS.Utils;
using System.Linq;

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
		public static Dictionary<Kingdom, List<City>> Declares = new();
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
		public static bool tryToStartDeclareWar(WorldTile pTile, string pPower)
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
				MapBox.instance.diplomacy.CallMethod("startWar", selected_kingdom, kingdom, AssetManager.war_types_library.get("Declare"), true);
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
		public static void AddToDASet()
		{
			foreach (ItemAsset mod in AssetManager.items_modifiers.list)
			{
				if (Main.DASet.ContainsKey($"{mod.id}_modifier_DA") && PowerButtons.ToggleValues.ContainsKey($"{mod.id}_modifier_DA"))
				{
					Main.DASet[$"{mod.id}_modifier_DA"] = PowerButtons.GetToggleValue($"{mod.id}_modifier_DA");
				}
				else if (PowerButtons.ToggleValues.ContainsKey($"{mod.id}_modifier_DA"))
				{
					Main.DASet.Add($"{mod.id}_modifier_DA", PowerButtons.GetToggleValue($"{mod.id}_modifier_DA"));
				}
			}
			foreach (ItemAsset item in AssetManager.items.list)
			{
				if (item.id[0] == '_' || ItemSettingsWindow.wrongItems.Contains(item.id))
				{
					continue;
				}
				if (item.materials.Count <= 0)
				{
					if (Main.DASet.ContainsKey($"{item.id}_DA") && PowerButtons.ToggleValues.ContainsKey($"{item.id}_DA"))
					{
						Main.DASet[$"{item.id}_DA"] = PowerButtons.GetToggleValue($"{item.id}_DA");
					}
					else if (PowerButtons.ToggleValues.ContainsKey($"{item.id}_DA"))
					{
						Main.DASet.Add($"{item.id}_DA", PowerButtons.GetToggleValue($"{item.id}_DA"));
					}


					continue;
				}
				foreach (string material in item.materials)
				{
					if (Main.DASet.ContainsKey($"{item.id}_DA_{material}") && PowerButtons.ToggleValues.ContainsKey($"{item.id}_DA_{material}"))
					{
						Main.DASet[$"{item.id}_DA_{material}"] = PowerButtons.GetToggleValue($"{item.id}_DA_{material}");
					}
					else if (PowerButtons.ToggleValues.ContainsKey($"{item.id}_DA_{material}"))
					{
						Main.DASet.Add($"{item.id}_DA_{material}", PowerButtons.GetToggleValue($"{item.id}_DA_{material}"));
					}
				}
				foreach (var set in Main.DASet.Keys.ToList())
				{
					if (PowerButtons.ToggleValues.ContainsKey(set))
					{
						Main.DASet[set] = PowerButtons.GetToggleValue(set);
					}
				}

			}
			DA_save.SaveDictionaryToFile(Main.DASet, Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", "Diplomacy_ArmySet.json"));
			Debug.Log("保存成功！");
		}
		public static bool clickTraitEditorRainButton(string pPowerId)
		{
			Config.selected_trait_editor = pPowerId;
			ScrollWindow.showWindow("trait_rain_editor");
			return true;
		}

		public static bool CityZoneGrowth(WorldTile pTile, string pPower)
		{
			City city = pTile.zone.city;
			if (city == null)
			{
				return true;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			city.data.get("ZoneGrowth", out bool flag, true);
			city.data.set("ZoneGrowth", !flag);

			string text;
			if (!flag)
			{
				text = "开启";
			}
			else
			{
				text = "关闭";
			}
			NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}", "地块拓展开关状态：" + text);
			return true;
		}
		public static bool GetItemSwitch(WorldTile pTile, string pPower)
		{
			City city = pTile.zone.city;
			if (city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			kingdom.data.set("ProhibitgiveItemKing", PowerButtons.GetToggleValue("国王装备禁用"));
			kingdom.data.set("ProhibitgiveItemLeader", PowerButtons.GetToggleValue("领主装备禁用"));
			kingdom.data.set("ProhibitgiveItemWarrior", PowerButtons.GetToggleValue("城市士兵装备禁用"));
			kingdom.data.set("ProhibitgiveItemGroupLeader", PowerButtons.GetToggleValue("将军装备禁用"));
			kingdom.data.set("ProhibitgiveItemGroupLeader", true);
			NewFunction.LogNewMessage(kingdom, "国家", "装备禁止获取的参数已设置");
			return true;
		}
		public static bool KingdomZoneGrowth(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			// foreach (City city in kingdom.cities)
			// {
			//       city.data.get("ZoneGrowth", out flag, true);
			//       city.data.set("ZoneGrowth", !flag);
			// }
			kingdom.data.get("ZoneGrowth", out bool flag, true);
			kingdom.data.set("ZoneGrowth", !flag);
			string text;
			if (!flag)
			{
				text = "开启";
			}
			else
			{
				text = "关闭";
			}
			NewFunction.LogNewMessage(kingdom, "国家", "地块拓展开关状态：" + text);
			return true;
		}
		public static bool tryToAddResources(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			foreach (City city in kingdom.cities)
			{
				foreach (var resource in Main.resourceSettings.Keys)
				{
					city.data.storage.change(resource, Main.resourceSettings[resource]);
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家", "成功添加资源");
			return true;
		}
		public static bool tryToAddTraits(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			List<string> pList = PlayerConfig.instance.data.trait_editor_gamma;
			foreach (City city in kingdom.cities)
			{
				foreach (Actor act in city.professionsDict[UnitProfession.Warrior])
				{
					if (act.Any())
					{
						if (pList.Count == 0) { return false; }
						int i = 0;
						while (i < pList.Count)
						{
							string pID = pList[i];
							if (AssetManager.traits.get(pID) == null) { pList.RemoveAt(i); }
							else { i++; }
						}
						if (act.asset.can_edit_traits)
						{
							foreach (string pTrait in pList)
							{
								act.addTrait(pTrait);
							}
							act.startShake(0.3f, 0.1f, true, true);
							act.startColorEffect(ActorColorEffect.White);
						}
					}
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家军队", "成功添加特质");
			return true;
		}
		public static bool tryToAddItems(WorldTile pTile, string pPower)
		{
			City city = pTile.zone.city;
			if (city == null)
			{
				return false;
			}

			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;

			List<Actor> actorsToAddItems = new();
			if (PowerButtons.GetToggleValue("ChooseCityWarrior"))
			{
				actorsToAddItems.AddRange(city.professionsDict[UnitProfession.Warrior]);
				NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "士兵成功添加装备！");
			}
			else if (PowerButtons.GetToggleValue("ChooseAllWarrior"))
			{
				foreach (City c in kingdom.cities)
				{
					actorsToAddItems.AddRange(c.professionsDict[UnitProfession.Warrior]);
				}
				NewFunction.LogNewMessage(kingdom, "装备已到达...", "士兵成功添加装备");
			}
			else if (PowerButtons.GetToggleValue("ChooseCityGeneral"))
			{
				City c = pTile.zone.city;
				if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
				{
					actorsToAddItems.Add(c.army.groupLeader);
					NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军成功添加装备");
				}
				else
				{
					NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军添加装备失败");
				}

			}
			else if (PowerButtons.GetToggleValue("ChooseAllGeneral"))
			{
				foreach (City c in kingdom.cities)
				{
					if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
					{
						actorsToAddItems.Add(c.army.groupLeader);
					}
				}
				NewFunction.LogNewMessage(kingdom, "装备已到达... ", " 将军成功添加装备");
			}
			else if (PowerButtons.GetToggleValue("ChooseKing"))
			{
				if (kingdom.king != null && kingdom.king.Any())
				{
					actorsToAddItems.Add(kingdom.king);
					NewFunction.LogNewMessage(kingdom, "装备已到达...", " 国王成功添加装备");
				}

			}
			else if (PowerButtons.GetToggleValue("ChooseLeader"))
			{
				if (city.leader != null && city.leader.Any())
				{
					actorsToAddItems.Add(city.leader);
					NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "领主成功添加装备");
				}

			}

			foreach (Actor act in actorsToAddItems)
			{
				if (act.Any())
				{
					foreach (ItemAsset item in AssetManager.items.list)
					{
						if (item.id[0] == '_' || ItemSettingsWindow.wrongItems.Contains(item.id) || item.materials.Count <= 0)
						{
							continue;
						}

						foreach (string material in item.materials)
						{
							string key = material.Length > 0 ? $"{item.id}_DA_{material}" : $"{item.id}_DA";
							if (PowerButtons.CustomButtons.ContainsKey(key) && PowerButtons.GetToggleValue(key))
							{
								ItemData data = ItemGenerator.generateItem(item, material, World.world.mapStats.year, act.kingdom, act.getName(), 1, act);
								data.modifiers.Clear();
								// if (MoreWindows.itemModifiers.ContainsKey(key.ToString()))
								// {
								//       foreach (ItemAsset modifier in MoreWindows.itemModifiers["1"])
								//       {
								//             // ItemGenerator.tryToAddMod(data, modifier);
								//             data.modifiers.Add(modifier.id);
								//       }
								// }
								foreach (ItemAsset mod in AssetManager.items_modifiers.list)
								{
									if (PowerButtons.GetToggleValue($"{mod.id}_modifier_DA"))
									{
										data.modifiers.Add(mod.id);
									}
								}
								ActorEquipmentSlot slot = act.equipment.getSlot(item.equipmentType);
								slot.setItem(data);
								act.setStatsDirty();
								act.startShake(0.3f, 0.1f, true, true);
								act.startColorEffect(ActorColorEffect.White);
							}

						}
					}
				}
			}

			return true;
		}
		public static bool tryToRemoveTraits(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			List<string> pList = PlayerConfig.instance.data.trait_editor_gamma;
			foreach (City city in kingdom.cities)
			{
				foreach (Actor act in city.professionsDict[UnitProfession.Warrior])
				{
					if (act.Any())
					{
						if (pList.Count == 0) { return false; }
						int i = 0;
						while (i < pList.Count)
						{
							string pID = pList[i];
							if (AssetManager.traits.get(pID) == null) { pList.RemoveAt(i); }
							else { i++; }
						}
						if (act.asset.can_edit_traits)
						{
							foreach (string pTrait in pList)
							{
								act.removeTrait(pTrait);
							}
							act.startShake(0.3f, 0.1f, true, true);
							act.startColorEffect(ActorColorEffect.White);
						}
					}
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家军队", "成功删除特质");
			return true;
		}
		public static bool tryToRemoveItem(WorldTile pTile, string pPower)
		{
			City city = pTile.zone.city;
			if (city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			List<Actor> actorsToAddItems = new();
			if (PowerButtons.GetToggleValue("ChooseCityWarrior"))
			{
				actorsToAddItems.AddRange(city.professionsDict[UnitProfession.Warrior]);
				NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "士兵成功添加装备！");
			}
			else if (PowerButtons.GetToggleValue("ChooseAllWarrior"))
			{
				foreach (City c in kingdom.cities)
				{
					actorsToAddItems.AddRange(c.professionsDict[UnitProfession.Warrior]);
				}
				NewFunction.LogNewMessage(kingdom, "装备已到达...", "士兵成功添加装备");
			}
			else if (PowerButtons.GetToggleValue("ChooseCityGeneral"))
			{
				City c = pTile.zone.city;
				if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
				{
					actorsToAddItems.Add(c.army.groupLeader);
					NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军成功添加装备");
				}
				else
				{
					NewFunction.LogNewMessage(kingdom, $"城市{c.data.name}装备已到达... ", "将军添加装备失败");
				}

			}
			else if (PowerButtons.GetToggleValue("ChooseAllGeneral"))
			{
				foreach (City c in kingdom.cities)
				{
					if (c.army != null && c.army.alive && c.army.countUnits() > 0 && c.army.groupLeader != null)
					{
						actorsToAddItems.Add(c.army.groupLeader);
					}
				}
				NewFunction.LogNewMessage(kingdom, "装备已到达... ", " 将军成功添加装备");
			}
			else if (PowerButtons.GetToggleValue("ChooseKing"))
			{
				if (kingdom.king != null && kingdom.king.Any())
				{
					actorsToAddItems.Add(kingdom.king);
					NewFunction.LogNewMessage(kingdom, "装备已到达...", " 国王成功添加装备");
				}

			}
			else if (PowerButtons.GetToggleValue("ChooseLeader"))
			{
				if (city.leader != null && city.leader.Any())
				{
					actorsToAddItems.Add(city.leader);
					NewFunction.LogNewMessage(kingdom, $"城市{city.data.name}装备已到达... ", "领主成功添加装备");
				}

			}

			foreach (Actor act in actorsToAddItems)
			{
				if (act.Any())
				{
					List<ActorEquipmentSlot> list = ActorEquipment.getList(act.equipment);
					if (list == null)
					{
						continue;
					}
					for (int i = 0; i < list.Count; i++)
					{
						ActorEquipmentSlot actorEquipmentSlot = list[i];
						if (actorEquipmentSlot.data != null)
						{
							if (PowerButtons.CustomButtons.ContainsKey($"{actorEquipmentSlot.data.id}_DA") && PowerButtons.GetToggleValue($"{actorEquipmentSlot.data.id}_DA"))
							{
								actorEquipmentSlot.emptySlot();
								act.setStatsDirty();
								act.dirty_sprite_item = true;
								act.startShake(0.3f, 0.1f, true, true);
								act.startColorEffect(ActorColorEffect.White);
							}
							else if (PowerButtons.CustomButtons.ContainsKey($"{actorEquipmentSlot.data.id}_DA_{actorEquipmentSlot.data.material}"))
							{
								if (PowerButtons.GetToggleValue($"{actorEquipmentSlot.data.id}_DA_{actorEquipmentSlot.data.material}"))
								{
									actorEquipmentSlot.emptySlot();
									act.setStatsDirty();
									act.dirty_sprite_item = true;
									act.startShake(0.3f, 0.1f, true, true);
									act.startColorEffect(ActorColorEffect.White);
								}

							}

						}
					}
				}



			}


			NewFunction.LogNewMessage(kingdom, "国家军队", "成功删除装备");
			return true;
		}
		#region 附庸颜色更新
		//调用宗主国进行循环
		public static void UpdateVassals()
		{
			var vassalsToRemove = new HashSet<Kingdom>();

			foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
			{
				if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
				{
					continue;
				}


				UpdateKingdomVassals(kingdom, vassalsToRemove);
			}

			// foreach (var kingdom in vassalsToRemove)
			// {
			//     RemoveVassals(kingdom);
			// }
		}
		//调用宗主国的附庸出来进行循环
		public static void UpdateKingdomVassals(Kingdom kingdom, HashSet<Kingdom> vassalsToRemove)
		{
			if (MoreGodPower.Vassals.TryGetValue(kingdom, out var vassals))
			{
				foreach (var vassal in vassals.ToList())
				{
					if (vassal == null || vassal.data == null)
					{
						vassals.Remove(vassal);
						continue;
					}
					// UpdateVassalColor(kingdom);

					if (PowerButtons.GetToggleValue("DA_关闭显示附庸颜色") && vassal.data.colorID == kingdom.data.colorID)
					{
						NewFunction.UpdateColor(vassal);
					}
					else if (!PowerButtons.GetToggleValue("DA_关闭显示附庸颜色") && vassal.data.colorID != kingdom.data.colorID)
					{
						UpdateVassalToKingdomColor(vassal, kingdom);
					}
					// UpdateVassalAlliance(vassal, kingdom);
				}

				if (vassals.Count == 0)
				{
					vassalsToRemove.Add(kingdom);
				}
			}
		}

		// public static void UpdateVassalColor(Kingdom kingdom)
		// {
		// 	var vassals = MoreGodPower.Vassals[kingdom].ToList();

		// 	for (int i = 0; i < vassals.Count; i++)
		// 	{
		// 		var vassal = vassals[i];
		// 		if (vassal == null || vassal.data == null)
		// 		{
		// 			MoreGodPower.Vassals[kingdom].Remove(vassal);
		// 			continue;
		// 		}

		// 		// int oldColorID;
		// 		// if (!vassal.data.get("oldColorID", out oldColorID))
		// 		// {
		// 		//     oldColorID = -1;
		// 		// }

		// 	}
		// }
		public static void UpdateVassalToKingdomColor(Kingdom vassal, Kingdom kingdom)
		{

			vassal.data.set("oldColorID", vassal.data.colorID);
			ColorAsset originalColor = vassal.getColor();
			string oldColor = NewFunction.Serialize(originalColor);

			vassal.data.set("oldColor", oldColor);

			vassal.data.colorID = kingdom.data.colorID;
			ColorAsset kingdomcolor = kingdom.getColor();
			vassal.updateColor(kingdomcolor);
			World.world.zoneCalculator.setDrawnZonesDirty();
			World.world.zoneCalculator.clearCurrentDrawnZones(true);
			World.world.zoneCalculator.redrawZones();
		}
		#endregion
	}
}
