using System.Windows.Forms;

namespace CashCommodities.Controls {
    partial class CashShopItemsControl {
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
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).BeginInit();
            this.SuspendLayout();
            // 
            // GridView
            // 
            this.GridView.AllowUserToAddRows = false;
            this.GridView.AllowUserToDeleteRows = false;
            this.GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.GridView.Location = new System.Drawing.Point(0, 0);
            this.GridView.Name = "GridView";
            this.GridView.RowHeadersWidth = 5;
            this.GridView.RowTemplate.Height = 50;
            this.GridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GridView.Size = new System.Drawing.Size(348, 309);
            this.GridView.TabIndex = 1;
            // 
            // CashShopItemsControl
            // 
            this.Controls.Add(this.GridView);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(490, 309);
            ((System.ComponentModel.ISupportInitialize)(this.GridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        internal DataGridView GridView;
    }
}
