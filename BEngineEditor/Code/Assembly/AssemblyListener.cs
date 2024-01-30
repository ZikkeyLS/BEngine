namespace BEngineEditor
{
	public class AssemblyListener
	{
		private FileSystemWatcher _scriptWatcher;
		private Timer _timer;
		private Project _project;

		private List<string> _filesChanged = new();

		public Action<List<string>> OnScriptsChanged;

		private const int MSDelay = 500;

		private Dictionary<string, FileSystemWatcher> _listeners = new Dictionary<string, FileSystemWatcher>();

		public void InitializeScriptWatch(Project project)
		{
			_project = project;
			_timer = new Timer((e) => OnTimerCallback(), null, 0, MSDelay);
		}

		public void UpdateScriptWatch()
		{
			RecursiveListenersAttach(_project.ProjectAssemblyDirectory);
		}

		private void RecursiveListenersAttach(string directory)
		{
			if (_listeners.ContainsKey(directory) == false)
			{
				_scriptWatcher = new FileSystemWatcher(directory, "*.cs");
				_scriptWatcher.EnableRaisingEvents = true;
				_scriptWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
									   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.CreationTime;
				_scriptWatcher.Created += (a, e) => OnFileChanged(e.FullPath);
				_scriptWatcher.Deleted += (a, e) => OnFileChanged(e.FullPath);
				_scriptWatcher.Changed += (a, e) => OnFileChanged(e.FullPath);
				_scriptWatcher.Renamed += (a, e) => OnFileChanged(e.FullPath);
				_scriptWatcher.Error += (a, e) => DeleteListnerDirectory(directory);
				_scriptWatcher.Disposed += (a, e) => DeleteListnerDirectory(directory);

				_listeners.Add(directory, _scriptWatcher);
			}

			try
			{
				string[] directories = Directory.GetDirectories(directory);
				for (int i = 0; i < directories.Length; i++)
					RecursiveListenersAttach(directories[i]);
			}
			catch
			{

			}
		}

		private void DeleteListnerDirectory(string directory)
		{
			_listeners.Remove(directory);
		}

		private void OnTimerCallback()
		{
			if (_filesChanged.Count != 0)
			{
				OnScriptsChanged.Invoke(_filesChanged);
			}

			_filesChanged.Clear();
		}

		private void OnFileChanged(string fullPath)
		{
			if (_filesChanged.Contains(fullPath) == false)
				_filesChanged.Add(fullPath);
		}
	}
}