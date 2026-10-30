using UnityEngine;

namespace Yemen4x4.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Arcade4x4Controller : MonoBehaviour
    {
        [SerializeField] private float acceleration = 9500f;
        [SerializeField] private float reverseAcceleration = 4500f;
        [SerializeField] private float steeringTorque = 2600f;
        [SerializeField] private float maxSpeed = 32f;
        [SerializeField] private Transform centerOfMass;
        [SerializeField] private WheelCollider[] drivenWheels;
        [SerializeField] private WheelCollider[] steerWheels;
        private Rigidbody body;
        private float throttle, steering;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            if (centerOfMass) body.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // UI touch controls call SetControls; keyboard axes remain useful in editor testing.
        public void SetControls(float drive, float turn)
        {
            throttle = Mathf.Clamp(drive, -1f, 1f);
            steering = Mathf.Clamp(turn, -1f, 1f);
        }

        private void FixedUpdate()
        {
            float drive = Mathf.Abs(throttle) > 0.01f ? throttle : Input.GetAxis("Vertical");
            float turn = Mathf.Abs(steering) > 0.01f ? steering : Input.GetAxis("Horizontal");
            if (body.velocity.magnitude < maxSpeed || drive < 0f)
            {
                float force = drive >= 0f ? acceleration : reverseAcceleration;
                foreach (var wheel in drivenWheels) if (wheel) wheel.motorTorque = drive * force / Mathf.Max(1, drivenWheels.Length);
            }
            foreach (var wheel in steerWheels) if (wheel) wheel.steerAngle = turn * 30f;
            body.AddTorque(transform.up * turn * steeringTorque * Mathf.Clamp01(body.velocity.magnitude / 4f), ForceMode.Force);
        }
    }
}
