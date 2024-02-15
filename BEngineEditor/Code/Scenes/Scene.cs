using BEngine;

namespace BEngineEditor
{
	public class Scene : AssetData
	{
		public string SceneName { get; set; } = "New Scene";
		public List<SceneEntity> Entities { get; set; } = new();

		public Scene()
		{

		}

		public Scene(string sceneName)
		{
			SceneName = sceneName;
		}

		public Scene(string sceneName, EditorProject project, string guid) : base(guid, project)
		{
			SceneName = sceneName;
		}

		public void LoadScene()
		{
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities[i].LoadScripts(Project.Scripting);
			}
		}

		public void ReloadScripts()
		{
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities[i].ReloadScripts(Project.Scripting);
			}
		}

		public void CallEvent(EventID id)
		{
			foreach (SceneEntity entity in Entities)
			{
				entity.Entity.CallEvent(id);
			}
		}

		public void RemoveEntity(SceneEntity entity)
		{
			List<SceneEntity> removeAlso = new();
			for (int i = 0; i < Entities.Count; i++)
			{
				if (Entities[i].Parent == entity.GUID)
				{
					removeAlso.Add(Entities[i]);
				}

				if (Entities[i].Children.Contains(entity.GUID))
				{
					Entities[i].Children.Remove(entity.GUID);
				}
			}

			Entities.Remove(entity);

			for (int i = 0; i < removeAlso.Count; i++)
			{
				RemoveEntity(removeAlso[i]);
			}
		}

		public SceneEntity CreateEntity(string name, SceneEntity? parent = null)
		{
			SceneEntity entity = new SceneEntity(name) { Parent = parent?.GUID };
			Entities.Add(entity);
			return entity;
		}

		public SceneEntity? GetEntity(string guid)
		{
			return Entities.Find((sceneEntity) => sceneEntity.GUID == guid);
		}
	}
}
