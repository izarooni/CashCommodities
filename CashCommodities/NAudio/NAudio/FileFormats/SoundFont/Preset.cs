using System;

namespace NAudio.SoundFont 
{
	/// <summary>
	/// A SoundFont Preset
	/// </summary>
	public class Preset 
	{
		private string name;
		private ushort patchNumber;
		private ushort bank;
		internal ushort startPresetZoneIndex;
		internal ushort endPresetZoneIndex;
		internal uint library;
		internal uint genre;
		internal uint morphology;
		private Zone[] zones;
		
		/// <summary>
		/// Preset name
		/// </summary>
		public string Name 
		{
			get 
			{
				return name;
			}
			set 
			{
				// TODO: validate
				name = value;
			}			
		}
		
		/// <summary>
		/// Patch Number
		/// </summary>
		public ushort PatchNumber 
		{
			get 
			{
				return patchNumber;			
			}
			set 
			{
				// TODO: validate
				patchNumber = value;
			}
		}
		
		/// <summary>
		/// Bank number
		/// </summary>
		public ushort Bank 
		{
			get 
			{
				return bank;
			}
			set 
			{
				// 0 - 127, GM percussion bank is 128
				// TODO: validate
				bank = value;
			}
		}
		
		/// <summary>
		/// Zones
		/// </summary>
		public Zone[] Zones
		{
			get
			{
				return zones;
			}
			set
			{
				zones = value;
			}
		}

		/// <summary>
		/// <see cref="Object.ToString"/>
		/// </summary>
		public override string ToString() 
		{
			return String.Format("{0}-{1} {2}",bank,patchNumber,name);
		}
	}
}