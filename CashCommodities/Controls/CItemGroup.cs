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
            int rowCount = GridView.RowCount;
            string[] replacements = TextBox.Lines;

            int loadedCount = GridView.RowCount;
            int replacementCount = Math.Min(loadedCount, replacements.Length);
            int addedCount = Math.Max(0, replacements.Length - loadedCount);

            InformationLabel.Text = string.Format(Resources.InformationLabel, loadedCount, replacementCount, addedCount);
        }

        private void ReplacementContent_TextChanged(object sender, EventArgs e) {
            UpdateInformationLabel();
        }

        private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
            UpdateInformationLabel();
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
