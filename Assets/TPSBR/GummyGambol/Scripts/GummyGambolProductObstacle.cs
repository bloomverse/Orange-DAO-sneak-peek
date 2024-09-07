using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class GummyGambolProductObstacle : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_FloorCollider;
        [SerializeField] private BoxCollider m_ActivationCollider;

        public int m_PlanesToObstacle, m_SeparatingPlanes;
        private int m_ProductIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(GummyGambolManager.Instance.ShowProductObstacleClue(m_ProductIndex));
            }
        }

        public void Init(GameObject a_ObstaclePrefab, GameObject a_EmptyPlanePrefab, Material a_FloorMaterial, ref int a_StartingPos, int a_PlaneOffset, int a_Level, Material[] a_ProductMaterials)
        {
            m_ProductIndex = Random.Range(0, a_ProductMaterials.Length);
            m_ActivationCollider.center = new Vector3(0, 0, a_StartingPos);

            int _StartPos = a_StartingPos;

            for (int i = 0; i < m_PlanesToObstacle; i++, a_StartingPos += a_PlaneOffset) 
            {
                Instantiate(a_EmptyPlanePrefab, new Vector3(0,0, a_StartingPos), Quaternion.identity, transform).GetComponent<MeshRenderer>().material = a_FloorMaterial;

                if (i == m_PlanesToObstacle - 1) 
                {
                    List<Material> _IncorrectProductMaterials = new List<Material>(a_ProductMaterials);

                    _IncorrectProductMaterials.RemoveAt(m_ProductIndex);

                    Instantiate(a_ObstaclePrefab, new Vector3(0, 0, a_StartingPos + a_PlaneOffset), Quaternion.identity, transform).GetComponent<GummyGambolProductPlane>().Init(a_FloorMaterial, a_ProductMaterials[m_ProductIndex], _IncorrectProductMaterials);
                }
            }

            int _Length = a_PlaneOffset * m_PlanesToObstacle;

            m_FloorCollider.center = new Vector3(0, -2.5f, (_Length / 2) - (a_PlaneOffset / 2));
            m_FloorCollider.size = new Vector3(a_PlaneOffset, 5, _Length);
        }
    }
}
