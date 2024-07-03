using System;
using System.Drawing;
using System.Windows.Forms;

using MapleLib.WzLib;

namespace CashCommodities.Controls {
    public partial class CommodityViewer : UserControl {
        public CommodityViewer() {
            InitializeComponent();
        }

        public void ForEachGroup(Action<CItemGroup> action) {
            foreach (TabPage ctrl in tabControl.Controls) {
                foreach (CItemGroup cig in ctrl.Controls) {
                    action(cig);
                }
            }
        }

        public void ClearAllGroups() {
            ForEachGroup(group => group.Clear());
        }

        public CItemGroup GetGroupByItemID(int itemID, bool legacyMode) {
            int category = (itemID / 10000);
            CItemGroup group;
            if (legacyMode) group = capsGroup;
            else if (category == 100) group = capsGroup;
            else if (category == 104) group = topsGroup;
            else if (category == 105) group = overallsGroup;
            else if (category == 106) group = bottomsGroup;
            else if (category == 107) group = shoesGroup;
            else if (category == 108) group = glovesGroup;
            else if (category == 109 || category == 502 || category == 170) group = weaponsGroup;
            else if (category == 110) group = capesGroup;
            else if (category == 111) group = ringsGroup;
            else if (category == 500) group = petsGroup;
            else if (category == 190) group = mountsGroup;
            else {
                category = itemID / 1000;
                if (category >= 1010 && category <= 1013 || category == 1032) group = facesGroup;
                else if (category >= 1020 && category <= 1025) group = eyesGroup;
                else {
                    group = etcGroup;
                }
            }

            return group;
        }

        public DataGridViewRow AddItem(int itemID, WzImageProperty img, Bitmap image, bool legacyMode) {
            int? price = img.GetFromPath("Price")?.GetInt();
            int period = img.GetFromPath("Period")?.GetInt() ?? 0;
            bool sale = img.GetFromPath("OnSale")?.GetInt() == 1;
            int gender = img.GetFromPath("gender")?.GetInt() ?? 2;
            int count = img.GetFromPath("count")?.GetInt() ?? 1;
            int priority = img.GetFromPath("Priority")?.GetInt() ?? 98;

            var row = AddItem(image, img.Name, itemID, price, period, sale, gender, count, priority, legacyMode);
            row.Tag = img;
            return row;
        }

        public DataGridViewRow AddItem(Bitmap image, string name, int itemID, int? price, int period, bool sale, int gender, int count, int priority, bool legacyMode) {
            var group = GetGroupByItemID(itemID, legacyMode);

            var row = new DataGridViewRow();
            row.CreateCells(group.GridView, image, name, itemID, price, period, sale, gender, count, priority);
            group.GridView.Rows.Add(row);

            return row;
        }
    }
}