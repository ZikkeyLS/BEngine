using BEngine;
using BEngineScripting;
using System.Runtime.Loader;

namespace BEngineCore
{
	public class Scripting
	{
		private List<CachedScript> _scripts = new();

		public List<CachedScript> Scripts => _scripts;

		public bool ReadyToUse = false;

		public void ReadScriptAssembly(string path)
		{
			_scripts.Clear();

			LoadInternalAssembly();

			AssemblyLoadContext context = new AssemblyLoadContext("LoadScripts", true);

			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var assembly = context.LoadFromStream(fs);
				foreach (Type type in assembly.GetExportedTypes())
				{
					if (type.IsSubclassOf(typeof(Script)))
						_scripts.Add(new CachedScript(type));
				}
			}

			context.Unload();

			ReadyToUse = true;
		}

		private void LoadInternalAssembly()
		{
			_scripts.Add(new CachedScript(typeof(BEngine.Transform)));
			_scripts.Add(new CachedScript(typeof(BEngine.ModelRenderer)));
			_scripts.Add(new CachedScript(typeof(BEngine.Camera)));
			_scripts.Add(new CachedScript(typeof(BEngine.Rigidbody)));
			_scripts.Add(new CachedScript(typeof(BEngine.CubeCollider)));
			_scripts.Add(new CachedScript(typeof(BEngine.SphereCollider)));
			_scripts.Add(new CachedScript(typeof(BEngine.CapsuleCollider)));
			_scripts.Add(new CachedScript(typeof(BEngine.PlaneCollider)));
		}

		public static void LoadInternalScriptingAPI()
		{
			 BEngine.InternalCalls.LoadInternalCallsAPI();
		}
	}
}
