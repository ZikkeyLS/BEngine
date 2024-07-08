using BEngineScripting;

namespace BEngine
{
	[Serializable]
	public class Entity : IDisposable
	{
		public readonly string GUID;
		public readonly Entity Parent;
		public readonly IReadOnlyList<Entity> Chilren;

		public string Name;
		internal List<Script> Scripts = new();
		private List<Script> _scriptCopy = new List<Script>();

		internal void CallEventLocal(EventID id, Script script, List<CachedScript>? cachedScripts = null)
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
					if (cachedScripts != null && RunInAny("OnStart", script, cachedScripts))
						script.OnStart();
					break;
				case EventID.EditorUpdate:
					if (cachedScripts != null && RunInAny("OnUpdate", script, cachedScripts))
						script.OnUpdate();
					break;
				case EventID.EditorDestroy:
					if (cachedScripts != null && RunInAny("OnDestroy", script, cachedScripts))
						script.OnDestroy();
					break;
				case EventID.EditorSelected:
					script.OnEditorSelected();
					break;
			}
		}

		internal void CallEvent(EventID id, List<CachedScript>? cachedScripts = null)
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
						if (cachedScripts != null && RunInAny("OnStart", Scripts[i], cachedScripts))
							Scripts[i]?.OnStart();
					break;
				case EventID.EditorUpdate:
					for (int i = 0; i < Scripts.Count; i++)
						if (cachedScripts != null && RunInAny("OnUpdate", Scripts[i], cachedScripts))
							Scripts[i]?.OnUpdate();
					break;
				case EventID.EditorFixedUpdate:
					for (int i = 0; i < Scripts.Count; i++)
						if (cachedScripts != null && RunInAny("OnFixedUpdate", Scripts[i], cachedScripts))
							Scripts[i]?.OnFixedUpdate();
					break;
				case EventID.EditorDestroy:
					for (int i = 0; i < Scripts.Count; i++)
						if (cachedScripts != null && RunInAny("OnDestroy", Scripts[i], cachedScripts))
							Scripts[i]?.OnDestroy();
					break;
				case EventID.EditorSelected:
					for (int i = 0; i < Scripts.Count; i++)
						Scripts[i]?.OnEditorSelected();
					break;
			}
		}

		private bool RunInAny(string methodName, Script script, List<CachedScript> cachedScripts)
		{
			return cachedScripts?.Find((cached) => cached.Name == script.GetType().Name && cached.Name == script.GetType().Name)?
				.Methods.Find((method) => method.Name == methodName)?.Attributes.Contains(nameof(RunInAny)) == true;
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
