using UnityEngine;
using UnityEngine.Analytics;

namespace TPSBR
{
	public class DummyProjectile : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public SceneContext Context { get; set; }

		public float MaxDistance => _damage.MaxDistance;
		public PiercingSetup Piercing => _piercing;

		// PRIVATE METHODS

		[SerializeField]
		private ProjectileDamage _damage;
		[SerializeField]
		private float _speed = 40f;
		[SerializeField]
		private PiercingSetup _piercing;

		private float _time;
		private float _duration;

		private Vector3 _start;
		public Vector3 _destination;

		private int _startFrame;

		private TrailRenderer[] _lineRenderers;

		// PUBLIC METHODS

		public void Fire(Vector3 start, Quaternion rotation, Vector3 destination)
		{
			_start = start;
			transform.position = start;
			transform.rotation = rotation;
			_destination = destination;
			_duration = Vector3.Magnitude(destination - this.transform.position) / _speed;
			_time = 0f;

			_startFrame = Time.frameCount;

			for (int i = 0; i < _lineRenderers.Length; i++)
			{
				_lineRenderers[i].Clear();
			}
            //this.transform.SetParent(null);
        }

		public float GetDamage(float distance)
		{
			return _damage.GetDamage(distance);
		}

		// MONOBEHAVIOR

		private void Awake()
		{
			_lineRenderers = GetComponentsInChildren<TrailRenderer>(true);
			//_start = this.transform.position;
			Destroy(this.gameObject, 0.1f);
        }

		private void Update()
		{
			if (_startFrame == Time.frameCount)
				return;

			_time += Time.deltaTime;

			if (_time >= _duration)
			{
				Context.ObjectCache.Return(this);
				return;
			}

			float progress = _time / _duration;
            transform.position = Vector3.Lerp(_start, _destination, progress);

            //transform.position += Vector3.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);
			//transform.position += transform.forward * _speed * Time.deltaTime;			
			//transform.LookAt(_destination);

        }
	}
}
