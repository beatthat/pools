using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatThat
{
	/// <summary>
	/// Static pool of generic List<T>. Unity has an internal class identical to this, 
	/// but seems to be inaccessible (literally 'internal'?).
	/// 
	/// In some sense, you can see why a class w this design might be declared internal
	/// because it allows any code to inject a list to the pool, but for now gonna assume 
	/// there's no malicious code out and about inside the game executable.
	/// 
	/// Lists returned from pool implement IDisposable to enable this usage:
	/// 
	/// <code>
	/// using(var list = ListPool<T>.Get()) {
	/// 	..
	/// }
	/// </code>
	/// 
	/// </summary>
	public static class ListPool<T>
	{
		public static ListPoolList<T> Get()
		{
			if(m_pool.Count > 0) {
				var list = m_pool[0];
				m_pool.RemoveAt(0);
				return list;
			}

			return new ListPoolList<T>();
		}

		public static void Return(ListPoolList<T> list)
		{
			if(m_pool.Contains(list)) {
				Debug.LogWarning("ListPool::Release called for a list that's already in the pool");
				return;
			}
			list.Clear();
			m_pool.Add(list);
		}

		// Analysis disable StaticFieldInGenericType
		private static readonly List<ListPoolList<T>> m_pool = new List<ListPoolList<T>>(1);
		// Analysis restore StaticFieldInGenericType
	}

	/// NOTE would be better to expose only a public interface but unity's Component::GetComponents functions require concrete List<T>
	public class ListPoolList<T> : List<T>, IDisposable
	{
		/// <summary>
		/// ListPoolList instances should be created only by ListPool
		/// </summary>
		internal ListPoolList() : base() {}

		#region IDisposable implementation
		public void Dispose ()
		{
			ListPool<T>.Return(this);
		}
		#endregion
	}

}
