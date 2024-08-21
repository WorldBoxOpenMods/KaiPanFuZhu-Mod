using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diplomacy_Army;

namespace Diplomacy_Diplomacy
{
    public class update
    {
        // 
        public static void updateVassal()
        {
            var vassalsToRemove = new HashSet<Kingdom>();

            foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
            {
                if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
                {
                    vassalsToRemove.Add(kingdom);
                    continue;
                }

                UpdateKingdomVassals(kingdom, ref vassalsToRemove);
            }

            foreach (var kingdomToRemove in vassalsToRemove)
            {
                RemoveVassals(kingdomToRemove);
            }
        }

        private static void UpdateKingdomVassals(Kingdom kingdom, ref HashSet<Kingdom> vassalsToRemove)
        {
            bool hasEnemies = kingdom.hasEnemies();
            var vassals = MoreGodPower.Vassals[kingdom].ToList();

            foreach (var vassal in vassals)
            {
                if (vassal == null || vassal.data == null)
                {
                    MoreGodPower.Vassals[kingdom].Remove(vassal);
                    continue;
                }

                UpdateVassalAlliance(vassal, kingdom, hasEnemies);
            }

            if (MoreGodPower.Vassals[kingdom].Count == 0)
            {
                vassalsToRemove.Add(kingdom);
            }
        }

        private static void UpdateVassalAlliance(Kingdom vassal, Kingdom kingdom, bool hasEnemies)
        {
            if (hasEnemies)
            {
                JoinVassalToWar(vassal, kingdom);
            }

            var vassalAlliance = vassal.getAlliance();
            var kingdomAlliance = kingdom.getAlliance();

            if (vassalAlliance != kingdomAlliance)
            {
                ChangeVassalAlliance(vassal, kingdom);
            }
        }

        private static void JoinVassalToWar(Kingdom vassal, Kingdom kingdom)
        {
            ListPool<War> wars = kingdom.getWars();
            foreach (War war in wars)
            {
                if (!war.hasKingdom(vassal))
                {
                    if (war.data.list_defenders.Contains(kingdom.id))
                    {
                        war.joinDefenders(vassal);
                    }
                    else if (war.data.list_attackers.Contains(kingdom.id))
                    {
                        war.joinAttackers(vassal);
                    }
                }
            }
        }

        private static void ChangeVassalAlliance(Kingdom vassal, Kingdom kingdom)
        {
            if (vassal.getAlliance() != null && kingdom.getAlliance() == null)
            {
                var alliance = vassal.getAlliance();
                alliance.kingdoms_hashset.Remove(vassal);
                vassal.allianceLeave(vassal.getAlliance());
                alliance.recalculate();
            }
            else if (vassal.getAlliance() != null && kingdom.getAlliance() != vassal.getAlliance())
            {
                var alliance2 = vassal.getAlliance();
                alliance2.kingdoms_hashset.Remove(vassal);
                vassal.allianceLeave(alliance2);
                var alliance = kingdom.getAlliance();
                alliance.kingdoms_hashset.Add(vassal);
                vassal.allianceJoin(alliance);
                alliance.recalculate();
                alliance2.recalculate();
                alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
            }
            else if (vassal.getAlliance() == null && kingdom.getAlliance() != null)
            {
                var alliance = kingdom.getAlliance();
                alliance.kingdoms_hashset.Add(vassal);
                vassal.allianceJoin(alliance);
                alliance.recalculate();
                alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
            }
        }

        private static void RemoveVassals(Kingdom kingdomToRemove)
        {
            var vassals = MoreGodPower.Vassals[kingdomToRemove].ToList();
            foreach (var vassal in vassals)
            {
                vassal.data.set("Vassal", false);
                vassal.data.set("suzerainID", "");
            }
            MoreGodPower.Vassals.Remove(kingdomToRemove);
        }
        // public static void updateVassal()
        // {
        //     var vassalsToRemove = new List<Kingdom>();

        //     foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
        //     {
        //         if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
        //         {
        //             vassalsToRemove.Add(kingdom);
        //             continue;
        //         }
        //         bool flag = kingdom.hasEnemies();
        //         var vassals = MoreGodPower.Vassals[kingdom].ToList();

        //         foreach (var vassal in vassals)
        //         {
        //             if (flag)
        //             {
        //                 ListPool<War> wars = kingdom.getWars();
        //                 foreach (War war in wars)
        //                 {
        //                     if (!war.hasKingdom(vassal))
        //                     {
        //                         if (war.data.list_defenders.Contains(kingdom.id))
        //                         {
        //                             war.joinDefenders(vassal);
        //                         }
        //                         else if (war.data.list_attackers.Contains(kingdom.id))
        //                         {
        //                             war.joinAttackers(vassal);
        //                         }
        //                     }
        //                 }
        //             }
        //             if (vassal == null || vassal.data == null)
        //             {
        //                 MoreGodPower.Vassals[kingdom].Remove(vassal);
        //                 vassal.data.set("Vassal", false);
        //                 vassal.data.set("suzerainID", "");
        //                 continue;
        //             }


        //         }

        //         if (MoreGodPower.Vassals[kingdom].Count <= 0)
        //         {
        //             vassalsToRemove.Add(kingdom);
        //         }
        //     }


        //     foreach (var kingdomToRemove in vassalsToRemove)
        //     {
        //         var vassals = MoreGodPower.Vassals[kingdomToRemove].ToList();
        //         foreach (var vassal in vassals)
        //         {
        //             vassal.data.set("Vassal", false);
        //             vassal.data.set("suzerainID", "");
        //         }
        //         MoreGodPower.Vassals.Remove(kingdomToRemove);
        //     }
        // }

        public static void updateTreaty()
        {
            NewFunction.updateTreaty(MoreGodPower.AllianceKingdoms, "互不侵犯", true);
            NewFunction.updateTreaty(MoreGodPower.DefenceKingdoms, "共同防御");
            NewFunction.updateTreaty(MoreGodPower.ArmyKingdoms, "军事通行");
        }
        public static void updateCities()
        {
            if (MoreGodPower.citiesCelebrate.Count > 0 && DateTime.Compare(Main.celebrationTime, DateTime.Now.ToLocalTime()) < 0)
            {
                Main.city = MoreGodPower.citiesCelebrate[0];
                if (Main.city.getTile() != null)
                {
                    EffectsLibrary.spawn("fx_fireworks", Main.city.getTile(), null, null, 0f, -1f, -1f);
                    Main.celebrationTime = DateTime.Now.ToLocalTime().AddSeconds(0.2);
                }
                MoreGodPower.citiesCelebrate.Remove(Main.city);
            }
        }
    }
}