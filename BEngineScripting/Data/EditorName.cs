
namespace BEngine
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class EditorName : Attribute
	{
		public string Name = string.Empty;
		public EditorName(string name) => this.Name = name;
	}
}
