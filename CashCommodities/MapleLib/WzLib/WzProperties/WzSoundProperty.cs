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

using System.IO;
using System;
using MapleLib.WzLib.Util;
using NAudio.Wave;
using MapleLib.Helpers;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace MapleLib.WzLib.WzProperties {
    /// <summary>
    /// A property that contains data for an MP3 file
    /// </summary>
    public class WzSoundProperty : WzExtended {
        private static readonly byte[] headerAob = {
            0x02,
            0x83, 0xEB, 0x36, 0xE4, 0x4F, 0x52, 0xCE, 0x11, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70,
            0x8B, 0xEB, 0x36, 0xE4, 0x4F, 0x52, 0xCE, 0x11, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70,
            0x00,
            0x01,
            0x81, 0x9F, 0x58, 0x05, 0x56, 0xC3, 0xCE, 0x11, 0xBF, 0x01, 0x00, 0xAA, 0x00, 0x55, 0x59, 0x5A
        };
        private int soundDataLength;

        internal WaveFormat wavFormat;

        public WzSoundProperty(string name) {
            Name = name;
        }

        public WzSoundProperty(string name, WzBinaryReader reader) : this(name) {
            reader.BaseStream.Position++;

            //note - soundDataLen does NOT include the length of the header.
            soundDataLength = reader.ReadCompressedInt();
            AudioDuration = reader.ReadCompressedInt();

            long headerOffset = reader.BaseStream.Position;
            reader.BaseStream.Position += headerAob.Length;

            int wavFormatLength = reader.ReadByte();
            reader.BaseStream.Position = headerOffset;

            HeaderAob = reader.ReadBytes(headerAob.Length + 1 + wavFormatLength);
            ParseHeader();

            Offset = reader.BaseStream.Position;
            AudioAob = reader.ReadBytes(soundDataLength);
        }

        public WzSoundProperty(string name, int audioDuration, byte[] headerAob, byte[] data) : this(name) {
            AudioDuration = audioDuration;
            HeaderAob = headerAob;
            AudioAob = data;
            ParseHeader();
        }

        public WzSoundProperty(string name, string file) : this(name) {
            using var reader = new Mp3FileReader(file);
            wavFormat = reader.Mp3WaveFormat;
            AudioDuration = (int)(reader.Length * 1000d / reader.WaveFormat.AverageBytesPerSecond);
            RebuildHeader();
            AudioAob = File.ReadAllBytes(file);
        }

        public byte[] AudioAob { get; set; }
        internal long Offset { get; }

        public byte[] HeaderAob { get; set; }

        /// <summary>
        /// Duration of the sound in milliseconds
        /// </summary>
        public int AudioDuration { get; set; }

        public int Frequency {
            get { return wavFormat?.SampleRate ?? 0; }
        }

        public override WzImageProperty DeepClone() {
            return new WzSoundProperty(Name, AudioDuration, HeaderAob, AudioAob);
        }

        public override void SetValue(object value) { }

        public override void WriteValue(WzBinaryWriter writer) {
            writer.WriteStringValue("Sound_DX8", 0x73, 0x1B);
            writer.Write((byte)0);
            writer.WriteCompressedInt(AudioAob.Length);
            writer.WriteCompressedInt(AudioDuration);
            writer.Write(HeaderAob);
            writer.Write(AudioAob);
        }

        public override void Dispose() {
            AudioAob = null;
        }

        public static string ByteArrayToString(byte[] ba) {
            var sb = new StringBuilder(ba.Length * 3);
            foreach (byte b in ba) {
                sb.AppendFormat("{0:x2} ", b);
            }
            return sb.ToString();
        }

        public void RebuildHeader(WzEncryption? encryption = null) {

            using var bw = new BinaryWriter(new MemoryStream());
            bw.Write(headerAob);

            byte[] wavHeader = StructToBytes(wavFormat);
            if (encryption != null) {
                byte[] aesIvKey = encryption.Value.GetAesIvKey();
                for (int i = 0; i < wavHeader.Length; i++) {
                    wavHeader[i] ^= aesIvKey[i];
                }
            }
            bw.Write((byte)wavHeader.Length);
            bw.Write(wavHeader, 0, wavHeader.Length);
            HeaderAob = ((MemoryStream)bw.BaseStream).ToArray();
        }

        private static byte[] StructToBytes<T>(T obj) {
            byte[] result = new byte[Marshal.SizeOf(obj)];
            var handle = GCHandle.Alloc(result, GCHandleType.Pinned);
            try {
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
                return result;
            } finally {
                handle.Free();
            }
        }

        private static T BytesToStruct<T>(byte[] data) where T : new() {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            } finally {
                handle.Free();
            }
        }

        private static T BytesToStructConstructorless<T>(byte[] data) {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
                Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject(), obj);
                return obj;
            } finally {
                handle.Free();
            }
        }

        private void ParseHeader(WzEncryption? encryption = null) {
            byte[] wavHeader = new byte[HeaderAob.Length - headerAob.Length - 1];
            Buffer.BlockCopy(HeaderAob, headerAob.Length + 1, wavHeader, 0, wavHeader.Length);

            if (wavHeader.Length < Marshal.SizeOf<WaveFormat>()) {
                return;
            }

            var wavFmt = BytesToStruct<WaveFormat>(wavHeader);

            if (Marshal.SizeOf<WaveFormat>() + wavFmt.ExtraSize != wavHeader.Length) {
                if (encryption != null) {
                    byte[] aesIvKey = encryption.Value.GetAesIvKey();
                    for (int i = 0; i < wavHeader.Length; i++) {
                        wavHeader[i] ^= aesIvKey[i];
                    }
                }

                wavFmt = BytesToStruct<WaveFormat>(wavHeader);

                if (Marshal.SizeOf<WaveFormat>() + wavFmt.ExtraSize != wavHeader.Length) {
                    ErrorLogger.Log(ErrorLevel.Critical, "parse sound header failed");
                    return;
                }
            }

            // parse to mp3 header
            if (wavFmt.Encoding == WaveFormatEncoding.MpegLayer3 && wavHeader.Length >= Marshal.SizeOf<Mp3WaveFormat>()) {
                wavFormat = BytesToStructConstructorless<Mp3WaveFormat>(wavHeader);
            } else if (wavFmt.Encoding == WaveFormatEncoding.Pcm) {
                wavFormat = wavFmt;
            } else {
                ErrorLogger.Log(ErrorLevel.MissingFeature, $"Unknown wave encoding {wavFmt.Encoding.ToString()}");
            }
        }

        public void SaveToFile(string file) {
            File.WriteAllBytes(file, AudioAob);
        }
    }
}
