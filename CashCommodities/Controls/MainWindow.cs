using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using CashCommodities.Controls;
using CashCommodities.Properties;
using MapleLib.WzLib;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace CashCommodities {
    public partial class MainWindow : Form {
        public MainWindow() {
            InitializeComponent();

            // create main cash shop tabs
            var categories = Enum.GetValues(typeof(CashShopTabType)).Cast<CashShopTabType>();
            foreach (var category in categories) {
                var tab = new CashShopTabControl() {
                    Dock = DockStyle.Fill,
                    Name = Enum.GetName(typeof(CashShopTabType), category),
                    Text = Enum.GetName(typeof(CashShopTabType), category),
                    CashShopMainTab = category,
                };
                mainPage.TabPages.Add(tab);
                CashShopTabs.Add(category, tab);
            }
        }
        private WzFile EtcFile { get; set; }
        public bool LoadFromFile { get; set; }
        public bool CreateCategories {
            get {
                return optionsCreateCategoryMenuItem.Checked;
            }
        }
        public bool LoadPictures {
            get {
                return optionsLoadPicturesMenuItem.Checked;
            }
        }
        internal Dictionary<CashShopTabType, CashShopTabControl> CashShopTabs { get; } = new Dictionary<CashShopTabType, CashShopTabControl>();

        private void LoadCommodities() {
            var commodityImg = EtcFile.WzDirectory["Commodity.img"] as WzImage;
            var categoryImg = EtcFile.WzDirectory["Category.img"] as WzImage;

            if (commodityImg == null || categoryImg == null) {
                MessageBox.Show("Unable to find Commodity.img or Category.img", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            commodityImg.ParseImage();
            categoryImg.ParseImage();

            foreach (var cashTab in CashShopTabs.Values) {
                cashTab.Clear();
            }

            // retrieve all subcategories from Category.img
            // subcategories are paired with main categories in datasets therefore
            // 1. main categories must only be created once
            // 2. subcategories must be added to the main category
            // 3. there cannot be duplicate main or subcategories
            foreach (var img in categoryImg.WzProperties) {
                // bug fix: Category 8 (Main) is a WzStringProperty while all 7 other categories are WzIntProperty
                var iProp = img["Category"] as WzIntProperty;
                CashShopTabType tabType;

                if (iProp != null)
                    tabType = (CashShopTabType)iProp.Value;
                else {
                    var sProp = img["category"] as WzStringProperty;
                    tabType = (CashShopTabType)int.Parse(sProp.Value);
                }

                CashShopTabs.TryGetValue(tabType, out var cashTab);
                if (cashTab == null) {
                    Logger.Log($"[MainWindow] Unable to find category {tabType}");
                    continue;
                }

                cashTab.AddSubCategory(img["CategorySub"].GetInt(), new CashShopItemsControl {
                    Dock = DockStyle.Fill,
                    Name = img["Name"].GetString(),
                    Text = img["Name"].GetString(),
                });
            }

            // load all item commodities and distribute them to their respective categories
            foreach (var img in commodityImg.WzProperties) {
                var item = new CommodityImage(img);
                if (item.Priority == 99) item.Priority = 98; // move down so newer items can be shown first
                item.Picture = GetCommodityPicture(item.ItemId);
                ItemCategory.SnCache.Add(item.SN);
                if (int.Parse(item.Node) > ItemCategory.IncrementingNode) {
                    ItemCategory.IncrementingNode = int.Parse(item.Node);
                }

                int cashTabIndex = item.SN / 10000000;
                var cashTab = (CashShopTabType)cashTabIndex;

                switch (cashTab) {
                    case CashShopTabType.Event:
                    case CashShopTabType.Equip:
                    case CashShopTabType.Use:
                    case CashShopTabType.SetUp:
                    case CashShopTabType.Etc:
                    case CashShopTabType.Pet:
                    case CashShopTabType.Package:
                    case CashShopTabType.Main:
                        if (!CashShopTabs[(CashShopTabType)cashTabIndex].AddItem(item)) {
                            Logger.Log($"Unable to find subcategory for item {item}");
                        }
                        break;
                    default:
                        Logger.Log($"Unable to find category for item {item}");
                        continue;
                }
            }
        }

        public Bitmap GetCommodityPicture(int itemId) {
            if (!LoadPictures) return null;

            var character = Wz.Character.GetFile();
            var item = Wz.Item.GetFile();

            var itemCategory = itemId / 10000;
            var itemType = ItemCategory.GetItemTypeByItemId(itemId);
            var equipType = ItemCategory.GetEquipTypeByItemId(itemId);

            if (itemType != ItemType.UNKNOWN) {
                if (item == null) return null;
                var dir = item.WzDirectory[itemType.ToString()];
                if (dir == null) return null;

                switch (itemType) {
                    case ItemType.Special: {
                        var img = dir[$"{itemCategory.ToString().PadLeft(4, '0')}.img"] as WzImage;
                        if (img == null) return null;
                        var node = img[itemId.ToString()];
                        if (node == null) return null;
                        return node["icon"]?.GetBitmap();
                    }
                    case ItemType.Pet: {
                        var img = dir[$"{itemId.ToString().PadLeft(7, '0')}.img"] as WzImage;
                        if (img == null) return null;
                        var icon = img["info"]["iconRaw"] ?? img["info"]["icon"];
                        return icon?.GetBitmap();
                    }
                    case ItemType.Etc:
                    case ItemType.Install:
                    case ItemType.Consume:
                    case ItemType.Cash: {
                        var img = dir[$"{itemCategory.ToString().PadLeft(4, '0')}.img"] as WzImage;
                        if (img == null) return null;
                        var node = img[itemId.ToString().PadLeft(8, '0')] as WzSubProperty;
                        if (node == null) return null;
                        var icon = node["info"]["iconRaw"] ?? node["info"]["icon"];
                        return icon?.GetBitmap();
                    }
                }
            } else if (equipType != EquipType.UNKNOWN) {
                if (character == null) return null;

                var dir = character.WzDirectory[equipType.ToString()];
                WzImage img = dir[$"{itemId.ToString().PadLeft(8, '0')}.img"] as WzImage;
                if (img == null) return null;

                var icon = img["info"]["iconRaw"] ?? img["info"]["icon"];
                if (icon == null) icon = img["default"]["default"];
                return icon?.GetBitmap();
            }

            Logger.Log($"Unable to find image for item {itemId}");
            return null;
        }

        #region IO Operations
        private void LoadFromWz(object sender, EventArgs e) {
            LoadFromFile = true;

            // prompt for file dialoge to load Etc.wz
            var dialog = new OpenFileDialog {
                Filter = "*.wz|*.wz",
                InitialDirectory = Directory.GetCurrentDirectory(),
                RestoreDirectory = true,
                CheckFileExists = true,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            var file = dialog.FileName;
            dialog.Dispose();

            var mapleVersion = WzUtil.GetBestEncryption(file);
            if (mapleVersion == null) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                EtcFile = Wz.Etc.LoadFile(file, mapleVersion.Value);
            } catch (Exception ex) {
                MessageBox.Show($"Unable to open the file: {ex.Message}", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // get Character.wz from same directory
            Wz.Character.LoadFile(Path.Combine(Path.GetDirectoryName(file), "Character"), mapleVersion.Value);
            // get Item.wz from same directory
            Wz.Item.LoadFile(Path.Combine(Path.GetDirectoryName(file), "Item"), mapleVersion.Value);

            LoadCommodities();
        }

        private void LoadFromImg(object sender, EventArgs e) {
            LoadFromFile = false;

            // open folder dialog to select the Etc folder
            var dialog = new FolderBrowserDialog {
                ShowNewFolderButton = false,
                SelectedPath = Directory.GetCurrentDirectory(),
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            var file = dialog.SelectedPath;
            dialog.Dispose();

            // check if Etc/Commodity.img exists
            if (!File.Exists($"{file}\\Etc\\Commodity.img")) {
                MessageBox.Show($"Unable to find Etc/Commodity.img in {file}", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var mapleVersion = WzUtil.GetBestEncryption($"{file}\\Etc\\Commodity.img");
            if (mapleVersion == null) {
                MessageBox.Show(Resources.ErrorWzEncryption, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EtcFile = Wz.Etc.LoadFile($"{file}\\Etc", (WzEncryption)mapleVersion);
            Wz.Character.LoadFile($"{file}\\Character", (WzEncryption)mapleVersion);
            Wz.Item.LoadFile($"{file}\\Item", (WzEncryption)mapleVersion);

            LoadCommodities();
        }
        private void ButtonSave_Click(object sender, EventArgs e) {
            if (EtcFile == null) {
                MessageBox.Show(Resources.ErrorMessageNothingToSave, Resources.Error);
                return;
            }

            if (LoadFromFile) SaveEtcWz();
            else SaveEtcData();
        }

        private void UpdateNodes() {
            var commodityImg = EtcFile.WzDirectory["Commodity.img"] as WzImage;
            commodityImg.ClearProperties();

            foreach (var cashTab in CashShopTabs.Values) {
                foreach (var cashPage in cashTab.SubCategoryControls.Values) {
                    for (var i = 0; i < cashPage.GridView.RowCount; i++) {
                        var row = cashPage.GridView.Rows[i];
                        var item = row.Tag as CommodityImage;
                        var node = item.CreateImageProperty();
                        row.Cells[CommodityPropertyType.Node.ToString()].Value = node.Name;
                        commodityImg.AddProperty(node);
                    }
                }
            }
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
            using var stream = File.Create(temp);
            using var w = new WzBinaryWriter(stream, EtcFile.Encryption);
            (EtcFile.WzDirectory["Commodity.img"] as WzImage)?.WriteImage(w);

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch {
                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            File.Move(temp, dialog.FileName);
            MessageBox.Show(Resources.MessageSuccess);
            dialog.Dispose();

            foreach (var cashTab in CashShopTabs.Values) {
                cashTab.Clear();
            }
            Wz.Etc.Dispose();
            Wz.Character.Dispose();
            Wz.Item.Dispose();
        }

        private void SaveEtcWz() {
            var dialog = new SaveFileDialog {
                FileName = "Etc.wz",
                Filter = Resources.FileSaveWzExt
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var tempFile = Path.GetTempFileName();
            var etc = Wz.Etc.GetFile();
            UpdateNodes();

            etc.SaveToDisk(tempFile);

            Wz.Etc.Dispose();
            Wz.Character.Dispose();
            Wz.Item.Dispose();

            if (File.Exists(dialog.FileName)) {
                try {
                    File.Delete(dialog.FileName);
                } catch (Exception) {
                    // rename to Etc_{timestamp}.wz
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var tempFileName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    tempFile = Path.Combine(Path.GetDirectoryName(dialog.FileName), $"{tempFileName}_{timestamp}.wz");

                    MessageBox.Show($"Unable to delete the file located at {dialog.FileName}. Using name {Path.GetFileName(tempFile)} instead.", "Deletion Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    Path.ChangeExtension(tempFile, ".wz");
                    var path = Path.GetDirectoryName(dialog.FileName);
                    File.Move(tempFile, Path.Combine(path, Path.GetFileName(tempFile)));
                    return;
                }
            }

            File.Move(tempFile, dialog.FileName);
            MessageBox.Show(Resources.MessageSuccess);
            dialog.Dispose();

            foreach (var cashTab in CashShopTabs.Values) {
                cashTab.Clear();
            }
        }

        private void OptionsCreateCategoryMenuItem_Click(object sender, EventArgs e) {
            // not implemented yet
            MessageBox.Show("Not implemented yet", Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            optionsCreateCategoryMenuItem.Checked = false;
        }
        #endregion
    }
}
