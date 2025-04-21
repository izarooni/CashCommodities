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

namespace MapleLib.WzLib
{
	public abstract class WzPropertyContainer : WzExtended
	{
		private readonly List<WzImageProperty> properties = new List<WzImageProperty>();

		public ReadOnlyCollection<WzImageProperty> WzProperties {
	        get {
		        return new ReadOnlyCollection<WzImageProperty>(properties);
	        }
        }
		public override WzObject this[string name] {
	        get {
		        foreach (var prop in properties) {
			        if (prop.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) {
				        return prop;
			        }
		        }
		        return null;
	        }
        }

		public void AddProperty(WzImageProperty prop) {
			foreach (var p in properties) {
				if (p.Name.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase)) {
					throw new Exception("Property with the same name already exists");
				}
			}

			properties.Add(prop);
			prop.Parent = Parent;
			prop.ParentImage = ParentImage;
			prop.WzFileParent = WzFileParent;

			if (ParentImage != null) ParentImage.Changed = true;
		}

		public void AddProperties(ICollection<WzImageProperty> props) {
			foreach (var prop in props) {
				AddProperty(prop);
			}
		}

		public void RemoveProperty(WzImageProperty prop) {
			if (properties.Remove(prop)) {
				prop.Parent = null;
				prop.ParentImage = null;
				prop.WzFileParent = null;
			}

			if (ParentImage != null) ParentImage.Changed = true;
        }

        public void ClearProperties() {
			foreach (var prop in properties) {
				prop.Parent = null;
				prop.ParentImage = null;
				prop.WzFileParent = null;
			}
			properties.Clear();

            if (ParentImage != null) ParentImage.Changed = true;
        }
    }
}