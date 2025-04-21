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

namespace MapleLib.WzLib.WzProperties {
    /// <summary>
    /// A property that is stored in the wz file with a byte and possibly followed by a float. If the 
    /// byte is 0, the value is 0, else the value is the float that follows.
    /// </summary>
    public class WzFloatProperty : WzImageProperty {

        public WzFloatProperty() { }

        public WzFloatProperty(string name) {
            Name = name;
        }

        public WzFloatProperty(string name, float value) {
            Name = name;
            Value = value;
        }
        public float Value { get; set; }

        public override string ToString() {
            return $"WzFloatProperty: {Name} = {Value}";
        }

        public override void SetValue(object value) {
            Value = (float)value;
        }

        public override WzImageProperty DeepClone() {
            return new WzFloatProperty(Name, Value);
        }

        public override void WriteValue(WzBinaryWriter writer) {
            writer.Write((byte)4);
            if (Value == 0f) {
                writer.Write((byte)0);
            } else {
                writer.Write((byte)0x80);
                writer.Write(Value);
            }
        }
        public override void Dispose() { }
    }
}
