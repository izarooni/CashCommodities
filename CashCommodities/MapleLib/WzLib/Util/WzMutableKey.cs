/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2015 haha01haha01 and contributors

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
using System.Security.Cryptography;

namespace MapleLib.WzLib.Util {
    public class WzMutableKey {

        private static readonly int BatchSize = 4096;
        private readonly byte[] aesIvKey;
        private readonly byte[] aesKey;

        private byte[] keys;
        public WzMutableKey(byte[] aesIvKey, byte[] aesKey) {
            this.aesIvKey = aesIvKey;
            this.aesKey = aesKey;
        }

        public byte this[int index] {
            get {
                if (keys == null || keys.Length <= index) {
                    EnsureKeySize(index + 1);
                }
                return keys[index];
            }
        }

        private void EnsureKeySize(int size) {
            // array already big enough
            if (keys != null && keys.Length >= size) {
                return;
            }

            size = (int)Math.Ceiling(1.0 * size / BatchSize) * BatchSize;
            byte[] newKeys = new byte[size];

            if (BitConverter.ToInt32(aesIvKey, 0) == 0) {
                keys = newKeys;
                return;
            }

            int startIndex = 0;

            if (keys != null) {
                Buffer.BlockCopy(keys, 0, newKeys, 0, keys.Length);
                startIndex = keys.Length;
            }

            using var aes = Rijndael.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = aesKey;
            aes.Mode = CipherMode.ECB;

            var memory = new MemoryStream(newKeys, startIndex, newKeys.Length - startIndex, true);
            var stream = new CryptoStream(memory, aes.CreateEncryptor(), CryptoStreamMode.Write);

            for (int i = startIndex; i < size; i += 16) {
                if (i == 0) {
                    byte[] block = new byte[16];
                    for (int j = 0; j < block.Length; j++) {
                        block[j] = aesIvKey[j % 4];
                    }
                    stream.Write(block, 0, block.Length);
                } else {
                    stream.Write(newKeys, i - 16, 16);
                }
            }

            stream.Flush();
            memory.Close();
            keys = newKeys;
        }
    }
}
