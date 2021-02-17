using System.Windows.Forms;

namespace CashCommodities.Controls {
    partial class CItemGroup {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.GridView = new System.Windows.Forms.DataGridView();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.Node = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDonor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Node,
            this.ItemID,
            this.price,
            this.IsDonor,
            this.Period});
            this.GridView.Dock = System.Windows.Forms.DockStyle.Left;
            this.GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.GridView.Location = new System.Drawing.Point(0, 23);
            this.GridView.Name = "GridView";
            this.GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GridView.Size = new System.Drawing.Size(323, 286);
            this.GridView.TabIndex = 1;
            this.GridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_CellMouseDoubleClick);
            this.GridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridView_RowsAdded);
            // 
            // TextBox
            // 
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox.Location = new System.Drawing.Point(323, 23);
            this.TextBox.MaxLength = 2147483647;
            this.TextBox.Multiline = true;
            this.TextBox.Name = "TextBox";
            this.TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox.Size = new System.Drawing.Size(167, 286);
            this.TextBox.TabIndex = 2;
            this.TextBox.TextChanged += new System.EventHandler(this.ReplacementContent_TextChanged);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProgressBar.Location = new System.Drawing.Point(0, 0);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(490, 23);
            this.ProgressBar.Step = 1;
            this.ProgressBar.TabIndex = 3;
            // 
            // Node
            // 
            this.Node.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Node.HeaderText = "Node";
            this.Node.Name = "Node";
            this.Node.ReadOnly = true;
            this.Node.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Node.Width = 39;
            // 
            // ItemID
            // 
            this.ItemID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ItemID.HeaderText = "Item ID";
            this.ItemID.Name = "ItemID";
            this.ItemID.ReadOnly = true;
            this.ItemID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemID.Width = 47;
            // 
            // price
            // 
            this.price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.price.HeaderText = "Price";
            this.price.Name = "price";
            this.price.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.price.Width = 5;
            // 
            // IsDonor
            // 
            this.IsDonor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.IsDonor.HeaderText = "Donor";
            this.IsDonor.Name = "IsDonor";
            this.IsDonor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IsDonor.Width = 42;
            // 
            // Period
            // 
            this.Period.HeaderText = "Period";
            this.Period.Name = "Period";
            // 
            // CItemGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.GridView);
            this.Controls.Add(this.ProgressBar);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CItemGroup";
            this.Size = new System.Drawing.Size(490, 309);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBar;
        internal System.Windows.Forms.TextBox TextBox;
        internal DataGridView GridView;
        private DataGridViewTextBoxColumn Node;
        private DataGridViewTextBoxColumn ItemID;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn IsDonor;
        private DataGridViewTextBoxColumn Period;
    }
}
