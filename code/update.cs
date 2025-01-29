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
            if (MoreGodPower.Vassals.Count > 0)
            {
                var vassalsToRemove = new HashSet<Kingdom>();

                foreach (var kingdom in MoreGodPower.Vassals.Keys.ToList())
                {
                    if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
                    {
                        vassalsToRemove.Add(kingdom);
                        continue;
                    }


                    UpdateKingdomVassals(kingdom, vassalsToRemove);
                }
                foreach (var kingdom in vassalsToRemove)
                {
                    RemoveVassals(kingdom);
                }
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
                    UpdateVassalAlliance(vassal, kingdom);
                }

                if (vassals.Count == 0)
                {
                    vassalsToRemove.Add(kingdom);
                }
            }
        }

        public static void UpdateVassalAlliance(Kingdom vassal, Kingdom kingdom)
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


        public static void JoinVassalToWar(Kingdom vassal, Kingdom kingdom)
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

        public static void ChangeVassalAlliance(Kingdom vassal, Kingdom kingdom)
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


        public static void UpdateDeclare()
        {
            var DeclareToRemove = new HashSet<Kingdom>();

            foreach (var kingdom in MoreGodPower.Declares.Keys.ToList())
            {
                if (kingdom == null || !kingdom.isAlive() || kingdom.data == null)
                {
                    continue;
                }

                // UpdateVassalColor(kingdom);
                UpdateKingdomDeclares(kingdom, ref DeclareToRemove);
            }

        }


        public static void UpdateKingdomDeclares(Kingdom kingdom, ref HashSet<Kingdom> DeclareToRemove)
        {
            if (MoreGodPower.Declares.TryGetValue(kingdom, out var Declares))
            {
                for (int i = 0; i < Declares.Count; i++)
                {
                    var city = Declares[i];
                    if (city == null || city.data == null)
                    {
                        Declares.RemoveAt(i);
                        i--;
                        continue;
                    }

                    // UpdateVassalAlliance(vassal, kingdom);
                }

                if (Declares.Count == 0)
                {
                    DeclareToRemove.Add(kingdom);
                }
            }
        }

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