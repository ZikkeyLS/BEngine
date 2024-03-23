using BEngineCore;

namespace BEngineCore
{
	public class AssetWatcher
	{
		private FileWatcher _assetWatcher;
		private Timer _timer;

		private const int MSDelay = 10000;

		public AssetWatcher(string path, AssetWriter writer)
		{
			_assetWatcher = new FileWatcher(path, "*.*");
			_assetWatcher.Created += writer.CreateAsset;
			_assetWatcher.Renamed += writer.RenameAsset;
			_assetWatcher.Deleted += writer.RemoveAsset;

			_timer = new Timer((a) => { writer.UpdateData(); }, null, 0, MSDelay);
		}
	}
}
