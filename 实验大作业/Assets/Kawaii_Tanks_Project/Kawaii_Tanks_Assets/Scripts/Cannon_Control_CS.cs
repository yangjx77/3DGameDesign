using UnityEngine;
using System.Collections;

// This script must be attached to "Cannon_Base".
namespace ChobiAssets.KTP
{
	
	public class Cannon_Control_CS : MonoBehaviour
	{
		[ Header ("Cannon movement settings")]
		[ Tooltip ("Rotation Speed. (Degree per Second)")] public float rotationSpeed = 10.0f;
		[ Tooltip ("Angle range for slowing down. (Degree)")] public float bufferAngle = 1.0f;
		[ Tooltip ("Maximum elevation angle. (Degree)")] public float maxElev = 15.0f;
		[ Tooltip ("Maximum depression angle. (Degree)")] public float maxDep = 10.0f;

		Transform thisTransform;
		Turret_Control_CS turretScript;
		float currentAng;


		void Awake ()
		{
			thisTransform = this.transform;
			turretScript = thisTransform.GetComponentInParent <Turret_Control_CS> ();
			if (turretScript == null) {
				Debug.LogError ("Cannon_Base cannot find Turret_Control_CS.");
				Destroy (this);
			}
			currentAng = thisTransform.localEulerAngles.x;
		}

		void FixedUpdate ()
		{
			// Calculate Angle.
			float targetAng;
			if (turretScript.isTracking) {
				targetAng = Manual_Angle ();
				targetAng += Mathf.DeltaAngle (0.0f, thisTransform.localEulerAngles.x) + turretScript.adjustAng.y;
			} else {
				targetAng = -Mathf.DeltaAngle (currentAng, 0.0f); 
			}
			if (Mathf.Abs (targetAng) > 0.01f) {
				// Calculate Speed Rate
				float speedRate = -Mathf.Lerp (0.0f, 1.0f, Mathf.Abs (targetAng) / (rotationSpeed * Time.fixedDeltaTime + bufferAngle)) * Mathf.Sign (targetAng);
				// Rotate
				currentAng += rotationSpeed * speedRate * Time.fixedDeltaTime;
				currentAng = Mathf.Clamp (currentAng, -maxElev, maxDep);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (currentAng, 0.0f, 0.0f));
			}
		}

		float Manual_Angle ()
		{
			float targetAng;
			targetAng = Mathf.Rad2Deg * (Mathf.Asin ((turretScript.localTargetPos.y - thisTransform.localPosition.y) / Vector3.Distance (thisTransform.localPosition, turretScript.localTargetPos)));
			return targetAng;
		}

		void Destroy ()
		{ // Called from "Damage_Control_CS".
			thisTransform.localRotation = Quaternion.Euler (new Vector3 (maxDep, 0.0f, 0.0f));
			Destroy (this);
		}

	}

}
