
namespace BEngineScripting
{
	public class Entity
	{
		private Script[] _scripts;
		private List<Script> _scriptCopy = new List<Script>();
	
		public void CallEvent(EventID id)
		{
			switch (id)
			{
				case EventID.Start:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnStart();
					break;
				case EventID.Update:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnUpdate();
					break;
				case EventID.Destroy:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnDestroy();
					break;
				case EventID.EditorStart:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnEditorStart();
					break;
				case EventID.EditorUpdate:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnEditorUpdate();
					break;
				case EventID.EditorDestroy:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnEditorDestroy();
					break;
				case EventID.EditorSelected:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnEditorSelected();
					break;
			}
		}

		public void MakeScriptsCopy()
		{
			for (int i = 0; i < _scripts.Length; i++)
				_scriptCopy.Add((Script)_scripts[i].Clone());
		}

		public void LoadScriptsCopy()
		{
			_scripts = _scriptCopy.ToArray();
		}
	}
}
