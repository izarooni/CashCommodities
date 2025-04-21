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

using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

using System;
using System.Collections.Generic;

namespace MapleLib.WzLib {

    public abstract class WzImageProperty : WzObject {
        public WzImage ParentImage { get; set; }
        public abstract WzImageProperty DeepClone();
        public abstract void WriteValue(WzBinaryWriter writer);
        public abstract void SetValue(object value);
        public override void Remove() {
            ((WzPropertyContainer)Parent).RemoveProperty(this);
        }

        public override WzObject this[string name] => throw new NotImplementedException();

        internal static void WritePropertyList(WzBinaryWriter writer, ICollection<WzImageProperty> properties) {
            writer.Write((ushort)0);
            writer.WriteCompressedInt(properties.Count);
            foreach (var prop in properties) {
                writer.WriteStringValue(prop.Name, 0, 1);
                if (prop is WzExtended extended) {
                    WriteExtendedValue(writer, extended);
                } else {
                    prop.WriteValue(writer);
                }
            }
        }

        internal static List<WzImageProperty> ParsePropertyList(uint offset, WzBinaryReader reader, WzObject parent, WzImage parentImg) {
            var entryCount = reader.ReadCompressedInt();
            var properties = new List<WzImageProperty>(entryCount);

            for (var i = 0; i < entryCount; i++) {
                var name = reader.ReadStringBlock(offset);
                var ptype = reader.ReadByte();

                if (ptype == 9) {
                    var eob = (int)(reader.ReadUInt32() + reader.BaseStream.Position);
                    WzImageProperty exProp = ParseExtendedProp(reader, offset, eob, name, parent, parentImg);
                    properties.Add(exProp);
                    if (reader.BaseStream.Position != eob) {
                        reader.BaseStream.Position = eob;
                    }
                    continue;
                }

                WzImageProperty prop = ptype switch {
                    0 => new WzNullProperty(name),

                    2 => new WzShortProperty(name, reader.ReadInt16()),
                    11 => new WzShortProperty(name, reader.ReadInt16()),

                    3 => new WzIntProperty(name, reader.ReadCompressedInt()),
                    19 => new WzIntProperty(name, reader.ReadCompressedInt()),

                    20 => new WzLongProperty(name, reader.ReadLong()),

                    4 => new WzFloatProperty(name, reader.ReadByte() == 128 ? reader.ReadSingle() : 0f),

                    5 => new WzDoubleProperty(name, reader.ReadDouble()),

                    8 => new WzStringProperty(name, reader.ReadStringBlock(offset)),
                    _ => throw new Exception("Unknown property type at ParsePropertyList")
                };

                prop.Parent = parent;
                prop.ParentImage = parentImg;
                prop.WzFileParent = parentImg.WzFileParent;

                properties.Add(prop);
            }
            return properties;
        }

        private static WzImageProperty ParseExtendedProp(WzBinaryReader reader, uint offset, int endOfBlock, string name, WzObject parent, WzImage imgParent) {
            byte ptype = reader.ReadByte();
            switch (ptype) {
                case 1:
                case 27:
                    return ExtractMore(reader, offset, endOfBlock, name, reader.ReadStringAtOffset(offset + reader.ReadInt32()), parent, imgParent);
                case 0:
                case 115:
                    return ExtractMore(reader, offset, endOfBlock, name, "", parent, imgParent);
                default:
                    throw new Exception($"Invalid byte read: {ptype} ");
            }
        }

        private static WzImageProperty ExtractMore(WzBinaryReader reader, uint offset, int eob, string name, string iname, WzObject parent, WzImage imgParent) {
            if (iname == "") {
                iname = reader.ReadString();
            }

            WzImageProperty prop = iname switch {
                "Property" => new WzSubProperty(name),
                "Canvas" => new WzCanvasWzProperty(name),
                "Shape2D#Vector2D" => new WzVectorProperty(name, reader),
                "Shape2D#Convex2D" => new WzConvexProperty(name),
                "Sound_DX8" => new WzSoundProperty(name, reader),
                "UOL" => new WzUOLProperty(name),
                _ => throw new ArgumentOutOfRangeException(nameof(iname), iname, null)
            };

            prop.Parent = parent;
            prop.ParentImage = imgParent;
            prop.WzFileParent = imgParent.WzFileParent;

            switch (prop) {
                case WzSubProperty sub:
                    reader.BaseStream.Position += 2; // Reserved?
                    sub.AddProperties(ParsePropertyList(offset, reader, prop, imgParent));
                    break;
                case WzCanvasWzProperty canvas:
                    reader.BaseStream.Position++;
                    if (reader.ReadByte() == 1) {
                        reader.BaseStream.Position += 2;
                        canvas.AddProperties(ParsePropertyList(offset, reader, prop, imgParent));
                    }
                    canvas.PngProperty = new WzPngProperty(reader, imgParent.ParseAudioVisual) {
                        Parent = canvas,
                        ParentImage = imgParent,
                        WzFileParent = imgParent.WzFileParent
                    };
                    break;
                case WzConvexProperty convex:
                    var entries = reader.ReadCompressedInt();
                    for (var i = 0; i < entries; i++) {
                        convex.AddProperty(ParseExtendedProp(reader, offset, 0, name, convex, imgParent));
                    }
                    break;
                case WzUOLProperty uol:
                    reader.BaseStream.Position++;
                    if (reader.ReadByte() == 1) {
                        uol.TargetPath = reader.ReadStringAtOffset(offset + reader.ReadInt32());
                    } else {
                        uol.TargetPath = reader.ReadString();
                    }
                    break;
            }

            return prop;
        }

        private static void WriteExtendedValue(WzBinaryWriter writer, WzExtended property) {
            writer.Write((byte)9);
            var beforePos = writer.BaseStream.Position;
            writer.Write(0); // Placeholder
            property.WriteValue(writer);
            var len = (int)(writer.BaseStream.Position - beforePos);
            var newPos = writer.BaseStream.Position;
            writer.BaseStream.Position = beforePos;
            writer.Write(len - 4);
            writer.BaseStream.Position = newPos;
        }
    }
}
