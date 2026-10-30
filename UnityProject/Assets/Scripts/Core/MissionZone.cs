using UnityEngine;
using Yemen4x4.Vehicles;

namespace Yemen4x4.Core
{
    public enum MissionZoneType { Boarding, Destination, Depot }

    /// <summary>Advances mission stages when the player's 4x4 enters a blue interaction zone.</summary>
    [RequireComponent(typeof(Collider))]
    public sealed class MissionZone : MonoBehaviour
    {
        private YemenGameWorld world;
        private MissionZoneType zoneType;

        public void Configure(YemenGameWorld owner, MissionZoneType type)
        {
            world = owner;
            zoneType = type;
            var zoneCollider = GetComponent<Collider>();
            zoneCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (world != null && other.GetComponentInParent<Arcade4x4Controller>() != null)
                world.OnMissionZoneEntered(zoneType);
        }
    }
}