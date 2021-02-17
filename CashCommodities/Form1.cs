using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

using CashCommodities.Controls;
using CashCommodities.Properties;

using MapleLib.WzLib;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities {
    public partial class Form1 : Form {

        private WzFile etcFile;
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
                var onSale = img.GetFromPath("OnSale")?.GetInt() == 1;
                var donor = img.GetFromPath("isDonor")?.GetInt() > 0;
                var snImg = img.GetFromPath("SN");
                if (snImg != null) {
                    ItemCategory.SnCache.Add(snImg.GetInt());

                    var node = int.Parse(img.Name);
                    if (node > largestNodeValue) {
                        largestNodeValue = node;
                    }
                }

                if (onSale) {
                    if (donor) DonorViewer.AddItem(img);
                    else RegularViewer.AddItem(img);
                }
            }

            RegularViewer.ResumeLayout();
            DonorViewer.ResumeLayout();
        }

        private void LoadFromWz() {
            var mapleVersion = WzTool.DetectMapleVersion(Resources.FileWzEtc, out var detectVersion);
            short? nVersion = null;
            if (WzTool.GetDecryptionSuccessRate(Resources.FileWzEtc, mapleVersion, ref nVersion) < 0.8) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            etcFile = new WzFile(Resources.FileWzEtc, mapleVersion);
            etcFile.ParseWzFile();

            commodityImg = etcFile.WzDirectory.GetImageByName("Commodity.img");
            commodityImg.ParseImage();

            AddItems(commodityImg.WzProperties);

        }

        private void LoadFromImg() {
            using (Stream stream = new FileStream(Resources.FileImgEtc, FileMode.Open, FileAccess.ReadWrite)) {
                commodityImg = new WzImage("Commodity.img", stream, WzMapleVersion.GMS);
                commodityImg.ParseImage();

                AddItems(commodityImg.WzProperties);
            }
        }

        private void ButtonLoad_Click(object sender, EventArgs e) {
            var etcDataExists = File.Exists(Resources.FileImgEtc);
            var etcWzExists = File.Exists(Resources.FileWzEtc);
            if (!etcDataExists && !etcWzExists) {
                MessageBox.Show(Resources.ErrorFileNotFound);
                return;
            }

            if (etcFile != null || commodityImg != null) {
                etcFile?.Dispose();
                etcFile = null;

                commodityImg?.Dispose();
                commodityImg = null;
            }

            ClearAllData();

            if (etcDataExists && etcWzExists) {
                var result = MessageBox.Show(Resources.ErrorMultipleSources, Resources.ErrorMultipleSourcesTitle, MessageBoxButtons.YesNo);
                if (result == DialogResult.OK) LoadFromImg();
                else LoadFromWz();
            } else if (etcWzExists) LoadFromWz();
            else LoadFromImg();

            Debug.WriteLine("Loaded {0} SNs", ItemCategory.SnCache.Count);
        }

        private void ButtonSave_Click(object sender, EventArgs e) {
            if (etcFile != null) SaveEtcWz();
            else if (commodityImg != null) SaveEtcData();
            else MessageBox.Show(Resources.ErrorFileNotFound);
        }

        private void SaveEtcData() {
            var dialog = new SaveFileDialog {
                FileName = "Commodity.img",
                Filter = Resources.FileSaveImgExt
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch {
                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}", "Deletion Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            UpdateNodes();

            var temp = Path.GetTempFileName();
            using (var stream = File.Create(temp)) {
                using (var w = new WzBinaryWriter(stream, WzMapleVersion.EMS, false)) {
                    commodityImg.SaveImage(w);
                }
            }
            commodityImg.Dispose();
            commodityImg = null;

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
            etcFile.SaveToDisk(tempFile);
            etcFile.Dispose();
            etcFile = null;

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch {
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
            void ApplyUpdates(TabControl.TabPageCollection pages) {
                foreach (TabPage tab in pages) {
                    var view = (CItemGroup) tab.Controls[0];
                    var table = view.GridView;
                    var replacements = view.TextBox.Lines;

                    for (var i = 0; i < table.RowCount; i++) {
                        var row = table.Rows[i];
                        var img = (WzImageProperty) row.Tag;

                        // replace price value
                        var priceObject = row.Cells[2]?.Value;
                        if (priceObject != null) {
                            var priceImg = img.GetFromPath("Price");

                            // convert to string then parse as int due to edited values being a string while original, un-values are still ints
                            var price = int.Parse(row.Cells[2].Value.ToString());
                            if (priceImg == null && price > 0) {
                                img.ParentImage.Changed = true;
                                img.WzProperties.Add(new WzIntProperty("Price", price));
                            } else if (priceImg != null && priceImg.GetInt() != price) {
                                img.ParentImage.Changed = true;
                                ((WzIntProperty) priceImg).Value = price;
                            }
                        }

                        // replace donor value
                        var donorImg = img.GetFromPath("isDonor");
                        var donor = int.Parse(row.Cells[3].Value.ToString()) == 1;
                        if (donorImg == null && donor) {
                            img.ParentImage.Changed = true;
                            img.WzProperties.Add(new WzIntProperty("isDonor", 1));
                        } else if (donorImg != null && (donorImg.GetInt() == 1) != donor) {
                            img.ParentImage.Changed = true;
                            ((WzIntProperty) donorImg).Value = donor ? 1 : 0;
                        }
                        
                        // replace period value
                        var periodImg = img.GetFromPath("Period");
                        var period = int.Parse(row.Cells[4].Value.ToString());
                        if (periodImg == null && period > 0) {
                            img.ParentImage.Changed = true;
                            img.WzProperties.Add(new WzIntProperty("Period", 1));
                        } else if (periodImg != null && periodImg.GetInt() != period) {
                            img.ParentImage.Changed = true;
                            ((WzIntProperty) periodImg).Value = period;
                        }
                    }

                    for (var i = 0; i < replacements.Length; i++) {
                        if (!int.TryParse(replacements[i], out var itemId)) {
                            MessageBox.Show(string.Format("Invalid ID at line {0}: {1}", i, replacements[i]));
                            return;
                        }

                        // replace existing item
                        if (i < table.RowCount) {
                            var row = table.Rows[i];
                            var img = (WzImageProperty) row.Tag;

                            var itemIdImg = img.GetFromPath("ItemId");
                            img.ParentImage.Changed = true;
                            ((WzIntProperty) itemIdImg).Value = itemId;
                        } else {
                            // insert new item
                            var isDonor = MainPage.SelectedIndex == 1;
                            var sn = ItemCategory.GenerateSn(isDonor, itemId);

                            var sub = new WzSubProperty((++largestNodeValue).ToString());
                            sub.AddProperty(new WzIntProperty("Count", 1));
                            sub.AddProperty(new WzIntProperty("Gender", 2));
                            sub.AddProperty(new WzIntProperty("ItemId", itemId));
                            sub.AddProperty(new WzIntProperty("OnSale", 1));
                            sub.AddProperty(new WzIntProperty("Period", 90));
                            sub.AddProperty(new WzIntProperty("Price", 1));
                            sub.AddProperty(new WzIntProperty("Priority", 99));
                            sub.AddProperty(new WzIntProperty("isDonor", isDonor ? 1 : 0));
                            sub.AddProperty(new WzIntProperty("SN", sn));
                            Debug.WriteLine("Generated SN for item {0}: {1}. Node {2}", itemId, sn, sub.Name);
                            commodityImg.AddProperty(sub);
                            sub.ParentImage.Changed = true;
                        }
                    }
                }
            }

            ApplyUpdates(RegularViewer.tabControl.TabPages);
            ApplyUpdates(DonorViewer.tabControl.TabPages);
        }

        private void DonorViewer_Load(object sender, EventArgs e) {

        }
    }
}
