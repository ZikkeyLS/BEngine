namespace BEngineEditor
{
	public class AssemblyListener
	{
		private FileWatcher _scriptWatcher;
		private Timer _timer;
		private EditorProject _project;

		private List<string> _filesChanged = new();
		public Action<List<string>> OnScriptsChanged;
		private const int MSDelay = 500;

		public void InitializeScriptWatch(EditorProject project)
		{
			_project = project;
			_timer = new Timer((e) => OnTimerCallback(), null, 0, MSDelay);
			_scriptWatcher = new FileWatcher(_project.ProjectAssemblyDirectory, "*.cs");

			_scriptWatcher.Created += OnFileChanged;
			_scriptWatcher.Deleted += OnFileChanged;
			_scriptWatcher.Changed += OnFileChanged;
			_scriptWatcher.Renamed += (o, n) => OnFileChanged(n);
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