
namespace BEngineEditor
{
	public class Screen
	{
		protected EditorWindow window;
		protected object[]? additional;

		public void Initialize(EditorWindow window, params object[]? additional)
		{
			this.window = window;
			this.additional = additional;
			Setup();
		}

		protected virtual void Setup() { }
		public virtual void Display() { }
	}
}
