using UnityEngine;
using UnityEngine.Events;

namespace Yemen4x4.Vehicles
{
    public sealed class FuelSystem : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float tankCapacity = 80f;
        [SerializeField, Min(0f)] private float consumptionPerSecond = 0.025f;
        [SerializeField] private UnityEvent<float> onFuelChanged;
        public float Fuel { get; private set; }
        public bool IsEmpty => Fuel <= 0.01f;

        private void Awake() { Fuel = tankCapacity; }
        private void Update()
        {
            if (IsEmpty) return;
            Fuel = Mathf.Max(0f, Fuel - consumptionPerSecond * Mathf.Abs(Input.GetAxis("Vertical")) * Time.deltaTime);
            onFuelChanged?.Invoke(Fuel / tankCapacity);
        }
        public void Refuel(float litres)
        {
            Fuel = Mathf.Clamp(Fuel + Mathf.Max(0f, litres), 0f, tankCapacity);
            onFuelChanged?.Invoke(Fuel / tankCapacity);
        }
        public void FillTank() => Refuel(tankCapacity);
    }
}
