namespace BEngineCore
{
	public class ShaderContext
	{
		private AssetReader _assetReader;
		private Dictionary<string, string> _shaders = new Dictionary<string, string>();

		public ShaderContext(AssetReader assetReader) 
		{
			_assetReader = assetReader;	
		}

		public void Add(string guid, string data)
		{
			_shaders.Add(guid, data);
		}

		public void Move(string oldGUID, string newGUID)
		{
			string data = _shaders[oldGUID];

			_shaders.Remove(oldGUID);
			_shaders.Add(newGUID, data);
		}

		public void Remove(string guid)
		{
			_shaders.Remove(guid);
		}

		public string? GetShaderData(string path)
		{
			foreach (string key in _shaders.Keys)
			{
				var asset = _assetReader.GetAsset(key);

				if (asset == null)
					continue;

				path = path.Replace("/", "\\");
				if (asset.GetAssetPath().Contains(path))
				{
					return _shaders[key];
				}
			}

			return null;
		}
	}
}