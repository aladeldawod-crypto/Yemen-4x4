using UnityEngine;
namespace Yemen4x4.Vehicles
{
 [RequireComponent(typeof(Rigidbody))]
 public sealed class Arcade4x4Controller:MonoBehaviour
 {
  [SerializeField] private float acceleration=9500f,reverseAcceleration=4500f,steeringTorque=2600f,maxSpeed=32f;
  private Rigidbody body;private float throttle,steering; private FuelSystem fuel; public float DriveInput=>Mathf.Abs(throttle)>.01f?Mathf.Abs(throttle):Mathf.Abs(Input.GetAxis("Vertical")); private FuelSystem fuel; public float DriveInput=>Mathf.Abs(throttle)>.01f?Mathf.Abs(throttle):Mathf.Abs(Input.GetAxis("Vertical"));
  private void Awake(){body=GetComponent<Rigidbody>();fuel=GetComponent<FuelSystem>();fuel=GetComponent<FuelSystem>();body.centerOfMass=new Vector3(0,-.35f,0);body.interpolation=RigidbodyInterpolation.Interpolate;body.collisionDetectionMode=CollisionDetectionMode.ContinuousDynamic;}
  public void SetControls(float drive,float turn){throttle=Mathf.Clamp(drive,-1,1);steering=Mathf.Clamp(turn,-1,1);}
  private void FixedUpdate(){float drive=Mathf.Abs(throttle)>.01f?throttle:Input.GetAxis("Vertical");float turn=Mathf.Abs(steering)>.01f?steering:Input.GetAxis("Horizontal");if(fuel&&fuel.IsEmpty)drive=0f;if(body.velocity.magnitude<maxSpeed||drive<0){float force=drive>=0?acceleration:reverseAcceleration;body.AddForce(transform.forward*drive*force,ForceMode.Force);}body.AddTorque(transform.up*turn*steeringTorque*Mathf.Clamp01(body.velocity.magnitude/4f),ForceMode.Force);}
 }
}
