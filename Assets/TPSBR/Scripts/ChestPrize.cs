using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace TPSBR
{
    public class ChestPrize : MonoBehaviour
    {
        [SerializeField]
		private AnimationClip _openAnimation;
		[SerializeField]
		private AnimationClip _closeAnimation;

        [Header("Audio")]
		[SerializeField]
		private AudioEffect _audioEffect;
		[SerializeField]
		private AudioSetup _openSound;
		[SerializeField]
		private AudioSetup _closeSound;

        private Animation      _animation;

        private Animation      _particles;

        [SerializeField] MMF_Player introEffect;

        public void Open()
		{
                if (_animation.clip != _openAnimation)
                {
                    _animation.clip = _openAnimation;
                    _animation.Play();
                    var player = FindObjectOfType<Player>();
                    _audioEffect.Play(_openSound, EForceBehaviour.ForceAny);
                    introEffect.PlayFeedbacks();
                }
        }

        public void Close(){
            if (_animation.clip != _closeAnimation)
                {
                    _animation.clip = _closeAnimation;
                    _animation.Play();

                    _audioEffect.Play(_closeSound, EForceBehaviour.ForceAny);
                }
        }

       private void Awake()
		{
			_animation = GetComponent<Animation>();
		}

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
