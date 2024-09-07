using UnityEngine;

namespace TPSBR
{
    [CreateAssetMenu(menuName ="MinigameSettings/GummyGambol")]
    public class GummyGambolSettings : ScriptableObject
    {
        public Material[] m_LevelMaterials;
        public float[] m_LevelSpeeds;
        public int[] m_LevelPlanes;
        public int m_InitialPlaneCount;
        public float m_ChangeLaneSpeed, m_StartingCountdown, m_PlaneOffset, m_LevelOffset, m_RouletteTimer;

        [Header("Plane Obstacles")]
        [Range(0, 1)]
        public float[] m_PlaneObstacleProbability;
        public PlaneObstaclePool[] m_PlaneObstacleDifficultyPool;

        [Header("Product Obstacles")]
        [Range(0, 1)]
        public float[] m_ProductObstacleProbability;

        public Sprite[] m_ProductSprites;
        public Material[] m_ProductMaterials;
    }

    [System.Serializable]
    public struct PlaneObstaclePool
    {
        public GGPlaneObstacleData[] m_PlaneObstacleDifficultySetups;
    }

    [System.Serializable]
    public struct ProductObstaclePool
    {
        public GGProductObstacleData[] m_ProductObstacleDifficultySetups;
    }

}
