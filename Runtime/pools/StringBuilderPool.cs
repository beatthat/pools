using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace BeatThat
{
	/// <summary>
	/// Static pool of StringBuilders. Unity has an internal class identical to this, 
	/// but seems to be inaccessible (literally 'internal'?).
	/// 
	/// In some sense, you can see why a class w this design might be declared internal
	/// because it allows any code to inject a list to the pool, but for now gonna assume 
	/// there's no malicious code out and about inside the game executable.
	/// </summary>
	public static class StringBuilderPool
	{
		public static StringBuilder Get()
		{
			if(m_pool.Count > 0) {
				var obj = m_pool[0];
				m_pool.RemoveAt(0);
				return obj;
			}

			return new StringBuilder();
		}

		public static void Return(StringBuilder b)
		{
			if(m_pool.Contains(b)) {
				Debug.LogWarning("StringBuilderPool::Release called for an object that's already in the pool");
				return;
			}
			b.Length = 0;
			m_pool.Add(b);
		}

		// Analysis disable StaticFieldInGenericType
		private static readonly List<StringBuilder> m_pool = new List<StringBuilder>(1);
		// Analysis restore StaticFieldInGenericType
	}
}
