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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

using MapleLib.WzLib.Util;

namespace MapleLib.WzLib.WzProperties {
    /// <summary>
    /// A property that contains the information for a bitmap
    /// </summary>
    public class WzPngProperty : WzImageProperty {
        private byte[] compressedBytes = null;
        internal int format;
        internal int format2;
        internal bool listWzUsed;
        internal long startOffset;

        private WzBinaryReader reader;

        public WzPngProperty() { }

        internal WzPngProperty(WzBinaryReader reader, bool parseNow) {
            this.reader = reader;
            Width = reader.ReadCompressedInt();
            Height = reader.ReadCompressedInt();
            format = reader.ReadCompressedInt();
            format2 = reader.ReadByte();
            reader.BaseStream.Position += 4;
            startOffset = reader.BaseStream.Position;

            if (parseNow) {
                GetPng(true);
            } else {
                int len = reader.ReadInt32() - 1;
                reader.BaseStream.Position += 1;
                reader.BaseStream.Position += len;
            }
        }

        public new string Name { get { return "PNG"; } }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Format {
            get { return format + format2; }
            set {
                format = value;
                format2 = 0;
            }
        }

        public bool ListWzUsed {
            get { return listWzUsed; }
            set {
                if (value == listWzUsed) return;
                listWzUsed = value;
                CompressPng(GetPng(false));
            }
        }

        public override void SetValue(object value) {
            if (value is Bitmap bmp) {
                Png = bmp;
            } else if (value is byte[] aob) {
                compressedBytes = aob;
            }
            
            throw new Exception($"Unknown value type: {value.GetType()}");
        }

        public override WzImageProperty DeepClone() {
            var clone = new WzPngProperty();
            clone.Png = GetPng(false);
            return clone;
        }

        public override void WriteValue(WzBinaryWriter writer) {
            throw new NotImplementedException("Cannot write a PngProperty");
        }

        public override void Dispose() {
            compressedBytes = null;
            Png?.Dispose();
            Png = null;
        }

        public byte[] GetCompressedBytes(bool cache) {
            if (compressedBytes != null) return compressedBytes;

            long startPosition = reader.BaseStream.Position;
            byte[] data = Array.Empty<byte>();

            reader.BaseStream.Position = startOffset;
            int length = reader.ReadInt32() - 1;
            reader.BaseStream.Position += 1;
            if (length > 0) {
                data = reader.ReadBytes(length);
            }
            reader.BaseStream.Position = startPosition;

            if (cache) {
                return compressedBytes = data;
            }
            return data;
        }

        private Bitmap png;

        public Bitmap Png {
            get {
                if (png != null) return png;
                if (compressedBytes == null) {
                    compressedBytes = GetCompressedBytes(true);
                }
                return ParsePng(true, compressedBytes);
            }
            private set {
                Png = value;
                CompressPng(value);
            }
        }

        public Bitmap GetPng(bool cache) {
            byte[] data = GetCompressedBytes(cache);
            return ParsePng(cache, data);
        }

