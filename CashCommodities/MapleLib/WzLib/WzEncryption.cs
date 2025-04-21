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

namespace MapleLib.WzLib {
    public static class WzVersionData {
        public static byte[] GetAesBlock(this WzEncryption encryption) {
            switch (encryption) {
                case WzEncryption.GMS:
                case WzEncryption.EMS:
                case WzEncryption.BMS:
                case WzEncryption.GETFROMZLZ:
                default: return MapleCryptoLib.CryptoConstants.WzAesBlock;
            }
        }

        public static byte[] GetAesIvKey(this WzEncryption encryption) {
            switch (encryption) {
                case WzEncryption.GMS: return new byte[] { 0x4D, 0x23, 0xC7, 0x2B };
                case WzEncryption.EMS: return new byte[] { 0xB9, 0x7D, 0x63, 0xE9 };
                case WzEncryption.BMS:
                case WzEncryption.GETFROMZLZ:
                default: return new byte[4];

            }
        }
    }

    public enum WzEncryption {
        GMS,
        EMS,
        BMS,
        GETFROMZLZ
    }
}