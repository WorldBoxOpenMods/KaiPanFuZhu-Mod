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
	class CityPowerWindow
	{
		public static string name = "CityControlWindow";
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

			initCityGodpower();
		}

		private static void initCityGodpower()
		{
			createTileButton(index++, content.transform, "移交城市", "移交城市", "移交城市的主权", new UnityAction(tryToHideWindow));
			createTileButton(index++, content.transform, "开发城市", "开发城市", "在指定城市的所有空地建造一栋一级住房", new UnityAction(tryToHideWindow2));
			createTileButton(index++, content.transform, "升级城市", "升级城市", "指定城市的所有建筑提升一级", new UnityAction(tryToHideWindow3));
			createTileButton(index++, content.transform, "扩张城市", "扩张城市", "将指定城市的领土向外扩张一圈", new UnityAction(tryToHideWindow4));
			createTileButton(index++, content.transform, "建造城墙", "建造城墙", "在指定城市的外围建造一圈箭塔", new UnityAction(tryToHideWindow5));
			createTileButton(index++, content.transform, "合并城市", "合并城市", "合并两座城市", new UnityAction(tryToHideWindow6));
			createTileButton(index++, content.transform, "宣称城市", "宣称城市", "指定国家获得该城市的宣称", new UnityAction(tryToHideWindow7));
			createTileButton(index++, content.transform, "取消宣称", "取消宣称", "指定取消该城市的宣称", new UnityAction(tryToHideWindow8));

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


		public static bool tryToTransferCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_city == null && !city.isCapitalCity())
			{
				MoreGodPower.selected_city = city;
				MoreGodPower.selected_kingdom = kingdom;
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "想要移交城市 " + data.name + " 的主权.....");
			}
			else if (MoreGodPower.selected_city != null)
			{
				if (kingdom == MoreGodPower.selected_kingdom)
					return false;
				MoreGodPower.selected_city.joinAnotherKingdom(kingdom);
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "将城市 " + data.name + " 移交给国家", "了。");
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}
		public static bool tryToDevelopCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			MoreGodPower.selected_city = city;
			MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
			var data = MoreGodPower.selected_city.data;
			List<TileZone> zones = Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>;
			if (zones.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有领土");
				return false;
			}
			List<WorldTile> worldTiles = new();
			string buildings;
			switch (((Race)Reflection.GetField(city.GetType(), city, "race")).id)
			{
				case "human":
					buildings = SB.house_human_0;
					break;
				case "elf":
					buildings = SB.house_elf_0;
					break;
				case "orc":
					buildings = SB.house_orc_0;
					break;
				case "dwarf":
					buildings = SB.house_dwarf_0;
					break;
				case "Xia":
					buildings = "house_Xia";
					break;
				case "Rome":
					buildings = "house_Rome";
					break;
				case "Arab":
					buildings = "house_Arab";
					break;
				case "Russia":
					buildings = "houseRussia";
					break;
				default:
					return false;
			}
			BuildingAsset buildingAsset = AssetManager.buildings.get(buildings);
			foreach (TileZone tileZone in zones)
			{
				WorldTile worldTile = tileZone.centerTile;
				if (worldTile.canBuildOn(buildingAsset) && worldTile.tile_left.canBuildOn(buildingAsset))
				{
					if (!worldTile.tile_right.canBuildOn(buildingAsset))
					{
						worldTile = worldTile.tile_left;
					}
					if (!worldTile.tile_up.tile_up.canBuildOn(buildingAsset))
					{
						worldTile = worldTile.tile_down;
					}
					if (worldTile.canBuildOn(buildingAsset))
					{
						worldTiles.Add(worldTile);
					}
				}
			}
			if (worldTiles.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有可供开发的领土");
				return false;
			}

			foreach (WorldTile worldTile in worldTiles)
			{
				Building building = MapBox.instance.buildings.CallMethod("addBuilding", buildings, worldTile, false, false, BuildPlacingType.New) as Building;
				city.addBuilding(building);
			}
			worldTiles.Clear();
			NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 开发完毕");
			return true;
		}
		public static bool tryToUpgradeCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			MoreGodPower.selected_city = city;
			MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
			var data = MoreGodPower.selected_city.data;
			if (city.buildings.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有建筑");
				return false;
			}
			List<Building> houses = new();
			foreach (Building building in city.buildings.getSimpleList())
			{
				if (((BuildingAsset)Reflection.GetField(building.GetType(), building, "asset")).upgradeTo != string.Empty)
				{
					houses.Add(building);
				}
			}
			if (houses.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有可以升级的建筑");
				return false;
			}
			foreach (Building building in houses)
			{
				string upgradeTo = ((BuildingAsset)Reflection.GetField(building.GetType(), building, "asset")).upgradeTo;
				BuildingAsset buildingAsset = AssetManager.buildings.get(upgradeTo);
				city.CallMethod("setBuildingDictID", building, false);
				building.CallMethod("setTemplate", buildingAsset);
				city.CallMethod("setBuildingDictID", building, true);
				building.CallMethod("initAnimationData");
				building.CallMethod("updateStats");
				(Reflection.GetField(building.GetType(), building, "data") as BuildingData).health = building.getMaxHealth();
				building.CallMethod("fillTiles");
			}
			houses.Clear();
			NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 升级完毕");
			return true;
		}
		public static bool tryToExpandCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			List<TileZone> zones = Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>;
			List<TileZone> tileZones = new();
			foreach (TileZone tileZone in zones)
			{
				foreach (TileZone tileZone2 in tileZone.neighboursAll)
				{
					if (tileZone2 != null && !zones.Contains(tileZone2) && !tileZones.Contains(tileZone2) && tileZone2.city == null)
					{
						tileZones.Add(tileZone2);
					}
				}
			}
			if (tileZones.Count == 0)
			{
				return false;
			}
			foreach (TileZone tileZone in tileZones)
			{
				city.CallMethod("addZone", tileZone);
			}
			return true;
		}
		public static bool tryToBuildWall(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			MoreGodPower.selected_city = city;
			MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
			var data = MoreGodPower.selected_city.data;
			List<TileZone> tileZones = new(Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>);
			if (tileZones.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有领土");
				return false;
			}
			List<WorldTile> tiles = new();
			foreach (TileZone tileZone in tileZones)
			{
				foreach (WorldTile worldTile in tileZone.tiles)
				{
					if (worldTile != null && !tiles.Contains(worldTile) && (worldTile.tile_up == null || worldTile.tile_up.zone.city != city || worldTile.tile_down == null || worldTile.tile_down.zone.city != city ||
						worldTile.tile_left == null || worldTile.tile_left.zone.city != city || worldTile.tile_right == null || worldTile.tile_right.zone.city != city))
					{
						tiles.Add(worldTile);
					}
				}
			}
			if (tiles.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有可供建造箭塔的领土");
				return false;
			}
			for (int i = 0; i < tiles.Count; i++)
			{
				if (tiles[i].tile_up == null || tiles[i].tile_up.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_down;
				}
				if (tiles[i].tile_left == null || tiles[i].tile_left.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_right;
				}
				if (tiles[i].tile_right == null || tiles[i].tile_right.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_left;
				}
			}
			for (int i = 0; i < tiles.Count - 1; i++)
			{
				for (int j = i + 1; j < tiles.Count; j++)
				{
					if (tiles[i].y < tiles[j].y)
					{
						WorldTile tile = tiles[i];
						tiles[i] = tiles[j];
						tiles[j] = tile;
					}
				}
			}
			for (int i = 0; i < tiles.Count - 1; i++)
			{
				for (int j = i + 1; j < tiles.Count; j++)
				{
					if (tiles[j].y != tiles[i].y)
					{
						break;
					}
					if (tiles[i].x < tiles[j].x)
					{
						WorldTile tile = tiles[i];
						tiles[i] = tiles[j];
						tiles[j] = tile;
					}
				}
			}
			List<WorldTile> worldTiles = new();
			string buildings;
			switch (((Race)Reflection.GetField(city.GetType(), city, "race")).id)
			{
				case "human":
					buildings = SB.watch_tower_human;
					break;
				case "elf":
					buildings = SB.watch_tower_elf;
					break;
				case "orc":
					buildings = SB.watch_tower_orc;
					break;
				case "dwarf":
					buildings = SB.watch_tower_dwarf;
					break;
				case "Xia":
					buildings = "watch_tower_Xia";
					break;
				case "Rome":
					buildings = "watch_tower_Rome";
					break;
				case "Arab":
					buildings = "watch_tower_Arab";
					break;
				default:
					return false;
			}
			foreach (WorldTile tile in tiles)
			{
				if (!(worldTiles.Contains(tile) || worldTiles.Contains(tile.tile_left) || worldTiles.Contains(tile.tile_right) || worldTiles.Contains(tile.tile_up)
					|| worldTiles.Contains(tile.tile_up.tile_left) || worldTiles.Contains(tile.tile_up.tile_right)))
				{
					Building building = MapBox.instance.buildings.CallMethod("addBuilding", buildings, tile, false, false, BuildPlacingType.New) as Building;
					city.addBuilding(building);
					worldTiles.Add(tile);
					worldTiles.Add(tile.tile_left);
					worldTiles.Add(tile.tile_right);
					worldTiles.Add(tile.tile_up);
				}
			}
			tileZones.Clear();
			worldTiles.Clear();
			tiles.Clear();
			NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 城墙建造完毕");
			return true;
		}
		public static bool tryToCombineCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			if (MoreGodPower.selected_city == null)
			{
				MoreGodPower.selected_city = pTile.zone.city;
				NewFunction.AddNewText("准备合并城市......", Toolbox.color_log_good, null);
			}
			else
			{
				if (pTile.zone.city == MoreGodPower.selected_city)
				{
					return false;
				}
				City city = pTile.zone.city;
				List<TileZone> zones = new(Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>);
				List<Building> buildings = new(city.buildings.getSimpleList());
				List<Actor> actors = city.units.getSimpleList();
				if (zones.Count > 0)
				{
					foreach (TileZone tileZone in zones)
					{
						MoreGodPower.selected_city.CallMethod("addZone", tileZone);
					}
				}
				if (actors.Count > 0)
				{
					foreach (Actor actor in actors)
					{
						if (actor == null) continue;
						MoreGodPower.selected_city.addNewUnit(actor, false);
					}
				}
				if (buildings.Count > 0)
				{
					foreach (Building building in buildings)
					{
						city.removeBuilding(building);
						MoreGodPower.selected_city.addBuilding(building);
					}
				}
				MoreGodPower.selected_city = null;
				zones.Clear();
				actors.Clear();
				buildings.Clear();
				NewFunction.AddNewText("城市合并完成", Toolbox.color_log_good, null);
			}
			return true;
		}
		public static bool tryToDeclareCity(WorldTile pTile, string pPower)
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
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "想要转移城市 " + data.name + " 的宣称.....");
			}
			else if (MoreGodPower.selected_city != null)
			{
				if (kingdom == MoreGodPower.selected_kingdom)
					return false;
				var data = MoreGodPower.selected_city.data;
				data.set("DeclareKingdomID", kingdom.data.id);
				data.set("Declare", true);
				kingdom.data.set("DeclareCity", true);
				if (MoreGodPower.Declares.ContainsKey(kingdom))
				{
					MoreGodPower.Declares[kingdom].Add(city);
				}
				else
				{
					MoreGodPower.Declares.Add(kingdom, new List<City>() { city });
				}
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "的城市 " + data.name + " 被国家", "宣称了。");
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}
		public static bool tryToRemoveDeclareCity(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			City city = pTile.zone.city;
			MoreGodPower.selected_city = city;
			MoreGodPower.selected_kingdom = Reflection.GetField(MoreGodPower.selected_city.GetType(), MoreGodPower.selected_city, "kingdom") as Kingdom;
			var data = MoreGodPower.selected_city.data;

			if (MoreGodPower.selected_city != null)
			{
				Kingdom kingdom=harmony_declare.GetDeclareKingdom(city);
				
				data.set("DeclareKingdomID", "");
				data.set("Declare", false);
				if (MoreGodPower.Declares.ContainsKey(kingdom))
				{
					MoreGodPower.Declares[kingdom].Remove(city);
				}
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom,  "的城市 " + data.name + " 取消宣称");
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}

		public static void tryToHideWindow()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToTransferCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow2()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToDevelopCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow3()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToUpgradeCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow4()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToExpandCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow5()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToBuildWall));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow6()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToCombineCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow7()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToDeclareCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow8()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToRemoveDeclareCity));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}


		public static void AddUnitOfCity(City city, int num)
		{
			if (city.getTile() == null)
			{
				return;
			}
			Actor actor;
			for (int i = 0; i < num; i++)
			{
				actor = MapBox.instance.units.spawnNewUnit("unit_" + city.data.race, city.getTile(), false, 0);
				city.addNewUnit(actor, true);
			}
		}
		public static void TransferCity(City city)
		{
			var kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_city == null && !city.isCapitalCity())
			{
				MoreGodPower.selected_city = city;
				MoreGodPower.selected_kingdom = kingdom;
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "想要移交城市 " + data.name + " 的主权.....");
			}
			else if (MoreGodPower.selected_city != null)
			{
				if (kingdom == MoreGodPower.selected_kingdom)
					return;
				MoreGodPower.selected_city.joinAnotherKingdom(kingdom);
				var data = MoreGodPower.selected_city.data;
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "将城市 " + data.name + " 移交给国家", "了。");
				MoreGodPower.selected_city = null;
				MoreGodPower.selected_kingdom = null;
			}
			return;
		}
		public static void DevelopCity(City city)
		{
			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			var data = city.data;
			List<TileZone> zones = Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>;
			if (zones.Count == 0)
			{
				NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 没有领土");
				return;
			}
			List<WorldTile> worldTiles = new();
			string buildings;
			switch (((Race)Reflection.GetField(city.GetType(), city, "race")).id)
			{
				case "human":
					buildings = SB.house_human_0;
					break;
				case "elf":
					buildings = SB.house_elf_0;
					break;
				case "orc":
					buildings = SB.house_orc_0;
					break;
				case "dwarf":
					buildings = SB.house_dwarf_0;
					break;
				default:
					return;
			}
			BuildingAsset buildingAsset = AssetManager.buildings.get(buildings);
			foreach (TileZone tileZone in zones)
			{
				WorldTile worldTile = tileZone.centerTile;
				if (worldTile.canBuildOn(buildingAsset) && worldTile.tile_left.canBuildOn(buildingAsset))
				{
					if (!worldTile.tile_right.canBuildOn(buildingAsset))
					{
						worldTile = worldTile.tile_left;
					}
					if (!worldTile.tile_up.tile_up.canBuildOn(buildingAsset))
					{
						worldTile = worldTile.tile_down;
					}
					if (worldTile.canBuildOn(buildingAsset))
					{
						worldTiles.Add(worldTile);
					}
				}
			}
			if (worldTiles.Count == 0)
			{
				NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 没有可供开发的领土");
				return;
			}

			foreach (WorldTile worldTile in worldTiles)
			{
				Building building = MapBox.instance.buildings.CallMethod("addBuilding", buildings, worldTile, false, false, BuildPlacingType.New) as Building;
				city.addBuilding(building);
			}
			worldTiles.Clear();
			NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 开发完毕");
		}
		public static void UpgradeCity(City city)
		{

			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			var data = city.data;
			if (city.buildings.Count == 0)
			{
				NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 没有建筑");
				return;
			}
			List<Building> houses = new();
			foreach (Building building in city.buildings.getSimpleList())
			{
				if (((BuildingAsset)Reflection.GetField(building.GetType(), building, "asset")).upgradeTo != string.Empty)
				{
					houses.Add(building);
				}
			}
			if (houses.Count == 0)
			{
				NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 没有可以升级的建筑");
			}
			foreach (Building building in houses)
			{
				string upgradeTo = ((BuildingAsset)Reflection.GetField(building.GetType(), building, "asset")).upgradeTo;
				BuildingAsset buildingAsset = AssetManager.buildings.get(upgradeTo);
				city.CallMethod("setBuildingDictID", building, false);
				building.CallMethod("setTemplate", buildingAsset);
				city.CallMethod("setBuildingDictID", building, true);
				building.CallMethod("initAnimationData");
				building.CallMethod("updateStats");
				(Reflection.GetField(building.GetType(), building, "data") as BuildingData).health = building.getMaxHealth();
				building.CallMethod("fillTiles");
			}
			houses.Clear();
			NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 升级完毕");
		}
		public static void ExpandCity(City city)
		{
			List<TileZone> zones = Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>;
			List<TileZone> tileZones = new();
			foreach (TileZone tileZone in zones)
			{
				foreach (TileZone tileZone2 in tileZone.neighboursAll)
				{
					if (tileZone2 != null && !zones.Contains(tileZone2) && !tileZones.Contains(tileZone2) && tileZone2.city == null)
					{
						tileZones.Add(tileZone2);
					}
				}
			}
			if (tileZones.Count == 0)
			{
				return;
			}
			foreach (TileZone tileZone in tileZones)
			{
				city.CallMethod("addZone", tileZone);
			}
		}
		public static void BuildWall(City city)
		{
			Kingdom kingdom = Reflection.GetField(city.GetType(), city, "kingdom") as Kingdom;
			var data = city.data;
			List<TileZone> tileZones = new(Reflection.GetField(city.GetType(), city, "zones") as List<TileZone>);
			if (tileZones.Count == 0)
			{
				NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 没有领土");
				return;
			}
			List<WorldTile> tiles = new();
			foreach (TileZone tileZone in tileZones)
			{
				foreach (WorldTile worldTile in tileZone.tiles)
				{
					if (worldTile != null && !tiles.Contains(worldTile) && (worldTile.tile_up == null || worldTile.tile_up.zone.city != city || worldTile.tile_down == null || worldTile.tile_down.zone.city != city ||
						worldTile.tile_left == null || worldTile.tile_left.zone.city != city || worldTile.tile_right == null || worldTile.tile_right.zone.city != city))
					{
						tiles.Add(worldTile);
					}
				}
			}
			if (tiles.Count == 0)
			{
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, "国家", "城市 " + data.name + " 没有可供建造箭塔的领土");
				return;
			}
			for (int i = 0; i < tiles.Count; i++)
			{
				if (tiles[i].tile_up == null || tiles[i].tile_up.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_down;
				}
				if (tiles[i].tile_left == null || tiles[i].tile_left.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_right;
				}
				if (tiles[i].tile_right == null || tiles[i].tile_right.zone.city != tiles[i].zone.city)
				{
					tiles[i] = tiles[i].tile_left;
				}
			}
			for (int i = 0; i < tiles.Count - 1; i++)
			{
				for (int j = i + 1; j < tiles.Count; j++)
				{
					if (tiles[i].y < tiles[j].y)
					{
						WorldTile tile = tiles[i];
						tiles[i] = tiles[j];
						tiles[j] = tile;
					}
				}
			}
			for (int i = 0; i < tiles.Count - 1; i++)
			{
				for (int j = i + 1; j < tiles.Count; j++)
				{
					if (tiles[j].y != tiles[i].y)
					{
						break;
					}
					if (tiles[i].x < tiles[j].x)
					{
						WorldTile tile = tiles[i];
						tiles[i] = tiles[j];
						tiles[j] = tile;
					}
				}
			}
			List<WorldTile> worldTiles = new();
			string buildings;
			switch (((Race)Reflection.GetField(city.GetType(), city, "race")).id)
			{
				case "human":
					buildings = SB.watch_tower_human;
					break;
				case "elf":
					buildings = SB.watch_tower_elf;
					break;
				case "orc":
					buildings = SB.watch_tower_orc;
					break;
				case "dwarf":
					buildings = SB.watch_tower_dwarf;
					break;
				default:
					return;
			}
			foreach (WorldTile tile in tiles)
			{
				if (!(worldTiles.Contains(tile) || worldTiles.Contains(tile.tile_left) || worldTiles.Contains(tile.tile_right) || worldTiles.Contains(tile.tile_up)
					|| worldTiles.Contains(tile.tile_up.tile_left) || worldTiles.Contains(tile.tile_up.tile_right)))
				{
					Building building = MapBox.instance.buildings.CallMethod("addBuilding", buildings, tile, false, false, BuildPlacingType.New) as Building;
					city.addBuilding(building);
					worldTiles.Add(tile);
					worldTiles.Add(tile.tile_left);
					worldTiles.Add(tile.tile_right);
					worldTiles.Add(tile.tile_up);
				}
			}
			tileZones.Clear();
			worldTiles.Clear();
			tiles.Clear();
			NewFunction.LogNewMessage(kingdom, "国家", "城市 " + data.name + " 城墙建造完毕");
		}

	}
}
