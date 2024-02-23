
namespace BEngine
{
	[Serializable]
	public class Entity : IDisposable
	{
		public string Name;
		public List<Script> Scripts = new();
		private List<Script> _scriptCopy = new List<Script>();

		public void Log(string message)
		{
			Logger.LogMessage(message);
		}
	
		public void CallEvent(EventID id)
		{
			switch (id)
			{
				case EventID.Start:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnStart();
					break;
				case EventID.Update:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnUpdate();
					break;
				case EventID.Destroy:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnDestroy();
					break;
				case EventID.EditorStart:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorStart();
					break;
				case EventID.EditorUpdate:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorUpdate();
					break;
				case EventID.EditorDestroy:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorDestroy();
					break;
				case EventID.EditorSelected:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorSelected();
					break;
			}
		}

		public T GetScript<T>() where T : Script
		{
			foreach (Script script in Scripts)
			{
				if (script is T)
					return (T)script;
			}

			return null;
		}

		public void MakeScriptsCopy()
		{
			for (int i = 0; i < Scripts.Count; i++)
				_scriptCopy.Add((Script)Scripts[i].Clone());
		}

		public void LoadScriptsCopy()
		{
			Scripts = _scriptCopy;
		}

		public void Dispose()
		{
			for (int i = 0; i < Scripts.Count; i++)
				Scripts[i].Dispose();

			for (int i = 0; i < _scriptCopy.Count; i++)
				_scriptCopy[i].Dispose();
		}
	}
}
