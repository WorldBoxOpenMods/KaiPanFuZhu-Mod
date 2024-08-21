using System;
using NCMS;
using NCMS.Utils;
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

namespace Diplomacy_Army
{
	[ModEntry]
	public class NewFunction
	{
		private static string path = ".\\players.json";
		public static Color color1 = new(1f, 0.1f, 0.1f, 0.2f);

		private static int chance = 0;
		private static int _opinion_close_borders = 0;
		private static int _opinion_same_wars = 0;
		private static int _opinion_total = 0;
		private static int _opinion_total_2 = 0;

		private static DiplomacyRelation relation;
		private static KingdomOpinion kingdomOpinion;
		private static KingdomOpinion kingdomOpinion2;
		private static ActorData actordata;
		private static ActorData actordata2;

		public static Treaty newTreaty;
		public static string message;
		public static string message2;

		public static Dictionary<string, string> localizedText = new();

		public static void AddNewText(string message, Color color, Sprite icon = null)
		{
			GameObject gameObject = HistoryHud.instance.GetObject();
			gameObject.name = "HistoryItem " + (HistoryHud.historyItems.Count + 1);
			gameObject.SetActive(true);

			gameObject.transform.Find("CText").GetComponent<Text>();
			gameObject.transform.SetParent(HistoryHud.contentGroup);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			component.localPosition = Vector3.zero;
			component.SetLeft(0.0f);

			float top = (float)HistoryHud.instance.CallMethod("recalcPositions");

			component.SetTop(top);
			component.sizeDelta = new Vector2(component.sizeDelta.x, 15f);
			gameObject.GetComponent<HistoryHudItem>().targetBottom = top;

			gameObject.GetComponent<HistoryHudItem>().textField.color = color;
			gameObject.GetComponent<HistoryHudItem>().textField.text = message;
			HistoryHud.historyItems.Add(gameObject.GetComponent<HistoryHudItem>());
			HistoryHud.instance.recalc = true;

			if (icon != null)
			{
				gameObject.transform.Find("Icon").GetComponent<Image>().sprite = icon;
			}

			gameObject.SetActive(true);
		}

		public static void LogNewMessage(Kingdom pKingdom, string pMessage = "")
		{
			if (pKingdom == null)
			{
				return;
			}
			pMessage = ">>:" + pMessage;
			if (Main.KingdomMessages.ContainsKey(pKingdom))
			{
				if (!Main.KingdomMessages[pKingdom].ContainsKey(pMessage))
				{
					Main.KingdomMessages[pKingdom].Add(pMessage, DateTime.Now.ToLocalTime().AddSeconds(3.0f));
				}
				else
				{
					Main.KingdomMessages[pKingdom][pMessage] = DateTime.Now.ToLocalTime().AddSeconds(3.0f);
				}
			}
			else
			{
				Dictionary<string, DateTime> notice = new() { { pMessage, DateTime.Now.ToLocalTime().AddSeconds(3.0f) } };
				Main.KingdomMessages.Add(pKingdom, notice);
			}
		}

		public static GameObject CreateNewButton(int index, Transform pParent, string pSprite, GodPower pID, string pDescription, UnityAction pCall = null, PowerButtonType type = PowerButtonType.Active)
		{
			((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name, pID.name);
			((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name + " Description", pDescription);
			float x = 108f + 36 * (index / 2);
			float y = 18f - 36 * (index % 2);
			Sprite sprite = Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + pSprite + ".jpg");
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("SettingsButton"), pParent);
			gameObject2.GetComponent<PowerButton>().type = type;
			Reflection.SetField<GodPower>(gameObject2.GetComponent<PowerButton>(), "godPower", pID);
			gameObject2.transform.name = pID.id;
			gameObject2.transform.localScale = new Vector2(1f, 1f);
			gameObject2.transform.localPosition = new Vector2(x, y);
			gameObject2.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
			gameObject2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
			if (pCall != null)
			{
				gameObject2.GetComponent<Button>().onClick.AddListener(pCall);
			}
			gameObject2.SetActive(true);
			localizedText.Add(pID.name, pID.name);
			localizedText.Add(pID.name + " Description", pDescription);
			return gameObject2;
		}

