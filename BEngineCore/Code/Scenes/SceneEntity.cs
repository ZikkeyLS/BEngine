using BEngine;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BEngineCore
{
	public class SceneEntity : IDisposable
	{
		public string GUID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		public string? Parent { get; set; }
		public List<string> Children { get; set; } = new();
		public List<SceneScript> Scripts { get; set; } = new();

		private Scene _scene => ProjectAbstraction.LoadedProject.LoadedScene;

		[JsonIgnore]
		public Entity Entity { get; set; } = new();

		public SceneEntity() { }

		public SceneEntity(string name)
		{
			GUID = Guid.NewGuid().ToString();
			Name = name;
			Entity.Name = name;
		}

		public bool ChildOf(SceneEntity entity)
		{
			return IsChildOf(this, entity);
		}

		private bool IsChildOf(SceneEntity current, SceneEntity result)
		{
			if (current.Parent == result.Parent && current.Parent != null)
				return true;

			if (current.Parent != null)
				return IsChildOf(_scene.GetEntity(current.Parent), result);

			return false;
		}

		public void AddScript(Scripting.CachedScript script)
		{
			Scripts.Add(new SceneScript() { Name = script.Name, Namespace = script.Namespace });
			CreateInstanseOf(script);
		}

		public bool RenameScript(SceneScript sceneScript, Scripting.CachedScript script)
		{
			bool success = true;

			foreach (SceneScriptField scriptField in sceneScript.Fields)
			{
				if (script.Fields.Find(cached => cached.Name == scriptField.Name) == null)
				{
					success = false;
					break;
				}
			}

			if (success)
			{
				sceneScript.Name = script.Name;
				sceneScript.Namespace = script.Namespace;
				CreateInstanseOf(script);
			}

			return success;
		}

		public void RemoveScript(SceneScript script)
		{
			Scripts.Remove(script);

			Script? removeRuntime = Entity.Scripts.Find((current) => current.GetType().Name == script.Name &&
				current.GetType().Namespace == script.Namespace);

			if (removeRuntime != null)
			{
				Entity.Scripts.Remove(removeRuntime);
				removeRuntime.Dispose();
			}

			script.Dispose();
		}

		public Script? GetScriptInstance(SceneScript sceneScipt)
		{
			for (int i = 0; i < Entity.Scripts.Count; i++)
			{
				Type scriptType = Entity.Scripts[i].GetType();
				Script currentScript = Entity.Scripts[i];
				if (scriptType.Name == sceneScipt.Name && scriptType.Namespace == sceneScipt.Namespace)
					return currentScript;
			}

			return null;
		}

		public void ReloadScripts(Scripting scripting)
		{
			Entity.Dispose();
			Entity = new();
			LoadScripts(scripting);
		}

		public void LoadScripts(Scripting scripting)
		{
			for (int i = 0; i < Scripts.Count; i++)
			{
				for (int j = 0; j < scripting.Scripts.Count; j++)
				{
					Scripting.CachedScript currentScript = scripting.Scripts[j];
					if (currentScript.Name == Scripts[i].Name && currentScript.Namespace == Scripts[i].Namespace)
					{
						Script script = CreateInstanseOf(currentScript);
						for (int k = 0; k < Scripts[i].Fields.Count; k++)
						{
							Type scriptType = script.GetType();
							SceneScriptField field = Scripts[i].Fields[k];
							scriptType.GetField(field.Name)?.SetValue(script, JsonSerializer.Deserialize(field.Value.Value, Type.GetType(field.Value.TypeFullName)));
						}
					}
				}
			}
		}

		private Script CreateInstanseOf(Scripting.CachedScript script)
		{
			Script instance = script.CreateInstance<Script>();
			Entity.Scripts.Add(instance);
			return instance;
		}

		public void Dispose()
		{
			for (int i = 0; i < Scripts.Count; i++)
				Scripts[i].Dispose();

			Entity.Dispose();
		}
	}
}
