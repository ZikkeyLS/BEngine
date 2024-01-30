using System.Collections;

namespace BEngineEditor
{
	public class DictionrySerializable<TKey, UValue> : IEnumerable<KeyValuePair<TKey, UValue>>
	{
		private LinkedList<KeyValuePair<TKey, UValue>>[] _values;
		private int capacity;
		public DictionrySerializable()
		{
			_values = new LinkedList<KeyValuePair<TKey, UValue>>[15];
		}
		public int Count => _values.Length;

		public void Add(TKey key, UValue val)
		{
			var hash = GetHashValue(key);
			if (_values[hash] == null)
			{
				_values[hash] = new LinkedList<KeyValuePair<TKey, UValue>>();
			}
			var keyPresent = _values[hash].Any(p => p.Key.Equals(key));
			if (keyPresent)
			{
				throw new Exception("Duplicate key has been found");
			}
			var newValue = new KeyValuePair<TKey, UValue>(key, val);
			_values[hash].AddLast(newValue);
			capacity++;
			if (Count <= capacity)
			{
				ResizeCollection();
			}
		}

		private void ResizeCollection()
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(TKey key)
		{
			var hash = GetHashValue(key);
			return _values[hash] == null ? false : _values[hash].Any(p =>
	p.Key.Equals(key));
		}

		public UValue GetValue(TKey key)
		{
			var hash = GetHashValue(key);
			return _values[hash] == null ? default(UValue) :
				_values[hash].First(m => m.Key.Equals(key)).Value;
		}
		public IEnumerator<KeyValuePair<TKey, UValue>> GetEnumerator()
		{
			return (from collections in _values
					where collections != null
					from item in collections
					select item).GetEnumerator();
		}

		private int GetHashValue(TKey key)
		{
			return (Math.Abs(key.GetHashCode())) % _values.Length;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public UValue this[TKey key]
		{
			get
			{
				int h = GetHashValue(key);
				if (_values[h] == null) throw new KeyNotFoundException("Keys not found");
				return _values[h].FirstOrDefault(p => p.Key.Equals(key)).Value;
			}
			set
			{
				int h = GetHashValue(key);
				_values[h] = new LinkedList<KeyValuePair<TKey, UValue>>();
				_values[h].AddLast(new KeyValuePair<TKey, UValue>
													(key, value));
			}
		}
	}

}
