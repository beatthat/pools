using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatThat
{
	/// <summary>
	/// Static pool of arrays of different sizes. 
	/// 
	/// Lists returned from pool implement IDisposable to enable this usage:
	/// 
	/// <code>
	/// using(var arr = ArrayPool<T>.Get()) {
	/// 	..
	/// }
	/// </code>
	/// 
	/// </summary>
	public static class ArrayPool<T>
	{
		public static ArrayPoolArray<T> Get(int size)
		{
			var pool = GetPool(size);
			if(pool.Count > 0) {
				var a = pool[0];
				pool.RemoveAt(0);
				return a;
			}
				
			return new ArrayPoolArray<T>(new T[size]);
		}

		public static ArrayPoolArray<T> GetCopy(T[] copyFrom)
		{
			var a = Get (copyFrom.Length);
			System.Array.Copy (copyFrom, a.array, copyFrom.Length);
			return a;
		}

		private static List<ArrayPoolArray<T>> GetPool(int size)
		{
			List<ArrayPoolArray<T>> pool;
			if(!m_pools.TryGetValue(size, out pool)) {
				pool = new List<ArrayPoolArray<T>>(1);
				m_pools[size] = pool;
			}
			return pool;
		}

		public static void Return(ArrayPoolArray<T> a)
		{
			var pool = GetPool(a.array.Length);

			// Analysis disable once ForCanBeConvertedToForeach
			for(var i = 0; i < pool.Count; i++) {
				var inP = pool[i];

				if(inP.array == a.array) {
					Debug.LogWarning("ListPool::Release called for a list that's already in the pool");
					return;
				}
			}

			Array.Clear(a.array, 0, a.array.Length);

			pool.Add(a);
		}

		// Analysis disable StaticFieldInGenericType
		private static readonly Dictionary<int, List<ArrayPoolArray<T>>> m_pools = new Dictionary<int, List<ArrayPoolArray<T>>>();
		// Analysis restore StaticFieldInGenericType
	}

	/// <summary>
	/// The type checked out by ArrayPool<T>.Get is actually this disposable to allow use of <c>using</c> blocks, e.g.
	/// 
	/// <c>
	/// using(var poolArray = ArrayPool<int>.Get(1)) {
	/// 	poolArray.array[0] = 1;
	/// }
	/// </c>
	/// </summary>
	public class ArrayPoolArray<T> : IDisposable
	{
		public ArrayPoolArray(T[] a)
		{
			this.array = a;
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			ArrayPool<T>.Return(this);
		}
		#endregion

		public T[] array { get; private set; }
		
	}

}
