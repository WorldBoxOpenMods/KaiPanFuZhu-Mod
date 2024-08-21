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
using Diplomacy_Army;
using UnityEngine.UI;
using Newtonsoft.Json;
using static Diplomacy_Army.Main;
using System.IO;

namespace Diplomacy_Army
{
	class DiplomacyPowerWindow
	{
		public static string name = "DiplomacyControlWindow";
		public static PowerButton powerButton;

		private static ScrollWindow window;
		public static GameObject content;
		private static GodPower power;
		private static int index = 0;
		public static Text contentText;
		public static Text RSText;
		public static float NYJG;
		public static string treaty;

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

			initDiplomacyGodpower();
		}

		private static void initDiplomacyGodpower()
		{
			// createTileButton(index++, content.transform, "指定宣战", "指定宣战", "强制国家之间开战", new UnityAction(tryToHideWindow4));
			// createTileButton(index++, content.transform, "指定和平", "指定和平", "强制国家之间和平", new UnityAction(tryToHideWindow3));
			createTileButton(index++, content.transform, "结盟", "结盟", "让国与国之间结盟", new UnityAction(tryToHideWindow));
			createTileButton(index++, content.transform, "互不侵犯", "互不侵犯", "互不侵犯", new UnityAction(tryToHideWindow2));
			createTileButton(index++, content.transform, "军事通行", "军事通行", "军事通行", new UnityAction(tryToHideWindow5));
			createTileButton(index++, content.transform, "共同防御", "共同防御", "共同防御", new UnityAction(tryToHideWindow6));
			createTileButton(index++, content.transform, "附庸", "附庸", "附庸", new UnityAction(tryToHideWindow7));
			createTileButton(index++, content.transform, "取消附庸", "取消附庸", "取消附庸", new UnityAction(tryToHideWindow8));
			PowerButtons.CreateButton("签订条约或撕毁", Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + "签订或撕毁条约" + ".jpg"),
"签订条约或撕毁", "当按钮打开时是撕毁条约，关闭是签订条约,", NewFunction.getPositionByIndex(index), ButtonType.Toggle, content.transform); index++;


