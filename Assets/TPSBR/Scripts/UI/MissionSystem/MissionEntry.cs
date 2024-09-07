using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class MissionEntry : MonoBehaviour
    {
        [SerializeField] private RectTransform m_MissionObjectivesParent;
        [SerializeField] private Animator m_Anim;
        [SerializeField] private Image m_MissionBackground;
        [SerializeField] private TMP_Text m_MissionTitle;
        [SerializeField] private Dictionary<int, MissionObjectiveParameters> m_MissionObjectiveParameters = new Dictionary<int, MissionObjectiveParameters>();
        [SerializeField] private List<ObjectiveEntry> m_ObjectiveEntries = new List<ObjectiveEntry>();
        [SerializeField] private Sprite m_NormalSprite, m_CompletedSprite;
        private int m_CurrentRank = 1;

        [SerializeField]
        private ParticleSystem doneEffect;

        public MissionEntry Init(int a_MissionId, string a_MissionTitle, List<Objective> a_MissionObjectives, GameObject a_MissionEntryPrefab)
        {
            m_MissionTitle.text = a_MissionTitle;

            foreach (Objective _Objective in a_MissionObjectives)
            {
                GameObject _GO = Instantiate(a_MissionEntryPrefab, m_MissionObjectivesParent);

                // if (_Objective.rank == m_CurrentRank) _GO.SetActive(true);

                if (_Objective.targetValue > 1)
                {
                    m_ObjectiveEntries.Add(_GO.AddComponent<CountObjectiveEntry>().Init(_Objective.objectiveId, _Objective.objectiveDescription, _Objective.rank, _Objective.targetValue, _Objective.currentValue));
                }
                else
                {
                    m_ObjectiveEntries.Add(_GO.AddComponent<SimpleObjectiveEntry>().Init(_Objective.objectiveId, _Objective.objectiveDescription, _Objective.rank));
                }

                if (!m_MissionObjectiveParameters.ContainsKey(_Objective.objectiveId))
                    m_MissionObjectiveParameters.Add(_Objective.objectiveId, (MissionObjectiveParameters)Activator.CreateInstance(MissionLib.m_MissionObjectiveParameters[new Tuple<int, int>(a_MissionId, _Objective.objectiveId)], new object[] { a_MissionId, _Objective.objectiveId }));
            }

            return this;
        }

        public bool Progress(int a_ObjectiveId)
        {
            Debug.Log(a_ObjectiveId + "a_ObjectiveId" + m_ObjectiveEntries.Count + " ");
            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {

                if (m_ObjectiveEntries[i].m_ObjectiveId == a_ObjectiveId && m_ObjectiveEntries[i].m_Rank == m_CurrentRank)
                {
                    if (m_ObjectiveEntries[i].Progress())
                    {
                        // Objective Complete
                        StartCoroutine(m_MissionObjectiveParameters[m_ObjectiveEntries[i].m_ObjectiveId].ObjectiveCompleted());

                        if (AllCurrentRankObjectivesDone())
                        {
                            //doneEffect.Play();
                            m_Anim.SetTrigger("complete");
                            m_CurrentRank++;
                            //StartCoroutine(WaitToClose(3f));
                        }

                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public void CleanUp()
        {

        }

        public void SetDefaultHeader() => m_MissionBackground.sprite = m_NormalSprite;
        public void SetCompletedHeader() => m_MissionBackground.sprite = m_CompletedSprite;

        public IEnumerator WaitToClose(float a_SecondsToWait)
        {
            yield return new WaitForSeconds(a_SecondsToWait);

            m_Anim.SetBool("open", false);
        }

        public void BeginMissionObjectives(bool a_OpenPanel = true)
        {
            //MissionSystem.Instance.m_MissionEntryPrefab.SetActive(false);

            Debug.Log("Obj Count: " + m_ObjectiveEntries.Count);

            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {
                if (m_ObjectiveEntries[i].m_Rank == m_CurrentRank && !m_ObjectiveEntries[i].m_Complete)
                {
                    m_ObjectiveEntries[i].transform.SetAsFirstSibling();
                    m_ObjectiveEntries[i].gameObject.SetActive(a_OpenPanel);
                    StartCoroutine(m_MissionObjectiveParameters[m_ObjectiveEntries[i].m_ObjectiveId].ObjectiveStart());
                }
                else if (m_ObjectiveEntries[i].gameObject.activeSelf)
                {
                    m_ObjectiveEntries[i].gameObject.SetActive(false);
                }
            }

            m_Anim.SetBool("open", a_OpenPanel);
        }

        public bool AllCurrentRankObjectivesDone()
        {
            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {
                if (m_ObjectiveEntries[i].m_Rank != m_CurrentRank) continue;

                if (!m_ObjectiveEntries[i].m_Complete)
                {
                    return false;
                }
            }

            return true;
        }
        public bool AllLastRankObjectivesDone()
        {
            if (m_CurrentRank <= 1) return false;

            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {
                if (m_ObjectiveEntries[i].m_Rank != (m_CurrentRank - 1)) continue;

                if (!m_ObjectiveEntries[i].m_Complete)
                {
                    return false;
                }
            }

            return true;
        }
        public bool AllRankObjectivesDone(int a_Rank)
        {
            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {
                if (m_ObjectiveEntries[i].m_Rank != a_Rank) continue;

                if (!m_ObjectiveEntries[i].m_Complete)
                {
                    return false;
                }
            }

            return true;
        }
        public void CloseMissionEntry() => m_Anim.SetBool("open", false);
        public void OpenMissionEntry()
        {
            m_Anim.SetBool("open", true);

            for (int i = 0; i < m_ObjectiveEntries.Count; i++)
            {
                if (m_ObjectiveEntries[i].m_Rank != m_CurrentRank) continue;

                m_ObjectiveEntries[i].gameObject.SetActive(true);
            }
        }
    }
}
