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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib.WzProperties {
    
    public class WzConvexProperty : WzPropertyContainer {

        public WzConvexProperty() { }

        public WzConvexProperty(string name) {
            Name = name;
        }

        public override void SetValue(object value) {
            throw new NotImplementedException();
        }

        public override WzImageProperty DeepClone() {
            var clone = new WzConvexProperty(Name);
            foreach (var prop in WzProperties) {
                clone.AddProperty(prop.DeepClone());
            }
            return clone;
        }

        public override void WriteValue(WzBinaryWriter writer) {
            var extendedProps = new List<WzExtended>(WzProperties.Count);
            foreach (var prop in WzProperties) {
                if (prop is WzExtended) {
                    extendedProps.Add((WzExtended)prop);
                }
            }
            
            writer.WriteStringValue("Shape2D#Convex2D", 0x73, 0x1B);
            writer.WriteCompressedInt(extendedProps.Count);
            for (int i = 0; i < extendedProps.Count; i++) {
                WzProperties[i].WriteValue(writer);
            }
        }

        public override void Dispose() {
            foreach (var prop in WzProperties) {
                prop.Dispose();
            }
            ClearProperties();
        }
    }
}
