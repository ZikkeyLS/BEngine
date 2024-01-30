
namespace BEngineEditor
{
	public class FileWatcher
	{
		private Dictionary<string, FileSystemWatcher> _listeners = new Dictionary<string, FileSystemWatcher>();

		private Timer _timer;
		private string _directory;
		private string _filter;

		public Action<string> Created;
		public Action<string> Deleted;
		public Action<string> Changed;
		public Action<string> Renamed;
		public Action<string> Error;
		public Action Disposed;

		public FileWatcher(string directory, string filter = "*.*", int updateMS = 500)
		{
			_directory = directory;
			_filter = filter;
			_timer = new Timer((e) => UpdateScriptWatch(), null, 0, updateMS);
		}

		private void UpdateScriptWatch()
		{
			RecursiveListenersAttach(_directory);
		}

		private void RecursiveListenersAttach(string directory)
		{
			if (_listeners.ContainsKey(directory) == false)
			{
				FileSystemWatcher subWatcher = new FileSystemWatcher(directory, _filter);
				subWatcher.EnableRaisingEvents = true;
				subWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
									   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.CreationTime;
				subWatcher.Created += (a, e) => Created?.Invoke(e.FullPath);
				subWatcher.Deleted += (a, e) => Deleted?.Invoke(e.FullPath);
				subWatcher.Changed += (a, e) => Changed?.Invoke(e.FullPath);
				subWatcher.Renamed += (a, e) => Renamed?.Invoke(e.FullPath);
				subWatcher.Error += (a, e) => 
				{ 
					DeleteListenerDirectory(directory);  
					Error?.Invoke(e.GetException().Message); 
				};
				subWatcher.Disposed += (a, e) =>
				{
					DeleteListenerDirectory(directory);
					Disposed?.Invoke();
				};

				_listeners.Add(directory, subWatcher);
			}

			try
			{
				string[] directories = Directory.GetDirectories(directory);
				for (int i = 0; i < directories.Length; i++)
				{
					DirectoryInfo current = new DirectoryInfo(directories[i]);
					if (current.Name != "bin" && current.Name != "obj")
						RecursiveListenersAttach(directories[i]);
				}
			}
			catch
			{

			}
		}

		private void DeleteListenerDirectory(string directory)
		{
			_listeners.Remove(directory);
		}
	}
}