        private byte[] Decompress(byte[] compressedBuffer, int decompressedSize) {
            byte[] buffer = new byte[decompressedSize];

            using var mem = new MemoryStream();
            mem.Write(compressedBuffer, 2, compressedBuffer.Length - 2);
            mem.Position = 0;

            using var zip = new DeflateStream(mem, CompressionMode.Decompress);
            zip.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private byte[] Compress(byte[] decompressedBuffer) {
            using var mem = new MemoryStream();
            using var zip = new DeflateStream(mem, CompressionMode.Compress, true);
            zip.Write(decompressedBuffer, 0, decompressedBuffer.Length);
            mem.Position = 0;
            byte[] buffer = new byte[mem.Length + 2];
            mem.Read(buffer, 2, buffer.Length - 2);
            Buffer.BlockCopy(new byte[] { 0x78, 0x9C }, 0, buffer, 0, 2);
            return buffer;
        }

        private Bitmap ParsePng(bool cache, byte[] data) {
            DeflateStream zlib;
            int uncompressedSize;
            int x = 0;
            int y = 0;
            int b = 0;
            int g = 0;
            Bitmap bmp = null;
            BitmapData bmpData;
            var imgParent = ParentImage;
            byte[] decBuf;

            using var reader = new BinaryReader(new MemoryStream(data));
            ushort header = reader.ReadUInt16();
            listWzUsed = header != 0x9C78 && header != 0xDA78 && header != 0x0178 && header != 0x5E78;

            if (!listWzUsed) {
                zlib = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
            } else {
                reader.BaseStream.Position -= 2;
                var dataStream = new MemoryStream();
                int blocksize = 0;
                int endOfPng = data.Length;

                while (reader.BaseStream.Position < endOfPng) {
                    blocksize = reader.ReadInt32();
                    for (int i = 0; i < blocksize; i++) {
                        dataStream.WriteByte((byte)(reader.ReadByte() ^ imgParent.WzReader.WzKey[i]));
                    }
                }
                dataStream.Position = 2;
                zlib = new DeflateStream(dataStream, CompressionMode.Decompress);
            }

            switch (format + format2) {
                case 1:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    uncompressedSize = Width * Height * 2;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    byte[] argb = new Byte[uncompressedSize * 2];
                    for (int i = 0; i < uncompressedSize; i++) {
                        b = decBuf[i] & 0x0F;
                        b |= (b << 4);
                        argb[i * 2] = (byte)b;
                        g = decBuf[i] & 0xF0;
                        g |= (g >> 4);
                        argb[i * 2 + 1] = (byte)g;
                    }
                    Marshal.Copy(argb, 0, bmpData.Scan0, argb.Length);
                    bmp.UnlockBits(bmpData);
                    break;
                case 2:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    uncompressedSize = Width * Height * 4;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    Marshal.Copy(decBuf, 0, bmpData.Scan0, decBuf.Length);
                    bmp.UnlockBits(bmpData);
                    break;
                case 513:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format16bppRgb565);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format16bppRgb565);
                    uncompressedSize = Width * Height * 2;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    Marshal.Copy(decBuf, 0, bmpData.Scan0, decBuf.Length);
                    bmp.UnlockBits(bmpData);
                    break;

                case 517:
                    bmp = new Bitmap(Width, Height);
                    uncompressedSize = Width * Height / 128;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    byte iB = 0;
                    for (int i = 0; i < uncompressedSize; i++) {
                        for (byte j = 0; j < 8; j++) {
                            iB = Convert.ToByte(((decBuf[i] & (0x01 << (7 - j))) >> (7 - j)) * 0xFF);
                            for (int k = 0; k < 16; k++) {
                                if (x == Width) {
                                    x = 0;
                                    y++;
                                }
                                bmp.SetPixel(x, y, Color.FromArgb(0xFF, iB, iB, iB));
                                x++;
                            }
                        }
                    }
                    break;

                case 1026:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                    bmpData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    uncompressedSize = Width * Height;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    decBuf = GetPixelDataDxt3(decBuf, Width, Height);
                    Marshal.Copy(decBuf, 0, bmpData.Scan0, decBuf.Length);
                    bmp.UnlockBits(bmpData);
                    break;

                default:
                    throw new Exception($"Unknown PNG format {format} {format2}");
            }
            if (cache) png = bmp;
            return bmp;
        }

