using UnityEngine;
using System.Collections.Generic;
using TPSBR;

namespace Fusion.Addons.KCC
{
    public partial class KCC
    {
        public MoveState MoveState { get; set; }

        public void SetAim(bool aim)
        {
            if (IsProxy == true)
                return;

            Data.Aim = aim;
        }

        public void SetRecoil(Vector2 recoil)
        {
            if (IsProxy == true)
                return;

            KCCData data = _renderData;
            data.Recoil = recoil;

            if (IsInFixedUpdate == true)
            {
                data = _fixedData;
                data.Recoil = recoil;
            }
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            if (!IsProxy)
            {
                Data.SpeedMultiplier = multiplier;
            }
        }

        partial void InitializeUserNetworkProperties(KCCNetworkContext networkContext, List<IKCCNetworkProperty> networkProperties)
        {
            networkProperties.Add(new KCCNetworkVector2<KCCNetworkContext>(networkContext, 0.0f, (context, value) => context.Data.Recoil = value, (context) => context.Data.Recoil, null));
            networkProperties.Add(new KCCNetworkBool<KCCNetworkContext>(networkContext, (context, value) => context.Data.IsGrounded = value, (context) => context.Data.IsGrounded, null));
            networkProperties.Add(new KCCNetworkBool<KCCNetworkContext>(networkContext, (context, value) => context.Data.Aim = value, (context) => context.Data.Aim, null));
            // New property for speed multiplier
            networkProperties.Add(new KCCNetworkFloat<KCCNetworkContext>(networkContext, 1.0f, (context, value) => context.Data.SpeedMultiplier = value, (context) => context.Data.SpeedMultiplier, null));
        }
    }

    public partial class KCCData
    {
        public bool Aim;
        public Vector2 Recoil;
        public float SpeedMultiplier { get; set; } = 1.0f;  // Default speed multiplier

        partial void CopyUserDataFromOther(KCCData other)
        {
            Aim = other.Aim;
            Recoil = other.Recoil;
            SpeedMultiplier = other.SpeedMultiplier;  // Ensure speed multiplier is copied
        }
    }
}
