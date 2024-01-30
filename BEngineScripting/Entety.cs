
namespace BEngineScripting
{
	public class Entety
	{
		private Script[] _scripts;

		public void CallEvent(EventID id, bool runtime = true)
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
						_scripts[i].OnStart();
					break;
				case EventID.EditorUpdate:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnUpdate();
					break;
				case EventID.EditorDestroy:
					for (int i = 0; i < _scripts.Length; i++)
						_scripts[i].OnDestroy();
					break;
			}
		}
	}
}
