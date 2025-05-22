using CashCommodities.Controls;

namespace CashCommodities {
    partial class MainWindow {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.MainPage = new System.Windows.Forms.TabControl();
            this.MenuRegular = new System.Windows.Forms.TabPage();
            this.RegularViewer = new CashCommodities.Controls.CommodityViewer();
            this.MenuDonor = new System.Windows.Forms.TabPage();
            this.DonorViewer = new CashCommodities.Controls.CommodityViewer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openFile = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.saveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.legacyMode = new System.Windows.Forms.ToolStripMenuItem();
            this.loadImages = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPage.SuspendLayout();
            this.MenuRegular.SuspendLayout();
            this.MenuDonor.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPage
            // 
            this.MainPage.Controls.Add(this.MenuRegular);
            this.MainPage.Controls.Add(this.MenuDonor);
            this.MainPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPage.Location = new System.Drawing.Point(0, 24);
            this.MainPage.Name = "MainPage";
            this.MainPage.Padding = new System.Drawing.Point(0, 0);
            this.MainPage.SelectedIndex = 0;
            this.MainPage.Size = new System.Drawing.Size(694, 437);
            this.MainPage.TabIndex = 4;
            // 
            // MenuRegular
            // 
            this.MenuRegular.Controls.Add(this.RegularViewer);
            this.MenuRegular.Location = new System.Drawing.Point(4, 22);
            this.MenuRegular.Name = "MenuRegular";
            this.MenuRegular.Padding = new System.Windows.Forms.Padding(5);
            this.MenuRegular.Size = new System.Drawing.Size(686, 411);
            this.MenuRegular.TabIndex = 0;
            this.MenuRegular.Text = "Regular";
            this.MenuRegular.UseVisualStyleBackColor = true;
            // 
            // RegularViewer
            // 
            this.RegularViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegularViewer.Location = new System.Drawing.Point(5, 5);
            this.RegularViewer.Name = "RegularViewer";
            this.RegularViewer.Size = new System.Drawing.Size(676, 401);
            this.RegularViewer.TabIndex = 0;
            // 
            // MenuDonor
            // 
            this.MenuDonor.Controls.Add(this.DonorViewer);
            this.MenuDonor.Location = new System.Drawing.Point(4, 22);
            this.MenuDonor.Name = "MenuDonor";
            this.MenuDonor.Padding = new System.Windows.Forms.Padding(5);
            this.MenuDonor.Size = new System.Drawing.Size(686, 411);
            this.MenuDonor.TabIndex = 1;
            this.MenuDonor.Text = "Donor";
            this.MenuDonor.UseVisualStyleBackColor = true;
            // 
            // DonorViewer
            // 
            this.DonorViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DonorViewer.Location = new System.Drawing.Point(5, 5);
            this.DonorViewer.Name = "DonorViewer";
            this.DonorViewer.Size = new System.Drawing.Size(676, 401);
            this.DonorViewer.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(694, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFile,
            this.openFolder,
            this.aToolStripMenuItem,
            this.saveAs});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // openFile
            // 
            this.openFile.Image = global::CashCommodities.Properties.Resources.folder;
            this.openFile.Name = "openFile";
            this.openFile.Size = new System.Drawing.Size(143, 26);
            this.openFile.Text = "Open File";
            this.openFile.Click += new System.EventHandler(this.LoadFromWz);
            // 
            // openFolder
            // 
            this.openFolder.Image = global::CashCommodities.Properties.Resources.folder;
            this.openFolder.Name = "openFolder";
            this.openFolder.Size = new System.Drawing.Size(143, 26);
            this.openFolder.Text = "Open Folder";
            this.openFolder.Click += new System.EventHandler(this.LoadFromImg);
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size(140, 6);
            // 
            // saveAs
            // 
            this.saveAs.Image = global::CashCommodities.Properties.Resources.disk;
            this.saveAs.Name = "saveAs";
            this.saveAs.Size = new System.Drawing.Size(143, 26);
            this.saveAs.Text = "Save As";
            this.saveAs.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.legacyMode,
            this.loadImages});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(61, 20);
            this.toolStripMenuItem2.Text = "Options";
            // 
            // legacyMode
            // 
            this.legacyMode.AutoToolTip = true;
            this.legacyMode.CheckOnClick = true;
            this.legacyMode.Name = "legacyMode";
            this.legacyMode.Size = new System.Drawing.Size(145, 22);
            this.legacyMode.Text = "Legacy Mode";
            this.legacyMode.ToolTipText = "Enable this if your Cash Shop lacks the tabs to \r\nseparate the items into their r" +
    "espective categories.";
            // 
            // loadImages
            // 
            this.loadImages.AutoToolTip = true;
            this.loadImages.CheckOnClick = true;
            this.loadImages.Name = "loadImages";
            this.loadImages.Size = new System.Drawing.Size(145, 22);
            this.loadImages.Text = "Load Images";
            this.loadImages.ToolTipText = "If you to show images for each item displayed when loading cash commodities.\r\nOnl" +
    "y works when opening a file, not when adding new items.";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 461);
            this.Controls.Add(this.MainPage);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Commodity Editor v1.3.1";
            this.MainPage.ResumeLayout(false);
            this.MenuRegular.ResumeLayout(false);
            this.MenuDonor.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl MainPage;
        private System.Windows.Forms.TabPage MenuRegular;
        private System.Windows.Forms.TabPage MenuDonor;
        private CommodityViewer RegularViewer;
        private CommodityViewer DonorViewer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openFile;
        private System.Windows.Forms.ToolStripMenuItem openFolder;
        private System.Windows.Forms.ToolStripSeparator aToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem legacyMode;
        private System.Windows.Forms.ToolStripMenuItem loadImages;
    }
}

