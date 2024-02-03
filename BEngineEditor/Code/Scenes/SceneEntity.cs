using BEngineScripting;
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
	}
}
