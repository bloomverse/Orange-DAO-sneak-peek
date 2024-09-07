using System.Collections.Generic;
using System;

namespace TPSBR
{
    public static class MissionLib
    {
        /// <summary>
        /// Class containing logic regarding mission objectives
        /// </summary>
        public static Dictionary<Tuple<int, int>, Type> m_MissionObjectiveParameters = new Dictionary<Tuple<int, int>, Type>()
        {
            // Tutorial
            { new Tuple<int, int>(1, 1), typeof(Tutorial_1_Look) },
            { new Tuple<int, int>(1, 2), typeof(Tutorial_2_Move) },
            { new Tuple<int, int>(1, 3), typeof(Tutorial_3_Jump) },
            { new Tuple<int, int>(1, 4), typeof(Tutorial_4_Weapon) },
            { new Tuple<int, int>(1, 5), typeof(Tutorial_5_Shooting) },
            { new Tuple<int, int>(1, 6), typeof(Tutorial_6_Reloading) },
            { new Tuple<int, int>(1, 7), typeof(Tutorial_7_Aiming) },
            { new Tuple<int, int>(1, 8), typeof(Tutorial_8_Jetpack) },
            { new Tuple<int, int>(1, 9), typeof(Tutorial_9_Spray) },
            { new Tuple<int, int>(1, 10), typeof(Tutorial_10_Boost) },
            { new Tuple<int, int>(1, 11), typeof(Tutorial_11_Chest) },
            { new Tuple<int, int>(1, 12), typeof(Tutorial_12_Boss) },
            { new Tuple<int, int>(1, 13), typeof(Tutorial_13_Lobby) },
            { new Tuple<int, int>(1, 14), typeof(Tutorial_14_HV_Enter) },
            { new Tuple<int, int>(1, 15), typeof(Tutorial_15_HV_Interact) },
            { new Tuple<int, int>(1, 16), typeof(Tutorial_16_HV_Game) },
            { new Tuple<int, int>(1, 17), typeof(Tutorial_17_HV_Email) },
            { new Tuple<int, int>(1, 777), typeof(Tutorial_Drones) },
        };
    }
}
