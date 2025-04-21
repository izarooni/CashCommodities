using System;
using System.Linq;
using System.Windows.Forms;

namespace CashCommodities.Controls {
    public partial class CashShopItemsControl : TabPage {

        public CashShopItemsControl() {
            InitializeComponent();

            WzContextMenu = new ContextMenuStrip();

            var properties = Enum.GetValues(typeof(CommodityPropertyType)).Cast<CommodityPropertyType>();
            foreach (var property in properties) {
                // for the record, adding AutoSize causes performance issues when editing rows en masse
                // it's best to leave it to the user to resize the columns
                switch (property) {
                    case CommodityPropertyType.Picture:
                        var pictureCol = new DataGridViewImageColumn {
                            HeaderText = property.ToString(),
                            Name = property.ToString(),
                            Width = 50,
                            ReadOnly = true,
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                        };
                        GridView.Columns.Add(pictureCol);
                        break;
                    case CommodityPropertyType.OnSale:
                        var saleCol = new DataGridViewCheckBoxColumn {
                            HeaderText = property.ToString(),
                            Name = property.ToString(),
                            Width = 50,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                        };
                        GridView.Columns.Add(saleCol);
                        break;
                    case CommodityPropertyType.Class:
                        var classCol = new DataGridViewComboBoxColumn {
                            HeaderText = property.ToString(),
                            Name = property.ToString(),
                            Width = 100,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            DataSource = Enum.GetValues(typeof(CommodityClassType)).Cast<CommodityClassType>().Select((type, i) => new {
                                Value = (int)type,
                                Name = type.ToString()
                            }).ToList(),
                            // Members represent a property associated with the DataSource object
                            DisplayMember = "Name",
                            ValueMember = "Value",
                        };
                        GridView.Columns.Add(classCol);
                        break;
                    default:
                        var col = new DataGridViewTextBoxColumn {
                            HeaderText = property.ToString(),
                            Name = property.ToString(),
                            Width = 50,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                        };
                        switch (property) {
                            case CommodityPropertyType.SN:
                            case CommodityPropertyType.Node:
                                col.ReadOnly = true;
                                break;
                        }
                        GridView.Columns.Add(col);
                        break;

                }
            }

            GridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            GridView.CurrentCellDirtyStateChanged += (s, e) => {
                if (GridView.CurrentCell is DataGridViewComboBoxCell) {
                    // force combo box value to change upon selection instead of waiting for user to unfocus the cell
                    GridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            GridView.MouseClick += OnGridCellClick;
            GridView.CellValueChanged += OnGridCellValueChanged;
        }

        private ContextMenuStrip WzContextMenu { get; set; }
        public int MainTabIndex { get; set; }
        public int SubTabIndex { get; set; }

        internal void AddItem(CommodityImage item) {
            CreateItem(item);
        }

        internal void CreateItem(CommodityImage item, int index = -1) {
            if (item.SN == 0) {
                item.SN = ItemCategory.GenerateSn(MainTabIndex, SubTabIndex);
            }

            var row = new DataGridViewRow();
            row.CreateCells(GridView,
                // order must match the ordinal order of CommodityProperty
                item.Node,
                item.Picture,
                item.SN,
                item.ItemId,
                item.Price,
                item.Period,
                item.OnSale,
                item.Gender,
                item.Count,
                item.Priority,
                (int)item.Class);
            row.Tag = item;
            item.Row = row;
            if (index < 0) GridView.Rows.Add(row);
            else GridView.Rows.Insert(index, row);
        }

        internal void Clear() {
            GridView.Rows.Clear();
        }

        #region Event Handlers
        private ToolStripMenuItem OnInsertAbove() {
            var item = new ToolStripMenuItem("Insert Above") {
                Name = "InsertAbove",
                Enabled = GridView.SelectedRows.Count == 1,
            };
            item.Click += (s, e) => {
                GridView.SuspendLayout();
                var selectedRow = GridView.SelectedRows[0];
                var index = selectedRow.Index;
                var newItem = new CommodityImage();
                CreateItem(newItem, index);
                GridView.ResumeLayout();
            };
            return item;
        }

        private ToolStripMenuItem OnInsertBelow() {
            var item = new ToolStripMenuItem("Insert Below") {
                Name = "InsertBelow",
                Enabled = GridView.SelectedRows.Count == 1,
            };
            item.Click += (s, e) => {
                GridView.SuspendLayout();
                var selectedRow = GridView.SelectedRows[0];
                var index = selectedRow.Index + 1;
                var newItem = new CommodityImage();
                CreateItem(newItem, index);
                GridView.ResumeLayout();
            };
            return item;
        }
        private ToolStripMenuItem OnDelete() {
            var item = new ToolStripMenuItem("Delete") {
                Name = "Delete",
                Enabled = GridView.SelectedRows.Count > 0,
            };
            item.Click += (s, e) => {
                GridView.SuspendLayout();
                foreach (DataGridViewRow row in GridView.SelectedRows) {
                    GridView.Rows.Remove(row);
                }
                GridView.ResumeLayout();
            };
            return item;
        }
        private ToolStripMenuItem OnReplace() {
            var text = Clipboard.GetText();
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var selectedRows = GridView.SelectedRows;
            var maxRows = GridView.RowCount;
            ToolStripMenuItem item;

            if (selectedRows.Count > 0) {
                var selectedRow = selectedRows[0];
                
                item = new ToolStripMenuItem($"Replace {Math.Min(lines.Length, maxRows - selectedRow.Index)} Item(s)") {
                    Name = "Paste",
                    Enabled = Clipboard.ContainsText() && GridView.SelectedRows.Count == 1
                };
                item.Click += (s, e) => {
                    if (!lines.All(line => int.TryParse(line, out _))) {
                        MessageBox.Show("Invalid data in clipboard. Please copy a list of integers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var itemIds = lines.Select(int.Parse);

                    var replacing = maxRows - selectedRow.Index;
                    // confirm operation
                    if (replacing < lines.Length) {
                        var result = MessageBox.Show($"You are about to replace {replacing} items and create {lines.Length - replacing}. Do you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result != DialogResult.Yes) return;
                    }

                    GridView.SuspendLayout();
                    for (int i = 0; i < lines.Length; i++) {
                        if (i + selectedRow.Index >= maxRows) {
                            var newItem = new CommodityImage();
                            newItem.ItemId = itemIds.ElementAt(i);
                            CreateItem(newItem);
                        } else {
                            var row = GridView.Rows[selectedRow.Index + i];
                            var rowCommodity = row.Tag as CommodityImage;
                            rowCommodity.ItemId = itemIds.ElementAt(i);
                            row.Cells["ItemId"].Value = rowCommodity.ItemId;
                        }
                    }
                    GridView.ResumeLayout();
                }; item.Click += (s, e) => {
                    if (!lines.All(line => int.TryParse(line, out _))) {
                        MessageBox.Show("Invalid data in clipboard. Please copy a list of integers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                
                    var itemIds = lines.Select(int.Parse);

                    var replacing = maxRows - selectedRow.Index;
                    // confirm operation
                    if (replacing < lines.Length) {
                        var result = MessageBox.Show($"You are about to replace {replacing} items and create {lines.Length - replacing}. Do you want to continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result != DialogResult.Yes) return;
                    }
                
                    GridView.SuspendLayout();
                    for (int i = 0; i < lines.Length; i++) {
                        if (i + selectedRow.Index >= maxRows) {
                            var newItem = new CommodityImage();
                            newItem.ItemId = itemIds.ElementAt(i);
                            CreateItem(newItem);
                        } else {
                            var row = GridView.Rows[selectedRow.Index + i];
                            var rowCommodity = row.Tag as CommodityImage;
                            rowCommodity.ItemId = itemIds.ElementAt(i);
                            row.Cells["ItemId"].Value = rowCommodity.ItemId;
                        }
                    }
                    GridView.ResumeLayout();
                };
            } else {
                item = new ToolStripMenuItem($"Nothing selected for replacement") {
                    Name = "Paste",
                    Enabled = false
                };
            }
            return item;
        }

        private ToolStripMenuItem OnInsert() {
            var text = Clipboard.GetText();
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var item = new ToolStripMenuItem($"Create {lines.Length} Items") {
                Name = "Create",
                Enabled = Clipboard.ContainsText(),
            };
            item.Click += (s, e) => {
                if (!lines.All(line => int.TryParse(line, out _))) {
                    MessageBox.Show("Invalid data in clipboard. Please copy a list of integers.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var itemIds = lines.Select(int.Parse);
                GridView.SuspendLayout();
                foreach (var itemId in itemIds) {
                    var newItem = new CommodityImage();
                    newItem.ItemId = itemId;
                    CreateItem(newItem);
                }
                GridView.ResumeLayout();
            };

            return item;
        }

        private void OnGridCellClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                WzContextMenu.Items.Clear();
                ToolStripItem[] items = { OnInsertAbove(), OnInsertBelow(), OnDelete(), OnReplace(), OnInsert() };
                WzContextMenu.Items.AddRange(new ToolStripItemCollection(WzContextMenu, items));
                WzContextMenu.Show(Cursor.Position);
            }
        }

        private void OnGridCellValueChanged(object sender, DataGridViewCellEventArgs e) {
            var row = GridView.Rows[e.RowIndex];
            var cell = row.Cells[e.ColumnIndex];

            var commodityItem = row.Tag as CommodityImage;
            commodityItem.SetValue((CommodityPropertyType)e.ColumnIndex, cell.Value);
        }
        #endregion
    }
}
