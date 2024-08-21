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
	class ArmyPowerWindow
	{
		public static string name = "ArmyControlWindow";
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

			initArmyGodpower();
		}

		private static void initArmyGodpower()
		{
			createTileButton(index++, content.transform, "集中进攻", "集中进攻", "所有军队前往指定城市", new UnityAction(tryToHideWindow));
			createTileButton(index++, content.transform, "局部进攻", "局部进攻", "指定一座城市的军队前往另一个城市", new UnityAction(tryToHideWindow2));
			createTileButton(index++, content.transform, "回防", "回防", "所有军队前往驻地城市", new UnityAction(tryToHideWindow3));
			createTileButton(index++, content.transform, "总动员", "总动员", "全国1/3成年人口获得青铜矛+皮革，并转职成战斗人员", new UnityAction(tryToHideWindow4));
			createTileButton(index++, content.transform, "局部动员", "局部动员", "指定城市2/3成年人口获得青铜矛+皮革，并转职成战斗人员", new UnityAction(tryToHideWindow5));
			createTileButton(index++, content.transform, "军团驻扎", "军团驻扎", "指定军团驻扎在某地", new UnityAction(tryToHideWindow6));
			createTileButton(index++, content.transform, "边境防守", "边境防守", "全国军团前往最近的边境城市", new UnityAction(tryToHideWindow7));
			createTileButton(index++, content.transform, "指定防守", "指定防守", "全国军团前往最近的与指定国家接壤的城市", new UnityAction(tryToHideWindow8));
			createTileButton(index++, content.transform, "全面进攻", "全面进攻", "全国军团前往最近的指定国家的城市", new UnityAction(tryToHideWindow9));
			// createTileButton(index++, content.transform, "腐败的军队", "腐败的军队", "全国士兵战斗力大幅下降", new UnityAction(tryToHideWindow10));
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


		public static bool tryToAttack_Kingdom(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			if (MoreGodPower.selected_kingdom == null)
			{
				var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要发动一次集中进攻......");
			}
			else
			{
				foreach (City city in MoreGodPower.selected_kingdom.cities)
				{
					if (city.army != null && city.army.countUnits() > 0)
					{
						NewFunction.MoveArmy(city.army, pTile.zone.city.getTile());
					}
				}
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "发动了一次集中进攻......");
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}
		public static bool tryToAttack_City(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			if (MoreGodPower.selected_city == null)
			{
				var kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
				MoreGodPower.selected_city = city;
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要发动一次局部进攻......");
			}
			else
			{
				if (MoreGodPower.selected_city.army != null && MoreGodPower.selected_city.army.countUnits() > 0)
				{
					MoreGodPower.selected_city.army.city = city;
					NewFunction.MoveArmy(MoreGodPower.selected_city.army, city.getTile());
					NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "发动了一次集中进攻......");
				}
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}
		public static bool tryToCallBackArmy(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			foreach (City city in kingdom.cities)
			{
				if (city.army != null && city.army.countUnits() > 0)
				{
					NewFunction.MoveArmy(city.army, city.getTile(), true);
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家", "全军撤退");
			return true;
		}
		public static bool tryToMobilize_kingdom(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			ItemAsset wItemAsset = AssetManager.items.get("spear");
			ItemAsset aItemAsset = AssetManager.items.get("armor");
			string wMaterial = "bronze";
            foreach (City city in kingdom.cities)
			{
				int num = city.professionsDict[UnitProfession.Unit].Count / 2;
				if (num > 0)
				{
					foreach (Actor actor in city.professionsDict[UnitProfession.Unit])
					{
						if (actor == null) continue;
						actor.CallMethod("setProfession", UnitProfession.Warrior, true);
						actor.ai.setJob("attacker");
						ItemData itemData = ItemGenerator.generateItem(wItemAsset, wMaterial, MapBox.instance.mapStats.year - 10, null, null, 1, actor);
						itemData.modifiers.Clear();
						ActorEquipmentSlot slot = actor.equipment.getSlot(wItemAsset.equipmentType);
						if (slot.data != null)
						{
							ItemTools.calcItemValues(slot.data);
							float num2 = ItemTools.s_value;
							ItemTools.calcItemValues(itemData);
							if (ItemTools.s_value > num2)
							{
								ItemData data = slot.data;
								slot.setItem(itemData);
							}
						}
						slot.setItem(itemData);

						itemData = ItemGenerator.generateItem(aItemAsset, wMaterial, MapBox.instance.mapStats.year - 10, null, null, 1, actor);
						itemData.modifiers.Clear();
						slot = actor.equipment.getSlot(aItemAsset.equipmentType);
						if (slot.data != null)
						{
							ItemTools.calcItemValues(slot.data);
							float num2 = ItemTools.s_value;
							ItemTools.calcItemValues(itemData);
							if (ItemTools.s_value > num2)
							{
								ItemData data = slot.data;
								slot.setItem(itemData);
							}
						}
						slot.setItem(itemData);
						actor.setStatsDirty();
						num--;
						if (num <= 0)
						{
							break;
						}
					}
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家", "动员完毕");
			return true;
		}
		public static bool tryToMobilize_city(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			var kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			string name = city.data.name;
			ItemAsset wItemAsset = AssetManager.items.get("spear");
			ItemAsset aItemAsset = AssetManager.items.get("armor");
			string wMaterial = "bronze";
            int num = city.professionsDict[UnitProfession.Unit].Count / 2;
			if (num > 0)
			{
				foreach (Actor actor in city.professionsDict[UnitProfession.Unit])
				{
					if (actor == null) continue;
					actor.CallMethod("setProfession", UnitProfession.Warrior, true);
					actor.ai.setJob("attacker");
					ItemData itemData = ItemGenerator.generateItem(wItemAsset, wMaterial, MapBox.instance.mapStats.year - 10, null, null, 1, actor);
					itemData.modifiers.Clear();
					ActorEquipmentSlot slot = actor.equipment.getSlot(wItemAsset.equipmentType);
					if (slot.data != null)
					{
						ItemTools.calcItemValues(slot.data);
						float num2 = ItemTools.s_value;
						ItemTools.calcItemValues(itemData);
						if (ItemTools.s_value > num2)
						{
							ItemData data = slot.data;
							slot.setItem(itemData);
						}
					}
					slot.setItem(itemData);

					itemData = ItemGenerator.generateItem(aItemAsset, wMaterial, MapBox.instance.mapStats.year - 10, null, null, 1, actor);
					itemData.modifiers.Clear();
					slot = actor.equipment.getSlot(aItemAsset.equipmentType);
					if (slot.data != null)
					{
						ItemTools.calcItemValues(slot.data);
						float num2 = ItemTools.s_value;
						ItemTools.calcItemValues(itemData);
						if (ItemTools.s_value > num2)
						{
							ItemData data = slot.data;
							slot.setItem(itemData);
						}
					}
					slot.setItem(itemData);
					actor.setStatsDirty();
					num--;
					if (num <= 0)
					{
						break;
					}
				}
			}
			NewFunction.LogNewMessage(kingdom, "国家", "的" + name + "动员完毕");
			return true;
		}
		public static List<Actor> units = new();
		public static bool tryToSettleArmy(WorldTile pTile, string pPower)
		{
			if (MoreGodPower.selected_city == null)
			{
				City city = null;
				foreach (WorldTile tile in pTile.zone.tiles)
				{
					units = Reflection.GetField(tile.GetType(), tile, "_units") as List<Actor>;
					if (units.Count > 0)
					{
						foreach (Actor actor in units)
						{
							if (actor == null) continue;
							if (actor.city != null && actor.is_group_leader)
							{
								city = actor.city;
								break;
							}
						}
					}
				}

				if (city == null)
				{
					return false;
				}
				MoreGodPower.selected_city = city;
				MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
				NewFunction.AddNewText("军团准备派驻。。。", Toolbox.color_log_good, null);
			}
			else
			{
				NewFunction.MoveArmy(MoreGodPower.selected_city.army, pTile, true);
				MoreGodPower.selected_city.army.city = pTile.zone.city;
				MoreGodPower.selected_kingdom = null;
				MoreGodPower.selected_city = null;
				NewFunction.AddNewText("军团开始派驻", Toolbox.color_log_good, null);
			}
			return true;
		}
		public static bool tryToCorruptArmy(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			kingdom.data.set("CorruptArmy", true);
			NewFunction.LogNewMessage(kingdom, "国家", "全国军队变得腐败");
			return true;
		}

		public static List<TileZone> tileZones = new();
		public static List<City> neighbours = new();
		public static List<City> borders = new();
		public static List<City> noBorders = new();
		public static bool tryToSettleAllArmy(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			borders.Clear();
			noBorders.Clear();
			MoreGodPower.selected_city = pTile.zone.city;
			MoreGodPower.selected_kingdom = (Kingdom)Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom");
			foreach (City city in MoreGodPower.selected_kingdom.cities)
			{
				if (NewFunction.isBorder(city, MoreGodPower.selected_kingdom))
				{
					borders.Add(city);
				}
				else if (!city.isCapitalCity())
				{
					noBorders.Add(city);
				}
			}
			if (noBorders.Count > 0 && borders.Count > 0)
			{
				foreach (City city in noBorders)
				{
					if (city.army != null)
					{
						MoreGodPower.selected_city = null;
						foreach (City city2 in borders)
						{
							if (city.getTile() != null && city2.getTile() != null && city.getTile().isSameIsland(city2.getTile()))
							{
								if (MoreGodPower.selected_city == null || Toolbox.DistTile(city.getTile(), city2.getTile()) < Toolbox.DistTile(city.getTile(), MoreGodPower.selected_city.getTile()))
								{
									MoreGodPower.selected_city = city2;
								}
							}
						}
						if (MoreGodPower.selected_city != null)
						{
							NewFunction.MoveArmy(city.army, MoreGodPower.selected_city.getTile(), true);
							city.army.city = pTile.zone.city;
						}
					}
				}
				NewFunction.AddNewText("军团开始前往边境", Toolbox.color_log_good, null);
			}
			MoreGodPower.selected_city = null;
			MoreGodPower.selected_kingdom = null;
			noBorders.Clear();
			borders.Clear();
			return true;
		}
		public static bool tryToSettleAllArmyOn(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "准备调遣军团前往边境");
			}
			else
			{
				if (MoreGodPower.selected_kingdom.cities.Count == 0)
				{
					NewFunction.LogNewMessage(kingdom, "国家", "没有军团可供指挥");
					MoreGodPower.selected_kingdom = null;
					return false;
				}
				if (MoreGodPower.selected_kingdom == kingdom)
				{
					MoreGodPower.selected_kingdom = null;
					return false;
				}
				borders.Clear();
				noBorders.Clear();
				foreach (City city2 in MoreGodPower.selected_kingdom.cities)
				{
					if (NewFunction.isBorder(city2, MoreGodPower.selected_kingdom, kingdom))
					{
						borders.Add(city2);
					}
					else if (!city2.isCapitalCity())
					{
						noBorders.Add(city2);
					}
				}
				if (noBorders.Count > 0 && borders.Count > 0)
				{
					foreach (City city2 in noBorders)
					{
						if (city2.army != null)
						{
							MoreGodPower.selected_city = null;
							foreach (City city3 in borders)
							{
								if (city2.getTile() != null && city3.getTile() != null && city2.getTile().isSameIsland(city3.getTile()))
								{
									if (MoreGodPower.selected_city == null || Toolbox.DistTile(city2.getTile(), city3.getTile()) < Toolbox.DistTile(city2.getTile(), MoreGodPower.selected_city.getTile()))
									{
										MoreGodPower.selected_city = city3;
									}
								}
							}
							if (MoreGodPower.selected_city != null)
							{
								NewFunction.MoveArmy(city2.army, MoreGodPower.selected_city.getTile(), true);
								city2.army.city = pTile.zone.city;
							}
						}
					}
					NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "派遣军队前往", "边境");
				}
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
				noBorders.Clear();
				borders.Clear();
			}
			return true;
		}
		public static bool tryToAttackKingdom_AllArmy(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "准备调遣军团进攻某个国家。。。");
			}
			else
			{
				if (MoreGodPower.selected_kingdom.cities.Count == 0)
				{
					NewFunction.LogNewMessage(kingdom, "国家", "没有军团可供指挥");
					MoreGodPower.selected_kingdom = null;
					return false;
				}
				if (MoreGodPower.selected_kingdom == kingdom)
				{
					MoreGodPower.selected_kingdom = null;
					return false;
				}
				borders.Clear();
				noBorders.Clear();
				foreach (City city2 in kingdom.cities)
				{
					if (NewFunction.isBorder(city2, kingdom, MoreGodPower.selected_kingdom))
					{
						borders.Add(city2);
					}
				}
				if (borders.Count > 0)
				{
					foreach (City city2 in MoreGodPower.selected_kingdom.cities)
					{
						if (city2.army != null)
						{
							MoreGodPower.selected_city = null;
							foreach (City city3 in borders)
							{
								if (city2.getTile() != null && city3.getTile() != null && city2.getTile().isSameIsland(city3.getTile()))
								{
									if (MoreGodPower.selected_city == null || Toolbox.DistTile(city2.getTile(), city3.getTile()) < Toolbox.DistTile(city2.getTile(), MoreGodPower.selected_city.getTile()))
									{
										MoreGodPower.selected_city = city3;
									}
								}
							}
							if (MoreGodPower.selected_city != null)
							{
								NewFunction.MoveArmy(city2.army, MoreGodPower.selected_city.getTile(), true);
								city2.army.city = pTile.zone.city;
							}
						}
					}
					NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "派遣军队进攻国家", "");
				}
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
				noBorders.Clear();
				borders.Clear();
			}
			return true;
		}



		public static void tryToHideWindow()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToAttack_Kingdom));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow2()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToAttack_City));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow3()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToCallBackArmy));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow4()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToMobilize_kingdom));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow5()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToMobilize_city));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow6()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToSettleArmy));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow7()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToSettleAllArmy));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow8()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToSettleAllArmyOn));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow9()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToAttackKingdom_AllArmy));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow10()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToCorruptArmy));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}

		public static void attackCity(Kingdom pKingdom, City tTarget)
		{
			foreach (City city in pKingdom.cities)
			{
				if (city.army != null && city.army.countUnits() > 0)
				{
					NewFunction.MoveArmy(city.army, tTarget.getTile());
				}
			}
			NewFunction.LogNewMessage(pKingdom, "国家", "对城市 " + tTarget.data.name + " 发动了集中进攻");
		}
		public static void attackKigdom(Kingdom pKingdom, Kingdom tTarget)
		{
			borders.Clear();
			noBorders.Clear();
			foreach (City city2 in tTarget.cities)
			{
				if (NewFunction.isBorder(city2, tTarget, pKingdom))
				{
					borders.Add(city2);
				}
			}
			if (borders.Count > 0)
			{
				foreach (City city2 in pKingdom.cities)
				{
					if (city2.army != null)
					{
						MoreGodPower.selected_city = null;
						foreach (City city3 in borders)
						{
							if (city2.getTile() != null && city3.getTile() != null && city2.getTile().isSameIsland(city3.getTile()))
							{
								if (MoreGodPower.selected_city == null || Toolbox.DistTile(city2.getTile(), city3.getTile()) < Toolbox.DistTile(city2.getTile(), MoreGodPower.selected_city.getTile()))
								{
									MoreGodPower.selected_city = city3;
								}
							}
						}
						if (MoreGodPower.selected_city != null)
						{
							NewFunction.MoveArmy(city2.army, MoreGodPower.selected_city.getTile());
						}
					}
				}
				MoreGodPower.selected_city = null;
				NewFunction.LogNewMessage(pKingdom, tTarget, "国家", "派遣军队进攻国家", "");
			}
			noBorders.Clear();
			borders.Clear();
		}
		public static void massArmy(Kingdom pKingdom, City tTarget)
		{
			foreach (City city in pKingdom.cities)
			{
				if (city.army != null && city.army.countUnits() > 0)
				{
					NewFunction.MoveArmy(city.army, tTarget.getTile(), true);
				}
			}
			NewFunction.LogNewMessage(pKingdom, "国家", "将军队聚集在城市 " + tTarget.data.name);
		}
		public static void DefenceKingdom(Kingdom pKingdom, Kingdom tTarget)
		{
			borders.Clear();
			noBorders.Clear();
			foreach (City city2 in pKingdom.cities)
			{
				if (NewFunction.isBorder(city2, pKingdom, tTarget))
				{
					borders.Add(city2);
				}
			}
			if (borders.Count > 0)
			{
				foreach (City city2 in pKingdom.cities)
				{
					if (city2.army != null)
					{
						MoreGodPower.selected_city = null;
						foreach (City city3 in borders)
						{
							if (city2.getTile() != null && city3.getTile() != null && city2.getTile().isSameIsland(city3.getTile()))
							{
								if (MoreGodPower.selected_city == null || Toolbox.DistTile(city2.getTile(), city3.getTile()) < Toolbox.DistTile(city2.getTile(), MoreGodPower.selected_city.getTile()))
								{
									MoreGodPower.selected_city = city3;
								}
							}
						}
						if (MoreGodPower.selected_city != null)
						{
							NewFunction.MoveArmy(city2.army, MoreGodPower.selected_city.getTile(), true);
						}
					}
				}
				MoreGodPower.selected_city = null;
				NewFunction.LogNewMessage(pKingdom, tTarget, "国家", "派遣军队驻防", "的边境");
			}
			noBorders.Clear();
			borders.Clear();
		}
		public static void Defence(Kingdom pKingdom)
		{
			borders.Clear();
			noBorders.Clear();
			foreach (City city2 in pKingdom.cities)
			{
				if (NewFunction.isBorder(city2, pKingdom))
				{
					borders.Add(city2);
				}
			}
			if (borders.Count > 0)
			{
				foreach (City city2 in pKingdom.cities)
				{
					if (city2.army != null)
					{
						MoreGodPower.selected_city = null;
						foreach (City city3 in borders)
						{
							if (city2.getTile() != null && city3.getTile() != null && city2.getTile().isSameIsland(city3.getTile()))
							{
								if (MoreGodPower.selected_city == null || Toolbox.DistTile(city2.getTile(), city3.getTile()) < Toolbox.DistTile(city2.getTile(), MoreGodPower.selected_city.getTile()))
								{
									MoreGodPower.selected_city = city3;
								}
							}
						}
						if (MoreGodPower.selected_city != null)
						{
							NewFunction.MoveArmy(city2.army, MoreGodPower.selected_city.getTile(), true);
						}
					}
				}
				MoreGodPower.selected_city = null;
				NewFunction.LogNewMessage(pKingdom, "国家", "派遣军队驻防边境");
			}
			noBorders.Clear();
			borders.Clear();
		}
	}
}
