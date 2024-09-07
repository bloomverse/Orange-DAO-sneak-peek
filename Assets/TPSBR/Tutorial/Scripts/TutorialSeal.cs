using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class TutorialSeal : MonoBehaviour
    {
        [SerializeField] private GameObject m_Seal;
        [SerializeField] private int m_ObjectiveId;
        [SerializeField] private bool m_Start;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                m_Seal.transform.DOLocalMoveY(0, 0.5f).OnComplete(() =>
                {
                    if (m_Start)
                    {
                        MissionSystem.Instance.BeginMission(1);
                    }
                    else
                    {
                        MissionSystem.Instance.Progress(1, m_ObjectiveId);
                    }
                });

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
