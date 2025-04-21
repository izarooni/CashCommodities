using System.Collections.Generic;

namespace CashCommodities {
    internal enum EquipType {
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
        PetEquip,
        UNKNOWN
    }

    internal enum ItemType {
        Special,
        Pet,
        Cash,
        Etc,
        Install,
        Consume,
        UNKNOWN
    }

    internal static class ItemCategory {
        public static ItemType GetItemTypeByItemId(int itemId) {
            int type = itemId / 10000;
            switch (type) {
                case 900:
                case 910:
                case 911:
                    return ItemType.Special;
                case 500: // Pet
                    return ItemType.Pet;
                case int n when n >= 501 && n <= 599:
                    return ItemType.Cash;
                case int n when n >= 400 && n <= 499:
                    return ItemType.Etc;
                case 301:
                case 399:
                    return ItemType.Install;
                case int n when n >= 200 && n <= 299:
                    return ItemType.Consume;
            }

            return ItemType.UNKNOWN;
        }

        public static EquipType GetEquipTypeByItemId(int itemId) {
            int type = itemId / 10000;
            switch (type) {
                case 101:
                case 102:
                case 103:
                case 112:
                case 114: return EquipType.Accessory;
                case 100: return EquipType.Cap;
                case 110: return EquipType.Cape;
                case 104: return EquipType.Coat;
                case 108: return EquipType.Glove;
                case 105: return EquipType.Longcoat;
                case 106: return EquipType.Pants;
                case 111: return EquipType.Ring;
                case 109: return EquipType.Shield;
                case 107: return EquipType.Shoes;
                case 190: return EquipType.TamingMob;
                case var _ when type >= 130 && type <= 170: return EquipType.Weapon;
                case 180:
                case 181:
                case 182:
                case 183: return EquipType.PetEquip;
            }

            return EquipType.UNKNOWN;
        }

        private static int _incrementingSn = 0;
        public static int IncrementingNode = 0;
        public static readonly HashSet<int> SnCache = new HashSet<int>();

        public static int GenerateSn(int mainTabIndex, int subTabIndex) {
            var sn = 10000000 * mainTabIndex;
            sn += subTabIndex / 100000; // sub category
            sn += ++_incrementingSn; // UID
            while (SnCache.Contains(sn)) {
                sn++;
            }
            SnCache.Add(sn);
            return sn;
        }
        public static string GenerateNodeName() {
            return (++IncrementingNode).ToString();
        }
    }
}
