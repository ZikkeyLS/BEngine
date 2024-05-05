
namespace BEngineCore
{
    public class ProjectRuntimeInfo
    {
        public Dictionary<uint, RuntimeScene> RuntimeScenes { get; set; } = new();
    }

    public class RuntimeScene
    {
        public string GUID { get; set; }
        public string Name { get; set; }
    }
}
