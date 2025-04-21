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
using System;
using System.Collections.ObjectModel;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;
using System.Linq;

namespace MapleLib.WzLib {
    public class WzImage : WzPropertyContainer {

        internal long TempFileStart = 0;
        internal long TempFileEnd = 0;

        public WzImage() { }
        public WzImage(string name) {
            Name = name;
        }
        public WzImage(string name, Stream dataStream, WzEncryption encryption) {
            Name = name;
            WzReader = new WzBinaryReader(dataStream, encryption.GetAesIvKey());
        }

        public bool Parsed { get; set; }
        public bool Changed { get; set; }
        public int BlockSize { get; set; }
        public int Checksum { get; set; }
        public uint Offset { get; set; } = 0;
        public bool ParseAudioVisual { get; set; }
        public WzBinaryReader WzReader { get; set; }

        public override string ToString() {
            return $"WzImage: {Name} ({WzProperties.Count} properties)";
        }

        public override void Dispose() {
            WzReader = null;
            Parsed = false;
            Changed = false;
            ClearProperties();
        }
        
        public new WzImageProperty this[string name] {
            get {
                if (WzReader != null && !Parsed) {
                    ParseImage();
                }
                foreach (var prop in WzProperties) {
                    if (String.Equals(prop.Name, name, StringComparison.CurrentCultureIgnoreCase)) {
                        return prop;
                    }
                }
                return null;
            }
            set {
                if (value == null) {
                    return;
                }
                value.Name = name;
                AddProperty(value);
            }
        }

        public override WzImageProperty DeepClone() {
            var img = new WzImage(Name);
            foreach (var p in WzProperties) {
                img.AddProperty(p.DeepClone());
            }
            return img;
        }
        
        public override void WriteValue(WzBinaryWriter writer) {
            throw new NotImplementedException();
        }
        public override void SetValue(object value) {
            throw new NotImplementedException();
        }
        
        public override void Remove() {
            ((WzDirectory)Parent).RemoveImage(this);
        }

        public void ParseImage() {
            if (Parsed) {
                return;
            }
            WzReader.BaseStream.Position = Offset;
            var b = WzReader.ReadByte();
            if (b != 0x73 || WzReader.ReadString() != "Property" || WzReader.ReadUInt16() != 0) {
                return;
            }
            AddProperties(ParsePropertyList(Offset, WzReader, this, this));
            Parsed = true;
        }

        public void WriteImage(WzBinaryWriter writer) {
            if (WzReader != null && !Parsed) {
                ParseImage();
            }
            if (Changed) {
                var imgProp = new WzSubProperty();
                var startPos = writer.BaseStream.Position;
                imgProp.AddProperties(WzProperties.Select(p => p.DeepClone()).ToList());
                imgProp.WriteValue(writer);
                writer.StringCache.Clear();
                BlockSize = (int)(writer.BaseStream.Position - startPos);
            } else {
                var pos = WzReader.BaseStream.Position;
                WzReader.BaseStream.Position = Offset;
                writer.Write(WzReader.ReadBytes(BlockSize));
                WzReader.BaseStream.Position = pos;
            }
        }
    }
}