			GameObject contentComponent = window.transform
			.Find("Background")
			.Find("Name").gameObject;
			contentText = contentComponent.GetComponent<Text>();
			contentText.text = "";
			contentText.supportRichText = true;
			contentText.transform.SetParent(window.transform
			.Find("Background")
			.Find("Scroll View")
			.Find("Viewport")
			.Find("Content"));
			contentComponent.SetActive(true);
			content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1000f);
			GameObject UIG = NCMS.Utils.GameObjects.FindEvenInactive("DA_UIG");
			var neDAsmdmyIG1 = GameObject.Instantiate(UIG, contentComponent.transform);
			neDAsmdmyIG1.transform.localPosition = new Vector2(0f, NYJG * 51.9125f - (2 * 40f));
			GameObject RSRef = GameObject.Find($"/Canvas Container Main/Canvas - Windows/windows/DAHelper/Background/Name");
			GameObject RSObj = GameObject.Instantiate(RSRef, contentComponent.transform);
			RSObj.SetActive(true);
			RSText = RSObj.GetComponent<Text>();
			RSText.text = LocalizedTextManager.getText($"numofyears", null);
			RSText.supportRichText = true;
			RSText.transform.SetParent(contentComponent.transform);
			var RSObjRTF = RSObj.GetComponent<RectTransform>();
			RSObjRTF.position = new Vector3(0, 0, 0);
			RSObjRTF.localPosition = new Vector3(20f, NYJG * -0.5431562f - (2 * 40f));


			float ngjg = contentText.preferredHeight;
			contentText.text += "\n";
			NYJG = contentText.preferredHeight - ngjg;
			var BVC = new Vector3(-20f, NYJG * -0.5431562f - (2 * 40f));
			var BVC2 = new Vector3(100f, NYJG * -0.5431562f - (2 * 40f));
			NCMS.Utils.PowerButtons.CreateButton("numofyearsXSLeftButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DALeft.png"),
	  		null, null, BVC, ButtonType.Click, contentComponent.transform, () => SetSCGL(-1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
			NCMS.Utils.PowerButtons.CreateButton("numofyearsXSRightButton", NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DARight.png"),
			null, null, BVC2, ButtonType.Click, contentComponent.transform, () => SetSCGL(1)).button.GetComponent<Image>().sprite = NCMS.Utils.Sprites.LoadSprite($"{Mod.Info.Path}/GameResources/ui/DAsmdmy.png");
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

		public static void tryToHideWindow()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToAlliance_Kingdom));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow2()
		{
			treaty = "互不侵犯";
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(TryToSignNewTreaty));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow3()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(MoreGodPower.tryToEndWar));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow4()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(MoreGodPower.tryToStartWar));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow5()
		{
			treaty = "军事通行";
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(TryToSignNewTreaty));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow6()
		{
			treaty = "共同防御";
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(TryToSignNewTreaty));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow7()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToVassal_Kingdom));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static void tryToHideWindow8()
		{
			power = Reflection.GetField(powerButton.GetType(), powerButton, "godPower") as GodPower;
			power.click_action = null;
			power.click_action = (PowerActionWithID)Delegate.Combine(power.click_action, new PowerActionWithID(tryToVassal_Kingdom2));
			ScrollWindow.get(name).clickHide();
			pbsInstance.clickPowerButton(powerButton);
		}
		public static bool TryToSignNewTreaty(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要与其他国家签订条约......");
			}
			else
			{
				if (kingdom == MoreGodPower.selected_kingdom)
				{
					return false;
				}
				if (!PowerButtons.GetToggleValue("签订条约或撕毁"))
				{
					NewFunction.signNewTreaty(kingdom, MoreGodPower.selected_kingdom, treaty, Main.numofyears, TreatyType.Alliance, true, true, false);
				}
				else
				{
					NewFunction.designTreaty(kingdom, MoreGodPower.selected_kingdom, treaty, true);
				}
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "和国家", "成功签订条约");
				MoreGodPower.selected_kingdom = null;
			}

			return true;
		}
		public static bool tryToVassal_Kingdom(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要被其他国家附庸......");
			}
			else
			{
				if (kingdom == MoreGodPower.selected_kingdom)
				{
					return false;
				}

				MoreGodPower.selected_kingdom.data.get("Vassal", out bool flag1, false);
				kingdom.data.get("Vassal", out bool flag2, false);

				if (flag1 || flag2 || (flag1 && flag2))
				{
					NewFunction.LogNewMessage(flag2 ? kingdom : MoreGodPower.selected_kingdom, "国家", "附庸失败");
					MoreGodPower.selected_kingdom = null;
					return false;
				}

				MoreGodPower.selected_kingdom.data.get("suzerain", out bool suzerain1, false);
				kingdom.data.get("suzerain", out bool suzerain2, false);

				if (suzerain1)
				{
					NewFunction.LogNewMessage(suzerain2 ? kingdom : MoreGodPower.selected_kingdom, "国家", "是宗主国，无法被附庸");
					MoreGodPower.selected_kingdom = null;
					return false;
				}

				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "被附庸");
				Alliance alliance;
				Alliance alliance2;
				MoreGodPower.endWar(kingdom, MoreGodPower.selected_kingdom);
				kingdom.data.set("suzerain", true);
				MoreGodPower.selected_kingdom.data.set("Vassal", true);
				MoreGodPower.selected_kingdom.data.set("suzerainID", kingdom.id);
				// MoreGodPower.selected_kingdom.updateColor(kingdom.kingdomColor);
				MoreGodPower.selected_kingdom.data.set("oldColorID", MoreGodPower.selected_kingdom.data.colorID);
				ColorAsset originalColor = MoreGodPower.selected_kingdom.getColor();
				string oldColor = NewFunction.Serialize(originalColor);

				MoreGodPower.selected_kingdom.data.set("oldColor", oldColor);

				MoreGodPower.selected_kingdom.data.colorID = kingdom.data.colorID;
				ColorAsset kingdomcolor = kingdom.getColor();
				MoreGodPower.selected_kingdom.updateColor(kingdomcolor);
				World.world.zoneCalculator.setDrawnZonesDirty();
				World.world.zoneCalculator.clearCurrentDrawnZones(true);
				World.world.zoneCalculator.redrawZones();

				if (MoreGodPower.Vassals.ContainsKey(kingdom))
				{
					MoreGodPower.Vassals[kingdom].Add(MoreGodPower.selected_kingdom);
				}
				else { MoreGodPower.Vassals.Add(kingdom, new List<Kingdom> { MoreGodPower.selected_kingdom }); }
				if (MoreGodPower.selected_kingdom.getAlliance() != null && kingdom.getAlliance() == null)
				{
					alliance2 = MoreGodPower.selected_kingdom.getAlliance();
					alliance2.kingdoms_hashset.Remove(MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom.allianceLeave(MoreGodPower.selected_kingdom.getAlliance());
					alliance2.recalculate();

					MoreGodPower.selected_kingdom = null;
					return true;
				}
				else if (MoreGodPower.selected_kingdom.getAlliance() != null && kingdom.getAlliance() != MoreGodPower.selected_kingdom.getAlliance())
				{
					alliance2 = MoreGodPower.selected_kingdom.getAlliance();
					alliance2.kingdoms_hashset.Remove(MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom.allianceLeave(MoreGodPower.selected_kingdom.getAlliance());
					alliance = kingdom.getAlliance();
					alliance.kingdoms_hashset.Add(MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom.allianceJoin(alliance);
					// alliance.join(MoreGodPower.selected_kingdom, true);
					alliance.recalculate();
					alliance2.recalculate();
					alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();

					MoreGodPower.selected_kingdom = null;
					return true;
				}
				else if (MoreGodPower.selected_kingdom.getAlliance() == null && kingdom.getAlliance() != null)
				{
					alliance = kingdom.getAlliance();
					alliance.kingdoms_hashset.Add(MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom.allianceJoin(alliance);
					// alliance.join(MoreGodPower.selected_kingdom, true);
					alliance.recalculate();
					alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
					// MapBox.instance.alliances.newAlliance(kingdom, MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom = null;
					return true;
				}


				// NewFunction.AddNewText("1", Color.black, null);
				// MapBox.instance.alliances.newAlliance(kingdom, MoreGodPower.selected_kingdom);
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}

		public static bool tryToVassal_Kingdom2(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "取消附庸");
			}
			MoreGodPower.selected_kingdom.data.get("Vassal", out bool flag, false);
			if (flag)
			{
				MoreGodPower.selected_kingdom.data.get("suzerainID", out string str, "");
				Kingdom pKingdom2 = null;
				if (str != "")
				{ pKingdom2 = World.world.kingdoms.getKingdomByID(str); }
				if (pKingdom2 != null)
				{
					MoreGodPower.Vassals[pKingdom2].Remove(MoreGodPower.selected_kingdom);

				}
			}
			MoreGodPower.selected_kingdom.data.set("Vassal", false);
			MoreGodPower.selected_kingdom.data.set("suzerainID", "");
			NewFunction.UpdateColor(MoreGodPower.selected_kingdom);
			MoreGodPower.selected_kingdom = null;

			return true;
		}
		public static bool tryToAlliance_Kingdom(WorldTile pTile, string pPower)
		{
			if (pTile.zone.city == null)
			{
				return false;
			}
			var kingdom = Reflection.GetField(pTile.zone.city.GetType(), pTile.zone.city, "kingdom") as Kingdom;
			if (MoreGodPower.selected_kingdom == null)
			{
				MoreGodPower.selected_kingdom = kingdom;
				NewFunction.LogNewMessage(kingdom, "国家", "想要与其他国家结盟......");
			}
			else
			{
				if (kingdom == MoreGodPower.selected_kingdom)
				{
					return false;
				}
				if (kingdom.hasAlliance() && MoreGodPower.selected_kingdom.hasAlliance())
				{
					return false;
				}
				// MoreGodPower.selected_kingdom.data.get("Vassal", out bool flag1, false);
				// kingdom.data.get("Vassal", out bool flag2, false);

				// if (flag1 || flag2 || (flag1 && flag2))
				// {
				// 	NewFunction.LogNewMessage(flag2 ? kingdom : MoreGodPower.selected_kingdom, "国家", "无法结盟");
				// 	MoreGodPower.selected_kingdom = null;
				// 	return false;
				// }
				NewFunction.LogNewMessage(MoreGodPower.selected_kingdom, kingdom, "国家", "和国家", "成功结盟");
				Alliance alliance;
				MoreGodPower.endWar(kingdom, MoreGodPower.selected_kingdom);
				if (MoreGodPower.selected_kingdom.getAlliance() != null && kingdom.getAlliance() == null)
				{
					alliance = MoreGodPower.selected_kingdom.getAlliance();
					alliance.kingdoms_hashset.Add(kingdom);
					kingdom.allianceJoin(alliance);
					alliance.recalculate();
					alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
					MoreGodPower.selected_kingdom = null;
					return true;
				}
				if (MoreGodPower.selected_kingdom.getAlliance() == null && kingdom.getAlliance() != null)
				{
					alliance = kingdom.getAlliance();
					alliance.kingdoms_hashset.Add(MoreGodPower.selected_kingdom);
					MoreGodPower.selected_kingdom.allianceJoin(alliance);
					alliance.recalculate();
					alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
					MoreGodPower.selected_kingdom = null;
					return true;
				}


				// NewFunction.AddNewText("1", Color.black, null);
				MapBox.instance.alliances.newAlliance(kingdom, MoreGodPower.selected_kingdom);
				MoreGodPower.selected_kingdom = null;
			}
			return true;
		}




		public static void SetSCGL(int i)
		{
			if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
			{
				if (i > 0) { Main.numofyears *= 10; }
				else { Main.numofyears /= 10; }
			}
			else if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) { Main.numofyears += i * 10; }
			else { Main.numofyears += i; }
			if (Main.numofyears < 0) { Main.numofyears = 0; }
			// int numofyears = Main.numofyears;
			// Main.numofyears = numofyears;
			string text = Path.Combine(Application.streamingAssetsPath + "/mods/emtystarvast/Diplomacy_Army", $"numofyears.json");
			if (Application.platform == RuntimePlatform.WindowsPlayer) { text = text.Replace("\\", "/"); }
			DAStorage NewStorage = new()
			{
				Name = "numofyears",
				nom = Main.numofyears
			};
			File.WriteAllText(text, JsonConvert.SerializeObject(NewStorage, Formatting.Indented));
			translate.init();
			RSText.text = LocalizedTextManager.getText($"numofyears", null);
		}
	}
}
