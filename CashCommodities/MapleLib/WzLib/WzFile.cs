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
using System.IO;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib {

    public class WzFile : WzObject {
        private byte[] aesIvKey;
        private short gameVersionHash;

        public WzFile(short gameGameVersion, WzEncryption encryption)
            : this("", gameGameVersion, encryption) { }

        public WzFile(string filePath, WzEncryption encryption)
            : this(filePath, -1, encryption) { }

        public WzFile(string filePath, short gameVersion, WzEncryption encryption) {
            FilePath = filePath;
            GameVersion = gameVersion;
            Encryption = encryption;
            Name = Path.GetFileName(filePath);

            if (encryption == WzEncryption.GETFROMZLZ) {
                var zlzPath = Path.Combine(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException(), "ZLZ.dll");
                if (!File.Exists(zlzPath)) {
                    throw new FileNotFoundException("ZLZ.dll not found in the same directory as the wz file.");
                }
                using var stream = File.OpenRead(zlzPath);
                aesIvKey = WzUtil.GetIvFromZlz(stream);
            } else {
                aesIvKey = encryption.GetAesIvKey();
            }
        }

        /// <summary>
        /// The parsed IWzDir after having called ParseWzDirectory(), this can either be a WzDirectory or a WzListDirectory
        /// </summary>
        public WzDirectory WzDirectory {
            get;
            set;
        }

        public override WzObject this[string name] {
            get { return WzDirectory[name]; }
        }

        private WzHeader WzHeader {
            get;
            set;
        }

        public short GameVersion {
            get;
            set;
        }

        private string FilePath {
            get;
        }

        public WzEncryption Encryption {
            get;
            set;
        }

        public WzBinaryReader WzReader { get; set; }

        public new WzObject Parent {
            get {
                return null;
            }
            set {
                throw new InvalidOperationException("Cannot set Parent on WzFile");
            }
        }

        public new WzFile WzFileParent {
            get {
                return this;
            }
            set {
                throw new InvalidOperationException("Cannot set WzFileParent on WzFile");
            }
        }

        public override string ToString() {
            return $"WzFile: {Name} ({WzDirectory.WzDirectories.Count} directories, {WzDirectory.WzImages.Count} images, {Encryption} Encryption)";
        }

        public override void Dispose() {
            WzDirectory?.Dispose();
            WzReader?.Dispose();
            WzReader = null;
        }

        private short GetVersionHash(int gameVersion) {
            int hash = 0;
            string sGameVersion = gameVersion.ToString();

            foreach (char c in sGameVersion) {
                hash = (32 * hash) + c + 1;
            }
            int b4 = (hash >> 24) & 0xFF;
            int b3 = (hash >> 16) & 0xFF;
            int b2 = (hash >> 8) & 0xFF;
            int b1 = hash & 0xFF;
            int decryptedVersionNumber = (0xff ^ b4 ^ b3 ^ b2 ^ b1);

            return (short)(gameVersionHash == decryptedVersionNumber
                ? Convert.ToInt16(hash)
                : 0);
        }

        private void CreateVersionHash() {
            int hash = 0;
            foreach (char ch in GameVersion.ToString()) {
                hash = (hash * 32) + (byte)ch + 1;
            }
            int a = (hash >> 24) & 0xFF,
                b = (hash >> 16) & 0xFF,
                c = (hash >> 8) & 0xFF,
                d = hash & 0xFF;
            gameVersionHash = (byte)~(a ^ b ^ c ^ d);
        }

        public void ParseWzFile() {
            if (FilePath == null) {
                throw new FileNotFoundException();
            }

            var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            WzReader = new WzBinaryReader(stream, Encryption);

            WzHeader = new WzHeader {
                Ident = WzReader.ReadString(4),
                FSize = WzReader.ReadUInt64(),
                FStart = WzReader.ReadUInt32(),
                Copyright = WzReader.ReadNullTerminatedString()
            };
            WzReader.WzHeader = WzHeader;

            WzReader.ReadBytes((int)(WzHeader.FStart - WzReader.BaseStream.Position));

            gameVersionHash = WzReader.ReadInt16();
            short versionHash = GetVersionHash(GameVersion);

            if (GameVersion < 1) {
                // brute force the game version
                for (short gameVersion = 0; gameVersion < short.MaxValue; gameVersion++) {
                    versionHash = GetVersionHash(gameVersion);
                    if (versionHash == 0) {
                        continue;
                    }

                    gameVersionHash = versionHash;
                    WzReader.GameVersionHash = versionHash;

                    long position = WzReader.BaseStream.Position;
                    try {
                        var dir = new WzDirectory {
                            WzReader = WzReader,
                            Name = Name,
                            GameVersionHash = versionHash,
                            AesIvKey = aesIvKey,
                            Parent = this,
                            WzFileParent = this
                        };
                        dir.ParseDirectory();
                        double successRate = WzUtil.GetDecryptionSuccessRate(dir);
                        if (successRate >= 0.8) {
                            WzDirectory = dir;
                            GameVersion = gameVersion;
                            return;
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                        WzReader.BaseStream.Position = position;
                    } finally {
                        WzReader.BaseStream.Position = position;
                    }
                }
                throw new Exception("Unable to determine the game version.");
            }

            WzReader.GameVersionHash = versionHash;
            var directory = new WzDirectory {
                WzReader = WzReader,
                Name = Name,
                GameVersionHash = versionHash,
                AesIvKey = aesIvKey,
                Parent = this,
                WzFileParent = this
            };
            directory.ParseDirectory();
            WzDirectory = directory;
        }

        public void SaveToDisk(string targetFilePath) {
            WzUtil.StringCache.Clear();

            CreateVersionHash();
            WzDirectory.SetHash(gameVersionHash);

            // write contents to a temporary file excluding header and other metadata
            var tempFile = Path.GetTempFileName();
            using var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            WzDirectory.GenerateDataFile(tempFile, tempStream);

            uint blockSize = WzDirectory.GetImgOffsets(WzDirectory.GetOffsets(WzHeader.FStart + 2));

            // essentially the reverse operation of parsing
            using var stream = File.Create(targetFilePath);
            using var writer = new WzBinaryWriter(stream, Encryption);
            writer.Header = WzHeader;
            writer.GameVersionHash = gameVersionHash;
            WzHeader.FSize = blockSize - WzHeader.FStart;

            // write WzHeader contents
            for (int i = 0; i < 4; i++) {
                writer.Write((byte)WzHeader.Ident[i]);
            }
            writer.Write(WzHeader.FSize);
            writer.Write(WzHeader.FStart);
            writer.WriteNullTerminatedString(WzHeader.Copyright);

            long fillerData = WzHeader.FStart - writer.BaseStream.Position;
            if (fillerData > 0) {
                writer.Write(new byte[(int)fillerData]);
            }

            writer.Write(gameVersionHash);

            WzDirectory.SaveDirectory(writer);
            writer.StringCache.Clear();

            WzDirectory.SaveImages(writer, tempStream);
            writer.StringCache.Clear();
            tempStream.Flush();
            tempStream.Close();
            File.Delete(tempFile);
        }

        public override void Remove() {
            throw new InvalidOperationException("Cannot remove WzFile");
        }
    }
}
