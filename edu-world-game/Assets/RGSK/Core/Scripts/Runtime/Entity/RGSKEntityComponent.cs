using UnityEngine;
using Fusion;
namespace RGSK
{
    public abstract class RGSKEntityComponent : NetworkBehaviour
    {
        public RGSKEntity Entity { get; private set; }
        public bool Initialized { get; private set; }

        public virtual void Initialize(RGSKEntity e)
        {
            Entity = e;
            Initialized = true;
        }

        public virtual void ResetValues()
        {

        }
    }
}