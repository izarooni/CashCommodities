using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

using CashCommodities.Controls;
using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities {
    public partial class MainForm : Form {

        private WzImage commodityImg;

        public static int LastNodeValue { get; set; }
        public bool LoadFromFile { get; set; }
        public bool LegacyMode => legacyMode.Checked;
        public bool LoadImages => loadImages.Checked;

        public MainForm() {
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

            RegularViewer.ForEachGroup(cig => cig.TextBox.Text = "");
            DonorViewer.ForEachGroup(cig => cig.TextBox.Text = "");

            foreach (var img in imgs) {
                var itemID = img.GetFromPath("ItemId").GetInt();
                var onSale = img.GetFromPath("OnSale")?.GetInt() == 1;
                var donor = img.GetFromPath("isDonor")?.GetInt() > 0;

                var snImg = img.GetFromPath("SN");
                if (snImg != null) {
                    var nSN = snImg.GetInt();
                    ItemCategory.SnCache.Add(nSN);
                    if (nSN / 10000000 == 1) donor = true;

                    var node = int.Parse(img.Name);
                    if (node > LastNodeValue) {
                        LastNodeValue = node;
                    }
                }

                var image = GetItemImage(itemID);

                if (onSale) {
                    var view = donor ? DonorViewer : RegularViewer;
                    var group = view.GetGroupByItemID(itemID, LegacyMode);
                    view.AddItem(itemID, img, image, donor && LegacyMode);
                    // get DataGridView from group
                    var grid = group.Controls.Find("GridView", true)[0] as DataGridView;
                    group.Controls.Find("TextBox", true)[0].Text += $"{(grid.RowCount < 2 ? "" : "\r\n")}{itemID}";
                }
            }

            RegularViewer.ResumeLayout();
            DonorViewer.ResumeLayout();
        }

        private Bitmap GetItemImage(int itemID) {
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

                    for (var i = 0; i < table.RowCount; i++) {
                        var row = table.Rows[i];
                        var img = (WzImageProperty)row.Tag;

                        var image = (Bitmap)row.Cells[0].Value;
                        var node = row.Cells[1].Value;
                        var itemId = int.Parse(row.Cells[2].Value.ToString());
                        var price = int.Parse(row.Cells[3].Value.ToString());
                        var period = int.Parse(row.Cells[4].Value.ToString());
                        var sale = (bool)row.Cells[5].Value;
                        var gender = int.Parse(row.Cells[6].Value.ToString());
                        var count = int.Parse(row.Cells[7].Value.ToString());
                        var priority = int.Parse(row.Cells[8].Value.ToString());
                        var sn = ItemCategory.GenerateSn(isDonor, itemId);

                        if (img == null) {
                            // insert new item
                            var sub = new WzSubProperty(node.ToString());
                            sub.AddProperty(new WzIntProperty("Count", count));
                            sub.AddProperty(new WzIntProperty("Gender", gender));
                            sub.AddProperty(new WzIntProperty("ItemId", itemId));
                            sub.AddProperty(new WzIntProperty("OnSale", sale ? 1 : 0));
                            sub.AddProperty(new WzIntProperty("Period", period));
                            sub.AddProperty(new WzIntProperty("Price", price));
                            sub.AddProperty(new WzIntProperty("Priority", priority));
                            sub.AddProperty(new WzIntProperty("isDonor", isDonor ? 1 : 0));
                            sub.AddProperty(new WzIntProperty("SN", sn));

                            Debug.WriteLine($"Generated SN for item {itemId}: {sn}. Node {sub.Name}");

                            commodityImg.AddProperty(sub);
                            sub.ParentImage.Changed = true;
                            continue;
                        }

                        var priceImg = img.GetFromPath("Price");
                        if (priceImg == null && price > 0) {
                            // create if not exists
                            img.WzProperties.Add(new WzIntProperty("Price", price));
                        } else if (priceImg != null && priceImg.GetInt() != price) {
                            // values don't match so marked it as changed
                            ((WzIntProperty)priceImg).Value = price;
                        }

                        var donorImg = img.GetFromPath("isDonor");
                        if (donorImg == null && isDonor) {
                            // create if not exists
                            img.WzProperties.Add(new WzIntProperty("isDonor", 1));
                        } else if (donorImg != null) {
                            // force donor to 1 if it's in the donor tab
                            ((WzIntProperty)donorImg).Value = isDonor ? 1 : 0;
                        }

                        var periodImg = img.GetFromPath("Period");
                        if (periodImg == null && period > 0) {
                            // create if not exists
                            img.WzProperties.Add(new WzIntProperty("Period", 98));
                        } else if (periodImg != null && periodImg.GetInt() != period) {
                            ((WzIntProperty)periodImg).Value = period;
                        }

                        {
                            var saleImg = img.GetFromPath("OnSale");
                            if (saleImg == null) {
                                // create if not exists
                                img.WzProperties.Add(new WzIntProperty("OnSale", sale ? 1 : 0));
                            } else if ((saleImg.GetInt() == 1) != sale) {
                                ((WzIntProperty)saleImg).Value = sale ? 1 : 0;
                            }
                        }

                        // replace gender value
                        var genderImg = img.GetFromPath("gender");
                        if (genderImg == null) {
                            img.WzProperties.Add(new WzIntProperty("gender", gender));
                        } else if (genderImg.GetInt() != gender) {
                            ((WzIntProperty)genderImg).Value = gender;
                        }

                        // replace count value
                        var countImg = img.GetFromPath("count");
                        if (countImg == null) {
                            img.WzProperties.Add(new WzIntProperty("count", count));
                        } else if (countImg != null && countImg.GetInt() != count) {
                            ((WzIntProperty)countImg).Value = count;
                        }

                        // replace priority value
                        var priorityImg = img.GetFromPath("Priority");
                        if (priorityImg == null) {
                            img.WzProperties.Add(new WzIntProperty("Priority", priority));
                        } else if (priorityImg != null) {
                            // replace old ones with new ones which push them back on the list
                            // keeping new content at the front
                            if (priorityImg.GetInt() == 99) {
                                priority = 98;
                            } else if (priorityImg.GetInt() == priority) {
                                continue;
                            }
                            ((WzIntProperty)priorityImg).Value = priority;
                        }
                    }
                }
            }

            ApplyUpdates(RegularViewer.tabControl.TabPages, false);
            ApplyUpdates(DonorViewer.tabControl.TabPages, true);
        }
    }
}
