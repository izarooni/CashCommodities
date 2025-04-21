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
using System.Collections;
using System.IO;
using System.Linq;

namespace MapleLib.WzLib.Util {
    public static class WzUtil {

        private const int WzHeader = 0x31474B50; //PKG1

        public static readonly Hashtable StringCache = new Hashtable();

        public static uint ROTL(uint x, byte n) {
            return x << n | x >> 32 - n;
        }

        public static uint ROTR(uint x, byte n) {
            return x >> n | x << 32 - n;
        }

        public static int GetCompressedIntLength(int i) {
            if (i > 127 || i < -127) {
                return 5;
            }
            return 1;
        }

        private static int GetEncodedStringLength(string s) {
            int len = 0;
            if (string.IsNullOrEmpty(s)) {
                return 1;
            }
            bool unicode = false;
            foreach (char c in s) {
                if (c > 255) {
                    unicode = true;
                }
            }
            if (unicode) {
                len += s.Length > 126 ? 5 : 1;
                len += s.Length * 2;
            } else {
                len += s.Length > 127 ? 5 : 1;
                len += s.Length;
            }
            return len;
        }

        public static int GetWzObjectValueLength(string s, byte type) {
            string storeName = type + "_" + s;
            if (s.Length > 4 && StringCache.ContainsKey(storeName)) {
                return 5;
            }
            
            StringCache[storeName] = 1;
            return 1 + GetEncodedStringLength(s);
        }

        public static T StringToEnum<T>(string name) {
            try {
                return (T) Enum.Parse(typeof(T), name);
            } catch {
                return default(T);
            }
        }

        private static int IsEligibleText(string str) {
            // make sure string only contains a-zA-Z0-9.
            return str.All(c => char.IsLetterOrDigit(c) || c == '.') ? str.Length : 0;
        }

        public static double GetDecryptionSuccessRate(WzDirectory dir) {
            var recognizedChars = 0d;
            var totalChars = 0d;
            foreach (var subdir in dir.WzDirectories) {
                recognizedChars += IsEligibleText(subdir.Name);
                totalChars += subdir.Name.Length;
            }
            foreach (var img in dir.WzImages) {
                recognizedChars += IsEligibleText(img.Name);
                totalChars += img.Name.Length;
            }
            return recognizedChars / totalChars;
        }

        public static double GetDecryptionSuccessRate(WzFile file) {
            return GetDecryptionSuccessRate(file.WzDirectory);
        }

        public static WzEncryption? GetBestEncryption(string wzFilePath) {
            WzEncryption[] cryptos = { WzEncryption.GMS, WzEncryption.EMS, WzEncryption.BMS };

            var bestSuccessRate = 0d;
            WzEncryption? bestEncryption = null;
            
            foreach (var encryption in cryptos) {
                try {
                    var wzFile = new WzFile(wzFilePath, encryption);
                    wzFile.ParseWzFile();
                    double successRate = GetDecryptionSuccessRate(wzFile);
                    if (successRate > bestSuccessRate) {
                        bestSuccessRate = successRate;
                        bestEncryption = encryption;
                    }
                } catch {
                    // bad decryption
                    continue;
                }
            }
            return bestEncryption;
        }

        public static bool IsListFile(string path) {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            return reader.ReadInt32() != WzHeader;
        }

        public static byte[] GetIvFromZlz(FileStream stream) {
            byte[] iv = new byte[4];
            stream.Seek(0x10040, SeekOrigin.Begin);
            stream.Read(iv, 0, 4);
            return iv;
        }
        private static byte[] GetAesKeyFromZlz(FileStream zlzStream) {
            byte[] aes = new byte[32];

            zlzStream.Seek(0x10060, SeekOrigin.Begin);
            for (int i = 0; i < 8; i++) {
                zlzStream.Read(aes, i * 4, 4);
                zlzStream.Seek(12, SeekOrigin.Current);
            }
            return aes;
        }
    }
}