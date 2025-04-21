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

using System.Collections.Generic;
using System.IO;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib {
    public class WzListFile {

        public List<string> Entries { get; } = new List<string>();
        public WzListFile(string filePath, WzEncryption encryption) {

            byte[] bytes = File.ReadAllBytes(filePath);
            using var memory = new MemoryStream(bytes);
            using var reader = new WzBinaryReader(memory, encryption);

            while (reader.PeekChar() != -1) {
                int stringLength = reader.ReadInt32();
                char[] encryptedString = new char[stringLength];

                for (int i = 0; i < stringLength; i++) {
                    encryptedString[i] = (char)reader.ReadInt16();
                }
                reader.ReadUInt16(); // encrypted null
                string decryptedStr = reader.DecryptString(encryptedString);
                Entries.Add(decryptedStr);
            }

            int lastIndex = Entries.Count - 1;
            string lastEntry = Entries[lastIndex];
            Entries[lastIndex] = lastEntry.Substring(0, lastEntry.Length - 1) + "g";
        }

        public void SaveToDisk(string filePath, WzEncryption encryption) {
            SaveToDisk(filePath, encryption.GetAesIvKey());
        }

        public void SaveToDisk(string filePath, byte[] aesIvKey) {
            var lastIndex = Entries.Count - 1;
            var lastEntry = Entries[lastIndex];

            Entries[lastIndex] = lastEntry.Substring(0, lastEntry.Length - 1) + "/";
            using var stream = File.Create(filePath);
            using var writer = new WzBinaryWriter(stream, aesIvKey);
            
            foreach (string s in Entries) {
                writer.Write(s.Length);
                char[] encryptedChars = writer.EncryptString(s + (char)0);
                foreach (char c in encryptedChars) {
                    writer.Write((short)c);
                }
            }
            Entries[lastIndex] = lastEntry.Substring(0, lastEntry.Length - 1) + "/";
            writer.Flush();
        }
    }
}
