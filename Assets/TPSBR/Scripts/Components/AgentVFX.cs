  using UnityEngine;
 using System.Collections.Generic;
 using DG.Tweening;
using System.Collections;

namespace TPSBR
{
	public class AgentVFX : MonoBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private GameObject _hit;
		[SerializeField]
		private GameObject _criticalHit;

		[SerializeField]
		private GameObject _shieldEffect;


		[SerializeField]
		private Transform _shieldOrigin;

		[SerializeField]
		private ParticleSystem _runEffect;
		

		[SerializeField]
		private AudioEffect[] _soundEffects;


		[SerializeField]
		private GameObject _rootObj;



		[SerializeField]
		private Material _deathEffectMaterial;




		private Agent _agent;



		// PUBLIC MEMBERS

		public void OnSpawned(Agent agent)
		{
			var health = GetComponent<Health>();
			health.HitTaken += OnHitTaken;
			_agent = agent;
			_agent.SprintAction += OnSprint;
		}

		public void OnDespawned()
		{
			var health = GetComponent<Health>();
			_agent.SprintAction -= OnSprint;
			if (health != null)
			{
				health.HitTaken -= OnHitTaken;
			}
		}

		public void PlaySound(AudioSetup sound, EForceBehaviour force = EForceBehaviour.None)
		{
			if (ApplicationSettings.IsStrippedBatch == true)
				return;

			_soundEffects.PlaySound(sound, force);
		}


		public void OnSprint(bool state){
			if(state){
				var emission = 	_runEffect.emission;
    			emission.enabled = true;
				
			}else{
				var emission = 	_runEffect.emission;
    			emission.enabled = false;
			}

		}

		// PRIVATE METHODS

		private void OnHitTaken(HitData hit)
		{

			Debug.Log(_agent.Health.CurrentShield + " Current shield");

			if (hit.Amount <= 0 || hit.Action != EHitAction.Damage)
				return;

			if (hit.Position == Vector3.zero)
				return;

			var hitPrefab = hit.IsCritical == true ? _criticalHit : _hit;
			SpawnHit(hitPrefab, hit.Position, hit.Normal);

			if(_agent.Health.CurrentShield>0){
				SpawnShield(_shieldEffect, _shieldOrigin.position, hit.Normal);
			}


			// Copy older materials array 


			if(!_agent.Health.IsAlive){

				//Dissolve
				//StartCoroutine(StartCopy());

				

			}
			
		}


		private IEnumerator StartCopy(){

			yield return new WaitForSeconds(2f);

			FindAllChildren(_rootObj.transform);
            	GetChildObjectsWithTag("Cloth",true);

				Debug.Log(childrenWithTag.Count + " Count");
					 var propertyName = "_Dissolve";
					var targetValue = 1f;
				

				for (int i = 0; i < childrenWithTag.Count; i++)
				{
					SkinnedMeshRenderer renderer = childrenWithTag[i].GetComponent<SkinnedMeshRenderer>();

					GameObject gObj = new GameObject();

					Quaternion newPos =  Quaternion.Euler(childrenWithTag[i].transform.rotation.x-90,childrenWithTag[i].transform.rotation.y+90,childrenWithTag[i].transform.rotation.z);
					gObj.transform.SetPositionAndRotation(childrenWithTag[i].transform.position,childrenWithTag[i].transform.rotation);
					MeshRenderer mr =gObj.AddComponent<MeshRenderer>();
					MeshFilter mf = gObj.AddComponent<MeshFilter>();

					Mesh mesh = new Mesh();
					renderer.BakeMesh(mesh);



					mf.mesh = mesh;
					var rmat = new Material(_deathEffectMaterial);
					mr.material = rmat;
					
					rmat.SetFloat(propertyName, 0f);
					rmat.DOFloat(targetValue, propertyName, 6f);


					childrenWithTag[i].SetActive(false);

					
					 //Material[] materials = renderer.materials;

				
    
					// Iterate through all materials and replace them with the desired material
					//for (int e = 0; e < materials.Length; e++)
					//{
						//materials[e] = _deathEffectMaterial;
						//Debug.Log(materials[e].HasProperty("_Dissolve")+ " prop---");
					//	materials[e] = new Material(_deathEffectMaterial);
					//	materials[e].SetFloat(propertyName, 0f);
					//	materials[e].DOFloat(targetValue, propertyName, 4f);
					//}
					
					// Assign the new materials array to the renderer
					//renderer.materials = materials;

				}

		}



		public List<GameObject> allChildren;
		public List<GameObject> childrenWithTag;
		 public void FindAllChildren(Transform transform)
    {
        int len = transform.childCount;
 
        for (int i = 0; i < len; i++)
        {
            allChildren.Add(transform.GetChild(i).gameObject);
 
            if (transform.GetChild(i).childCount > 0)
                FindAllChildren(transform.GetChild(i).transform);
        }
    }
		public void GetChildObjectsWithTag(string _tag, bool active)
		{
			foreach (GameObject child in allChildren)
			{
				//Debug.Log(child.tag+ " TAG");
				if (child.tag == _tag && (child.activeSelf==true)){
					//	Debug.Log("Obj " + child.name);
						childrenWithTag.Add(child);
				}
			
			}
		}


		private void SpawnShield(GameObject hitPrefab, Vector3 position, Vector3 normal)
		{
			var hit = _agent.Context.ObjectCache.Get(hitPrefab, transform);
			var rotation = normal != Vector3.zero ? Quaternion.LookRotation(normal) : Quaternion.identity;
			hit.transform.SetPositionAndRotation(position, rotation);
			
			_agent.Context.ObjectCache.ReturnDeferred(hit, 2f);
		}




		private void SpawnHit(GameObject hitPrefab, Vector3 position, Vector3 normal)
		{
			var hit = _agent.Context.ObjectCache.Get(hitPrefab, transform);
			var rotation = normal != Vector3.zero ? Quaternion.LookRotation(normal) : Quaternion.identity;
			hit.transform.SetPositionAndRotation(position, rotation);

			_agent.Context.ObjectCache.ReturnDeferred(hit, 2f);
		}
	}
}
