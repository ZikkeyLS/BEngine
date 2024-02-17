using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngineEditor
{
	internal class EditorTypeResolver
	{
		public bool IsInClassList(Type current, params Type[] typeList)
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
