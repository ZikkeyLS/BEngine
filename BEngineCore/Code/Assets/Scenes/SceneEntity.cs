using BEngine;
using BEngineScripting;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace BEngineCore
{
	public class SceneEntity : IDisposable
	{
		public string GUID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		
	    [JsonIgnore] public SceneEntity? Parent { get; set; }
		public string? ParentBase { get; set; }

		[JsonIgnore] public List<SceneEntity> Children { get; set; } = new();
		public List<string> ChildrenBase { get; set; } = new();

		public List<SceneScript> Scripts { get; set; } = new();

		private Scene _scene => ProjectAbstraction.LoadedProject.LoadedScene;

		[JsonIgnore]
		public Entity Entity { get; set; } = new();

		public SceneEntity() 
		{

		}

		public SceneEntity(string name)
		{
			GUID = Guid.NewGuid().ToString();
			SetName(name);
		}

		public void SetName(string name)
		{
			Name = name;
			Entity.Name = Name;
		}

		public void LoadInheritance()
		{
			SetName(Name);

			if (ParentBase != null && ParentBase != string.Empty)
			{
				Parent = _scene.GetEntity(ParentBase);
			}

			foreach (string childBase in ChildrenBase)
			{
				SceneEntity? child = _scene.GetEntity(childBase);
				if (child != null)
					Children.Add(child);
			}
		}

		public void SetParent(string guid)
		{
			SceneEntity? entity = _scene.GetEntity(guid);
			if (entity != null)
				SetParent(entity);
		}

		public void SetParent(SceneEntity entity)
		{
			Parent = entity;
			ParentBase = entity.GUID;
		}

		public bool AddChild(SceneEntity entity)
		{
			if (Children.Contains(entity))
				return false;
			Children.Add(entity);
			ChildrenBase.Add(entity.GUID);
			return true;
		}

		public bool RemoveChild(SceneEntity entity)
		{
			if (Children.Contains(entity) == false)
				return false;
			Children.Remove(entity);
			ChildrenBase.Remove(entity.GUID);
			return true;
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
				return IsChildOf(current.Parent, result);

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
			Script? removeRuntime = Entity.Scripts.Find((current) => current.GetType().Name == script.Name &&
				current.GetType().Namespace == script.Namespace);

			if (removeRuntime != null)
			{
				Entity.CallEventLocal(EventID.Destroy, removeRuntime);
				Entity.CallEventLocal(EventID.EditorDestroy, removeRuntime);

				Entity.Scripts.Remove(removeRuntime);
				removeRuntime.Dispose();
			}

			Scripts.Remove(script);

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
							SceneScriptValue? value = field.Value;
							if (value != null)
							{
								Type? type = ScriptingUtils.GetTypeByName(value.TypeFullName);

								if (type != null)
								{
									object? result = JsonSerializer.Deserialize(value.Value, type);
									scriptType.GetField(field.Name)?.SetValue(script, result);
								}
							}
						}
					}
				}
			}
		}

		private Script CreateInstanseOf(Scripting.CachedScript script)
		{
			Script instance = script.CreateInstance<Script>();
			instance.GetType().GetField("Entity")?.SetValue(instance, Entity);
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
