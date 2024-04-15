using System.Text.Json;

namespace BEngine
{
	public static class JsonUtils
	{
		public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions()
		{
			WriteIndented = true,
			PreferredObjectCreationHandling = System.Text.Json.Serialization.JsonObjectCreationHandling.Replace
		};

		public static T? Deserialize<T>(Stream data)
		{
			return JsonSerializer.Deserialize<T>(data, DefaultOptions);
		}

		public static T? Deserialize<T>(string data)
		{
			return JsonSerializer.Deserialize<T>(data, DefaultOptions);
		}

		public static T? Deserialize<T>(string data, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<T>(data, options);
		}

		public static object? Deserialize(string data, Type type)
		{
			return JsonSerializer.Deserialize(data, type, DefaultOptions);
		}

		public static object? Deserialize(string data, Type type, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize(data, type, options);
		}

		public static string Serialize<T>(T value)
		{
			return JsonSerializer.Serialize(value, DefaultOptions);
		}

		public static string Serialize<T>(T value, JsonSerializerOptions options)
		{
			return JsonSerializer.Serialize(value, options);
		}

		public static string Serialize(object value)
		{
			return JsonSerializer.Serialize(value, DefaultOptions);
		}

		public static string Serialize(object value, JsonSerializerOptions options)
		{
			return JsonSerializer.Serialize(value, options);
		}
	}
}
