using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace CashCommodities {
    internal enum ItemType {
        Accessory,
        Cap,
        Cape,
        Coat,
        Glove,
        Longcoat,
        Pants,
        Ring,
        Shield,
        Shoes,
        TamingMob,
        Weapon,
        UNKNOWN
    }

    internal static class ItemCategory {

        public static readonly List<int> SnCache = new List<int>();

        public static ItemType GetTypeByItemID(int itemID) {
            int type = itemID / 10000;
            switch (type) {
                case 101:
                case 102:
                case 103:
                case 112:
                case 114: return ItemType.Accessory;
                case 100: return ItemType.Cap;
                case 110: return ItemType.Cape;
                case 104: return ItemType.Coat;
                case 108: return ItemType.Glove;
                case 105: return ItemType.Longcoat;
                case 106: return ItemType.Pants;
                case 111: return ItemType.Ring;
                case 109: return ItemType.Shield;
                case 107: return ItemType.Shoes;
                case 190: return ItemType.TamingMob;
                case 170: return ItemType.Weapon;
            }

            return ItemType.UNKNOWN;
        }

        public static int GenerateSn(bool donor, int itemId) {
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
                throw new InvalidOperationException();
            }

            return 10;
        }
    }
}

