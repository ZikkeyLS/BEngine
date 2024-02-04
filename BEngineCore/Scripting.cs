using BEngine;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Principal;

namespace BEngineCore
{
	public class Scripting
	{
		public class CachedField
		{
			public string Name;
			public string Type;
		}

		public class CachedScript
		{
			public readonly Type Type;
			public readonly string Name;
			public readonly string? Namespace;
			public readonly string? Fullname;
			public readonly List<CachedField> Fields = new();
			public readonly List<string> Methods = new();

			public CachedScript(Type type)
			{
				Type = type;

				Name = type.Name;
				Namespace = type.Namespace;
				Fullname = type.FullName;

				ReadFields();
				ReadMethods();
			}

#pragma warning disable CS8603 // Always not null, so ignore this warning this time.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
			public T CreateInstance<T>(object[]? args = null) where T : Script => (T)Activator.CreateInstance(Type, args);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8603

			private void ReadFields()
			{
				FieldInfo[] properties = Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < properties.Length; i++)
				{
					Fields.Add(new CachedField () { Name = properties[i].Name, Type = properties[i].FieldType.Name });
				}
			}

			private void ReadMethods()
			{
				MethodInfo[] methods = Type.GetMethods();
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i].Name.StartsWith('.') == false)
						Methods.Add(methods[i].Name);
				}
			}
		}

		private List<CachedScript> _scripts = new();

		public List<CachedScript> Scripts => _scripts;

		public bool ReadyToUse = false;

		public void ReadScriptAssembly(string path)
		{
			_scripts.Clear();

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

		public static void LoadInternalScriptingAPI()
		{
			 BEngine.InternalCalls.LoadInternalCallsAPI();
		}
	}
}
