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

namespace MapleLib.WzLib.WzProperties
{
	/// <summary>
	/// A property that is stored in the wz file with a signed byte and possibly followed by an int. If the 
	/// signed byte is equal to -128, the value is the int that follows, else the value is the byte.
	/// </summary>
	public class WzIntProperty : WzImageProperty
	{

		public WzIntProperty() { }

		public WzIntProperty(string name) {
			Name = name;
		}

		public WzIntProperty(string name, int value) : this(name) {
			Value = value;
		}
		
		public int Value { get; set; }

        public override string ToString() {
            return $"WzIntProperty: {Name} = {Value}";
        }

        public override void SetValue(object value)
        {
            Value = System.Convert.ToInt32(value);
        }

		public override WzImageProperty DeepClone()
        {
            return new WzIntProperty(Name, Value);
        }

		public override void WriteValue(Util.WzBinaryWriter writer)
		{
			writer.Write((byte)3);
			writer.WriteCompressedInt(Value);
		}

		public override void Dispose()
		{
		}
	}
}