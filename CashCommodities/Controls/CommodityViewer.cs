using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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

        public bool AddItem(WzImageProperty img) {
            int itemID = img.GetFromPath("ItemId").GetInt();
            int? price = img.GetFromPath("Price")?.GetInt();
            int donor = img.GetFromPath("isDonor")?.GetInt() ?? 0;
            int period = img.GetFromPath("Period")?.GetInt() ?? 0;

            int category = (itemID / 10000);
            CItemGroup group;
            if (category == 100) group = capsGroup;
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
                    Debug.WriteLine("unhandled item {0}. Node {1}", itemID, img.Name);
                    return false;
                }
            }

            var row = new DataGridViewRow();
            row.CreateCells(group.GridView, img.Name, itemID, price, donor, period);
            row.Tag = img;
            group.GridView.Rows.Add(row);

            return true;
        }
    }
}
