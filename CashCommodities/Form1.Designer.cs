using CashCommodities.Controls;

namespace CashCommodities {
    partial class Form1 {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.MenuStrip = new System.Windows.Forms.ToolStrip();
            this.ButtonLoad = new System.Windows.Forms.ToolStripButton();
            this.ButtonSave = new System.Windows.Forms.ToolStripButton();
            this.MainPage = new System.Windows.Forms.TabControl();
            this.MenuRegular = new System.Windows.Forms.TabPage();
            this.MenuDonor = new System.Windows.Forms.TabPage();
            this.RegularViewer = new CashCommodities.Controls.CommodityViewer();
            this.DonorViewer = new CashCommodities.Controls.CommodityViewer();
            this.MenuStrip.SuspendLayout();
            this.MainPage.SuspendLayout();
            this.MenuRegular.SuspendLayout();
            this.MenuDonor.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonLoad,
            this.ButtonSave});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(644, 25);
            this.MenuStrip.TabIndex = 3;
            this.MenuStrip.Text = "Menu";
            // 
            // ButtonLoad
            // 
            this.ButtonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ButtonLoad.Image = global::CashCommodities.Properties.Resources.folder;
            this.ButtonLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonLoad.Name = "ButtonLoad";
            this.ButtonLoad.Size = new System.Drawing.Size(23, 22);
            this.ButtonLoad.Text = "Load";
            this.ButtonLoad.Click += new System.EventHandler(this.ButtonLoad_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ButtonSave.Image = global::CashCommodities.Properties.Resources.disk;
            this.ButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(23, 22);
            this.ButtonSave.Text = "Save";
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // MainPage
            // 
            this.MainPage.Controls.Add(this.MenuRegular);
            this.MainPage.Controls.Add(this.MenuDonor);
            this.MainPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPage.Location = new System.Drawing.Point(0, 25);
            this.MainPage.Name = "MainPage";
            this.MainPage.Padding = new System.Drawing.Point(0, 0);
            this.MainPage.SelectedIndex = 0;
            this.MainPage.Size = new System.Drawing.Size(644, 416);
            this.MainPage.TabIndex = 4;
            // 
            // MenuRegular
            // 
            this.MenuRegular.Controls.Add(this.RegularViewer);
            this.MenuRegular.Location = new System.Drawing.Point(4, 22);
            this.MenuRegular.Name = "MenuRegular";
            this.MenuRegular.Padding = new System.Windows.Forms.Padding(5);
            this.MenuRegular.Size = new System.Drawing.Size(636, 390);
            this.MenuRegular.TabIndex = 0;
            this.MenuRegular.Text = "Regular";
            this.MenuRegular.UseVisualStyleBackColor = true;
            // 
            // MenuDonor
            // 
            this.MenuDonor.Controls.Add(this.DonorViewer);
            this.MenuDonor.Location = new System.Drawing.Point(4, 22);
            this.MenuDonor.Name = "MenuDonor";
            this.MenuDonor.Padding = new System.Windows.Forms.Padding(5);
            this.MenuDonor.Size = new System.Drawing.Size(636, 390);
            this.MenuDonor.TabIndex = 1;
            this.MenuDonor.Text = "Donor";
            this.MenuDonor.UseVisualStyleBackColor = true;
            // 
            // RegularViewer
            // 
            this.RegularViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegularViewer.Location = new System.Drawing.Point(5, 5);
            this.RegularViewer.Name = "RegularViewer";
            this.RegularViewer.Size = new System.Drawing.Size(626, 380);
            this.RegularViewer.TabIndex = 0;
            // 
            // DonorViewer
            // 
            this.DonorViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DonorViewer.Location = new System.Drawing.Point(5, 5);
            this.DonorViewer.Name = "DonorViewer";
            this.DonorViewer.Size = new System.Drawing.Size(626, 380);
            this.DonorViewer.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 441);
            this.Controls.Add(this.MainPage);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Commodity Editor";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.MainPage.ResumeLayout(false);
            this.MenuRegular.ResumeLayout(false);
            this.MenuDonor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip MenuStrip;
        private System.Windows.Forms.ToolStripButton ButtonLoad;
        private System.Windows.Forms.ToolStripButton ButtonSave;
        private System.Windows.Forms.TabControl MainPage;
        private System.Windows.Forms.TabPage MenuRegular;
        private System.Windows.Forms.TabPage MenuDonor;
        private CommodityViewer RegularViewer;
        private CommodityViewer DonorViewer;
    }
}

