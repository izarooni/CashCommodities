using System;
using System.Windows.Forms;

using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities.Controls {
    public partial class CItemGroup : UserControl {

        public CItemGroup() {
            InitializeComponent();
        }

        public void Clear() {
            SuspendLayout();

            ProgressBar.Value = 0;
            GridView.ClearSelection();
            GridView.Rows.Clear();

            ResumeLayout();
        }

        private void ReplacementContent_TextChanged(object sender, EventArgs e) {
            string[] lines = TextBox.Lines;
            ProgressBar.Value = Math.Min(ProgressBar.Maximum, Math.Max(0, lines.Length));
        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            ProgressBar.Maximum = e.RowIndex;
        }

        private void DataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (e.ColumnIndex != 2) return;
            DataGridViewCell cell = GridView.Rows[e.RowIndex].Cells[2];
            int nValue = (int) cell.Value;

            GridView.SuspendLayout();
            foreach (DataGridViewRow row in GridView.Rows) {
                WzImageProperty img = (WzImageProperty) row.Tag;
                WzImageProperty priceImg = img.GetFromPath("Price");

                row.Cells[2].Value = nValue;
                img.ParentImage.Changed = true;
                if (priceImg == null) {
                    img.WzProperties.Add(new WzIntProperty("Price", nValue));
                } else {
                    ((WzIntProperty) priceImg).Value = nValue;
                }
            }
            GridView.ResumeLayout();
        }
    }
}
