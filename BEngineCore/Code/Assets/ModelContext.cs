namespace BEngineCore
{
	public class ModelContext
	{
		private AssetReader _assetReader;

		public readonly List<AssetMetaData> Assets = new();
		public readonly Dictionary<string, Model> Loaded = new();

		public ModelContext(AssetReader reader)
		{
			_assetReader = reader;
		}

		public Model? GetModel(string guid)
		{
			if (Loaded.TryGetValue(guid, out Model? value))
			{
				return value;
			}
			else
			{
				AssetMetaData? asset = _assetReader.GetAsset(guid);
				if (asset == null)
					return null;

				Model result = new Model(asset);
				Loaded.Add(guid, result);
				return result;
			}
		}

		public void AddGUID(AssetMetaData asset)
		{
			Assets.Add(asset);
		}

		public void GUIDMoved(string guid, string newPath)
		{
			if (Loaded.TryGetValue(guid, out Model? value))
			{
				value.Dispose();
				AssetMetaData? asset = _assetReader.GetAsset(guid);
				if (asset == null)
					return;
				Loaded[guid] = new Model(asset);
			}
		}

		public void RemoveGUID(AssetMetaData asset)
		{
			Assets.Remove(asset);
		
			if (Loaded.TryGetValue(asset.GUID, out Model? value))
			{
				value.Dispose();
				Loaded.Remove(asset.GUID);
			}
		}
	}
}