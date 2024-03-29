﻿/*  MapleLib - A general-purpose MapleStory library
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
using System.Collections;
using System.IO;
using System.Linq;

using MapleLib.MapleCryptoLib;

namespace MapleLib.WzLib.Util {
    public class WzTool {

        public static Hashtable StringCache = new Hashtable();

        public static UInt32 RotateLeft(UInt32 x, byte n) {
            return (UInt32) (((x) << (n)) | ((x) >> (32 - (n))));
        }

        public static UInt32 RotateRight(UInt32 x, byte n) {
            return (UInt32) (((x) >> (n)) | ((x) << (32 - (n))));
        }

        public static int GetCompressedIntLength(int i) {
            if (i > 127 || i < -127)
                return 5;
            return 1;
        }

        public static int GetEncodedStringLength(string s) {
            int len = 0;
            if (string.IsNullOrEmpty(s))
                return 1;
            bool unicode = false;
            foreach (char c in s)
                if (c > 255)
                    unicode = true;
            if (unicode) {
                if (s.Length > 126)
                    len += 5;
                else
                    len += 1;
                len += s.Length * 2;
            } else {
                if (s.Length > 127)
                    len += 5;
                else
                    len += 1;
                len += s.Length;
            }
            return len;
        }

        public static int GetWzObjectValueLength(string s, byte type) {
            string storeName = type + "_" + s;
            if (s.Length > 4 && StringCache.ContainsKey(storeName)) {
                return 5;
            } else {
                StringCache[storeName] = 1;
                return 1 + GetEncodedStringLength(s);
            }
        }

        public static T StringToEnum<T>(string name) {
            try {
                return (T) Enum.Parse(typeof(T), name);
            } catch {
                return default(T);
            }
        }

        private static int GetRecognizedCharacters(string source) {
            int result = 0;
            foreach (char c in source)
                if (0x20 <= c && c <= 0x7E)
                    result++;
            return result;
        }

        public static double GetDecryptionSuccessRate(string wzPath, WzMapleVersion encVersion, ref short? version) {
            WzFile wzf;
            if (version == null)
                wzf = new WzFile(wzPath, encVersion);
            else
                wzf = new WzFile(wzPath, (short) version, encVersion);
            wzf.ParseWzFile();
            if (version == null) version = wzf.Version;
            int recognizedChars = 0;
            int totalChars = 0;
            foreach (WzDirectory wzdir in wzf.WzDirectory.WzDirectories) {
                recognizedChars += GetRecognizedCharacters(wzdir.Name);
                totalChars += wzdir.Name.Length;
            }
            foreach (WzImage wzimg in wzf.WzDirectory.WzImages) {
                recognizedChars += GetRecognizedCharacters(wzimg.Name);
                totalChars += wzimg.Name.Length;
            }
            wzf.Dispose();
            return (double) recognizedChars / (double) totalChars;
        }

        public static int DetectMapleVersion(string path, string fileName) {
            var values = Enum.GetValues(typeof(WzMapleVersion)).Cast<WzMapleVersion>();
            foreach (var v in values) {
                using (FileStream stream = new FileStream(path + "\\" + fileName, FileMode.Open, FileAccess.Read)) {
                    using (WzBinaryReader reader = new WzBinaryReader(stream, v.EncryptionKey())) {
                        byte b = reader.ReadByte();
                        if (b != 0x73 || reader.ReadString() != "Property" || reader.ReadUInt16() != 0) continue;
                        return (int) v;
                    }
                }
            }
            return -1;
        }

        public static WzMapleVersion DetectMapleVersion(string wzFilePath, out short fileVersion) {
            Hashtable mapleVersionSuccessRates = new Hashtable();
            short? version = null;
            mapleVersionSuccessRates.Add(WzMapleVersion.CHIRITHY, GetDecryptionSuccessRate(wzFilePath, WzMapleVersion.CHIRITHY, ref version));
            mapleVersionSuccessRates.Add(WzMapleVersion.SERENITY, GetDecryptionSuccessRate(wzFilePath, WzMapleVersion.SERENITY, ref version));
            mapleVersionSuccessRates.Add(WzMapleVersion.GMS, GetDecryptionSuccessRate(wzFilePath, WzMapleVersion.GMS, ref version));
            mapleVersionSuccessRates.Add(WzMapleVersion.EMS, GetDecryptionSuccessRate(wzFilePath, WzMapleVersion.EMS, ref version));
            mapleVersionSuccessRates.Add(WzMapleVersion.BMS, GetDecryptionSuccessRate(wzFilePath, WzMapleVersion.BMS, ref version));
            fileVersion = (short) version;
            WzMapleVersion mostSuitableVersion = WzMapleVersion.GMS;
            double maxSuccessRate = 0;
            foreach (DictionaryEntry mapleVersionEntry in mapleVersionSuccessRates)
                if ((double) mapleVersionEntry.Value > maxSuccessRate) {
                    mostSuitableVersion = (WzMapleVersion) mapleVersionEntry.Key;
                    maxSuccessRate = (double) mapleVersionEntry.Value;
                }
            if (maxSuccessRate < 0.7 && File.Exists(Path.Combine(Path.GetDirectoryName(wzFilePath), "ZLZ.dll")))
                return WzMapleVersion.GETFROMZLZ;
            else return mostSuitableVersion;
        }

        public const int WzHeader = 0x31474B50; //PKG1

        public static bool IsListFile(string path) {
            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            bool result = reader.ReadInt32() != WzHeader;
            reader.Close();
            return result;
        }

        private static byte[] Combine(byte[] a, byte[] b) {
            byte[] result = new byte[a.Length + b.Length];
            Array.Copy(a, 0, result, 0, a.Length);
            Array.Copy(b, 0, result, a.Length, b.Length);
            return result;
        }
    }
}