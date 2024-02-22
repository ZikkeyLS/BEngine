using BEngine;
using System.Reflection;

namespace BEngineCore
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

		public Scene(string sceneName, ProjectAbstraction project, string guid) : base(guid, project)
		{
			SceneName = sceneName;
		}

		protected override void OnPreSave()
		{
			foreach (SceneEntity entity in Entities)
			{
				foreach (Script script in entity.Entity.Scripts)
				{
					FieldInfo[] fields = script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

					foreach(FieldInfo field in fields)
					{
						if (field.IsInitOnly)
							continue;

						SceneScript? resultScript = entity.Scripts.Find((sceneScript) => sceneScript.Name == script.GetType().Name 
							&& sceneScript.Namespace == script.GetType().Namespace);

						if (resultScript == null)
							continue;

						object? resultValue = field.GetValue(script);

						if (resultValue == null)
							continue;

						if (resultScript.ContainsField(field.Name))
						{
							resultScript.ChangeField(field.Name, resultValue);
						}
						else
						{
							resultScript.AddField(field.Name, resultValue);
						}
					}
				}
			}
		}

		public void LoadScene()
		{
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities[i].LoadInheritance();
				Entities[i].LoadScripts(Project.Scripting);
			}
		}

		public void ReloadScripts()
		{
			Save<Scene>();

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
				if (Entities[i].Parent == entity)
				{
					removeAlso.Add(Entities[i]);
				}

				Entities[i].RemoveChild(entity);
			}

			Entities.Remove(entity);

			for (int i = 0; i < removeAlso.Count; i++)
			{
				RemoveEntity(removeAlso[i]);
			}
		}

		public SceneEntity CreateEntity(string name, SceneEntity? parent = null)
		{
			SceneEntity entity = new SceneEntity(name) { Parent = parent };
			Entities.Add(entity);
			return entity;
		}

		public SceneEntity? GetEntity(string guid)
		{
			return Entities.Find((sceneEntity) => sceneEntity.GUID == guid);
		}
	}
}
