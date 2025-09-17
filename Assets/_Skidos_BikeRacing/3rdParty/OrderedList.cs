using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Data_MainProject
{
	[Serializable]
	public class OrderedList_BikeRace<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
	{
		public /*private */Dictionary<TKey, int> _order;
		private int _lastOrder;
		private readonly SortedList<TKey, TValue> _internalList;
		
		private readonly bool _sorted;
		private readonly OrderComparer _comparer;
		
		private readonly object _lockObject = new Object();
		
		public OrderedList_BikeRace()
		{

			IEnumerable<KeyValuePair<TKey, TValue>> pairedCollection = null; //nee
			bool sorted = false; //sorted netiek supportéts - ja grbi sorted tad lieto sorted dict, #√£$%& !


			_sorted = sorted;
			
			if (pairedCollection == null) pairedCollection = new Dictionary<TKey, TValue>();
			
			if (_sorted)
			{
				_internalList = new SortedList<TKey, TValue>(
					(pairedCollection as Dictionary<TKey, TValue>) ?? pairedCollection.ToDictionary(i => i.Key, i => i.Value)
					);
			}
			else
			{
				_order = new Dictionary<TKey, int>();
				_comparer = new OrderComparer(ref _order);
				_internalList = new SortedList<TKey, TValue>(_comparer);
				// keep prder of the IDictionary
				foreach (var kvp in pairedCollection)
				{
					Add(kvp);
				}
				
			}
			
		}


		
		public bool Sorted
		{
			get { return _sorted; }
		}
		
		private class OrderComparer : Comparer<TKey>
		{
			public Dictionary<TKey, int> Order { private get; set; }
			
			public OrderComparer(ref Dictionary<TKey, int> order)
			{
				Order = order;
			}
			
			public override int Compare(TKey x, TKey y)
			{
				var xo = Order[x];
				var yo = Order[y];
				return xo.CompareTo(yo);
			}
		}
		
		private void ReOrder()
		{
			var i = 0;
			_order = _order.OrderBy(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => i++);
			_comparer.Order = _order;
			_lastOrder = _order.Values.Max() + 1;
		}
		
		public void Add(TKey key, TValue value)
		{
			lock (_lockObject)
			{
				if (!_sorted)
				{
					_order.Add(key, _lastOrder);
					_lastOrder++;
					//very rare event
					if (_lastOrder == int.MaxValue) ReOrder();
				}
				
				_internalList.Add(key, value);
			}
		}
		
		public bool Remove(TKey key)
		{
			lock (_lockObject)
			{
				var result = _internalList.Remove(key);
				if (!_sorted) _order.Remove(key);
				return result;
			}
		}
		
		// Other IDictionary<> + IDictionary members implementation wrapping around _internalList
		// ...
		//    }
		//}
		
		public bool ContainsKey(TKey key)
		{
			//return _internalList.ContainsKey(key);
			return _order.ContainsKey(key);
		}
		
		public bool TryGetValue(TKey key, out TValue value)
		{
			if(_order.ContainsKey(key)){
				value = _internalList[key];
				return true;
			} else {
				value = default(TValue);
				return false;
			}

		}

		public TValue this[TKey key]
		{
			get { return _internalList[key]; }

			set {
					if(_order.ContainsKey(key)){
						_internalList[key] = value;  //maina vértíbu
					} else {
						Add(key, value);//pievieno jaunu ierakstu
					}
			}
		}
		
		public ICollection<TKey> Keys { get { return _internalList.Keys; } }
		
		ICollection IDictionary.Values
		{
			get { return (_internalList as IDictionary).Values; }
		}
		
		ICollection IDictionary.Keys
		{
			get { return (_internalList as IDictionary).Keys; }
		}
		
		public ICollection<TValue> Values { get { return _internalList.Values; } }
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _internalList.GetEnumerator();
		}
		
		public void Remove(object key)
		{
			(_internalList as IDictionary).Remove(key);
		}
		
		object IDictionary.this[object key]
		{
			get { return (_internalList as IDictionary)[key]; }
			set { (_internalList as IDictionary)[key] = value; }
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}
		
		public bool Contains(object key)
		{
			return (_internalList as IDictionary).Contains(key);
		}
		
		public void Add(object key, object value)
		{
			(_internalList as IDictionary).Add(key, value);
		}
		
		public void Clear()
		{
			lock (_lockObject)
			{
				_internalList.Clear();
				if (!_sorted) _order.Clear();
			}
		}
		
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return (_internalList as IDictionary).GetEnumerator();
		}
		
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _internalList.Contains(item);
		}
		
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			(_internalList as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
		}
		
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			lock (_lockObject)
			{
				var result = (_internalList as IDictionary<TKey, TValue>).Remove(item);
				if (!_sorted) _order.Remove(item.Key);
				return result;
			}
		}
		
		public void CopyTo(Array array, int index)
		{
			(_internalList as IDictionary).CopyTo(array, index);
		}
		
		public int Count
		{
			get { return _internalList.Count; }
		}
		
		public object SyncRoot
		{
			get { return (_internalList as IDictionary).SyncRoot; }
		}
		
		public bool IsSynchronized
		{
			get { return (_internalList as IDictionary).IsSynchronized; }
		}
		
		public bool IsReadOnly
		{
			get { return (_internalList as IDictionary<TKey, TValue>).IsReadOnly; }
		}
		
		public bool IsFixedSize
		{
			get { return (_internalList as IDictionary).IsFixedSize; }
		}
	}
	
}