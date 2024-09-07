

using UnityEngine;

namespace TPSBR
{
	public interface IInteraction
	{
		public string     Name        { get; }
		public string     Description { get; }
		public string     LongDescription { get; }
		public Vector3    HUDPosition { get; }
		public int    	  HUDID		  { get; }
		public bool       IsActive    { get; }
		//public GameObject GameObject { get; }
	}

	public interface IPickup : IInteraction
	{
	}
}
