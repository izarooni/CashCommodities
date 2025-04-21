using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CashCommodities.Controls;

namespace CashCommodities {
    internal class CommodityImage {

        public CommodityImage() {
            Node = ItemCategory.GenerateNodeName();
            Count = 1;
            Price = 4000;
            Period = 90;
            Priority = 99;
            Gender = 2;
            OnSale = true;
            Class = CommodityClassType.None;
        }

        public CommodityImage(WzImageProperty property) {
            int GetOrDefault(string name, int defaultValue) {
                var intProp = property[name] as WzIntProperty;
                if (intProp == null) return defaultValue;
                return intProp.Value;
            }

            Node = property.Name;
            SN = (property["SN"] as WzIntProperty)?.Value ?? throw new Exception("SN property not found");
            ItemId = (property["ItemId"] as WzIntProperty)?.Value ?? throw new Exception("ItemId property not found");
            Count = GetOrDefault("Count", 1);
            Price = GetOrDefault("Price", 4000);
            Period = GetOrDefault("Period", 90);
            Priority = GetOrDefault("Priority", 98);
            Gender = GetOrDefault("Gender", 2);
            OnSale = GetOrDefault("OnSale", 1) == 1;
            Class = (CommodityClassType)GetOrDefault("Class", (int)CommodityClassType.None);
        }

        public string Node { get; set; }
        public int SN { get; set; }
        public int ItemId { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int Period { get; set; }
        public int Priority { get; set; }
        public int Gender { get; set; }
        public bool OnSale { get; set; }
        public CommodityClassType Class { get; set; }
        public Bitmap Picture { get; set; }

        public int SubCategory => SN / 100000 % 100;
        public DataGridViewRow Row { get; set; }

        public override string ToString() {
            return $"Item(SN={SN}, ItemId={ItemId}, OnSale={OnSale}, Priority={Priority}, Class={Class})";
        }

        public WzImageProperty CreateImageProperty() {
            var img = new WzSubProperty(Node);
            img.AddProperty(new WzIntProperty("SN", SN));
            img.AddProperty(new WzIntProperty("ItemId", ItemId));
            img.AddProperty(new WzIntProperty("Count", Count));
            img.AddProperty(new WzIntProperty("Price", Price));
            img.AddProperty(new WzIntProperty("Period", Period));
            img.AddProperty(new WzIntProperty("Priority", Priority));
            img.AddProperty(new WzIntProperty("Gender", Gender));
            img.AddProperty(new WzIntProperty("OnSale", OnSale ? 1 : 0));
            if (Class != CommodityClassType.None) {
                // save some space but omitting the class if it's None
                img.AddProperty(new WzIntProperty("Class", (int)Class));
            }
            return img;
        }

        public void SetValue(CommodityPropertyType propertyType, object value) {
            switch (propertyType) {
                case CommodityPropertyType.Class:
                case CommodityPropertyType.SN:
                case CommodityPropertyType.ItemId:
                case CommodityPropertyType.Count:
                case CommodityPropertyType.Price:
                case CommodityPropertyType.Period:
                case CommodityPropertyType.Priority:
                case CommodityPropertyType.Gender:
                    if (!(value is int i)) {
                        MessageBox.Show("Invalid value type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    switch (propertyType) {
                        case CommodityPropertyType.Class:
                            Class = (CommodityClassType)i;
                            break;
                        case CommodityPropertyType.SN: SN = i; break;
                        case CommodityPropertyType.ItemId: ItemId = i; break;
                        case CommodityPropertyType.Count: Count = i; break;
                        case CommodityPropertyType.Price: Price = i; break;
                        case CommodityPropertyType.Period: Period = i; break;
                        case CommodityPropertyType.Priority: Priority = i; break;
                        case CommodityPropertyType.Gender: Gender = i; break;
                    }
                    Logger.Log($"Updated {propertyType} to {i}");
                    break;
                case CommodityPropertyType.OnSale:
                    if (!(value is bool b)) {
                        MessageBox.Show("Invalid value type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    switch (propertyType) {
                        case CommodityPropertyType.OnSale: OnSale = b; break;
                    }
                    Logger.Log($"Updated {propertyType} to {b}");
                    break;
            }
        }
    }
}
