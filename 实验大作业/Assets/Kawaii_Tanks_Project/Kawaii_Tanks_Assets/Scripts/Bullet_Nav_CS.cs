using UnityEngine;
using System.Collections;

namespace ChobiAssets.KTP
{
	
	public class Bullet_Nav_CS : MonoBehaviour
	{
		[ Header ("Bullet settings")]
		[ Tooltip ("Life time of the bullet. (Sec)")] public float lifeTime = 5.0f;
		[ Tooltip ("Prefab of the broken effect.")] public GameObject brokenObject;

		// Set by "Fire_Spawn".
		[HideInInspector] public float attackForce;

		Transform thisTransform;
		Rigidbody thisRigidbody;
		bool isLive = true;
		bool isRayHit = false;
		int layerMask = ~((1 << 10) + (1 << 2)); // Ignore Layer 2(Ignore Raycast) & Layer 10(suspensions).
		Vector3 nextPos;
		Transform hitTransform;
		Vector3 hitNormal;
		Ray ray;

		void Awake ()
		{
			thisTransform = transform;
			thisRigidbody = GetComponent <Rigidbody> ();
			ray = new Ray ();
			Destroy (this.gameObject, lifeTime);
		}

		void FixedUpdate ()
		{
			if (isLive) {
				thisTransform.LookAt (thisTransform.position + thisRigidbody.velocity);
				if (isRayHit == false) {
					ray.origin = thisTransform.position;
					ray.direction = thisRigidbody.velocity;
					RaycastHit raycastHit;
					Physics.Raycast (ray, out raycastHit, thisRigidbody.velocity.magnitude * Time.fixedDeltaTime, layerMask);
					if (raycastHit.collider) {
						nextPos = raycastHit.point;
						hitTransform = raycastHit.collider.transform;
						hitNormal = raycastHit.normal;
						isRayHit = true;
					}
				} else {
					thisTransform.position = nextPos;
					thisRigidbody.position = nextPos;
					isLive = false;
					Hit ();
				}
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			if (isLive) {
				isLive = false;
				hitTransform = collision.collider.transform;
				hitNormal = collision.contacts [0].normal;
				Hit ();
			}
		}

		void Hit ()
		{
			// Create brokenObject.
			if (brokenObject) {
				Instantiate (brokenObject, thisTransform.position, Quaternion.identity);
			}
			// Calculate hitEnergy.
			float hitAngle = Mathf.Abs (90.0f - Vector3.Angle (thisRigidbody.velocity, hitNormal));
			float hitEnergy = attackForce * Mathf.Lerp (0.0f, 1.0f, Mathf.Sqrt (hitAngle / 90.0f));
			//
			if (hitTransform) {
				// Find "Armor_Collider", and calculate the hitEnergy.
				Armor_Collider_CS armorScript = hitTransform.GetComponent <Armor_Collider_CS> ();
				if (armorScript) {
					hitEnergy *= armorScript.damageMultiplier;
				}
				// Find "Damage_Control_CS", and send the hitEnergy.
				Damage_Control_CS damageScript = hitTransform.root.GetComponent <Damage_Control_CS> ();
				if (damageScript) {
					damageScript.Get_Damage (hitEnergy);
				}
			}
			Destroy (this.gameObject);
		}
	}

}
