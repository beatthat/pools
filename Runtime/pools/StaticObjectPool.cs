using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatThat
{
	internal static class StaticObjectPoolHelper
	{
		private static readonly Type[] NO_TYPES = new Type[0];
		private static readonly object[] NO_OBJECTS = new object[0];

		public static T Create<T>()// where T : class
		{
			var c = typeof(T).GetConstructor(NO_TYPES);
			return (T)c.Invoke(NO_OBJECTS);
		}
	}

	/// <summary>
	/// A bare bones static object pool for Plain Old CSharp Objects (where probably only reason to pool is to avoid allocations)
	/// To be poolable, an object must have a zero-arg constructor. 
	/// Additionally, pool objects can implement the Poolable interface and receive a callback when they are returned to the pool.
	/// </summary>
	public static class StaticObjectPool<T>// where T : class
	{
		// Analysis disable StaticFieldInGenericType
		private static int m_createdCount;
		// Analysis restore StaticFieldInGenericType

		private const int WARN_ON_CREATE_COUNT_THRESHOLD = 1000;

		public static T Get()
		{
			if(m_pool.Count > 0) {
				var e = m_pool[0];
				m_pool.RemoveAt(0);
				return e;
			}

			if(++m_createdCount > WARN_ON_CREATE_COUNT_THRESHOLD) { 
				Debug.LogWarning("[" + Time.frameCount + "] StaticObjectPool<" + typeof(T).Name + ">::Get has created " 
					+ m_createdCount + " pool objects. There may be a leak.");
			}

			return StaticObjectPoolHelper.Create<T>();
		}

		public static void Return(T obj)
		{
			if(m_pool.Contains(obj)) {
				Debug.LogWarning("StaticObjectPool::Release called for a list that's already in the pool");
				return;
			}
				
			var pObj = obj as Poolable;
			if(pObj != null) {
				pObj.OnReturnedToPool();
			}
			m_pool.Add(obj);
		}

		// Analysis disable StaticFieldInGenericType
		private static readonly List<T> m_pool = new List<T>(1);
		// Analysis restore StaticFieldInGenericType
	}

	public interface Poolable
	{
		void OnReturnedToPool();
	}
}