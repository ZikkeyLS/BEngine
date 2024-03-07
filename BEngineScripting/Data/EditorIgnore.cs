
namespace BEngine
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EditorIgnore : Attribute
	{
		public EditorIgnore() { }
	}
}
