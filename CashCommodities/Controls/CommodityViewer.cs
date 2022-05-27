using System.Drawing;
using System.Windows.Forms;
using MapleLib.WzLib;

namespace CashCommodities.Controls {
    public partial class CommodityViewer : UserControl {
        public CommodityViewer() {
            InitializeComponent();
        }

        public void ClearData() {
            foreach (TabPage page in tabControl.Controls) {
                var group = (CItemGroup) page.Controls[0];
                group.Clear();
            }
        }

        public bool AddItem(int itemID, WzImageProperty img, Bitmap image, bool legacyMode) {
            int? price = img.GetFromPath("Price")?.GetInt();
            int donor = img.GetFromPath("isDonor")?.GetInt() ?? 0;
            int period = img.GetFromPath("Period")?.GetInt() ?? 0;
            bool sale = img.GetFromPath("OnSale")?.GetInt() == 1;
            int gender = img.GetFromPath("gender")?.GetInt() ?? 2;
            int count = img.GetFromPath("count")?.GetInt() ?? 1;

            int category = (itemID / 10000);
            CItemGroup group;
            if (legacyMode) {
                group = capsGroup;
                donor = 1;
            } else if (category == 100) group = capsGroup;
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
                    donor = 0;
                    group = etcGroup;
                }
            }

            var row = new DataGridViewRow();
            row.CreateCells(group.GridView, image, img.Name, itemID, price, donor, period, sale, gender, count);
            row.Tag = img;
            group.GridView.Rows.Add(row);

            return true;
        }
    }
}