using BEngine;
using BEngineScripting;
using static BEngineCore.Scripting;

namespace BEngineCore
{
	public class SceneScriptValue
	{
		public string Value { get; set; }
		public string TypeFullName { get; set; }
	}

	public class SceneScriptField
	{
		public string Name { get; set; }
		public bool IsEditable { get; set; }
		public bool IsVisible { get; set; }
		public SceneScriptValue? Value { get; set; }

		public SceneScriptField() { }
	}

	public class ScriptFieldData
	{
		public Dictionary<string, FieldData> Data = new();
	}

	public class SceneScript : IDisposable
	{
		public string GUID { get; set; }
		public string? Namespace { get; set; }
		public string Name { get; set; }
		public List<SceneScriptField> Fields { get; set; } = new();

		public SceneScript() {  }

		public void VerifyFields(CachedScript script)
		{
			RemoveEntriesCheck(script);
			AddEntriesCheck(script);
		}

		private void RemoveEntriesCheck(CachedScript script)
		{
			List<SceneScriptField> removeEntries = new();

			foreach (SceneScriptField key in Fields)
			{
				if (script.Fields.Find((member) => member.Name == key.Name) == null)
				{
					removeEntries.Add(key);
				}
			}

			for (int i = 0; i < removeEntries.Count(); i++)
			{
				Fields.Remove(removeEntries[i]);
			}
		}

		private void AddEntriesCheck(CachedScript script)
		{
			List<string> addEntries = new();

			foreach (CachedField field in script.Fields)
			{
				if (ContainsField(field.Name) == false)
					addEntries.Add(field.Name);
			}

			for (int i = 0; i < addEntries.Count(); i++)
			{
				Fields.Add(new SceneScriptField() { Name = addEntries[i], Value = default, IsEditable = true, IsVisible = true });
			}
		}

		public bool ContainsField(string name)
		{
			SceneScriptField? field = Fields.Find((field) => field.Name == name);
			return field != null;
		}

		public bool AddField(string name, object value)
		{
			SceneScriptField? field = Fields.Find((field) => field.Name == name);
			if (field == null)
				Fields.Add(new SceneScriptField() { Name = name,
					Value = new SceneScriptValue() { Value = JsonUtils.Serialize(value), TypeFullName = value.GetType().FullName }, 
					IsEditable = true, IsVisible = true });
			else
				return false;

			return true;
		}

		public bool ChangeField(string name, object value)
		{
			SceneScriptField? field = Fields.Find((field) => field.Name == name);
			if (field != null)
				field.Value = new SceneScriptValue() { Value = JsonUtils.Serialize(value), TypeFullName = value.GetType().FullName };
			else
				return false;

			return true;
		}

		public void DeleteField(string name)
		{
			SceneScriptField? field = Fields.Find((field) => field.Name == name);
			if (field != null)
				Fields.Remove(field);
		}

		public void Dispose()
		{

		}
	}
}
