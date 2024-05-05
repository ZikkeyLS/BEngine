
namespace BEngineCore
{
    public class ProjectRuntimeInfo
    {
        public ProjectRuntimeInfo()
        {

        }

        public Dictionary<uint, RuntimeScene> RuntimeScenes { get; set; } = new();
    }

    public class RuntimeScene
    {
        public string GUID { get; set; }
        public string Name { get; set; }

        public RuntimeScene()
        {

        }
       
        public RuntimeScene(string guid, string sceneName)
        {
            GUID = guid;
            Name = sceneName;
        }
    }
}
