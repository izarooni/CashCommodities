using MapleLib.WzLib;

using System;
using System.Windows.Forms;

namespace CashCommodities {
    public class CashItem {

        public CashItem(string node) {
            Node = node;
        }

        public CashItem(WzImageProperty img) : this(img.Name) {
            SN = img.GetFromPath("SN").GetInt();
            ItemId = img.GetFromPath("ItemId").GetInt();
            OnSale = img.GetFromPath("OnSale")?.GetInt() == 1;
            Price = img.GetFromPath("Price")?.GetInt() ?? 3000;
            Period = img.GetFromPath("Period")?.GetInt() ?? 0;
            Gender = img.GetFromPath("gender")?.GetInt() ?? 2;
            Count = img.GetFromPath("count")?.GetInt() ?? 1;
            Priority = img.GetFromPath("Priority")?.GetInt() ?? 99;
            Priority = Math.Max(1, Priority - 1);

            int classType = img.GetFromPath("Class")?.GetInt() ?? (int)ClassType.None;
            Class = (ClassType)classType;

            Image = img;
        }

        public DataGridViewRow CreateRow(DataGridView grid) {
            var row = new DataGridViewRow();
            row.CreateCells(grid,
                null, // picture
                Node,
                ItemId,
                Price,
                Period,
                OnSale,
                Gender,
                Count,
                Priority,
                (int)Class
            );
            row.Tag = this;
            return row;
        }

        public WzImageProperty Image { get; set; }

        public bool IsDonor => SN / 10000000 == 1;

        public string Node { get; set; }

        public int SN { get; set; }

        public int ItemId { get; set; }

        public bool OnSale { get; set; } = true;

        public int Price { get; set; } = 4000;

        public int Period { get; set; } = 90;

        public int Gender { get; set; } = 2;

        public int Count { get; set; } = 1;

        public int Priority { get; set; } = 99;

        public ClassType Class { get; set; } = ClassType.None;

        internal void SetValue(string name, object value) {
            // im just lazy
            var sValue = value as string; // for text-based columns
            var bValue = value as bool?; // for checkbox-based columns
            var iValue = value as int?; // for numeric or datasource-based columns
            int.TryParse(sValue, out var piValue);

            switch (name) {
                case "SN":
                    SN = piValue;
                    break;
                case "ItemID":
                    ItemId = piValue;
                    break;
                case "OnSale":
                    OnSale = bValue.Value;
                    break;
                case "Price":
                    Price = piValue;
                    break;
                case "Period":
                    Period = piValue;
                    break;
                case "Gender":
                    Gender = piValue;
                    break;
                case "Count":
                    Count = piValue;
                    break;
                case "Priority":
                    Priority = piValue;
                    break;
                case "Class":
                    Class = (ClassType)(int)iValue;
                    break;
                default:
                    throw new ArgumentException($"Invalid property name: {name}", nameof(name));
            }
        }
    }
}
