using System;
using System.Drawing;
using System.Windows.Forms;

using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities.Controls {
    public partial class CItemGroup : UserControl {

        public CItemGroup() {
            InitializeComponent();

            TextBox.TextChanged += OnTextChanged;
            TextBox.PreviewKeyDown += OnPreviewKeyDown;

            // synchronize scrolling between the DataGridView and the TextBox

            GridView.Scroll += (sender, e) => {
                // if textbox is focused
                if (TextBox.Focused) return;

                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll) {
                    var rowIndex = GridView.FirstDisplayedScrollingRowIndex;
                    // scroll corresponding line in TextBox to the top of the TextBox
                    if (rowIndex >= 0) {
                        var index = TextBox.GetFirstCharIndexFromLine(rowIndex);
                        var line = TextBox.GetLineFromCharIndex(index);
                        var text = TextBox.Lines[line];
                        TextBox.Select(index, 0);
                        TextBox.ScrollToCaret();
                    }
                }
            };
            GridView.CellClick += (sender, e) => {
                // get row
                var row = GridView.Rows[e.RowIndex];
                // select corresponding line in TextBox
                var index = TextBox.GetFirstCharIndexFromLine(e.RowIndex);
                var line = TextBox.GetLineFromCharIndex(index);
                var text = TextBox.Lines[line];
                TextBox.Select(index, text.Length);
                TextBox.ScrollToCaret();
            };

            TextBox.KeyUp += (sender, e) => {
                if (GridView.RowCount == 0) return;
                // get current line number of TextBox
                var index = TextBox.GetFirstCharIndexOfCurrentLine();
                var line = TextBox.GetLineFromCharIndex(index);

                var row = Math.Min(line, GridView.RowCount - 1);
                row = Math.Max(0, row);

                GridView.FirstDisplayedScrollingRowIndex = row;
            };
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (string.IsNullOrEmpty(TextBox.Text)) return;

            // get current line number of TextBox
            var index = TextBox.GetFirstCharIndexOfCurrentLine();
            var line = TextBox.GetLineFromCharIndex(index);
            var text = TextBox.Lines[line];

            // if pasting, cancel
            if (e.KeyCode == Keys.V && e.Control) {
                // get clipboard text
                var clipboard = Clipboard.GetText();
                var clipboardRows = clipboard.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;

                // if text is empty insert all rows, otherwise insert clipboard rows - 1
                var rows = string.IsNullOrEmpty(text) ? clipboardRows : clipboardRows - 1;
                GridView.SuspendLayout();
                for (var i = line; i < line + rows; i++) {
                    // if text is empty it's probably a new row added by the user
                    var insertRow = new DataGridViewRow();
                    insertRow.CreateCells(GridView,
                        null, // image
                        ++MainForm.LastNodeValue, // node
                        "", // itemId
                        4000, // price
                        90, // period
                        true, // sale
                        2, // gender
                        1, // count
                        99 // priority
                        );
                    GridView.Rows.Insert(i, insertRow);
                }
                GridView.ResumeLayout();
            }

            // if backspace
            if (e.KeyCode == Keys.Back) {
                bool remove = false;
                if (string.IsNullOrEmpty(text)) {
                    // if the line is empty, remove the row from the DataGridView
                    remove = true;

                    // bug fix: if there are two blank lines and one is deleted, delete the first row via forcing 0 index
                    if (line == 1 && GridView.RowCount == 1) line = 0;
                } else {
                    // if the line is not empty, check if the row in the DataGridView is empty
                    line = Math.Max(0, line - 1);
                    var row = GridView.Rows[line];
                    if (string.IsNullOrEmpty((string)row.Cells[2].Value)) {
                        // if the row is empty, remove the row from the DataGridView
                        remove = true;
                    }
                }
                if (remove) {
                    GridView.Rows.RemoveAt(line);
                }
            }

            // if delete
            if (e.KeyCode == Keys.Delete) {
                bool remove = false;
                if (string.IsNullOrEmpty(text)) {
                    // if the line is empty, remove the row from the DataGridView
                    remove = true;
                    line = Math.Min(line, GridView.RowCount - 1);
                } else {
                    // if the line is not empty, check if the row in the DataGridView is empty
                    line = Math.Min(line + 1, TextBox.Lines.Length);
                    var row = GridView.Rows[line];
                    if (string.IsNullOrEmpty((string)row.Cells[2].Value)) {
                        // if the row is empty, remove the row from the DataGridView
                        remove = true;
                    }
                }
                if (remove) {
                    GridView.Rows.RemoveAt(line);
                }
            }
        }

        private void OnTextChanged(object sender, EventArgs e) {
            //var image = (Bitmap)row.Cells[0].Value;
            //var node = row.Cells[1].Value;
            //var itemId = int.Parse(row.Cells[2].Value.ToString());
            //var price = int.Parse(row.Cells[3].Value.ToString());
            //var period = int.Parse(row.Cells[4].Value.ToString());
            //var sale = (bool)row.Cells[5].Value;
            //var gender = int.Parse(row.Cells[6].Value.ToString());
            //var count = int.Parse(row.Cells[7].Value.ToString());
            //var priority = int.Parse(row.Cells[8].Value.ToString());

            // synchronize the text in the TextBox with the DataGridView
            GridView.SuspendLayout();
            var lines = TextBox.Lines;
            for (var i = 0; i < lines.Length; i++) {
                // synchronize existing rows with the text in the TextBox
                if (i < GridView.RowCount) {
                    if (!string.IsNullOrEmpty(lines[i])) {
                        // if the row exists, synchronize the text
                        GridView.Rows[i].Cells[2].Value = lines[i];
                        continue;
                    }

                    // itemId is also empty which assumes the row has already been added, skip
                    if (string.IsNullOrEmpty(GridView.Rows[i].Cells[2].Value.ToString())) continue;

                    // if text is empty it's probably a new row added by the user
                    var insertRow = new DataGridViewRow();
                    insertRow.CreateCells(GridView,
                        null, // image
                        ++MainForm.LastNodeValue, // node
                        "", // itemId
                        4000, // price
                        90, // period
                        true, // sale
                        2, // gender
                        1, // count
                        99 // priority
                        );
                    GridView.Rows.Insert(i, insertRow);
                    continue;
                }

                var newRow = new DataGridViewRow();
                newRow.CreateCells(GridView,
                    null, // image
                    ++MainForm.LastNodeValue, // node
                    lines[i], // itemId
                    4000, // price
                    90, // period
                    true, // sale
                    2, // gender
                    1, // count
                    99 // priority
                 );
                GridView.Rows.Add(newRow);
            }

            // delete all extra rows after the last line
            while (GridView.RowCount > lines.Length) {
                GridView.Rows.RemoveAt(GridView.RowCount - 1);
            }
            GridView.ResumeLayout();
        }

        public void Clear() {
            SuspendLayout();

            GridView.ClearSelection();
            GridView.Rows.Clear();

            ResumeLayout();
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
