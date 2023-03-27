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
            System.Windows.Forms.ToolStrip toolStrip1;
            this.GridView = new System.Windows.Forms.DataGridView();
            this.Image = new System.Windows.Forms.DataGridViewImageColumn();
            this.Node = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDonor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Period = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.onSale = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.InformationLabel = new System.Windows.Forms.Label();
            this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(490, 25);
            toolStrip1.TabIndex = 5;
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Image,
            this.Node,
            this.ItemID,
            this.price,
            this.IsDonor,
            this.Period,
            this.onSale,
            this.gender,
            this.count,
            this.Priority});
            this.GridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.GridView.Location = new System.Drawing.Point(0, 25);
            this.GridView.Name = "GridView";
            this.GridView.RowHeadersWidth = 5;
            this.GridView.RowTemplate.Height = 50;
            this.GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GridView.Size = new System.Drawing.Size(348, 284);
            this.GridView.TabIndex = 1;
            this.GridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_CellMouseDoubleClick);
            this.GridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridView_RowsAdded);
            // 
            // Image
            // 
            this.Image.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Image.HeaderText = "Image";
            this.Image.Name = "Image";
            this.Image.ReadOnly = true;
            this.Image.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Image.Width = 42;
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
            this.ItemID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ItemID.Width = 47;
            // 
            // price
            // 
            this.price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.price.HeaderText = "Price";
            this.price.Name = "price";
            this.price.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.price.Width = 37;
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
            this.Period.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Period.HeaderText = "Period";
            this.Period.Name = "Period";
            this.Period.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Period.Width = 43;
            // 
            // onSale
            // 
            this.onSale.FalseValue = "0";
            this.onSale.HeaderText = "Sale";
            this.onSale.IndeterminateValue = "0";
            this.onSale.Name = "onSale";
            this.onSale.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.onSale.TrueValue = "1";
            this.onSale.Width = 40;
            // 
            // gender
            // 
            this.gender.HeaderText = "Gender";
            this.gender.Name = "gender";
            this.gender.Width = 40;
            // 
            // count
            // 
            this.count.HeaderText = "Count";
            this.count.Name = "count";
            this.count.Width = 45;
            // 
            // TextBox
            // 
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.TextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox.Location = new System.Drawing.Point(348, 25);
            this.TextBox.MaxLength = 2147483647;
            this.TextBox.Multiline = true;
            this.TextBox.Name = "TextBox";
            this.TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox.Size = new System.Drawing.Size(142, 284);
            this.TextBox.TabIndex = 2;
            this.TextBox.TextChanged += new System.EventHandler(this.ReplacementContent_TextChanged);
            // 
            // InformationLabel
            // 
            this.InformationLabel.AutoSize = true;
            this.InformationLabel.BackColor = System.Drawing.Color.White;
            this.InformationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InformationLabel.Location = new System.Drawing.Point(3, 2);
            this.InformationLabel.Name = "InformationLabel";
            this.InformationLabel.Size = new System.Drawing.Size(60, 18);
            this.InformationLabel.TabIndex = 4;
            this.InformationLabel.Text = "ur mom";
            this.InformationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Priority
            // 
            this.Priority.HeaderText = "Priorty";
            this.Priority.Name = "Priority";
            // 
            // CItemGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GridView);
            this.Controls.Add(this.InformationLabel);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CItemGroup";
            this.Size = new System.Drawing.Size(490, 309);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal System.Windows.Forms.TextBox TextBox;
        internal DataGridView GridView;
        private Label InformationLabel;
        private DataGridViewImageColumn Image;
        private DataGridViewTextBoxColumn Node;
        private DataGridViewTextBoxColumn ItemID;
        private DataGridViewTextBoxColumn Price;
        private DataGridViewTextBoxColumn IsDonor;
        private DataGridViewTextBoxColumn Period;
        private DataGridViewCheckBoxColumn OnSale;
        private DataGridViewTextBoxColumn Gender;
        private DataGridViewTextBoxColumn Count;
        private DataGridViewTextBoxColumn price;
        private DataGridViewCheckBoxColumn onSale;
        private DataGridViewTextBoxColumn gender;
        private DataGridViewTextBoxColumn count;
        private DataGridViewTextBoxColumn Priority;
    }
}
