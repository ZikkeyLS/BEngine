
namespace BEngine
{
	public enum EventID : ushort
	{
		Start,
		Update,
		Destroy,
		FixedUpdate,

		EditorStart = 100,
		EditorUpdate,
		EditorFixedUpdate,
		EditorDestroy,
		EditorSelected,
	}

	public class Script : ICloneable, IDisposable
	{
		public readonly Entity Entity;

		public virtual void OnStart()
		{
			
		}

		public virtual void OnUpdate()
		{

		}

		public virtual void OnFixedUpdate()
		{

		}

		public virtual void OnDestroy()
		{

		}


		public virtual void OnEditorStart()
		{

		}

		public virtual void OnEditorFixedUpdate()
		{

		}

		public virtual void OnEditorUpdate()
		{

		}

		public virtual void OnEditorDestroy()
		{

		}

		public virtual void OnEditorSelected()
		{

		}

		public void Log(string message)
		{
			Logger.LogMessage(message);
		}

		public T GetScript<T>() where T : Script
		{
			return Entity.GetScript<T>();
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void Dispose()
		{

		}
	}
}
