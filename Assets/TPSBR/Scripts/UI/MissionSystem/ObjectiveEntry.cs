using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TPSBR
{
    public class ObjectiveEntry : MonoBehaviour
    {
        [SerializeField] protected Animator m_Anim;
        public TMP_Text m_Description;
        public string m_DescriptionString;
        public int m_ObjectiveId, m_Rank;
        public bool m_Complete = false;

        public virtual bool Progress() { return false; }
        public virtual string GetDescription() { return m_Description.text; }
        public virtual void UpdateGUI() { }
        public virtual void ShowObjectiveEntry()
        {
            m_Anim?.SetBool("show", true);
        }
    }

    public class SimpleObjectiveEntry : ObjectiveEntry
    {
        public SimpleObjectiveEntry Init(int a_ObjectiveId, string a_Description, int a_Rank)
        {
            m_ObjectiveId = a_ObjectiveId;
            m_Anim = GetComponent<Animator>();
            m_Description = transform.GetChild(1).GetComponent<TMP_Text>();
            m_DescriptionString = a_Description;
            m_Description.text = a_Description;
            m_Rank = a_Rank;

            return this;
        }
        public override bool Progress() 
        {
            m_Complete = true;
            m_Anim.SetBool("complete", m_Complete);

            return m_Complete; 
        }
        public override void UpdateGUI()
        {
            base.UpdateGUI();
        }
    }

    public class CountObjectiveEntry : ObjectiveEntry
    {
        public int m_TargetValue;
        public int m_StartingValue;

        public CountObjectiveEntry Init(int a_ObjectiveId, string a_Description, int a_Rank, int a_TargetValue, int a_StartingValue = 0)
        {
            m_ObjectiveId = a_ObjectiveId;
            m_Anim = GetComponent<Animator>();
            m_Description = transform.GetChild(1).GetComponent<TMP_Text>();
            m_DescriptionString = a_Description;
            m_TargetValue = a_TargetValue;
            m_StartingValue = a_StartingValue;
            m_Description.text = GetDescription();
            m_Rank = a_Rank;

            return this;
        }
        public override bool Progress() 
        { 
            m_StartingValue++;
            UpdateGUI();

            bool _Done = m_StartingValue >= m_TargetValue;

            if (_Done)
            {
                m_Anim.SetBool("complete", true);
                m_Complete = true;
            }

            return _Done; 
        }
        public override string GetDescription() => "(" + m_StartingValue + "/" + m_TargetValue + ") " + m_DescriptionString;
        public override void UpdateGUI()
        {
            m_Description.text = GetDescription();
        }
    }
}
