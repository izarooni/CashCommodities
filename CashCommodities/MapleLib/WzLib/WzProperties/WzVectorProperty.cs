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

using System.Drawing;
using System.Xml.Linq;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib.WzProperties {
    /// <summary>
    /// A property that contains an x and a y value
    /// </summary>
    public class WzVectorProperty : WzExtended {

        public WzVectorProperty() { }

        public WzVectorProperty(string name) {
            Name = name;
        }

        public WzVectorProperty(string name, WzBinaryReader reader) : this(name) {
            X = new WzIntProperty("X", reader.ReadCompressedInt()) {
                Parent = this,
                ParentImage = ParentImage,
                WzFileParent = WzFileParent
            };
            Y = new WzIntProperty("Y", reader.ReadCompressedInt()) {
                Parent = this,
                ParentImage = ParentImage,
                WzFileParent = WzFileParent
            };
        }

        public WzVectorProperty(string name, WzIntProperty x, WzIntProperty y) : this(name) {
            X = x;
            Y = y;
        }

        public WzIntProperty X { get; set; }

        public WzIntProperty Y { get; set; }

        public Point Point {
            get {
                return new Point(X.Value, Y.Value);
            }
        }

        public override string ToString() {
            return $"WzVectorProperty: {Name} = ({X.Value}, {Y.Value})";
        }

        public override void SetValue(object value) {
            if (value is Point) {
                X.Value = ((Point)value).X;
                Y.Value = ((Point)value).Y;
            } else {
                X.Value = ((Size)value).Width;
                Y.Value = ((Size)value).Height;
            }
        }

        public override WzImageProperty DeepClone() {
            return new WzVectorProperty(Name, X, Y);
        }

        public override void WriteValue(WzBinaryWriter writer) {
            writer.WriteCompressedInt(X.Value);
            writer.WriteCompressedInt(Y.Value);
        }

        public override void Dispose() {
            X.Dispose();
            Y.Dispose();
            X = null;
            Y = null;
        }
    }
}
