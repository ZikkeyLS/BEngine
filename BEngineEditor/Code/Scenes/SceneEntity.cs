using BEngine;
using BEngineCore;
using System.Text.Json.Serialization;

namespace BEngineEditor
{
	public class SceneEntity
	{
		public string GUID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		public string? Parent { get; set; }
		public List<string> Children { get; set; } = new();
		public List<SceneScript> Scripts { get; set; } = new();

		[JsonIgnore]
		public Entity Entity { get; set; } = new();

		public SceneEntity() { }

		public SceneEntity(string name) 
		{
			GUID = Guid.NewGuid().ToString();
			Name = name;
			Entity.Name = name;
		}

		public void AddScript(Scripting.CachedScript script)
		{
			Scripts.Add(new SceneScript() { Name = script.Name, Namespace = script.Namespace });
			CreateInstanseOf(script);
		}

		public void LoadScripts(Scripting scripting)
		{
			for (int i = 0; i < Scripts.Count; i++) 
			{
				for (int j = 0; j < scripting.Scripts.Count; j++)
				{
					Scripting.CachedScript currentScript = scripting.Scripts[i];
					if (currentScript.Name == Scripts[i].Name && currentScript.Namespace == Scripts[i].Namespace)
						CreateInstanseOf(currentScript);
				}
			}
		}

		private void CreateInstanseOf(Scripting.CachedScript script)
		{
			Entity.Scripts.Add(script.CreateInstance<Script>());
		}
	}
}
