using BEngine;
using System.Reflection;

namespace BEngineScripting
{
	public class CachedField
	{
		public string Name;
		public string Type;
	}

	public class CachedMethod
	{
		public string Name;
		public string[] Attributes;
	}

	public class CachedScript
	{
		public readonly Type Type;
		public readonly string Name;
		public readonly string? Namespace;
		public readonly string? Fullname;
		public readonly List<CachedField> Fields = new();
		public readonly List<CachedMethod> Methods = new();

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
				Fields.Add(new CachedField() { Name = properties[i].Name, Type = properties[i].FieldType.Name });
			}
		}

		private void ReadMethods()
		{
			MethodInfo[] methods = Type.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].Name.StartsWith('.') == false)
				{
					var attributes = methods[i].GetCustomAttributes();
					string[] parsedAttributes = new string[attributes.Count()];

					int j = 0;
					foreach (var item in attributes)
					{
						parsedAttributes[j] = item.GetType().Name;
						j += 1;
					}

					Methods.Add(new CachedMethod() { Name = methods[i].Name, Attributes = parsedAttributes });
				}
			}
		}
	}
}
