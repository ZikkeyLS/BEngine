namespace BEngineEditor
{
	public class AssemblyListener
	{
		private Project _project;
		private FileSystemWatcher _scriptWatcher;
		private Timer _timer;

		private List<string> _filesChanged = new();

		public Action<List<string>> OnScriptsChanged;

		private const int MSDelay = 1000;

		public void StartWatchOnScripts(Project project)
		{
			_project = project;

			_scriptWatcher = new FileSystemWatcher(_project.ProjectAssemblyDirectory, "*.cs");
			_scriptWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
								   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			_scriptWatcher.Created += (a, e) => OnFileChanged(e.FullPath);
			_scriptWatcher.Deleted += (a, e) => OnFileChanged(e.FullPath);
			_scriptWatcher.Changed += (a, e) => OnFileChanged(e.FullPath);
			_scriptWatcher.EnableRaisingEvents = true;

			_timer = new Timer((e) => OnTimerCallback(), null, 0, MSDelay);
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