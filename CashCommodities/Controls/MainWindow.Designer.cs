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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileDropdownMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOpenFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileDivider1MenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.fileSaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsDropdownMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsCreateCategoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsLoadPicturesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainPage = new System.Windows.Forms.TabControl();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileDropdownMenu, this.optionsDropdownMenu
            });
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(644, 24);
            this.mainMenuStrip.TabIndex = 7;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // fileDropdownMenu
            // 
            this.fileDropdownMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.fileOpenMenuItem, this.fileOpenFolderMenuItem,
                this.fileDivider1MenuItem, this.fileSaveAsMenuItem
            });
            this.fileDropdownMenu.Name = "fileDropdownMenu";
            this.fileDropdownMenu.Size = new System.Drawing.Size(37, 20);
            this.fileDropdownMenu.Text = "File";
            // 
            // fileOpenMenuItem
            // 
            this.fileOpenMenuItem.Image = global::CashCommodities.Properties.Resources.folder;
            this.fileOpenMenuItem.Name = "fileOpenMenuItem";
            this.fileOpenMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fileOpenMenuItem.Text = "Open File";
            this.fileOpenMenuItem.Click += new System.EventHandler(this.LoadFromWz);
            // 
            // fileOpenFolderMenuItem
            // 
            this.fileOpenFolderMenuItem.Image = global::CashCommodities.Properties.Resources.folder;
            this.fileOpenFolderMenuItem.Name = "fileOpenFolderMenuItem";
            this.fileOpenFolderMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fileOpenFolderMenuItem.Text = "Open Folder";
            this.fileOpenFolderMenuItem.Click += new System.EventHandler(this.LoadFromImg);
            // 
            // fileDivider1MenuItem
            // 
            this.fileDivider1MenuItem.Name = "fileDivider1MenuItem";
            this.fileDivider1MenuItem.Size = new System.Drawing.Size(149, 6);
            // 
            // fileSaveAsMenuItem
            // 
            this.fileSaveAsMenuItem.Image = global::CashCommodities.Properties.Resources.disk;
            this.fileSaveAsMenuItem.Name = "fileSaveAsMenuItem";
            this.fileSaveAsMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fileSaveAsMenuItem.Text = "Save As";
            this.fileSaveAsMenuItem.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // optionsDropdownMenu
            // 
            this.optionsDropdownMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.optionsCreateCategoryMenuItem, this.optionsLoadPicturesMenuItem
            });
            this.optionsDropdownMenu.Name = "optionsDropdownMenu";
            this.optionsDropdownMenu.Size = new System.Drawing.Size(61, 20);
            this.optionsDropdownMenu.Text = "Options";
            // 
            // optionsCreateCategoryMenuItem
            // 
            this.optionsCreateCategoryMenuItem.AutoToolTip = true;
            this.optionsCreateCategoryMenuItem.CheckOnClick = true;
            this.optionsCreateCategoryMenuItem.Name = "optionsCreateCategoryMenuItem";
            this.optionsCreateCategoryMenuItem.Size = new System.Drawing.Size(187, 22);
            this.optionsCreateCategoryMenuItem.Text = "Create CashShopTabs";
            this.optionsCreateCategoryMenuItem.ToolTipText = "Disable if you don\'t want categories created under \'Event\' tab";
            this.optionsCreateCategoryMenuItem.Click += new System.EventHandler(this.OptionsCreateCategoryMenuItem_Click);
            // 
            // optionsLoadPicturesMenuItem
            // 
            this.optionsLoadPicturesMenuItem.AutoToolTip = true;
            this.optionsLoadPicturesMenuItem.CheckOnClick = true;
            this.optionsLoadPicturesMenuItem.Name = "optionsLoadPicturesMenuItem";
            this.optionsLoadPicturesMenuItem.Size = new System.Drawing.Size(187, 22);
            this.optionsLoadPicturesMenuItem.Text = "Load Images";
            this.optionsLoadPicturesMenuItem.ToolTipText = "If you to show images for each item displayed when loading cash commodities.\r\nOnl" + "y works when opening a file, not when adding new items.";
            // 
            // mainPage
            // 
            this.mainPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPage.Location = new System.Drawing.Point(0, 24);
            this.mainPage.Name = "mainPage";
            this.mainPage.Padding = new System.Drawing.Point(0, 0);
            this.mainPage.SelectedIndex = 0;
            this.mainPage.Size = new System.Drawing.Size(644, 417);
            this.mainPage.TabIndex = 4;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 441);
            this.Controls.Add(this.mainPage);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Commodity Editor v2";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileDropdownMenu;
        private System.Windows.Forms.ToolStripMenuItem fileOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOpenFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileDivider1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsDropdownMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsCreateCategoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsLoadPicturesMenuItem;
        private System.Windows.Forms.TabControl mainPage;
    }
}

