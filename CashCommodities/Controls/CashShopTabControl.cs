using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CashCommodities.Controls {
    public partial class CashShopTabControl : TabPage {
        public CashShopTabControl() {
            InitializeComponent();

            TabControl = new TabControl {
                Dock = DockStyle.Fill,
                Padding = new Point(0, 0),
                Margin = new Padding(0, 0, 0, 0),
            };
            Controls.Add(TabControl);
        }

        private TabControl TabControl { get; set; }
        internal Dictionary<int, CashShopItemsControl> SubCategoryControls { get; } = new Dictionary<int, CashShopItemsControl>();
        internal CashShopTabType CashShopMainTab { get; set; }

        public void Clear() {
            foreach (var control in SubCategoryControls.Values) {
                control.Clear();
            }
            
            TabControl.TabPages.Clear();
            SubCategoryControls.Clear();
        }

        internal void AddSubCategory(int subTabIndex, CashShopItemsControl control) {
            Logger.Log($"[MainWindow] Adding subcategory {control.Name} to {CashShopMainTab}");
            control.MainTabIndex = (int)CashShopMainTab;
            control.SubTabIndex = subTabIndex;
            TabControl.TabPages.Add(control);
            SubCategoryControls.Add(subTabIndex, control);
        }

        internal bool AddItem(CommodityImage item) {
            SubCategoryControls.TryGetValue(item.SubCategory, out var control);
            if (control == null) return false;
            
            control.AddItem(item);
            return true;
        }
    }
}
