using BEngine;

namespace BEngineCore
{
	public class ProjectAbstraction
	{
		public static ProjectAbstraction LoadedProject { get; private set; }

		protected Graphics graphics;
		protected Physics physics;
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
		public Physics Physics => physics;
		public Input Input => input;
		public Time Time => time;

		public bool Runtime { get; private set; } = false;
		public bool Pause { get; private set; } = false;
		public bool Editor { get; private set; } = false;

		public ProjectAbstraction(EngineWindow window, bool editor = false)
		{
			physics = window.Physics;
			graphics = window.Graphics;
			input = window.Input;
			Editor = editor;
		}

		public void LoadProject()
		{
			LoadedProject = this;
			LoadProjectData();
		}

		public void SwipeRuntime()
		{
			if (Runtime)
			{
				StopRuntime();
			}
			else
			{
				StartRuntime();
			}
		}

		public void SwipePause()
		{
			Pause = !Pause;
		}

		public void SetEditor(bool editor)
		{
			Editor = editor;
		}

		public void StartRuntime()
		{
			Runtime = true;
			LoadedScene?.CallEvent(EventID.Start);
		}

		public void StopRuntime()
		{
			Runtime = false;
			// just temp try to reload scene
			physics.ClearInstance();
			TryLoadScene(loadedScene.GUID, true, false);

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
					if (Runtime)
					{
						StopRuntime();
					}

					BaseLoadScene(scene, savePrevious);
				}
				else
				{
					OnSceneLongLoad(scene);
				}
			}

			return scene;
		}

		public void TryLoadScene(Scene scene, bool fastLoad = false, bool savePrevious = true)
		{
			if (fastLoad)
			{
				BaseLoadScene(scene, savePrevious);
			}
			else
			{
				OnSceneLongLoad(scene);
			}
		}

		private void BaseLoadScene(Scene scene, bool savePrevious)
		{
			if (savePrevious)
				loadedScene?.Save<Scene>();

			graphics.ResetCameraHandler();

			loadedScene = scene;
			loadedScene.LoadScene();
			OnSceneLoaded();
		}

		public virtual void OnSceneLoaded()
		{

		}

		public virtual void OnSceneLongLoad(Scene scene)
		{

		}
	}
}
