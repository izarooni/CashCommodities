/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2009, 2010, 2015 Snow and haha01haha01

 * This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib {

    public class WzDirectory : WzObject {
        private readonly List<WzDirectory> wzDirectories = new List<WzDirectory>();
        private readonly List<WzImage> wzImages = new List<WzImage>();

        private int offsetSize;

        public WzDirectory() { }

        public WzDirectory(string name) {
            Name = name;
        }

        private int BlockSize { get; set; }
        private int Checksum { get; set; }
        private uint FileOffset { get; set; }
        internal byte[] AesIvKey { get; set; }
        public WzBinaryReader WzReader { get; set; }
        public short GameVersionHash { get; set; }

        public ReadOnlyCollection<WzImage> WzImages {
            get {
                return new ReadOnlyCollection<WzImage>(wzImages);
            }
        }
        public ReadOnlyCollection<WzDirectory> WzDirectories {
            get {
                return new ReadOnlyCollection<WzDirectory>(wzDirectories);
            }
        }

        public override WzObject this[string name] {
            get {
                foreach (var img in WzImages) {
                    if (String.Equals(img.Name, name, StringComparison.CurrentCultureIgnoreCase)) {
                        return img;
                    }
                }

                foreach (var subdir in WzDirectories) {
                    if (String.Equals(subdir.Name, name, StringComparison.CurrentCultureIgnoreCase)) {
                        return subdir;
                    }
                }
                return null;
            }
        }

        public override string ToString() {
            return $"{Name} ({wzImages.Count} images, {wzDirectories.Count} directories)";
        }

        public override void Dispose() {
            foreach (var dir in WzDirectories) {
                dir.Dispose();
            }
            wzDirectories.Clear();
            foreach (var img in WzImages) {
                img.Dispose();
            }
            wzImages.Clear();
        }

        internal void ParseDirectory() {
            int entryCount = WzReader.ReadCompressedInt();
            for (int i = 0; i < entryCount; i++) {
                byte type = WzReader.ReadByte();
                string objectName = null;

                long position = 0;
                switch (type) {
                    case 1: {
                        WzReader.ReadInt32();
                        WzReader.ReadInt16();
                        WzReader.ReadOffset();
                        continue;
                    }
                    case 2: {
                        int stringOffset = WzReader.ReadInt32();
                        position = WzReader.BaseStream.Position;

                        WzReader.BaseStream.Position = WzReader.WzHeader.FStart + stringOffset;
                        type = WzReader.ReadByte();
                        objectName = WzReader.ReadString();
                        break;
                    }
                    case 3: // WzDirectory
                    case 4: // WzImage
                        objectName = WzReader.ReadString();
                        position = WzReader.BaseStream.Position;
                        break;
                }

                WzReader.BaseStream.Position = position;

                int blockSize = WzReader.ReadCompressedInt();
                int checksum = WzReader.ReadCompressedInt();
                uint readerOffset = WzReader.ReadOffset();

                if (type == 3) {
                    var subdir = new WzDirectory {
                        Name = objectName,
                        BlockSize = blockSize,
                        Checksum = checksum,
                        FileOffset = readerOffset,
                        Parent = this
                    };
                    subdir.GameVersionHash = GameVersionHash;
                    subdir.AesIvKey = AesIvKey;
                    subdir.WzReader = WzReader;
                    subdir.WzFileParent = WzFileParent;
                    wzDirectories.Add(subdir);
                } else {
                    var img = new WzImage(objectName) {
                        BlockSize = blockSize,
                        Checksum = checksum,
                        Offset = readerOffset,
                        Parent = this,  
                    };
                    img.WzReader = WzReader;
                    img.WzFileParent = WzFileParent;
                    wzImages.Add(img);
                }
            }

            foreach (var subdir in WzDirectories) {
                WzReader.BaseStream.Position = subdir.FileOffset;
                subdir.ParseDirectory();
            }
        }

        internal void SaveDirectory(WzBinaryWriter writer) {
            FileOffset = (uint)writer.BaseStream.Position;

            int entryCount = WzDirectories.Count + WzImages.Count;
            if (entryCount == 0) {
                BlockSize = 0;
                return;
            }

            writer.WriteCompressedInt(entryCount);
            foreach (var img in WzImages) {
                writer.WriteWzObjectValue(img.Name, 4);
                writer.WriteCompressedInt(img.BlockSize);
                writer.WriteCompressedInt(img.Checksum);
                writer.WriteOffset(img.Offset);
            }

            foreach (var dir in WzDirectories) {
                writer.WriteWzObjectValue(dir.Name, 3);
                writer.WriteCompressedInt(dir.BlockSize);
                writer.WriteCompressedInt(dir.Checksum);
                writer.WriteOffset(dir.FileOffset);
            }

            foreach (var dir in WzDirectories) {
                if (dir.BlockSize > 0) {
                    dir.SaveDirectory(writer);
                } else {
                    writer.Write((byte)0);
                }
            }
        }

        internal void SaveImages(WzBinaryWriter writer, FileStream targetFileStream) {
            foreach (var img in WzImages) {
                if (img.Changed) {
                    targetFileStream.Position = img.TempFileStart;
                    byte[] buffer = new byte[img.BlockSize];
                    targetFileStream.Read(buffer, 0, img.BlockSize);
                    writer.Write(buffer);
                } else {
                    img.WzReader.BaseStream.Position = img.TempFileStart;
                    writer.Write(img.WzReader.ReadBytes((int)(img.TempFileEnd - img.TempFileStart)));
                }
            }
            foreach (var dir in WzDirectories) {
                dir.SaveImages(writer, targetFileStream);
            }
        }

        internal int GenerateDataFile(string fileName, FileStream fileStream) {
            BlockSize = 0;

            int entryCount = WzDirectories.Count + WzImages.Count;
            if (entryCount == 0) {
                offsetSize = 1;
                return BlockSize = 0;
            }

            BlockSize = WzUtil.GetCompressedIntLength(entryCount);
            offsetSize = WzUtil.GetCompressedIntLength(entryCount);

            foreach (var img in WzImages) {
                if (img.Changed) {
                    // write the image contents into memory
                    using var memory = new MemoryStream();
                    using var writer = new WzBinaryWriter(memory, WzFileParent.Encryption);
                    img.WriteImage(writer);

                    // calculate the checksum
                    img.Checksum = 0;
                    foreach (byte b in memory.ToArray()) {
                        img.Checksum += b;
                    }

                    // write the memory into the file
                    img.TempFileStart = fileStream.Position;
                    fileStream.Write(memory.ToArray(), 0, (int)memory.Length);
                    img.TempFileEnd = fileStream.Position;
                } else {
                    img.TempFileStart = img.Offset;
                    img.TempFileEnd = img.Offset + img.BlockSize;
                }

                int nameLen = WzUtil.GetWzObjectValueLength(img.Name, 4);
                BlockSize += nameLen;
                int imgLen = img.BlockSize;

                BlockSize += WzUtil.GetCompressedIntLength(imgLen);
                BlockSize += imgLen;
                BlockSize += WzUtil.GetCompressedIntLength(img.Checksum);
                BlockSize += 4;

                offsetSize += nameLen;
                offsetSize += WzUtil.GetCompressedIntLength(imgLen);
                offsetSize += WzUtil.GetCompressedIntLength(img.Checksum);
                offsetSize += 4;
            }

            foreach (var subdir in WzDirectories) {
                int nameLen = WzUtil.GetWzObjectValueLength(subdir.Name, 3);
                BlockSize += nameLen;
                BlockSize += subdir.GenerateDataFile(fileName, fileStream);
                BlockSize += WzUtil.GetCompressedIntLength(subdir.BlockSize);
                BlockSize += WzUtil.GetCompressedIntLength(subdir.Checksum);
                BlockSize += 4;

                offsetSize += nameLen;
                offsetSize += WzUtil.GetCompressedIntLength(subdir.BlockSize);
                offsetSize += WzUtil.GetCompressedIntLength(subdir.Checksum);
                offsetSize += 4;
            }
            return BlockSize;
        }

        internal uint GetOffsets(uint curOffset) {
            FileOffset = curOffset;
            curOffset += (uint)offsetSize;
            foreach (var dir in WzDirectories) {
                curOffset = dir.GetOffsets(curOffset);
            }
            return curOffset;
        }

        internal uint GetImgOffsets(uint curOffset) {
            foreach (var img in WzImages) {
                img.Offset = curOffset;
                curOffset += (uint)img.BlockSize;
            }

            foreach (var dir in WzDirectories) {
                curOffset = dir.GetImgOffsets(curOffset);
            }
            return curOffset;
        }

        public void ParseImages() {
            foreach (var img in WzImages) {
                if (WzReader.BaseStream.Position != img.Offset) {
                    WzReader.BaseStream.Position = img.Offset;
                }
                img.ParseImage();
            }

            foreach (var subdir in WzDirectories) {
                if (WzReader.BaseStream.Position != subdir.FileOffset) {
                    WzReader.BaseStream.Position = subdir.FileOffset;
                }
                subdir.ParseImages();
            }
        }

        internal void SetHash(short newHash) {
            GameVersionHash = newHash;
            foreach (var dir in WzDirectories) {
                dir.SetHash(newHash);
            }
        }

        public void AddImage(WzImage img) {
            img.Parent = this;
            img.WzFileParent = WzFileParent;
            wzImages.Add(img);
        }

        public void AddDirectory(WzDirectory dir) {
            dir.Parent = this;
            dir.WzFileParent = WzFileParent;
            wzDirectories.Add(dir);
        }

        public void ClearImages() {
            foreach (var img in WzImages) {
                img.Parent = null;
            }
            wzImages.Clear();
        }

        public void ClearDirectories() {
            foreach (var dir in WzDirectories) {
                dir.Parent = null;
            }
            wzDirectories.Clear();
        }

        public List<WzImage> GetAllImages() {
            var imgs = new List<WzImage>();
            imgs.AddRange(WzImages);
            foreach (var subDir in WzDirectories) {
                imgs.AddRange(subDir.GetAllImages());
            }
            return imgs;
        }

        public void RemoveImage(WzImage img) {
            if (wzImages.Remove(img)) {
                img.Parent = null;
                img.ParentImage = null;
                img.WzFileParent = null;
            }
        }

        public void RemoveDirectory(WzDirectory dir) {
            if (wzDirectories.Remove(dir)) {
                dir.Parent = null;
                dir.WzFileParent = null;
            }
        }

        public int CountImages() {
            int count = WzImages.Count;
            foreach (var subdir in WzDirectories) {
                count += subdir.CountImages();
            }
            return count;
        }

        public override void Remove() {
            ((WzDirectory)Parent).RemoveDirectory(this);
        }
    }
}
