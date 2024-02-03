namespace BEngineEditor
{
	public class SceneEntity
	{
		public string GUID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		public SceneEntity Parent { get; set; }
		public List<SceneEntity> Chilren { get; set; } = new();
		public List<SceneScript> Scripts { get; set; } = new();

		public SceneEntity() { }

		public SceneEntity(string name) 
		{
			GUID = Guid.NewGuid().ToString();
			Name = name;
		}
	}
}
