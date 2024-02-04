
namespace BEngine
{
	public enum EventID : ushort
	{
		Start,
		Update,
		Destroy,

		EditorStart = 100,
		EditorUpdate,
		EditorDestroy,
		EditorSelected
	}

	public class Script : ICloneable
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

		public virtual void OnEditorSelected()
		{

		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