        private void CompressPng(Bitmap bmp) {
            byte[] buf = new byte[bmp.Width * bmp.Height * 8];
            format = 2;
            format2 = 0;
            Width = bmp.Width;
            Height = bmp.Height;

            int curPos = 0;
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++) {
                    var curPixel = bmp.GetPixel(j, i);
                    buf[curPos] = curPixel.B;
                    buf[curPos + 1] = curPixel.G;
                    buf[curPos + 2] = curPixel.R;
                    buf[curPos + 3] = curPixel.A;
                    curPos += 4;
                }
            compressedBytes = Compress(buf);
            if (!listWzUsed) return;
            using var mem = new MemoryStream();
            using var writer = new WzBinaryWriter(mem, WzEncryption.GMS.GetAesIvKey());
            writer.Write(2);
            for (int i = 0; i < 2; i++) {
                writer.Write((byte)(compressedBytes[i] ^ writer.WzKey[i]));
            }
            writer.Write(compressedBytes.Length - 2);
            for (int i = 2; i < compressedBytes.Length; i++)
                writer.Write((byte)(compressedBytes[i] ^ writer.WzKey[i - 2]));
            compressedBytes = mem.GetBuffer();
        }

        private static byte[] GetPixelDataDxt3(byte[] rawData, int width, int height) {
            byte[] pixel = new byte[width * height * 4];

            var colorTable = new Color[4];
            int[] colorIdxTable = new int[16];
            byte[] alphaTable = new byte[16];
            for (int y = 0; y < height; y += 4) {
                for (int x = 0; x < width; x += 4) {
                    int off = x * 4 + y * width;
                    ExpandAlphaTable(alphaTable, rawData, off);
                    ushort u0 = BitConverter.ToUInt16(rawData, off + 8);
                    ushort u1 = BitConverter.ToUInt16(rawData, off + 10);
                    ExpandColorTable(colorTable, u0, u1);
                    ExpandColorIndexTable(colorIdxTable, rawData, off + 12);

                    for (int j = 0; j < 4; j++) {
                        for (int i = 0; i < 4; i++) {
                            SetPixel(pixel,
                                x + i,
                                y + j,
                                width,
                                colorTable[colorIdxTable[j * 4 + i]],
                                alphaTable[j * 4 + i]);
                        }
                    }
                }
            }

            return pixel;
        }

        private static void SetPixel(byte[] pixelData, int x, int y, int width, Color color, byte alpha) {
            int offset = (y * width + x) * 4;
            pixelData[offset + 0] = color.B;
            pixelData[offset + 1] = color.G;
            pixelData[offset + 2] = color.R;
            pixelData[offset + 3] = alpha;
        }

        private static void ExpandColorTable(Color[] color, ushort u0, ushort u1) {
            color[0] = RGB565ToColor(u0);
            color[1] = RGB565ToColor(u1);
            color[2] = Color.FromArgb(0xff, (color[0].R * 2 + color[1].R + 1) / 3, (color[0].G * 2 + color[1].G + 1) / 3, (color[0].B * 2 + color[1].B + 1) / 3);
            color[3] = Color.FromArgb(0xff, (color[0].R + color[1].R * 2 + 1) / 3, (color[0].G + color[1].G * 2 + 1) / 3, (color[0].B + color[1].B * 2 + 1) / 3);
        }

        private static void ExpandColorIndexTable(int[] colorIndex, byte[] rawData, int offset) {
            for (int i = 0; i < 16; i += 4, offset++) {
                colorIndex[i + 0] = (rawData[offset] & 0x03);
                colorIndex[i + 1] = (rawData[offset] & 0x0c) >> 2;
                colorIndex[i + 2] = (rawData[offset] & 0x30) >> 4;
                colorIndex[i + 3] = (rawData[offset] & 0xc0) >> 6;
            }
        }

        private static void ExpandAlphaTable(byte[] alpha, byte[] rawData, int offset) {
            for (int i = 0; i < 16; i += 2, offset++) {
                alpha[i + 0] = (byte)(rawData[offset] & 0x0f);
                alpha[i + 1] = (byte)((rawData[offset] & 0xf0) >> 4);
            }
            for (int i = 0; i < 16; i++) {
                alpha[i] = (byte)(alpha[i] | (alpha[i] << 4));
            }
        }

        private static Color RGB565ToColor(ushort val) {
            const int rgb565_mask_r = 0xf800;
            const int rgb565_mask_g = 0x07e0;
            const int rgb565_mask_b = 0x001f;
            int r = (val & rgb565_mask_r) >> 11;
            int g = (val & rgb565_mask_g) >> 5;
            int b = (val & rgb565_mask_b);
            var c = Color.FromArgb(
                (r << 3) | (r >> 2),
                (g << 2) | (g >> 4),
                (b << 3) | (b >> 2));
            return c;
        }
    }
}
