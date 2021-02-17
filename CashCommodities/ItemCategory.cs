using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashCommodities {
    internal static class ItemCategory
    {

        public static readonly List<int> SnCache = new List<int>();

        public static int GenerateSn( bool donor, int itemId) {
            var sn = donor ? 10000000 : 20000000;
            sn += GetCategory(itemId, donor) * 100000;

            var i = 1;
            while (SnCache.Contains(sn + i)) {
                i++;
            }

            sn += i;

            SnCache.Add(sn);
            return sn;
        }

        public static int GetCategory(int itemID, bool donor) {
            var type = itemID / 10000;

            if (type == 100) return 0; // Donor.Hat and Equip.Hat

            if (donor && type >= 101 && type <= 103) return 1; // Donor.Acc

            if (!donor && type == 101) return 1; // Equip.Face
            if (!donor && type == 102) return 2; // Equip.Eye

            if (type == 104) return donor ? 2 : 3; // Overall
            if (type == 105) return donor ? 3 : 4; // Top
            if (type == 106) return donor ? 4 : 5; // Bottom
            if (type == 107) return donor ? 5 : 6; // Shoe
            if (type == 108) return donor ? 6 : 7; // Glove
            if (type == 170) return donor ? 7 : 8; // Weapon

            if (!donor && type == 111) return 9; // Equip.Ring

            if (type == 110) return donor ? 8 : 11; // Cape
            if (type == 500) return donor ? 9 : 0; // Pet

            if (!donor) {
                Debug.WriteLine("unhandled item: {0}", itemID);
                throw new InvalidOperationException();
            }

            return 10;
        }
    }
}
