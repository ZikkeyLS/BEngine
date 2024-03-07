
namespace BEngine
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EditorShowAt : Attribute
	{
		public int Placement = 1;
		public EditorShowAt(int placement = 1) => this.Placement = placement;
	}
}
