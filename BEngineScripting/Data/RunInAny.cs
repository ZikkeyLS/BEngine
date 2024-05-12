
namespace BEngine
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class RunInAny : Attribute
	{
		public RunInAny() { }
	}
}
