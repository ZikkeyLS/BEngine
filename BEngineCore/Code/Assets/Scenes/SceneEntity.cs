using BEngine;
using BEngineScripting;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BEngineCore
{
	public struct FieldData
	{
		public MemberTypes Type;
		public MemberInfo Info;
		public object Data;
	}

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
			if (IsChildOf(entity, this))
			{
				return;
			}

			if (entity == null)
			{
				ClearParent();
				return;
			}

			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}

			Parent = entity;
			ParentBase = entity.GUID;
			entity.AddChild(this);
		}

		public void ClearParent()
		{
			if (Parent != null)
			{
				Parent.RemoveChild(this);
				Parent = null;
				ParentBase = null;
			}
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

		private bool IsChildOf(SceneEntity child, SceneEntity parent)
		{
			if (child.Parent == parent && parent != null)
				return true;

			if (child.Parent != null)
				return IsChildOf(child.Parent, parent);

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

		public Dictionary<string, FieldData> CopyScriptData(SceneScript script)
		{
			Dictionary<string, FieldData> data = new();
			Script? scriptRuntime = Entity.Scripts.Find((current) => current.GetType().Name == script.Name &&
				current.GetType().Namespace == script.Namespace);

			if (scriptRuntime != null)
			{
				List<MemberInfo> fields = scriptRuntime.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance).
					ToList().FindAll((member) => member.MemberType == MemberTypes.Field | member.MemberType == MemberTypes.Property);

				foreach (MemberInfo field in fields)
				{
					object? value = null;
					MemberTypes type = MemberTypes.Field;

					switch (field.MemberType)
					{
						case MemberTypes.Field:
							value = ((FieldInfo)field).GetValue(scriptRuntime);
							type = MemberTypes.Field;
							break;
						case MemberTypes.Property:
							value = ((PropertyInfo)field).GetValue(scriptRuntime);
							type = MemberTypes.Property;
							break;
					}

					if (value != null)
					{
						data.Add(field.Name, new FieldData() { Type = type, Info = field, Data = value });
					}
				}
			}

			return data;
		}

		public void PasteScriptData(SceneScript script, Dictionary<string, FieldData> data)
		{
			Script? scriptRuntime = Entity.Scripts.Find((current) => current.GetType().Name == script.Name &&
				current.GetType().Namespace == script.Namespace);

			if (scriptRuntime != null)
			{
				foreach (var field in data)
				{
					switch (field.Value.Type)
					{
						case MemberTypes.Field:
							((FieldInfo)field.Value.Info).SetValue(scriptRuntime, field.Value.Data);
							break;
						case MemberTypes.Property:
							((PropertyInfo)field.Value.Info).SetValue(scriptRuntime, field.Value.Data);
							break;
					}
				}
			}
		}

		public void ResetScript(SceneScript script)
		{
			int resetRuntimeIndex = Entity.Scripts.FindIndex(0, (current) => current.GetType().Name == script.Name &&
				current.GetType().Namespace == script.Namespace);

			if (resetRuntimeIndex != -1)
				Entity.Scripts[resetRuntimeIndex] = (Script?)Activator.CreateInstance(Entity.Scripts[resetRuntimeIndex].GetType());
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
									object? result = JsonUtils.Deserialize(value.Value, type);
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
