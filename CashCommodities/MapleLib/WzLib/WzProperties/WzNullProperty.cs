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
    /// <summary>
    /// A property that's value is null
    /// </summary>
    public class WzNullProperty : WzImageProperty {

        public WzNullProperty() { }

        public WzNullProperty(string propName) {
            Name = propName;
        }

        public override void SetValue(object value) {
            throw new InvalidOperationException("Cannot set value to WzNullProperty");
        }

        public override WzImageProperty DeepClone() {
            return new WzNullProperty(Name);
        }

        public override void WriteValue(Util.WzBinaryWriter writer) {
            writer.Write((byte)0);
        }

        public override void Dispose() {
            Name = null;
        }
    }
}
