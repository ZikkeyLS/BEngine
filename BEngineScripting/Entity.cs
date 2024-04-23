
namespace BEngine
{
	[Serializable]
	public class Entity : IDisposable
	{
		public readonly string GUID;

		public string Name;
		internal List<Script> Scripts = new();
		private List<Script> _scriptCopy = new List<Script>();

		internal void CallEventLocal(EventID id, Script script)
		{
			switch (id)
			{
				case EventID.Start:
					script.OnStart();
					break;
				case EventID.Update:
					script.OnUpdate();
					break;
				case EventID.Destroy:
					script.OnDestroy();
					break;
				case EventID.EditorStart:
					script.OnEditorStart();
					break;
				case EventID.EditorUpdate:
					script.OnEditorUpdate();
					break;
				case EventID.EditorDestroy:
					script.OnEditorDestroy();
					break;
				case EventID.EditorSelected:
					script.OnEditorSelected();
					break;
			}
		}

		internal void CallEvent(EventID id)
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
				case EventID.FixedUpdate:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnFixedUpdate();
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
				case EventID.EditorFixedUpdate:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorFixedUpdate();
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

		internal void MakeScriptsCopy()
		{
			for (int i = 0; i < Scripts.Count; i++)
				_scriptCopy.Add((Script)Scripts[i].Clone());
		}

		internal void LoadScriptsCopy()
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
