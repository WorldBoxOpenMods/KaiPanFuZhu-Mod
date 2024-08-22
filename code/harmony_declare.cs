using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplomacy_Army
{
    public class harmony_declare
    {
        public static bool CheckDeclare(City city)
        {
            city.data.get("Declare", out bool flag);

            if (flag)
            {
                city.data.get("DeclareKingdomID", out string str, "");
                Kingdom kingdom = World.world.kingdoms.getKingdomByID(str);

                if (kingdom != null)
                {
                    // if (!MoreGodPower.Vassals.ContainsKey(suzerain))
                    // {
                    //     MoreGodPower.Vassals.Add(suzerain, new List<Kingdom> { kingdom });
                    // }
                    // else if (!MoreGodPower.Vassals[suzerain].Contains(kingdom))
                    // {
                    //     MoreGodPower.Vassals[suzerain].Add(kingdom);
                    // }
                    return true;
                }
            }

            return false;
        }
    }
}