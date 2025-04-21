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

//uncomment to enable automatic UOL resolving, comment to disable it

using System.Drawing;
using MapleLib.WzLib.Util;

namespace MapleLib.WzLib.WzProperties {
    /// <summary>
    /// A property that's value is a string
    /// </summary>
    public class WzUOLProperty : WzExtended {

        private WzObject target;
        private string targetPath;

        public WzUOLProperty() { }

        public WzUOLProperty(string name) {
            Name = name;
        }

        public WzUOLProperty(string name, string value) : this(name) {
            TargetPath = value;
        }

        public string TargetPath {
            get {
                return targetPath;
            }
            set {
                target = null;
                targetPath = value;
            }
        }

        public WzObject Target {
            get {
                if (target != null) return target;

                string[] paths = TargetPath.Split('/');
                WzObject start = this;

                foreach (string path in paths) {
                    start = path == ".." ? start.Parent : start[path];
                }
                return target = start;
            }
        }

        public override string ToString() {
            return $"UOL Property: {Name} = {TargetPath}";
        }

        public override void SetValue(object value) {
            TargetPath = (string)value;
        }

        public override WzImageProperty DeepClone() {
            return new WzUOLProperty(Name, TargetPath);
        }

        public override void WriteValue(WzBinaryWriter writer) {
            writer.WriteStringValue("UOL", 0x73, 0x1B);
            writer.Write((byte)0);
            writer.WriteStringValue(TargetPath, 0, 1);
        }

        public override void Dispose() {
            target = null;
        }

        public new string GetString() {
            return Target?.GetString();
        }
        public new short GetShort() {
            return Target.GetShort();
        }
        public new int GetInt() {
            return Target.GetInt();
        }
        public new long GetLong() {
            return Target.GetLong();
        }
        public new float GetFloat() {
            return Target.GetFloat();
        }
        public new double GetDouble() {
            return Target.GetDouble();
        }
        public override Bitmap GetBitmap() {
            return Target?.GetBitmap();
        }
    }
}
