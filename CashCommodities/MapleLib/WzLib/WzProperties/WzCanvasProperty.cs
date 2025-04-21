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

namespace MapleLib.WzLib.WzProperties {
    
    public class WzCanvasWzProperty : WzPropertyContainer {

        public override string ToString() {
            string parsed = PngProperty != null ? "Parsed" : "Unparsed";
            return $"WzCanvasWzProperty: {Name} ({parsed})";
        }

        public override void SetValue(object value) {
            PngProperty.SetValue(value);
        }

        public override WzImageProperty DeepClone() {
            var clone = new WzCanvasWzProperty(Name);
            foreach (var prop in WzProperties) {
                clone.AddProperty(prop.DeepClone());
            }
            clone.PngProperty = (WzPngProperty)PngProperty.DeepClone();
            return clone;
        }

        public override void WriteValue(Util.WzBinaryWriter writer) {
            writer.WriteStringValue("Canvas", 0x73, 0x1B);
            writer.Write((byte)0);
            if (WzProperties.Count > 0) {
                writer.Write((byte)1);
                WritePropertyList(writer, WzProperties);
            } else {
                writer.Write((byte)0);
            }
            writer.WriteCompressedInt(PngProperty.Width);
            writer.WriteCompressedInt(PngProperty.Height);
            writer.WriteCompressedInt(PngProperty.format);
            writer.Write((byte)PngProperty.format2);
            writer.Write(0);
            byte[] bytes = PngProperty.GetCompressedBytes(false);
            writer.Write(bytes.Length + 1);
            writer.Write((byte)0);
            writer.Write(bytes);
        }
        
        public override void Dispose() {
            PngProperty.Dispose();
            PngProperty = null;
            foreach (var prop in WzProperties) {
                prop.Dispose();
            }
        }

        public WzPngProperty PngProperty { get; set; }
        
        public WzCanvasWzProperty() { }
        
        public WzCanvasWzProperty(string name) {
            Name = name;
        }
    }
}
