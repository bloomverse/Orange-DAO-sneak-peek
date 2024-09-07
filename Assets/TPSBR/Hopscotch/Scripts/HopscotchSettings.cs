using UnityEngine;

namespace TPSBR
{
    [CreateAssetMenu(menuName = "MinigameSettings/Hopscotch")]
    public class HopscotchSettings : ScriptableObject
    {
        public int m_ScorePerCoin, m_ScorePerCheckpoint, m_ScorePerRun;
        public GameObject m_CheckpointPlatformPrefab, m_BloomieCoinPrefab;
        public int m_MaxLevel;
        public float m_CameraFarClippingPlane;

        [Header("Stage 1 - Moving Platforms | Data")]
        public GameObject m_MovingPlatformPrefab;
        public Vector3 m_StartingPosition, m_StartingRotation;
        public MovingPlatformData[] m_Platforms;

        [Header("Stage 2 - Moving Platforms | Data")]
        public GameObject m_ProductPuzzlePrefab;
        public Material[] m_PuzzleMaterials;
        public float m_ProductPuzzleZOffset;
        public int m_ProductPuzzleCount;

        [Header("Stage 3 - Jetpack | Data")]
        public GameObject m_JetpackLevel;
        public GameObject m_JetpackPowerUp;
        public float m_JetpackBarMaxValue;
        public float[] m_RingSpeeds, m_RingMaxSpeeds;

        [Header("Stage 4 - Boost | Data")]
        public GameObject m_BoostLevel;
        public GameObject m_BoostPowerUp, m_GummyProjectilePrefab;
        public Material[] m_GummyProjectileMaterials;
        public float m_GummyProjectileLaunchSpeed, m_GummyProjectileLaunchMaxSpeed, m_GummyProjectileLaunchAngle, m_ProjectileLaunchInterval, m_BoostBarMaxValue, m_GummyProjectileDespawnTime, m_GummyProjectileMinDespawnTime;
        public int m_GummyProjectilePoolCount;

        [Header("End")]
        public GameObject m_EndPlatformPrefab;
        public GameObject m_BoostPowerUpLinePrefab;
        public int m_BoostPowerUpLines, m_BoostPowerUpDistance;
    }

    [System.Serializable]
    public struct MovingPlatformData
    {
        public Material m_Material;
        public Vector3 m_Direction, m_NextPlatformOffset;
        public float m_Radius, m_Speed, m_MaxSpeed;
    }
}
