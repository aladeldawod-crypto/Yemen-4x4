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

        private Rigidbody body;
        private float throttle;
        private float steering;
        private FuelSystem fuel;

        public float DriveInput => Mathf.Abs(throttle) > .01f
            ? Mathf.Abs(throttle)
            : Mathf.Abs(Input.GetAxis("Vertical"));

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            fuel = GetComponent<FuelSystem>();
            body.centerOfMass = new Vector3(0f, -.35f, 0f);
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void SetControls(float drive, float turn)
        {
            throttle = Mathf.Clamp(drive, -1f, 1f);
            steering = Mathf.Clamp(turn, -1f, 1f);
        }

        private void FixedUpdate()
        {
            float drive = Mathf.Abs(throttle) > .01f ? throttle : Input.GetAxis("Vertical");
            float turn = Mathf.Abs(steering) > .01f ? steering : Input.GetAxis("Horizontal");

            if (fuel != null && fuel.IsEmpty) drive = 0f;

            if (body.velocity.magnitude < maxSpeed || drive < 0f)
            {
                float force = drive >= 0f ? acceleration : reverseAcceleration;
                body.AddForce(transform.forward * drive * force, ForceMode.Force);
            }

            body.AddTorque(transform.up * turn * steeringTorque *
                Mathf.Clamp01(body.velocity.magnitude / 4f), ForceMode.Force);
        }
    }
}