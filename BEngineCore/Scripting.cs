using BEngineScripting;
using System.Reflection;
using System.Runtime.Loader;

namespace BEngineCore
{
	public class Scripting
	{
		public class CachedScript
		{
			public readonly Type Type;
			public readonly string Name;
			public readonly string? Namespace;
			public readonly string? Fullname;
			public readonly List<string> MembersNames = new();
			public readonly List<string> MethodsNames = new();

			public CachedScript(Type type)
			{
				Type = type;

				Name = type.Name;
				Namespace = type.Namespace;
				Fullname = type.FullName;

				ReadMembers();
				ReadMethods();
			}

#pragma warning disable CS8603 // Always not null, so ignore this warning this time.
			public object CreateInstance(object[] args) => Activator.CreateInstance(Type, args);
#pragma warning restore CS8603

			private void ReadMembers()
			{
				MemberInfo[] members = Type.GetMembers();
				for (int i = 0; i < members.Length; i++)
				{
					MethodsNames.Add(members[i].Name);
				}
			}

			private void ReadMethods()
			{
				MethodInfo[] methods = Type.GetMethods();
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i].Name.StartsWith('.') == false)
						MethodsNames.Add(methods[i].Name);
				}
			}
		}

		private List<CachedScript> _scripts = new();

		public void Initialize(string path)
		{
			ReadScriptingAssembly(path);
		}

		public void ReadScriptingAssembly(string path)
		{
			_scripts.Clear();

			AssemblyLoadContext context = new AssemblyLoadContext(name: "ReadScripts", isCollectible: true);

			Assembly dll = context.LoadFromAssemblyPath(path);
			foreach (Type type in dll.GetExportedTypes())
			{
				if (type.IsSubclassOf(typeof(Script)))
					_scripts.Add(new CachedScript(type));
			}

			context.Unload();
		}
	}
}
