using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatThat
{
	/// <summary>
	/// Static pool of generic Dictionary<K,V>. Unity has an internal class identical to this, 
	/// but seems to be inaccessible (literally 'internal'?).
	/// 
	/// In some sense, you can see why a class w this design might be declared internal
	/// because it allows any code to inject a list to the pool, but for now gonna assume 
	/// there's no malicious code out and about inside the game executable.
	/// 
	/// Dictionaries returned from pool implement IDisposable to enable this usage:
	/// 
	/// <code>
	/// using(var d = DictionaryPool<K,V>.Get()) {
	/// 	..
	/// }
	/// </code>
	/// 
	/// </summary>
	public static class DictionaryPool<K,V>
	{
		public static PooledDictionary<K,V> Get(IDictionary<K,V> copyFrom = null)
		{
			if(m_pool.Count > 0) {
				var d = m_pool[0];
				m_pool.RemoveAt(0);

				if (copyFrom != null) {
					foreach (var kv in copyFrom) {
						d[kv.Key] = kv.Value;
					}
				}

				return d;
			}

			return new PooledDictionary<K,V>();
		}

		public static void Return(PooledDictionary<K,V> d)
		{
			if(m_pool.Contains(d)) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				Debug.LogWarning("DictionaryPool::Release called for a list that's already in the pool");
				#endif
				return;
			}
			d.Clear();
			m_pool.Add(d);
		}

		// Analysis disable StaticFieldInGenericType
		private static readonly List<PooledDictionary<K,V>> m_pool = new List<PooledDictionary<K,V>>(1);
		// Analysis restore StaticFieldInGenericType
	}

	/// NOTE would be better to expose only a public interface but unity's Component::GetComponents functions require concrete List<T>
	public class PooledDictionary<K,V> : Dictionary<K,V>, IDisposable
	{
		/// <summary>
		/// ListPoolList instances should be created only by ListPool
		/// </summary>
		internal PooledDictionary() : base() {}

		#region IDisposable implementation
		public void Dispose ()
		{
			DictionaryPool<K,V>.Return(this);
		}
		#endregion
	}

}
