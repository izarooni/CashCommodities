﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities.Controls {
    public partial class CItemGroup : UserControl {

        public CItemGroup() {
            InitializeComponent();

            TextBox.TextChanged += OnTextChanged;
            TextBox.PreviewKeyDown += OnPreviewKeyDown;

            Class.Width = 100;
            Class.DataSource = Enum.GetValues(typeof(ClassType)).Cast<ClassType>().Select((type, i) => new {
                Name = type.ToString(),
                Value = (int)type
            }).ToList();
            Class.DisplayMember = "Name";
            Class.ValueMember = "Value";

            GridView.CurrentCellDirtyStateChanged += (sender, e) => {
                if (GridView.IsCurrentCellDirty) {
                    GridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            GridView.CellValueChanged += (sender, e) => {
                var row = GridView.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];

                var item = row.Tag as CashItem;
                item.SetValue(cell.OwningColumn.Name, cell.Value);
            };

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
                if (e.RowIndex < 1) return;

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

        public List<CashItem> RemoveQueue { get; set; } = new List<CashItem>();

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
                    var item = new CashItem((++MainWindow.LastNodeValue).ToString());
                    var row = item.CreateRow(GridView);
                    GridView.Rows.Insert(i, row);
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

        internal void OnTextChanged(object sender, EventArgs e) {
            // synchronize the text in the TextBox with the DataGridView
            GridView.SuspendLayout();
            var lines = TextBox.Lines;
            for (var i = 0; i < lines.Length; i++) {
                CashItem item;

                // synchronize existing rows with the text in the TextBox
                if (i < GridView.RowCount) {
                    item = GridView.Rows[i].Tag as CashItem;

                    if (!string.IsNullOrEmpty(lines[i])) {
                        // if the row exists, synchronize the text
                        item.SetValue("ItemID", lines[i]);
                        GridView.Rows[i].Cells[2].Value = lines[i];
                        continue;
                    }

                    // itemId is also empty which assumes the row has already been added, skip
                    if (string.IsNullOrEmpty(GridView.Rows[i].Cells[2].Value.ToString())) continue;
                }

                // new rows
                item = new CashItem((++MainWindow.LastNodeValue).ToString());
                var insertRow = item.CreateRow(GridView);
                GridView.Rows.Insert(i, insertRow);
            }

            // delete all extra rows after the last line
            for (var i = GridView.RowCount - 1; i >= lines.Length; i--) {
               var item = GridView.Rows[i].Tag as CashItem;
                if (item != null) {
                    RemoveQueue.Add(item);
                }
                GridView.Rows.RemoveAt(i);
            }
            GridView.ResumeLayout();
        }

        public void Clear() {
            SuspendLayout();

            GridView.ClearSelection();
            GridView.Rows.Clear();
            TextBox.Clear();
            RemoveQueue.Clear();

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
