using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEngine
{
	public struct Vector3Bool
	{
		public bool x { get; set; }
		public bool y { get; set; }
		public bool z { get; set; }

		public Vector3Bool()
		{

		}

		public Vector3Bool(bool x, bool y, bool z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}
