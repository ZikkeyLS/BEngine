
namespace BEngineCore
{
	public class ProjectAbstraction
	{
		public static ProjectAbstraction LoadedProject { get; private set; }

		protected Graphics graphics;
		protected Input input;
		protected Time time = new();
		protected Scripting scripting = new();
		protected AssetReader reader;
		protected Logger logger = new(true);
		protected Scene loadedScene;

		public Scripting Scripting => scripting;
		public AssetReader AssetsReader => reader;
		public Logger Logger => logger;
		public Scene LoadedScene => loadedScene;
		public Graphics Graphics => graphics;
		public Input Input => input;
		public Time Time => time;

		public ProjectAbstraction(EngineWindow window)
		{
			graphics = window.Graphics;
			input = window.Input;
		}

		public void LoadProject()
		{
			LoadedProject = this;

			LoadProjectData();
		}

		public virtual void LoadProjectData()
		{

		}

		public Scene? TryLoadScene(string metaID, bool fastLoad = false, bool savePrevious = true)
		{
			Scene? scene = AssetData.Load<Scene>(metaID, this);

			if (scene != null)
			{
				if (fastLoad)
				{
					if (savePrevious)
						loadedScene?.Save<Scene>();
					loadedScene = scene;
					loadedScene.LoadScene();
					OnSceneLoaded();
				}
				else
				{
					OnSceneLongLoad(scene);
				}
			}

			return scene;
		}

		public void TryLoadScene(Scene scene, bool fastLoad = false)
		{
			if (fastLoad)
			{
				loadedScene?.Save<Scene>();
				loadedScene = scene;
				loadedScene.LoadScene();
				OnSceneLoaded();
			}
			else
			{
				OnSceneLongLoad(scene);
			}
		}

		public virtual void OnSceneLoaded()
		{

		}

		public virtual void OnSceneLongLoad(Scene scene)
		{

		}
	}
}