		public static GameObject CreateNewButtonOnWindow(Vector2 pLocalPosition, Transform pParent, string pSprite, GodPower pID, string pDescription, UnityAction pCall = null, PowerButtonType type = PowerButtonType.Active)
		{

			((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name, pID.name);
			((Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText")).Add(pID.name + " Description", pDescription);

			Sprite sprite = Sprites.LoadSprite(".\\Mods\\KaiPanFuZhu Mod\\Sprites\\" + pSprite + ".jpg");
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(GameObject.Find("SettingsButton"), pParent);
			gameObject2.GetComponent<PowerButton>().type = type;
			Reflection.SetField<GodPower>(gameObject2.GetComponent<PowerButton>(), "godPower", pID);
			gameObject2.transform.name = pID.id;
			gameObject2.transform.localScale = new Vector2(1f, 1f);
			gameObject2.transform.localPosition = pLocalPosition;
			gameObject2.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
			gameObject2.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
			if (pCall != null)
			{
				gameObject2.GetComponent<Button>().onClick.AddListener(pCall);
			}
			gameObject2.SetActive(true);
			localizedText.Add(pID.name, pID.name);
			localizedText.Add(pID.name + " Description", pDescription);
			return gameObject2;
		}

		public static Vector2 getPositionByIndex(int index)
		{

			// Starting position by x
			float startX = 50;

			// Starting position by y
			float startY = -20;

			// Buttons size + gap between
			float sizeWithGap = 40;

			// Buttons per row
			int buttonsPerRow = 5;

			// Calculating points
			float positionX = startX + (index * sizeWithGap) - (Mathf.Floor(index / buttonsPerRow) * sizeWithGap * buttonsPerRow);
			float positionY = startY - (Mathf.Floor(index / buttonsPerRow) * sizeWithGap);

			return new Vector2(positionX, positionY);
		}

		public static JObject ReadHtml(string html)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(html);
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "application/json";
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			StreamReader streamReader = new(httpWebResponse.GetResponseStream(), Encoding.UTF8);
			JObject jObject = (JObject)JToken.ReadFrom(new JsonTextReader(streamReader));
			httpWebResponse.Close();
			return jObject;
		}

		public static JObject ReadJson(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}
			StreamReader streamReader = new(path);
			JObject result = (JObject)JToken.ReadFrom(new JsonTextReader(streamReader));
			streamReader.Close();
			return result;
		}

		public static void Director(string dir, List<JObject> jObjects)
		{
			DirectoryInfo d = new(dir);
			FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
			JObject jObject;
			foreach (FileSystemInfo fsinfo in fsinfos)
			{
				if (fsinfo.Name.Contains("names"))     //判断是否是配置文件
				{
					jObject = ReadJson(dir + "\\" + fsinfo.Name);
					if (jObject != null)
					{
						jObjects.Add(jObject);
					}
				}
			}

		}

		public static void WriteToJson(string path, string name)
		{
			if (!File.Exists(path))
			{
				return;
			}
			JObject jobject = JObject.Parse(File.ReadAllText(path, Encoding.UTF8));
			JObject jobject2 = new(new JProperty("name", name));
			jobject["gameCmdDataList"].Last.AddAfterSelf(jobject2);
			string contents = Convert.ToString(jobject);
			File.WriteAllText(path, contents);
		}

		public static void LogNewMessage(Kingdom pKingdom, string pMessage = "", string pMessage2 = "")
		{
			var kingdomColor = ((ColorAsset)Reflection.GetField(pKingdom.GetType(), pKingdom, "kingdomColor")).getColorText();
			var Message = string.Concat(new string[]
			{
				pMessage,
				"  <color=",
				Toolbox.colorToHex(kingdomColor, true),
				">",
				pKingdom.name,
				"</color>  ",
				pMessage2
			});
			NewFunction.AddNewText(Message, Toolbox.color_log_neutral);
		}

		public static void LogNewMessage(Kingdom pKingdom, Kingdom pKingdom2, string pMessage = "", string pMessage2 = "", string pMessage3 = "")
		{
			var kingdomColor = ((ColorAsset)Reflection.GetField(pKingdom.GetType(), pKingdom, "kingdomColor")).getColorText();
			var kingdomColor2 = ((ColorAsset)Reflection.GetField(pKingdom2.GetType(), pKingdom2, "kingdomColor")).getColorText();
			var Message = string.Concat(new string[]
			{
				pMessage,
				"  <color=",
				Toolbox.colorToHex(kingdomColor, true),
				">",
				pKingdom.name,
				"</color>  ",
				pMessage2,
				"  <color=",
				Toolbox.colorToHex(kingdomColor2, true),
				">",
				pKingdom2.name,
				"</color>  ",
				pMessage3
			});
			NewFunction.AddNewText(Message, Toolbox.color_log_neutral);
		}

		public static bool ExistKingdom(string Name)
		{
			foreach (Kingdom kingdom in MapBox.instance.kingdoms.list_civs)
			{
				if (kingdom.name.Equals(Name))
				{
					return true;
				}
			}
			return false;
		}

		public static City getCityByName(string Name)
		{
			if (MapBox.instance.cities.list.Count == 0)
			{
				return null;
			}
			foreach (City city in MapBox.instance.cities.list)
			{
				if (city.data.name.Equals(Name))
				{
					return city;
				}
			}
			return null;
		}

		public static Kingdom GetKingdomByID(string ID)
		{
			foreach (Kingdom kingdom in MapBox.instance.kingdoms.list_civs)
			{
				if (kingdom.id.Equals(ID))
				{
					return kingdom;
				}
			}
			return null;
		}

		public static int getLengthOfString(string ID)
		{
			int length = 0;
			foreach (char c in ID)
			{
				length += 4;
				if (c <= 'Z' && c >= 'A')
				{
					length--;
					if (c == 'I')
					{
						length--;
					}
				}
				if ((c <= 'z' && c >= 'a') || (c <= '9' && c >= '0'))
				{
					length -= 2;
					if (c == 'i' || c == 'l' || c == 't' || c == 'f')
					{
						length--;
					}
				}
				if (c == ' ')
				{
					length -= 3;
				}
			}
			return length;
		}

		public static void MoveArmy(UnitGroup pArmy, WorldTile pTile, bool isSettle = false)
		{
			if (pTile == null || pArmy == null)
			{
				return;
			}
			Reflection.SetField<TileZone>(pArmy.city, "target_attack_zone", pTile.zone);
			var units = Reflection.GetField(pArmy.GetType(), pArmy, "units") as ActorContainer;
			if (units.Count < pArmy.city.getArmy() * 2 / 3 && !isSettle)
			{
				foreach (Actor actor in pArmy.city.professionsDict[UnitProfession.Warrior])
				{
					if (actor == null) continue;
					if (!units.Contains(actor))
					{
						pArmy.addUnit(actor);
					}
				}
			}
			if (units.Count > 0)
			{
				foreach (Actor actor in units.getSimpleList())
				{
					if (actor == null) { pArmy.city.units.Remove(actor); continue; }
					actor.cancelAllBeh(null);
					((AiSystemActor)Reflection.GetField(actor.GetType(), actor, "ai")).setJob("attacker");
					Reflection.SetField<WorldTile>(actor, "beh_tile_target", pTile);
					actor.goTo(pTile);
					if (isSettle && pTile.zone.city != null)
					{
						actor.city = pTile.zone.city;
					}
				}
			}
		}

		public static void tryNewTreaty(Kingdom pKingdom)
		{
			//tryNewAllianceTreaty(pKingdom);
			//trySignAllianceTreaty(pKingdom);
			tryNewArmyTreaty(pKingdom);
			//tryNewDefenceTreaty(pKingdom);
		}

		public static void tryNewAllianceTreaty(Kingdom pKingdom)
		{
			if (pKingdom.king == null)
			{
				return;
			}
			if (!pKingdom.hasEnemies())
			{
				return;
			}
			actordata = Reflection.GetField(pKingdom.king.GetType(), pKingdom.king, "data") as ActorData;
			if (actordata.diplomacy < 10)
			{
				return;
			}
			int power = NewFunction.getPowerOfKingdom(pKingdom);
			int power_allEnemies = 0;
			foreach (Kingdom item in pKingdom.getEnemiesKingdoms())
			{
				power_allEnemies += NewFunction.getPowerOfKingdom(item);
			}
			kingdomsClosed.Clear();
			NewFunction.getClosedKingdoms(pKingdom);
			if (kingdomsClosed.Count == 0)
			{
				return;
			}
			int power_another;
			foreach (Kingdom kingdom in kingdomsClosed)
			{
				if (!pKingdom.isEnemy(kingdom))
				{
					power_another = NewFunction.getPowerOfKingdom(kingdom);
					if (power_allEnemies + power_another > power * 1.5 && power_another > power * 0.5 && power_another < power * 2
						&& (!Treaty.kingdomsTryNewTreaty.ContainsKey(kingdom) || !Treaty.kingdomsTryNewTreaty[kingdom].ContainsKey(pKingdom)))
					{
						if (!Treaty.kingdomsTryNewTreaty.ContainsKey(kingdom))
						{
							Dictionary<Kingdom, TreatyType> keyValues = new();
							Treaty.kingdomsTryNewTreaty.Add(kingdom, keyValues);
						}
						Treaty.kingdomsTryNewTreaty[kingdom].Add(pKingdom, TreatyType.Alliance);
						NewFunction.LogNewMessage(pKingdom, kingdom, "", "准备与", "签订互不侵犯条约");
						return;
					}
				}
			}
			kingdomsClosed.Clear();
		}

		public static void trySignAllianceTreaty(Kingdom pKingdom)
		{
			if (pKingdom.king == null)
			{
				return;
			}
			if (!Treaty.kingdomsTryNewTreaty.ContainsKey(pKingdom) || Treaty.kingdomsTryNewTreaty[pKingdom].Count == 0)
			{
				return;
			}
			actordata = Reflection.GetField(pKingdom.king.GetType(), pKingdom.king, "data") as ActorData;
			if (actordata.diplomacy < 10)
			{
				return;
			}
			int power = NewFunction.getPowerOfKingdom(pKingdom);
			int power_allEnemies = 0;
			foreach (Kingdom item in pKingdom.getEnemiesKingdoms())
			{
				power_allEnemies += NewFunction.getPowerOfKingdom(item);
			}
			int power_another;
			foreach (var item in Treaty.kingdomsTryNewTreaty[pKingdom])
			{
				power_another = NewFunction.getPowerOfKingdom(item.Key);
				if (power_allEnemies > 0)
				{
					if (power_allEnemies + power_another > power * 1.5)
					{
						signNewTreaty(item.Key, pKingdom, "互不侵犯", 30, TreatyType.Alliance, true);
						NewFunction.LogNewMessage(item.Key, pKingdom, "", "与", "签订互不侵犯条约");
						Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
						return;
					}
					else if (item.Key.king != null)
					{
						actordata2 = Reflection.GetField(item.Key.king.GetType(), item.Key.king, "data") as ActorData;
						chance = 100 * actordata2.diplomacy / (actordata.diplomacy + actordata2.diplomacy);
						if (Toolbox.randomInt(1, 100) < chance)
						{
							signNewTreaty(item.Key, pKingdom, "互不侵犯", 30, TreatyType.Alliance, true);
							NewFunction.LogNewMessage(item.Key, pKingdom, "", "与", "签订互不侵犯条约");
							Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
							return;
						}
						else
						{
							NewFunction.LogNewMessage(pKingdom, item.Key, "", "拒绝与", "签订互不侵犯条约");
							Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
							return;
						}
					}
				}
				else
				{
					if (power_another > power * 1.5)
					{
						signNewTreaty(item.Key, pKingdom, "互不侵犯", 30, TreatyType.Alliance, true);
						return;
					}
					else if (power_another >= power * 0.5 && power_another <= power * 1.5 && item.Key.king != null)
					{
						actordata2 = Reflection.GetField(item.Key.king.GetType(), item.Key.king, "data") as ActorData;
						chance = 100 * actordata2.diplomacy / (actordata.diplomacy + actordata2.diplomacy);
						if (Toolbox.randomInt(1, 100) < chance)
						{
							signNewTreaty(item.Key, pKingdom, "互不侵犯", 30, TreatyType.Alliance, true);
							NewFunction.LogNewMessage(item.Key, pKingdom, "", "与", "签订互不侵犯条约");
							Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
							return;
						}
						else if (actordata.diplomacy > 20)
						{
							MapBox.instance.diplomacy.CallMethod("startWar", pKingdom, item.Key, false);
							NewFunction.LogNewMessage(pKingdom, item.Key, "", "拒绝与", "签订互不侵犯条约");
							Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
							return;
						}
					}
					else
					{
						NewFunction.LogNewMessage(pKingdom, item.Key, "", "拒绝与", "签订互不侵犯条约");
						Treaty.kingdomsTryNewTreaty[pKingdom].Remove(item.Key);
						return;
					}
				}

			}
		}

		public static void tryNewArmyTreaty(Kingdom pKingdom)
		{
			if (pKingdom.getEnemiesKingdoms().Count > 0 && pKingdom.king != null)
			{
				if (!MoreGodPower.KingdomsTryArmy.ContainsKey(pKingdom) || MoreGodPower.KingdomsTryArmy[pKingdom].Count == 0)
				{
					return;
				}
				if (MoreGodPower.AllianceKingdoms.ContainsKey(pKingdom) && MoreGodPower.AllianceKingdoms[pKingdom].Count > (actordata.diplomacy + 9) / 10)
				{
					MoreGodPower.KingdomsTryArmy[pKingdom].Clear();
					return;
				}
				foreach (Kingdom kingdom in MoreGodPower.KingdomsTryArmy[pKingdom])
				{
					if (pKingdom.isEnemy(kingdom))
					{
						MoreGodPower.KingdomsTryArmy[pKingdom].Remove(kingdom);
						break;
					}
					if (MoreGodPower.AllianceKingdoms.ContainsKey(kingdom) && MoreGodPower.AllianceKingdoms[kingdom].Count > (actordata2.diplomacy + 9) / 10
						&& !MoreGodPower.AllianceKingdoms[kingdom].ContainsKey(pKingdom))
					{
						MoreGodPower.KingdomsTryArmy[pKingdom].Remove(kingdom);
						break;
					}
					if (kingdom.king != null && !MoreGodPower.KingdomsOwnedByPlayer.Contains(kingdom))
					{
						actordata = Reflection.GetField(pKingdom.king.GetType(), pKingdom.king, "data") as ActorData;
						actordata2 = Reflection.GetField(kingdom.king.GetType(), kingdom.king, "data") as ActorData;
						chance = actordata.diplomacy * 30 / (actordata.diplomacy + actordata2.diplomacy);

						relation = MapBox.instance.diplomacy.getRelation(pKingdom, kingdom);
						relation.CallMethod("recalculate", pKingdom, kingdom, relation);
						kingdomOpinion2 = MapBox.instance.diplomacy.getOpinion(kingdom, pKingdom);
						_opinion_total_2 = (int)Reflection.GetField(kingdomOpinion2.GetType(), kingdomOpinion2, "_opinion_total");

						if (_opinion_total_2 < 0)
						{
							chance /= 2;
						}
						if (Toolbox.randomInt(1, 100) < chance)
						{
							signNewTreaty(pKingdom, kingdom, "军事通行", 30, TreatyType.Army);
							signNewTreaty(pKingdom, kingdom, "互不侵犯", 30, TreatyType.Alliance, true, false);
							MoreGodPower.KingdomsTryArmy[pKingdom].Clear();
							return;
						}
						MoreGodPower.KingdomsTryArmy[pKingdom].Remove(kingdom);
						break;
					}
				}
			}
		}

		public static void tryNewDefenceTreaty(Kingdom pKingdom)
		{
			if (pKingdom.getEnemiesKingdoms().Count > 0 && pKingdom.king != null)
			{
				actordata = Reflection.GetField(pKingdom.king.GetType(), pKingdom.king, "data") as ActorData;
				if (MoreGodPower.AllianceKingdoms.ContainsKey(pKingdom) && MoreGodPower.AllianceKingdoms[pKingdom].Count > (actordata.diplomacy + 9) / 10)
				{
					return;
				}
				foreach (Kingdom kingdom in pKingdom.getEnemiesKingdoms())
				{
					if (kingdom.king != null && !MoreGodPower.KingdomsOwnedByPlayer.Contains(kingdom))
					{
						actordata2 = Reflection.GetField(kingdom.king.GetType(), kingdom.king, "data") as ActorData;
						if (!MoreGodPower.DefenceKingdoms.ContainsKey(kingdom) || MoreGodPower.DefenceKingdoms[kingdom].Count < (actordata2.diplomacy + 19) / 20)
						{
							relation = MapBox.instance.diplomacy.getRelation(pKingdom, kingdom);
							relation.CallMethod("recalculate", pKingdom, kingdom, relation);

							kingdomOpinion = MapBox.instance.diplomacy.getOpinion(pKingdom, kingdom);
							_opinion_close_borders = (int)Reflection.GetField(kingdomOpinion.GetType(), kingdomOpinion, "_opinion_close_borders");
							_opinion_total = (int)Reflection.GetField(kingdomOpinion.GetType(), kingdomOpinion, "_opinion_power");

							kingdomOpinion2 = MapBox.instance.diplomacy.getOpinion(kingdom, pKingdom);
							_opinion_total_2 = (int)Reflection.GetField(kingdomOpinion2.GetType(), kingdomOpinion2, "_opinion_power");
							if (_opinion_close_borders < 0 && _opinion_total < 50 && _opinion_total_2 < 50)
							{
								chance = actordata.diplomacy * 10 / (actordata.diplomacy + actordata2.diplomacy);
								_opinion_total_2 = (int)Reflection.GetField(kingdomOpinion2.GetType(), kingdomOpinion2, "_opinion_total");
								if (_opinion_total_2 < 0)
								{
									chance /= 2;
								}
								if (Toolbox.randomInt(1, 100) < chance)
								{
									signNewTreaty(pKingdom, kingdom, "共同防御", 30, TreatyType.Defence);
									signNewTreaty(pKingdom, kingdom, "军事通行", 30, TreatyType.Army);
									signNewTreaty(pKingdom, kingdom, "互不侵犯", 30, TreatyType.Alliance, true, false);
									return;
								}
							}
						}
					}
				}
			}
		}

		public static void signNewTreaty(Kingdom pKingdom, Kingdom tTarget, string text, int time, TreatyType type, bool clearDiplomacy = false, bool showText = true, bool isAuto = true)
		{
			Dictionary<Kingdom, Dictionary<Kingdom, int>> treaty = new();
			if (text == "互不侵犯")
			{
				treaty = MoreGodPower.AllianceKingdoms;
			}
			else if (text == "军事通行")
			{
				treaty = MoreGodPower.ArmyKingdoms;
			}
			else if (text == "共同防御")
			{
				treaty = MoreGodPower.DefenceKingdoms;
			}
			if (isAuto)
			{
				time = Main.numofyears;
			}
			if (!treaty.ContainsKey(pKingdom))
			{
				treaty.Add(pKingdom, new Dictionary<Kingdom, int>());
			}
			if (!treaty.ContainsKey(tTarget))
			{
				treaty.Add(tTarget, new Dictionary<Kingdom, int>());
			}
			if (treaty[pKingdom].ContainsKey(tTarget))
			{
				treaty[pKingdom].Remove(tTarget);

			}
			if (treaty[tTarget].ContainsKey(pKingdom))
			{
				treaty[tTarget].Remove(pKingdom);

			}

			treaty[pKingdom].Add(tTarget, MapBox.instance.mapStats.year + time);
			treaty[tTarget].Add(pKingdom, MapBox.instance.mapStats.year + time);
			if (pKingdom.capital != null)
			{
				MoreGodPower.citiesCelebrate.Add(pKingdom.capital);
			}
			if (tTarget.capital != null)
			{
				MoreGodPower.citiesCelebrate.Add(tTarget.capital);
			}
			//if (clearDiplomacy)
			//{
			//pKingdom.civs_allies.Remove(tTarget);
			//tTarget.civs_allies.Remove(pKingdom);
			//}
			if (showText)
			{
				LogNewMessage(pKingdom, "与" + tTarget.name + "签订了" + text + "，期限" + time.ToString() + "年");
				LogNewMessage(tTarget, "与" + pKingdom.name + "签订了" + text + "，期限" + time.ToString() + "年");
			}
			message = pKingdom.id + "+" + tTarget.id + "+" + text;
			message2 = pKingdom.id + "+" + tTarget.id + "+" + text;
			newTreaty = new Treaty(pKingdom, tTarget, time, type, text);
			if (Main.treaties.ContainsKey(message) || Main.treaties.ContainsKey(message2))
			{
				if (Main.treaties.ContainsKey(message))
				{
					newTreaty = Main.treaties[message];
					Main.treaties.Remove(message);
				}
				if (Main.treaties.ContainsKey(message2))
				{
					newTreaty = Main.treaties[message2];
					Main.treaties.Remove(message2);
				}
			}
			else
			{
				newTreaty = new Treaty(pKingdom, tTarget, time, type, text);
			}
			Main.treaties.Add(message, newTreaty);
		}

		public static void updateTreaty(Dictionary<Kingdom, Dictionary<Kingdom, int>> treaty, string text, bool resetDiplomacy = false)
		{
			if (treaty.Count > 0)
			{
				foreach (var item in treaty)
				{
					if (item.Key != null && item.Key.data != null && !item.Key.data.alive)
					{
						if (item.Value.Count > 0)
						{
							foreach (var item2 in item.Value)
							{
								//清楚对方国家与己方的条约关系
								treaty[item2.Key].Remove(item.Key);
							}
							item.Value.Clear();
						}
						treaty.Remove(item.Key);
						return;
					}
					if (item.Value.Count > 0)
					{
						foreach (var item2 in item.Value)
						{
							if (item2.Value < MapBox.instance.mapStats.year)
							{
								designTreaty(item.Key, item2.Key, text, resetDiplomacy);
								return;
							}
							//if (resetDiplomacy)
							//{
							//	item.Key.civs_allies.Remove(item2.Key);
							//	item2.Key.civs_allies.Remove(item.Key);
							//}
						}
					}
				}
			}

		}

		public static void designTreaty(Kingdom kingdom1, Kingdom kingdom2, string text, bool resetDiplomacy = false)
		{
			if (kingdom1 == null || kingdom2 == null) { return; }
			Dictionary<Kingdom, Dictionary<Kingdom, int>> treaty = new();
			if (text == "互不侵犯")
			{
				treaty = MoreGodPower.AllianceKingdoms;
			}
			else if (text == "军事通行")
			{
				treaty = MoreGodPower.ArmyKingdoms;
			}
			else if (text == "共同防御")
			{
				treaty = MoreGodPower.DefenceKingdoms;
			}

			if (!(treaty.ContainsKey(kingdom1) && treaty[kingdom1].ContainsKey(kingdom2)))
			{
				return;
			}
			//if (resetDiplomacy)
			//{
			//	kingdom1.civs_allies[kingdom2] = true;
			//	kingdom2.civs_allies[kingdom1] = true;
			//}

			NewFunction.LogNewMessage(kingdom1, kingdom2, "", "与", "的" + text + "条约已过期");
			NewFunction.LogNewMessage(kingdom1, "与" + kingdom2.name + "的" + text + "条约已过期");
			NewFunction.LogNewMessage(kingdom2, "与" + kingdom1.name + "的" + text + "条约已过期");
			treaty[kingdom1].Remove(kingdom2);
			treaty[kingdom2].Remove(kingdom1);
			Main.treaties.Remove(kingdom1.id + "+" + kingdom2.id + "+" + text);
			Main.treaties.Remove(kingdom2.id + "+" + kingdom1.id + "+" + text);
		}

		public static bool isBorder(City city, Kingdom kingdom, Kingdom tTarget = null)
		{
			ArmyPowerWindow.tileZones = (List<TileZone>)Reflection.GetField(city.GetType(), city, "zones");
			if (ArmyPowerWindow.tileZones.Count > 0)
			{
				Kingdom kingdom2;
				foreach (TileZone tileZone in ArmyPowerWindow.tileZones)
				{
					foreach (TileZone tileZone2 in tileZone.neighboursAll)
					{
						if (tileZone2.city != null)
						{
							kingdom2 = Reflection.GetField(tileZone2.city.GetType(), tileZone2.city, "kingdom") as Kingdom;
							//|| (kingdom2 == kingdom && tileZone2.city.isCapitalCity())
							if ((kingdom2 != kingdom && tTarget == null) || (tTarget != null && kingdom2 == tTarget))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private static List<Kingdom> kingdomsClosed = new();
		private static List<TileZone> cityZones = new();
		public static void getClosedKingdoms(Kingdom pKingdom)
		{
			if (pKingdom.cities.Count == 0)
			{
				return;
			}
			Kingdom kingdom2;
			foreach (City city in pKingdom.cities)
			{
				cityZones = (List<TileZone>)Reflection.GetField(city.GetType(), city, "zones");
				if (cityZones.Count > 0)
				{
					foreach (TileZone tileZone in cityZones)
					{
						foreach (TileZone tileZone2 in tileZone.neighboursAll)
						{
							if (tileZone2.city != null)
							{
								kingdom2 = Reflection.GetField(tileZone2.city.GetType(), tileZone2.city, "kingdom") as Kingdom;
								if (kingdom2 != pKingdom && !kingdomsClosed.Contains(kingdom2))
								{
									kingdomsClosed.Add(kingdom2);
								}
							}
						}
					}

				}
			}
		}

		public static int getPowerOfKingdom(Kingdom pKingdom)
		{
			if (pKingdom == null)
			{
				return 0;
			}
			return pKingdom.getPopulationTotal() * 3 - pKingdom.getArmy();
		}
		public static void UpdateColor(Kingdom kingdom)
		{
			// 获取并检查原始颜色
			kingdom.data.get("oldColor", out string oldColor);
			if (!string.IsNullOrEmpty(oldColor))
			{
				ColorAsset oldColorA = Deserialize(oldColor);
				kingdom.updateColor(oldColorA);
				World.world.zoneCalculator.setDrawnZonesDirty();
				World.world.zoneCalculator.clearCurrentDrawnZones(true);
				World.world.zoneCalculator.redrawZones();
			}

            // 获取并检查原始颜色ID
            kingdom.data.get("oldColorID", out int oldColorID);
            if (oldColorID != -1)
			{
				kingdom.data.colorID = oldColorID;
			}
		}
		public static ColorAsset Deserialize(string s)
		{
			var parts = s.Split(',');
			if (parts.Length != 4)
			{
				throw new ArgumentException("Invalid input string");
			}
			var pColorMain = parts[0];
			var pColorMain2 = parts[1];
			var pColorBanner = parts[2];
			var indexId = int.Parse(parts[3]);

			var result = new ColorAsset(pColorMain, pColorMain2, pColorBanner)
			{
				index_id = indexId
			};

			result.initColor(); // 重新计算颜色值
			return result;
		}
		public static string Serialize(ColorAsset colorAsset)
        {
            return $"{colorAsset.color_main},{colorAsset.color_main_2},{colorAsset.color_banner},{colorAsset.index_id}";
        }

	}
}