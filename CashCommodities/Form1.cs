using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using CashCommodities.Controls;
using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities {
    public partial class Form1 : Form {

        private WzImage commodityImg;

        private int largestNodeValue;

        public Form1() {
            InitializeComponent();
        }

        private void ClearAllData() {
            ItemCategory.SnCache.Clear();

            RegularViewer.ClearData();
            DonorViewer.ClearData();
        }

        private void AddItems(List<WzImageProperty> imgs) {
            RegularViewer.SuspendLayout();
            DonorViewer.SuspendLayout();

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
                    if (node > largestNodeValue) {
                        largestNodeValue = node;
                    }
                }

                var image = GetItemImage(itemID);

                if (onSale) {
                    if (donor) DonorViewer.AddItem(itemID, img, image, CheckBoxLegacy.Checked);
                    else RegularViewer.AddItem(itemID, img, image, false);
                }
            }

            RegularViewer.ResumeLayout();
            DonorViewer.ResumeLayout();
        }

        private Bitmap GetItemImage(int itemID) {
            if (!CheckBoxImages.Checked) return null;

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

        private void LoadFromWz() {
            var mapleVersion = WzTool.DetectMapleVersion("Etc.wz", out var detectVersion);
            short? nVersion = null;
            if (WzTool.GetDecryptionSuccessRate("Etc.wz", mapleVersion, ref nVersion) < 0.8) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var etc = Wz.Etc.LoadFile("Etc", mapleVersion);
            Wz.Character.LoadFile("Character", mapleVersion);
            Wz.Item.LoadFile("Item", mapleVersion);

            commodityImg = etc.WzDirectory.GetImageByName("Commodity.img");
            commodityImg.ParseImage();

            AddItems(commodityImg.WzProperties);
        }

        private void LoadFromImg() {
            var mapleVersion = WzTool.DetectMapleVersion("Data\\Etc", "Commodity.img");
            if (mapleVersion == -1) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var etc = Wz.Etc.LoadFile($"Data\\Etc", (WzMapleVersion)mapleVersion);
            Wz.Character.LoadFile($"Data\\Character", (WzMapleVersion)mapleVersion);
            Wz.Item.LoadFile($"Data\\Item", (WzMapleVersion)mapleVersion);

            commodityImg = etc.WzDirectory.GetImageByName("Commodity.img");
            commodityImg.ParseImage();

            AddItems(commodityImg.WzProperties);
        }

        private void ButtonLoad_Click(object sender, EventArgs e) {
            var etcDataExists = Directory.Exists("Data\\Etc");
            var etcWzExists = File.Exists("Etc.wz");
            if (!etcDataExists && !etcWzExists) {
                MessageBox.Show(Resources.ErrorFileNotFound);
                return;
            }

            Wz.Etc.Dispose();
            Wz.Character.Dispose();
            Wz.Item.Dispose();
            commodityImg?.Dispose();
            commodityImg = null;

            ClearAllData();

            if (etcDataExists && etcWzExists) {
                var result = MessageBox.Show(Resources.ErrorMultipleSources, Resources.ErrorMultipleSourcesTitle, MessageBoxButtons.YesNo);
                if (result == DialogResult.OK) LoadFromImg();
                else LoadFromWz();
            } else if (etcWzExists) LoadFromWz();
            else LoadFromImg();
        }

        private void ButtonSave_Click(object sender, EventArgs e) {
            if (commodityImg == null) {
                MessageBox.Show(Resources.ErrorMessageNothingToSave, Resources.Error);
                return;
            }

            if (File.Exists("Etc.wz")) SaveEtcWz();
            else SaveEtcData();
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

            ButtonLoad_Click(null, null);
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
                } catch (Exception e) {
                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}", "Deletion Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            File.Move(tempFile, dialog.FileName);
            dialog.Dispose();
            MessageBox.Show(Resources.MessageSuccess);

            ButtonLoad_Click(null, null);
        }

        private void UpdateNodes() {
            void ApplyUpdates(TabControl.TabPageCollection pages, bool isDonor) {
                foreach (TabPage tab in pages) {
                    var view = (CItemGroup)tab.Controls[0];
                    var table = view.GridView;
                    var replacements = view.TextBox.Lines;

                    for (var i = 0; i < table.RowCount; i++) {
                        var row = table.Rows[i];
                        var img = (WzImageProperty)row.Tag;

                        {
                            // replace price value
                            var priceObject = row.Cells[3]?.Value;
                            if (priceObject != null) {
                                var priceImg = img.GetFromPath("Price");

                                // convert to string then parse as int due to edited values being a string while original, un-values are still ints
                                var price = int.Parse(row.Cells[3].Value.ToString());
                                if (priceImg == null && price > 0) {
                                    img.ParentImage.Changed = true;
                                    img.WzProperties.Add(new WzIntProperty("Price", price));
                                } else if (priceImg != null && priceImg.GetInt() != price) {
                                    img.ParentImage.Changed = true;
                                    ((WzIntProperty) priceImg).Value = price;
                                }
                            }
                        }

                        {
                            // replace donor value
                            var donorImg = img.GetFromPath("isDonor");
                            var donor = int.Parse(row.Cells[4].Value.ToString()) == 1;
                            if (donorImg == null && donor) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("isDonor", 1));
                            } else if (donorImg != null && (donorImg.GetInt() == 1) != donor) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty) donorImg).Value = donor ? 1 : 0;
                            }
                        }

                        {
                            // replace period value
                            var periodImg = img.GetFromPath("Period");
                            var period = int.Parse(row.Cells[5].Value.ToString());
                            if (periodImg == null && period > 0) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("Period", 1));
                            } else if (periodImg != null && periodImg.GetInt() != period) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty) periodImg).Value = period;
                            }
                        }

                        {
                            // replace OnSale value
                            var saleImg = img.GetFromPath("OnSale");
                            var sale = (bool) row.Cells[6].Value;
                            if (saleImg == null) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("OnSale", sale ? 1 : 0));
                            } else if ((saleImg.GetInt() == 1) != sale) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty)saleImg).Value = sale ? 1 : 0;
                            }
                        }

                        {
                            // replace gender value
                            var genderImg = img.GetFromPath("gender");
                            var gender = int.Parse(row.Cells[7].Value.ToString());
                            if (genderImg == null) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("gender", gender));
                            } else if (genderImg.GetInt() != gender) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty)genderImg).Value = gender;
                            }
                        }

                        {
                            // replace count value
                            var countImg = img.GetFromPath("count");
                            var count = int.Parse(row.Cells[8].Value.ToString());
                            if (countImg == null) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("count", count));
                            } else if (countImg != null && countImg.GetInt() != count) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty) countImg).Value = count;
                            }
                        }
                    }

                    for (var i = 0; i < replacements.Length; i++) {
                        if (!int.TryParse(replacements[i], out var itemId)) {
                            MessageBox.Show($"Invalid ID at line {i}: {replacements[i]}");
                            return;
                        }

                        // replace existing item
                        if (i < table.RowCount) {
                            var row = table.Rows[i];
                            var img = (WzImageProperty)row.Tag;

                            var itemIdImg = img.GetFromPath("ItemId");
                            img.ParentImage.Changed = true;
                            ((WzIntProperty)itemIdImg).Value = itemId;
                        } else {
                            // insert new item
                            var sn = ItemCategory.GenerateSn(isDonor, itemId);

                            var sub = new WzSubProperty((++largestNodeValue).ToString());
                            sub.AddProperty(new WzIntProperty("Count", 1));
                            sub.AddProperty(new WzIntProperty("Gender", 2));
                            sub.AddProperty(new WzIntProperty("ItemId", itemId));
                            sub.AddProperty(new WzIntProperty("OnSale", 1));
                            sub.AddProperty(new WzIntProperty("Period", 90));
                            sub.AddProperty(new WzIntProperty("Price", 4000));
                            sub.AddProperty(new WzIntProperty("Priority", 99));
                            sub.AddProperty(new WzIntProperty("isDonor", isDonor ? 1 : 0));
                            sub.AddProperty(new WzIntProperty("SN", sn));
                            Debug.WriteLine($"Generated SN for item {itemId}: {sn}. Node {sub.Name}");
                            commodityImg.AddProperty(sub);
                            sub.ParentImage.Changed = true;
                        }
                    }
                }
            }

            ApplyUpdates(RegularViewer.tabControl.TabPages, false);
            ApplyUpdates(DonorViewer.tabControl.TabPages, true);
        }
    }
}
