
namespace BEngineScripting
{
	public enum EventID : ushort
	{
		Start,
		Update,
		Destroy,

		EditorStart = 100,
		EditorUpdate,
		EditorDestroy
	}

	public class Script
	{
		public virtual void OnStart()
		{

		}

		public virtual void OnUpdate()
		{

		}

		public virtual void OnDestroy()
		{

		}


		public virtual void OnEditorStart()
		{

		}

		public virtual void OnEditorUpdate()
		{

		}

		public virtual void OnEditorDestroy()
		{

		}
	}
}
