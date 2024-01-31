﻿

namespace BEngineEditor 
{
	[Serializable]
	public class AssetData
	{
		public string Path = string.Empty;
		public uint InternalID = 0;

		private AssetData()
		{

		}

		public AssetData(string path, uint id)
		{
			Path = path;
			InternalID = id;
		}
	}
}