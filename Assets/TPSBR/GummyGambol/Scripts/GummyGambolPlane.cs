using UnityEngine;

namespace TPSBR
{
    public class GummyGambolPlane : MonoBehaviour
    {
        [SerializeField] private MeshRenderer m_MeshRenderer;
        [SerializeField] private Transform m_Obstacles;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GummyGambolManager.Instance.Checkpoint();
            }
        }

        public GameObject Init(Material a_Material)
        {
            m_MeshRenderer.material = a_Material;

            return gameObject;
        }

        public GameObject Init(Material a_Material, GGPlaneObstacleData a_PlaneObstacleData)
        {
            m_MeshRenderer.material = a_Material;

            int n = a_PlaneObstacleData.m_ObstacleLaneDatas.Length;

            for (int i = 0; i < n; i++)
            {
                int _Lane = (int)a_PlaneObstacleData.m_ObstacleLaneDatas[i].m_Lane;
                int m = a_PlaneObstacleData.m_ObstacleLaneDatas[i].m_ObstaclesToActivate.Length;

                for (int j = 0; j < m; j++)
                {
                    m_Obstacles.GetChild(_Lane).GetChild(a_PlaneObstacleData.m_ObstacleLaneDatas[i].m_ObstaclesToActivate[j]).gameObject.SetActive(true);
                }
            }

            return gameObject;
        }
    }
}
