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
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib.WzProperties {

    public class WzSubProperty : WzPropertyContainer {

        public WzSubProperty() { }

        public WzSubProperty(string name) {
            Name = name;
        }

        public override string ToString() {
            return $"WzSubWzProperty: {Name} ({WzProperties.Count} properties)";
        }

        public override WzImageProperty DeepClone() {
            var clone = new WzSubProperty(Name);
            foreach (var prop in WzProperties) {
                clone.AddProperty(prop.DeepClone());
            }
            return clone;
        }

        public override void SetValue(object value) {
            throw new NotImplementedException("Can't set value of WzSubProperty");
        }

        public override void WriteValue(WzBinaryWriter writer) {
            writer.WriteStringValue("Property", 0x73, 0x1B);
            WritePropertyList(writer, WzProperties);
        }

        public override void Dispose() {
            foreach (var prop in WzProperties) {
                prop.Dispose();
            }
            ClearProperties();
        }
    }
}
