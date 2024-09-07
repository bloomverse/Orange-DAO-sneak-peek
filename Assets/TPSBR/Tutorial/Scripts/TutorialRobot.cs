using DG.Tweening;
using Fusion;
using System.Collections;
//using Unity.VisualScripting;
using UnityEngine;

namespace TPSBR
{
    public class TutorialRobot : NetworkBehaviour, IHitTarget
    {
        private enum RobotState
        {
            Idle = 1,
            Moving = 2,
            Attack1 = 3,
            Attack2 = 4,
        }

        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_LaserWindUp, m_LaserLoop, m_LaserWindDown;
        [SerializeField] private Transform m_hitIndicatorPivot, m_RobotLeftPos, m_RobotRightPos;
        private Transform m_Player;
        [SerializeField] private EGA_Laser m_LeftLaser, m_RightLaser;
        [SerializeField] private SkinnedMeshRenderer m_MeshRenderer;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private UnityEngine.UI.Image m_HPBar;
        [SerializeField] private GameObject m_DamageFire1, m_DamageFire2;
        [SerializeField] private CapsuleCollider m_Collider;
        

        private WaitForSeconds m_LaserAttackTime = new WaitForSeconds(7f), m_HitFeedbackDuration = new WaitForSeconds(0.25f), m_BetweenActionsWait = new WaitForSeconds(1f);
        private Color m_OffColor = Color.black, m_OnColor = Color.white, m_HitColor = Color.red, m_DefaultColor;
        [SerializeField] private float m_HP;
        private float m_MaxHP, m_HurtHPThreshold = 0.6f, m_SeverlyHurtHPThreshold = 0.3f;
        private bool m_Hit = false, m_IsLeft = false, m_IsMoving = false, m_Action = false, m_First = true, m_LastActionMove = false, m_Hurt = false, m_SeverlyHurt = false, m_LasersActive = false;

        [Header("Ragdoll")]
        public Rigidbody[] rigidbodies = new Rigidbody[11];

        Transform IHitTarget.HitPivot => m_hitIndicatorPivot != null ? m_hitIndicatorPivot : transform;
         public override void Spawned()
		{
            Debug.Log("Spawend robot");
        }

        void Awake()
        {
            m_MeshRenderer.material.SetColor("_EmissionColor", m_OffColor);
            m_DefaultColor = m_MeshRenderer.material.GetColor("_BaseColor");
            m_MaxHP = m_HP;

            foreach (var rb in rigidbodies)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        private void Start()
        {
            //gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!m_Action && m_Player != null)
            {
                if (m_First)
                {
                    StartCoroutine(MoveSide());
                    m_First = false;
                    m_Action = true;
                }
                else
                {
                    if (m_LastActionMove)
                    {
                        StartCoroutine(LaserAttack());
                    }
                    else
                    {
                        StartCoroutine(MoveSide());
                    }

                    m_Action = true;
                }
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (m_Player != null && !m_IsMoving && m_Animator)
            {
                m_Animator.SetLookAtWeight(1);
                m_Animator.SetLookAtPosition(m_Player.position);

                if (m_LasersActive)
                {
                    m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_Player.position);
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_Player.position);
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                }
                else
                {
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                }
            }
            else
            {
                m_Animator.SetLookAtWeight(0);
            }
        }

        public void Init()
        {
            m_Player = FindObjectOfType<TutorialLaserTarget>().transform;
            Debug.Log(m_Player  + " mplayer");
        }

        public IEnumerator MoveSide()
        {
            m_IsMoving = true;
            m_Animator.SetBool("moveSide", true);

            if (m_IsLeft)
            {
                transform.DORotateQuaternion(Quaternion.LookRotation(m_RobotRightPos.position - transform.position), 0.5f);
                transform.DOMove(m_RobotRightPos.position, 2f).OnComplete(() =>
                {
                    m_Animator.SetBool("moveSide", false);
                    m_IsMoving = false;
                    m_IsLeft = m_IsMoving;
                    transform.DORotateQuaternion(Quaternion.LookRotation(m_Player.position - transform.position), 0.5f);
                });
            }
            else
            {
                transform.DORotateQuaternion(Quaternion.LookRotation(m_RobotLeftPos.position - transform.position), 0.5f).OnComplete(() =>
                {
                    transform.DOMove(m_RobotLeftPos.position, 2f).OnComplete(() =>
                    {
                        m_Animator.SetBool("moveSide", false);
                        m_IsMoving = false;
                        m_IsLeft = !m_IsMoving;
                        transform.DORotateQuaternion(Quaternion.LookRotation(m_Player.position - transform.position), 0.5f);
                    });
                });

            }

            yield return new WaitForSeconds(3f);

            m_LastActionMove = true;
            m_Action = false;
        }

        public IEnumerator LaserAttack()
        {
            m_Animator.SetBool("attack2", true);
            m_AudioSource.PlayOneShot(m_LaserWindUp);
            m_MeshRenderer.material.DOColor(m_OnColor, "_EmissionColor", 2f).OnComplete(() =>
            {
                float _Distance = 0;

                m_LasersActive = true;
                m_LeftLaser.gameObject.SetActive(m_LasersActive);
                m_RightLaser.gameObject.SetActive(m_LasersActive);

                m_AudioSource.PlayOneShot(m_LaserLoop);

                DOTween.To(() => _Distance, x => _Distance = x, 50f, 1f).OnUpdate(() =>
                {
                    m_LeftLaser.MaxLength = _Distance;
                    m_RightLaser.MaxLength = _Distance;
                });
            });

            yield return m_LaserAttackTime;

            m_Animator.SetBool("attack2", false);
            m_AudioSource.Stop();

            m_LasersActive = false;
            m_LeftLaser.gameObject.SetActive(m_LasersActive);
            m_RightLaser.gameObject.SetActive(m_LasersActive); 
            m_LeftLaser.MaxLength = 0;
            m_RightLaser.MaxLength = 0;

            yield return m_BetweenActionsWait;

            m_LastActionMove = false;
            m_Action = false;
        }

        private void SetColor(Color a_Color)
        {
            m_MeshRenderer.material.SetColor("_BaseColor", a_Color);
        }

        private IEnumerator Hit()
        {
            m_Hit = true;
            SetColor(m_HitColor);

            yield return m_HitFeedbackDuration;

            SetColor(m_DefaultColor);
            m_Hit = false;
        }

        public void Die()
        {
            m_Player = null;
            StopAllCoroutines();
            m_LeftLaser.gameObject.SetActive(false);
            m_RightLaser.gameObject.SetActive(false);
            m_AudioSource.Stop();

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            m_Collider.enabled = true;
            m_Animator.enabled = false;
            TutorialManager.Instance.EndFinalBoss(transform.position);
            MissionSystem.Instance.Progress(1, 12);
        }

         void IHitTarget.ProcessHit(ref HitData hit)
        {
            Debug.Log("Processing hitr");
            m_HP -= hit.Amount;
            m_HPBar.fillAmount = m_HP / m_MaxHP;

            if (!m_Hurt && m_HPBar.fillAmount <= m_HurtHPThreshold)
            {
                m_Hurt = true;
                m_DamageFire1.SetActive(true);
            }
            if (!m_SeverlyHurt && m_HPBar.fillAmount <= m_SeverlyHurtHPThreshold)
            {
                m_SeverlyHurt = true;
                m_DamageFire2.SetActive(true);
            }

            if (!m_Hit)
            {
                StartCoroutine(Hit());
            }

            if (m_HP <= 0)
            {
                Die();
            }
        }
    }
}
