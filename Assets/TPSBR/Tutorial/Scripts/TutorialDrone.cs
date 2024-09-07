using DG.Tweening;
using Fusion;
using System.Collections;
using UnityEngine;

namespace TPSBR
{
    public class TutorialDrone : NetworkBehaviour, IHitTarget
    {
        [SerializeField] private Transform m_hitIndicatorPivot;
        [SerializeField] private MeshRenderer m_MeshRenderer;
        private WaitForSeconds m_HitFeedbackDuration = new WaitForSeconds(0.25f);
        private Color m_HitColor = Color.red, m_DefaultColor;
        [SerializeField] private float m_HP;
        private float m_YStart;
        private float m_IdleBobDistance = -0.25f;
        private bool m_Hit = false;
       
        Transform IHitTarget.HitPivot => m_hitIndicatorPivot != null ? m_hitIndicatorPivot : transform;
        
        public override void Spawned()
		{
            Debug.Log("Spawend robot");
        }

        void Awake()
        {
            m_YStart = transform.position.y;
            m_DefaultColor = m_MeshRenderer.material.GetColor("_BaseColor");
        }

        private void Start()
        {
            transform.DOMoveY(m_YStart - m_IdleBobDistance, 1f).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            try{
                TutorialManager.Instance.DestroyDrone(transform.position + Vector3.up);
            }catch(System.SystemException e){
                Debug.Log(e.ToString());
            }
             
        }

        private void Update()
        {
            if(transform.parent!=null){
                transform.LookAt(transform.parent.position);
            }
            
        }

        void IHitTarget.ProcessHit(ref HitData hit)
        {
            Debug.Log("processing hit..........");
            m_HP -= hit.Amount;

            if (!m_Hit)
            {
                StartCoroutine(Hit());
            }

            if (m_HP <= 0)
            {
                Destroy(gameObject);
            }
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
    }
}
