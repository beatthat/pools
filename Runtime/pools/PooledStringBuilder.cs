using System;
using System.Text;

namespace BeatThat.Pools
{

    /// <summary>
    /// The type checked out by PoolableStringBuilder.Get is actually this disposable to allow use of <c>using</c> blocks, e.g.
    /// 
    /// <c>
    /// using(var sb = PoolableStringBuilder.Get()) {
    ///     sb.stringBuilder.Append('a');
    /// }
    /// </c>
    /// </summary>
    public class PooledStringBuilder : IDisposable, Poolable
    {
        #region IDisposable implementation
        public void Dispose()
        {
            StaticObjectPool<PooledStringBuilder>.Return(this);
        }

        public static PooledStringBuilder Get()
        {
            return StaticObjectPool<PooledStringBuilder>.Get();
        }

        public void OnReturnedToPool()
        {
            this.stringBuilder.Length = 0;
        }
        #endregion

        public StringBuilder stringBuilder { get { return m_stringBuilder; } }
        private StringBuilder m_stringBuilder = new StringBuilder();
    }

}

