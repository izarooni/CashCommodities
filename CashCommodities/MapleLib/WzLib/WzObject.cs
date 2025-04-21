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
using System.Drawing;
using MapleLib.WzLib.WzProperties;

namespace MapleLib.WzLib {

    /// <summary>
    /// An abstract class for wz objects
    /// </summary>
    public abstract class WzObject : IDisposable {

        public abstract void Dispose();

        public string Name {
            get;
            set;
        }

        public WzObject Parent { get; set; }

        public WzFile WzFileParent { get; set; }

        public abstract WzObject this[string name] { get; }

        public string FullPath {
            get {
                if (this is WzFile) {
                    return ((WzFile)this).WzDirectory.Name;
                }
                string result = Name;
                var currObj = this;
                while (currObj.Parent != null) {
                    currObj = currObj.Parent;
                    result = currObj.Name + @"\" + result;
                }
                return result;
            }
        }

        public WzObject GetChildFromPath(string path) {
            string[] seperatedPath = path.Split("/".ToCharArray());

            WzObject currentObject = this;
            foreach (string level in seperatedPath) {
                if (level == "..") {
                    currentObject = currentObject.Parent;
                    continue;
                }

                switch (currentObject) {
                    case WzFile file:
                        currentObject = file.WzDirectory[level];
                        break;
                    case WzDirectory directory:
                        currentObject = directory[level];
                        break;
                    case WzImage img:
                        currentObject = img[level];
                        break;
                    case WzImageProperty imgProp:
                        currentObject = imgProp[level];
                        break;
                    default:
                        return null;
                }
            }
            return currentObject;
        }

        public abstract void Remove();

        public string GetString() {
            return this is WzStringProperty prop ? prop.Value : throw new Exception("Not a string property");
        }

        public short GetShort() {
            return this is WzShortProperty prop ? prop.Value : throw new Exception("Not a short property");
        }

        public int GetInt() {
            return this is WzIntProperty prop ? prop.Value : throw new Exception("Not an int property");
        }

        public long GetLong() {
            return this is WzLongProperty prop ? prop.Value : throw new Exception("Not a long property");
        }

        public float GetFloat() {
            return this is WzFloatProperty prop ? prop.Value : throw new Exception("Not a float property");
        }

        public double GetDouble() {
            return this is WzDoubleProperty prop ? prop.Value : throw new Exception("Not a double property");
        }
        public virtual Bitmap GetBitmap() {
            return this is WzCanvasWzProperty prop ? prop.PngProperty.GetPng(false) : throw new Exception("Not a canvas image");
        }
    }
}
