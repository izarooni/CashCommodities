using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using CashCommodities.Controls;
using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities {
    public partial class MainWindow : Form {

        private WzImage commodityImg;

        public static int LastNodeValue { get; set; }
        public bool LoadFromFile { get; set; }
        public bool LegacyMode => legacyMode.Checked;
        public bool LoadImages => loadImages.Checked;

        public MainWindow() {
            InitializeComponent();
        }

        private void ClearAllData() {
            ItemCategory.SnCache.Clear();

            RegularViewer.ClearAllGroups();
            DonorViewer.ClearAllGroups();
        }

        private void AddItems(List<WzImageProperty> imgs) {
            RegularViewer.SuspendLayout();
            DonorViewer.SuspendLayout();

            ClearAllData();

            var items = imgs.Select(i => new CashItem(i)).OrderBy(i => i.Priority).ToList();

            foreach (var item in items) {
                ItemCategory.SnCache.Add(item.SN);

                var node = int.Parse(item.Image.Name);
                if (node > LastNodeValue) {
                    LastNodeValue = node;
                }

                var picture = GetItemPicture(item.ItemId);

                if (item.OnSale) {
                    var view = item.IsDonor ? DonorViewer : RegularViewer;
                    var group = view.GetGroupByItemID(item.ItemId, LegacyMode);
                    view.AddItem(item, picture, item.IsDonor && LegacyMode);
                    // get DataGridView from group
                    var grid = group.Controls.Find("GridView", true)[0] as DataGridView;
                    group.Controls.Find("TextBox", true)[0].Text += $"{(grid.RowCount < 2 ? "" : "\r\n")}{item.ItemId}";
                }
            }

            RegularViewer.ResumeLayout();
            DonorViewer.ResumeLayout();
        }

        private Bitmap GetItemPicture(int itemID) {
            if (!LoadImages) return null;

            var character = Wz.Character.GetFile();
            var item = Wz.Item.GetFile();
            int fileType = itemID / 1000000;

            if (fileType == 1 && character != null) {
                var type = ItemCategory.GetTypeByItemID(itemID);
                if (type == ItemType.UNKNOWN) {
                    Logger.Log($"Couldn't figure out item type, skipping image retrieval: {itemID}");
                    return null;
                }

                var dir = character.WzDirectory[type.ToString()];
                WzImage img = dir[$"{itemID.ToString().PadLeft(8, '0')}.img"] as WzImage;
                if (img == null) return null;

                var icon = (img.GetFromPath("info/iconRaw") ?? img.GetFromPath("info/icon"));
                if (icon == null) icon = img.GetFromPath("default/default");
                return icon?.GetBitmap();
            }

            if (fileType == 5 && item != null) {
                var dir = item.WzDirectory["Pet"];
                WzImage img = dir[$"{itemID}.img"] as WzImage;
                if (img == null) return null;

                var icon = (img.GetFromPath("info/iconRaw") ?? img.GetFromPath("info/icon"));
                return icon?.GetBitmap();
            }

            return null;
        }

        private void LoadFromWz(object sender, EventArgs e) {
            LoadFromFile = true;

            // prompt for file dialoge to load Etc.wz
            var dialog = new OpenFileDialog {
                Filter = "*.wz|*.wz",
                InitialDirectory = Directory.GetCurrentDirectory(),
                CheckFileExists = true,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            var file = dialog.FileName;
            dialog.Dispose();

            var mapleVersion = WzTool.DetectMapleVersion(file, out var detectVersion);
            short? nVersion = null;
            if (WzTool.GetDecryptionSuccessRate(file, mapleVersion, ref nVersion) < 0.8) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            WzFile etc;
            try {
                etc = Wz.Etc.LoadFile(file, mapleVersion);
                commodityImg = etc.WzDirectory.GetImageByName("Commodity.img");
                if (commodityImg == null) {
                    MessageBox.Show("Unable to find Commodity.img or Category.img", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                commodityImg.ParseImage();
                commodityImg.Changed = true;
            } catch (Exception ex) {
                MessageBox.Show($"Unable to open the file: {ex.Message}", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // get Character.wz from same directory
            Wz.Character.LoadFile(Path.Combine(Path.GetDirectoryName(file), "Character"), mapleVersion);
            // get Item.wz from same directory
            Wz.Item.LoadFile(Path.Combine(Path.GetDirectoryName(file), "Item"), mapleVersion);

            AddItems(commodityImg.WzProperties);
        }

        private void LoadFromImg(object sender, EventArgs e) {
            LoadFromFile = false;

            // open folder dialog to select the Etc folder
            var dialog = new FolderBrowserDialog {
                ShowNewFolderButton = false,
                SelectedPath = Directory.GetCurrentDirectory()
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            var file = dialog.SelectedPath;
            dialog.Dispose();

            var mapleVersion = WzTool.DetectMapleVersion($"{file}\\Etc", "Commodity.img");
            if (mapleVersion == -1) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var etc = Wz.Etc.LoadFile($"{file}\\Etc", (WzMapleVersion)mapleVersion);
            Wz.Character.LoadFile($"{file}\\Character", (WzMapleVersion)mapleVersion);
            Wz.Item.LoadFile($"{file}\\Item", (WzMapleVersion)mapleVersion);

            commodityImg = etc.WzDirectory.GetImageByName("Commodity.img");
            if (commodityImg == null) {
                MessageBox.Show("Unable to find Commodity.img or Category.img", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            commodityImg.ParseImage();
            commodityImg.Changed = true;

            AddItems(commodityImg.WzProperties);
        }
        private void ButtonSave_Click(object sender, EventArgs e) {
            if (commodityImg == null) {
                MessageBox.Show(Resources.ErrorMessageNothingToSave, Resources.Error);
                return;
            }

            if (LoadFromFile) SaveEtcWz();
            else SaveEtcData();

            ClearAllData();
        }

        private void SaveEtcData() {
            var dialog = new SaveFileDialog {
                FileName = "Commodity.img",
                Filter = Resources.FileSaveImgExt
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            UpdateNodes();

            var temp = Path.GetTempFileName();
            using (var stream = File.Create(temp)) {
                using (var w = new WzBinaryWriter(stream, commodityImg.WzFileParent.MapleVersion, false)) {
                    commodityImg.SaveImage(w);
                }
            }
            commodityImg.Dispose();
            commodityImg = null;

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch {
                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            File.Move(temp, dialog.FileName);
            dialog.Dispose();
            MessageBox.Show(Resources.MessageSuccess);
        }

        private void SaveEtcWz() {
            var dialog = new SaveFileDialog {
                FileName = "Etc.wz",
                Filter = Resources.FileSaveWzExt
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            UpdateNodes();

            var tempFile = Path.GetTempFileName();
            var etc = Wz.Etc.GetFile();
            etc.SaveToDisk(tempFile);

            Wz.Etc.Dispose();
            Wz.Character.Dispose();
            Wz.Item.Dispose();

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch (Exception) {
                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}. Using name {tempFile} instead.", "Deletion Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Path.ChangeExtension(tempFile, ".wz");
                    var path = Path.GetDirectoryName(dialog.FileName);
                    File.Move(tempFile, Path.Combine(path, Path.GetFileName(tempFile)));
                    return;
                }
            }
            File.Move(tempFile, dialog.FileName);
            dialog.Dispose();
            MessageBox.Show(Resources.MessageSuccess);
        }

        private void UpdateNodes() {
            void ApplyUpdates(TabControl.TabPageCollection pages, bool isDonor) {
                foreach (TabPage tab in pages) {
                    var view = (CItemGroup)tab.Controls[0];
                    var table = view.GridView;

                    WzImageProperty img;

                    foreach (var item in view.RemoveQueue) {
                        commodityImg[item.Node]?.Remove();
                    }

                    for (var i = 0; i < table.RowCount; i++) {
                        var row = table.Rows[i];
                        var item = (CashItem)row.Tag;
                        var itemProp = item.Image as WzImageProperty;

                        var sn = ItemCategory.GenerateSn(isDonor, item.ItemId);

                        if (itemProp == null) {
                            // insert new item
                            var sub = new WzSubProperty(item.Node);
                            sub.AddProperty(new WzIntProperty("Count", item.Count));
                            sub.AddProperty(new WzIntProperty("Gender", item.Gender));
                            sub.AddProperty(new WzIntProperty("ItemId", item.ItemId));
                            sub.AddProperty(new WzIntProperty("OnSale", item.OnSale ? 1 : 0));
                            sub.AddProperty(new WzIntProperty("Period", item.Period));
                            sub.AddProperty(new WzIntProperty("Price", item.Price));
                            sub.AddProperty(new WzIntProperty("Priority", item.Priority));
                            sub.AddProperty(new WzIntProperty("Class", (int)item.Class));
                            sub.AddProperty(new WzIntProperty("SN", sn));

                            Debug.WriteLine($"Generated SN for item {item.ItemId}: {sn}. Node {sub.Name}");

                            commodityImg.AddProperty(sub);
                            sub.ParentImage.Changed = true;
                            continue;
                        }

                        img = itemProp.GetFromPath("isDonor");
                        img?.Remove();

                        img = itemProp.GetFromPath("Price");
                        if (img == null && item.Price > 0) {
                            // create if not exists
                            itemProp.WzProperties.Add(new WzIntProperty("Price", item.Price));
                        } else if (img != null && img.GetInt() != item.Price) {
                            // values don't match so marked it as changed
                            ((WzIntProperty)img).Value = item.Price;
                        }

                        img = itemProp.GetFromPath("Period");
                        if (img == null && item.Period > 0) {
                            // create if not exists
                            itemProp.WzProperties.Add(new WzIntProperty("Period", 98));
                        } else if (img != null && img.GetInt() != item.Period) {
                            ((WzIntProperty)img).Value = item.Period;
                        }

                        img = itemProp.GetFromPath("OnSale");
                        if (img == null) {
                            // create if not exists
                            itemProp.WzProperties.Add(new WzIntProperty("OnSale", item.OnSale ? 1 : 0));
                        } else if ((img.GetInt() == 1) != item.OnSale) {
                            ((WzIntProperty)img).Value = item.OnSale ? 1 : 0;
                        }

                        // replace gender value
                        img = itemProp.GetFromPath("gender");
                        if (img == null) {
                            itemProp.WzProperties.Add(new WzIntProperty("gender", item.Gender));
                        } else if (img.GetInt() != item.Gender) {
                            ((WzIntProperty)img).Value = item.Gender;
                        }

                        // replace count value
                        img = itemProp.GetFromPath("count");
                        if (img == null) {
                            itemProp.WzProperties.Add(new WzIntProperty("count", item.Count));
                        } else if (img != null && img.GetInt() != item.Count) {
                            ((WzIntProperty)img).Value = item.Count;
                        }

                        // replace priority value
                        img = itemProp.GetFromPath("Priority");
                        if (img == null) {
                            itemProp.WzProperties.Add(new WzIntProperty("Priority", item.Priority));
                        } else if (img != null) {
                            ((WzIntProperty)img).Value = item.Priority;
                        }

                        img = itemProp.GetFromPath("Class");
                        if (img == null) {
                            itemProp.WzProperties.Add(new WzIntProperty("Class", (int)item.Class));
                        } else if (img is WzIntProperty classImg) {
                            if (classImg.Value == (int)ClassType.None) img.Remove();
                            else classImg.Value = (int)item.Class;
                        }
                    }
                }
            }

            ApplyUpdates(RegularViewer.tabControl.TabPages, false);
            ApplyUpdates(DonorViewer.tabControl.TabPages, true);
        }
    }
}
