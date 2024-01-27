
namespace BEngineEditor
{
	public class Screen
	{
		protected EditorWindow window;

		public void Initialize(EditorWindow window)
		{
			this.window = window;
			Setup();
		}

		protected virtual void Setup() { }
		public virtual void Display() { }
	}
}
