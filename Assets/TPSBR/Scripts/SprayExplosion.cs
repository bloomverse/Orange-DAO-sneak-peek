using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace TPSBR
{
    public class SprayExplosion : NetworkBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]
        private GameObject _sprayPrefab;

        public override void Spawned()
		{
			if (Object.HasStateAuthority == true)
			{
				Debug.Log("Spray explosion");

				Transform trans = gameObject.transform;
				if(Runner.GetPhysicsScene().Raycast(trans.position, Vector3.down, out RaycastHit hitInfo, 30f) == true){
					Debug.Log("Spray Position: " + hitInfo.point);

					var networkBehaviour = _sprayPrefab.GetComponent<NetworkBehaviour>();
					NetworkBehaviour spray = Runner.Spawn(networkBehaviour, hitInfo.point, Quaternion.LookRotation(hitInfo.normal * -1), Object.StateAuthority);
					
					Debug.Log("Spray Object: " + spray.transform.position);
				}
			}
		}
    }
}
