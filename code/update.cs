using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Diplomacy_Army;
using NCMS.Utils;

namespace Diplomacy_Army
{
    public static class Update
    {
        public static void UpdateVassals()
        {
            var vassalsToRemove = new HashSet<Kingdom>();

            foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
            {
                if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
                {
                    vassalsToRemove.Add(kingdom);
                    continue;
                }

                UpdateVassalColor(kingdom);
                UpdateKingdomVassals(kingdom, vassalsToRemove);
            }

            foreach (var kingdom in vassalsToRemove)
            {
                RemoveVassals(kingdom);
            }
        }

        private static void UpdateKingdomVassals(Kingdom kingdom, HashSet<Kingdom> vassalsToRemove)
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

                    UpdateVassalAlliance(vassal, kingdom);
                }

                if (vassals.Count == 0)
                {
                    vassalsToRemove.Add(kingdom);
                }
            }
        }

        private static void UpdateVassalAlliance(Kingdom vassal, Kingdom kingdom)
        {
            var hasEnemies = kingdom.hasEnemies();

            if (hasEnemies)
            {
                JoinVassalToWar(vassal, kingdom);
            }

            var currentAlliance = vassal.getAlliance();
            var kingdomAlliance = kingdom.getAlliance();

            if (currentAlliance != kingdomAlliance)
            {
                ChangeVassalAlliance(vassal, kingdom);
            }
        }

        private static void UpdateVassalColor(Kingdom kingdom)
        {
            var vassals = MoreGodPower.Vassals[kingdom].ToList();

            foreach (var vassal in vassals)
            {
                if (PowerButtons.GetToggleValue("DA_关闭显示附庸颜色"))
                {
                    NewFunction.UpdateColor(vassal);
                }
                else
                {
                    UpdateVassalToKingdomColor(vassal, kingdom);
                }
            }
        }

        private static void UpdateVassalToKingdomColor(Kingdom vassal, Kingdom kingdom)
        {
            if (vassal.data.colorID != kingdom.data.colorID)
            {
                vassal.data.set("oldColorID", vassal.data.colorID);
                vassal.data.set("oldColor", NewFunction.Serialize(vassal.getColor()));

                vassal.data.colorID = kingdom.data.colorID;
                vassal.updateColor(kingdom.getColor());
                World.world.zoneCalculator.setDrawnZonesDirty();
                World.world.zoneCalculator.redrawZones();
            }
        }

        private static void JoinVassalToWar(Kingdom vassal, Kingdom kingdom)
        {
            var wars = kingdom.getWars();
            foreach (var war in wars)
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
            var currentAlliance = vassal.getAlliance();
            var kingdomAlliance = kingdom.getAlliance();

            if (currentAlliance != null && kingdomAlliance != currentAlliance)
            {
                currentAlliance.kingdoms_hashset.Remove(vassal);
                vassal.allianceLeave(currentAlliance);
                currentAlliance.recalculate();
            }

            if (kingdomAlliance != null)
            {
                kingdomAlliance.kingdoms_hashset.Add(vassal);
                vassal.allianceJoin(kingdomAlliance);
                kingdomAlliance.recalculate();
                kingdomAlliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
            }
        }

        private static void RemoveVassals(Kingdom kingdom)
        {
            foreach (var vassal in MoreGodPower.Vassals[kingdom])
            {
                vassal.data.set("Vassal", false);
                vassal.data.set("suzerainID", "");
            }
            MoreGodPower.Vassals.Remove(kingdom);
        }


        // 
        // public static void updateVassal()
        // {
        //     var vassalsToRemove = new HashSet<Kingdom>();

        //     foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
        //     {
        //         if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
        //         {
        //             vassalsToRemove.Add(kingdom);
        //             UpdateVassalColor(kingdom, ref vassalsToRemove);
        //             continue;
        //         }
        //         UpdateVassalColor(kingdom, ref vassalsToRemove);
        //         UpdateKingdomVassals(kingdom, ref vassalsToRemove);
        //     }

        //     foreach (var kingdomToRemove in vassalsToRemove)
        //     {
        //         RemoveVassals(kingdomToRemove);
        //     }
        // }

        // private static void UpdateKingdomVassals(Kingdom kingdom, ref HashSet<Kingdom> vassalsToRemove)
        // {
        //     bool hasEnemies = kingdom.hasEnemies();
        //     var vassals = MoreGodPower.Vassals[kingdom].ToList();

        //     foreach (var vassal in vassals)
        //     {
        //         if (vassal == null || vassal.data == null)
        //         {
        //             MoreGodPower.Vassals[kingdom].Remove(vassal);
        //             continue;
        //         }


        //         UpdateVassalAlliance(vassal, kingdom, hasEnemies);
        //     }

        //     if (MoreGodPower.Vassals[kingdom].Count == 0)
        //     {
        //         vassalsToRemove.Add(kingdom);
        //     }
        // }

        // private static void UpdateVassalAlliance(Kingdom vassal, Kingdom kingdom, bool hasEnemies)
        // {
        //     if (hasEnemies)
        //     {
        //         JoinVassalToWar(vassal, kingdom);
        //     }

        //     var vassalAlliance = vassal.getAlliance();
        //     var kingdomAlliance = kingdom.getAlliance();

        //     if (vassalAlliance != kingdomAlliance)
        //     {
        //         ChangeVassalAlliance(vassal, kingdom);
        //     }
        // }
        // private static void UpdateVassalColor(Kingdom kingdom, ref HashSet<Kingdom> vassalsToRemove)
        // {

        //     var vassals = MoreGodPower.Vassals[kingdom].ToList();

        //     foreach (var vassal in vassals)
        //     {
        //         if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
        //         {
        //             NewFunction.UpdateColor(vassal);
        //             continue;
        //         }
        //         if (vassal == null || vassal.data == null)
        //         {
        //             MoreGodPower.Vassals[kingdom].Remove(vassal);
        //             continue;
        //         }
        //         vassal.data.get("oldColorID", out int num);
        //         if (PowerButtons.GetToggleValue("DA_关闭显示附庸颜色"))
        //         {
        //             NewFunction.UpdateColor(vassal);
        //         }
        //         else if(num==vassal.data.colorID)
        //         {
        //             vassal.data.set("oldColorID", vassal.data.colorID);
        //             ColorAsset originalColor = vassal.getColor();
        //             string oldColor = NewFunction.Serialize(originalColor);

        //             vassal.data.set("oldColor", oldColor);

        //             vassal.data.colorID = kingdom.data.colorID;
        //             ColorAsset kingdomcolor = kingdom.getColor();
        //             vassal.updateColor(kingdomcolor);
        //             World.world.zoneCalculator.setDrawnZonesDirty();
        //             World.world.zoneCalculator.clearCurrentDrawnZones(true);
        //             World.world.zoneCalculator.redrawZones();
        //         }


        //     }

        //     if (MoreGodPower.Vassals[kingdom].Count == 0)
        //     {
        //         vassalsToRemove.Add(kingdom);
        //     }
        // }

        // private static void JoinVassalToWar(Kingdom vassal, Kingdom kingdom)
        // {
        //     ListPool<War> wars = kingdom.getWars();
        //     foreach (War war in wars)
        //     {
        //         if (!war.hasKingdom(vassal))
        //         {
        //             if (war.data.list_defenders.Contains(kingdom.id))
        //             {
        //                 war.joinDefenders(vassal);
        //             }
        //             else if (war.data.list_attackers.Contains(kingdom.id))
        //             {
        //                 war.joinAttackers(vassal);
        //             }
        //         }
        //     }
        // }

        // private static void ChangeVassalAlliance(Kingdom vassal, Kingdom kingdom)
        // {
        //     if (vassal.getAlliance() != null && kingdom.getAlliance() == null)
        //     {
        //         var alliance = vassal.getAlliance();
        //         alliance.kingdoms_hashset.Remove(vassal);
        //         vassal.allianceLeave(vassal.getAlliance());
        //         alliance.recalculate();
        //     }
        //     else if (vassal.getAlliance() != null && kingdom.getAlliance() != vassal.getAlliance())
        //     {
        //         var alliance2 = vassal.getAlliance();
        //         alliance2.kingdoms_hashset.Remove(vassal);
        //         vassal.allianceLeave(alliance2);
        //         var alliance = kingdom.getAlliance();
        //         alliance.kingdoms_hashset.Add(vassal);
        //         vassal.allianceJoin(alliance);
        //         alliance.recalculate();
        //         alliance2.recalculate();
        //         alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
        //     }
        //     else if (vassal.getAlliance() == null && kingdom.getAlliance() != null)
        //     {
        //         var alliance = kingdom.getAlliance();
        //         alliance.kingdoms_hashset.Add(vassal);
        //         vassal.allianceJoin(alliance);
        //         alliance.recalculate();
        //         alliance.data.timestamp_member_joined = MapBox.instance.getCurWorldTime();
        //     }
        // }

        // private static void RemoveVassals(Kingdom kingdomToRemove)
        // {
        //     var vassals = MoreGodPower.Vassals[kingdomToRemove].ToList();
        //     foreach (var vassal in vassals)
        //     {
        //         vassal.data.set("Vassal", false);
        //         vassal.data.set("suzerainID", "");
        //     }
        //     MoreGodPower.Vassals.Remove(kingdomToRemove);
        // }
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