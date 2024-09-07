using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class GummyGambolProductPlane : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_LeftLaneObstacleCollider, m_CenterLaneObstacleCollider, m_RightLaneObstacleCollider;
        [SerializeField] private MeshRenderer[] m_LeftLaneProducts, m_CenterLaneProducts, m_RightLaneProducts;
        [SerializeField] private MeshRenderer m_PlaneMeshRenderer;

        public void Init(Material a_PlaneMaterial, Material a_CorrectMaterial, List<Material> a_IncorrectMaterials)
        {
            m_PlaneMeshRenderer.material = a_PlaneMaterial;

            int _Rand = Random.Range(0, 3);

            if (_Rand == 0)
            {
                m_LeftLaneObstacleCollider.enabled = false;

                m_LeftLaneProducts[1].material = a_CorrectMaterial;
                m_LeftLaneProducts[1].gameObject.SetActive(true);

                int _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_CenterLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_CenterLaneProducts[1].gameObject.SetActive(true);
                a_IncorrectMaterials.RemoveAt(_Index);

                _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_RightLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_RightLaneProducts[1].gameObject.SetActive(true);
            }
            else if (_Rand == 1)
            {
                m_CenterLaneObstacleCollider.enabled = false;

                m_CenterLaneProducts[1].material = a_CorrectMaterial;
                m_CenterLaneProducts[1].gameObject.SetActive(true);

                int _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_LeftLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_LeftLaneProducts[1].gameObject.SetActive(true);
                a_IncorrectMaterials.RemoveAt(_Index);

                _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_RightLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_RightLaneProducts[1].gameObject.SetActive(true);
            }
            else
            {
                m_RightLaneObstacleCollider.enabled = false;

                m_RightLaneProducts[1].material = a_CorrectMaterial;
                m_RightLaneProducts[1].gameObject.SetActive(true);

                int _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_LeftLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_LeftLaneProducts[1].gameObject.SetActive(true);
                a_IncorrectMaterials.RemoveAt(_Index);

                _Index = Random.Range(0, a_IncorrectMaterials.Count);

                m_CenterLaneProducts[1].material = a_IncorrectMaterials[_Index];
                m_CenterLaneProducts[1].gameObject.SetActive(true);
            }
        }
    }
}
