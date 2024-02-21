namespace BEngineCore
{
	public class ModelContext
	{
		private AssetReader _assetReader;

		public readonly List<string> GUIDs = new();
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
				string path = _assetReader.GetAssetPath(guid);
				if (path == string.Empty)
					return null;

				Model result = new Model(path);
				Loaded.Add(guid, result);
				return result;
			}
		}

		public void AddGUID(string guid)
		{
			GUIDs.Add(guid);
		}

		public void GUIDMoved(string guid, string newPath)
		{
			GUIDs.Add(guid);

			if (Loaded.TryGetValue(guid, out Model? value))
			{
				value.Dispose();
				Loaded[guid] = new Model(newPath);
			}
		}

		public void RemoveGUID(string guid)
		{
			Loaded.TryGetValue(guid, out Model? result);
			if (result != null)
			{
				Loaded.Remove(guid);
				result.Dispose();
			}
		}
	}
}