using BEngine;
using BEngineCore;
using System.Reflection;

namespace BEngineEditor
{
	internal abstract class TypeResolver
	{
		public struct ResolverData
		{
			public ProjectContext ProjectContext;
			public PropertiesScreen Properties;
			public SceneScript SceneScript;
			public Script Script;
			public MemberInfo Field;
			public Type FieldType;
			public string FieldName;
		}

		public abstract void Resolve(ResolverData data);

		public static bool IsInClassList(Type current, params Type[] typeList)
		{
			for (int i = 0; i < typeList.Length; i++)
			{
				if (current == typeList[i])
					return true;
			}

			return false;
		}
	}
}
