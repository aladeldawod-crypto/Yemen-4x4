using UnityEngine;

namespace Yemen4x4.World
{
    public enum RoadSurface { CityAsphalt, DesertSand, RuralDirt, Mud }

    [CreateAssetMenu(menuName = "Yemen 4x4/Road Surface Profile")]
    public sealed class RoadSurfaceProfile : ScriptableObject
    {
        public RoadSurface surface;
        [Range(0.15f, 1f)] public float grip = 0.85f;
        [Range(0f, 1f)] public float rollingResistance = 0.2f;
        [Range(0f, 1f)] public float dustAmount;
        [Range(0f, 1f)] public float mudAmount;

        public float GetGripMultiplier()
        {
            switch (surface)
            {
                case RoadSurface.DesertSand: return 0.72f;
                case RoadSurface.RuralDirt: return 0.82f;
                case RoadSurface.Mud: return 0.48f;
                default: return 1f;
            }
        }
    }
}
