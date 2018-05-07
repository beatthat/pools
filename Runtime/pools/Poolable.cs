using UnityEngine;


namespace BeatThat.ObjectLifecycle
{
	/// <summary>
	/// Poolable objects support activation and passivation.
	/// Contractually, 
	/// 
	/// 1) when an object is checked out of an ObjectPool, Poolable::Activate is called, 
	/// e.g. to unhide the object
	/// 2) when an object is returned to an ObjectPool, Poolable::Passivate is called, 
	/// e.g. to stop animation, disable renders
	/// 3) Users of the object must *NEVER* destroy the object, but rather call Poolable::Delete, 
	/// which generally returns the object to the pool
	/// </summary>
	public interface Poolable
	{
		/// <summary>
		/// The deletion delegate for a Poolable object.
		/// Generally, when an object's lifecycle is being managed by an ObjectPool,
		/// the ObjectPool will assign a deletionDelegate that responds to Poolable::Delete()
		/// calls by passivating the object and returning it to the pool.
		/// If an object's lifeycle is *NOT* being managed by an ObjectPool, e.g.
		/// if you directly instantiate a Poolable object somewhere,
		/// then the deletationDelegate should be left NULL, and the object's Poolable::Delete() 
		/// should respond by destroying the object.
		/// 
		/// The basic contract behavior desscribed above can be inherited 
		/// via PoolableBase (for POCO types)
		/// or via TTPoolableBase (for UnityEngine.Component types)
		/// </summary>
		/// <value>
		/// The deletion delegate.
		/// </value>
		System.Action deletionDelegate 
		{
			get; set;
		}
		
		void Activate();
		void Passivate();
		
		// Generally should call the deletion delegate set by the object's owner,
		// which, in the case of an objectpool, will passivate the object and return it to the pool
		void Delete();
	}

	public static class PoolableExt
	{

		public static void Delete(this Poolable a)
		{
			if(a != null) {
				if(a.deletionDelegate != null) {
					a.deletionDelegate();
				}
				else {
					if(a is Component) {
						GameObject go = (a as Component).gameObject;
						if(go != null) {
							GameObject.Destroy(go);
						}
					}
				}
			}
		}

		public static void Destroy(this Poolable a)
		{
			if(a != null) {
				a.deletionDelegate = null;
				a.Delete();
			}
		}
	}


}
