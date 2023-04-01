using System;
using System.Windows.Forms;

using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities.Controls {
    public partial class CItemGroup : UserControl {

        public CItemGroup() {
            InitializeComponent();
        }

        public void Clear() {
            SuspendLayout();

            GridView.ClearSelection();
            GridView.Rows.Clear();

            ResumeLayout();
        }

        private void UpdateInformationLabel() {
            var replacements = TextBox.Lines;

            var loadedCount = GridView.RowCount;
            var totalCount = replacements.Length;
            var replacementCount = 0;

            for (var i = 0; i < replacements.Length; i++) {
                var rep = replacements[i];

                // decrement from total for each invalid id and blank line
                if (rep.Length == 0) {
                    totalCount--;
                    continue; // blank text due to new line?
                }
                if (!int.TryParse(rep, out var _)) {
                    totalCount--;
                    continue;
                }

                // count number of different IDs; replacements
                if (i < GridView.RowCount) {
                    var existing = GridView.Rows[i].Cells[2].Value.ToString();
                    if (!string.Equals(existing, rep, StringComparison.CurrentCultureIgnoreCase)) {
                        replacementCount++;
                    }
                }
            }

            //replacementCount = Math.Min(loadedCount, totalCount);
            var addedCount = Math.Max(0, totalCount - loadedCount);

            InformationLabel.Text = string.Format(Resources.InformationLabel, loadedCount, replacementCount, addedCount);
        }

        private void ReplacementContent_TextChanged(object sender, EventArgs e) {
            UpdateInformationLabel();
        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            UpdateInformationLabel();
        }

        private void DataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
            if (e.ColumnIndex != 3) return;

            // when double clicking price, change all rows in the same column to the same value
            DataGridViewCell cell = GridView.Rows[e.RowIndex].Cells[3];
            int nValue = (int)cell.Value;

            GridView.SuspendLayout();
            foreach (DataGridViewRow row in GridView.Rows) {
                WzImageProperty img = (WzImageProperty)row.Tag;
                WzImageProperty priceImg = img.GetFromPath("Price");

                row.Cells[3].Value = nValue;
                img.ParentImage.Changed = true;
                if (priceImg == null) {
                    img.WzProperties.Add(new WzIntProperty("Price", nValue));
                } else {
                    ((WzIntProperty)priceImg).Value = nValue;
                }
            }
            GridView.ResumeLayout();
        }
    }
}
