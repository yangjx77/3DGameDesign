using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_ANDROID || UNITY_IPHONE
using UnityStandardAssets.CrossPlatformInput;
#endif

// This script must be attached to "Turret_Base".
namespace ChobiAssets.KTP
{
	
	public class Turret_Control_CS : MonoBehaviour
	{
		[Header ("Turret movement settings")]
		[Tooltip ("Maximum Rotation Speed. (Degree per Second)")] public float rotationSpeed = 15.0f;
		[Tooltip ("Time to reach the maximum speed. (Sec)")] public float acceleration_Time = 0.2f;
		[Tooltip ("Angle range for slowing down. (Degree)")] public float bufferAngle = 5.0f;
		[Tooltip ("Name of Image for marker.")] public string markerName = "Marker";
		[Tooltip ("Assign the 'Gun_Camera'.")] public GunCamera_Control_CS gunCameraScript;

		// Reference to "Cannon_Control_CS".
		[HideInInspector] public bool isTracking = true;
		[HideInInspector] public Vector3 targetPos;
		[HideInInspector] public Vector3 localTargetPos;
		[HideInInspector] public Vector3 adjustAng;

		Transform thisTransform;
		Transform rootTransform;
		float currentAng;
		Transform targetTransform;
		Vector3 targetOffset;
		Vector3 previousMousePos;
		float speedRate;
		Image markerImage;
		int layerMask = ~((1 << 2) + (1 << 9) + (1 << 10)); // Layer 2 = Ignore Ray, Layer 9 = Wheels, Layer 10 = Ignore All.
		float targetAng;
		bool gunCamFlag;

		#if UNITY_ANDROID || UNITY_IPHONE
		float spherecastRadius = 1.5f;
		bool isButtonDown = false;
		#else
		float spherecastRadius = 0.5f;
		#endif

		ID_Control_CS idScript;


		void Awake ()
		{
			thisTransform = this.transform;
			rootTransform = thisTransform.root;
			currentAng = thisTransform.localEulerAngles.y;
			if (gunCameraScript == null) {
				Debug.LogError ("'Gun Camera Script' is not assigned."); 
			}
			Find_Image ();
		}

		void Find_Image ()
		{
			// Find Marker Image.
			if (string.IsNullOrEmpty (markerName) == false) {
				GameObject markerObject = GameObject.Find (markerName);
				if (markerObject) {
					markerImage = markerObject.GetComponent <Image> ();
				}
			}
			if (markerImage) {
				markerImage.enabled = false;
			} else {
				Debug.LogWarning (markerName + "Image for Marker cannot be found in the scene.");
			}
		}

		void LateUpdate ()
		{
			if (idScript && idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				Mobile_Input () ;
				#else
				Desktop_Input ();
				#endif
				Marker_Control ();
			}
		}

		void Marker_Control ()
		{
			if (markerImage) {
				if (isTracking) {
					markerImage.transform.position = Camera.main.WorldToScreenPoint (targetPos);
					if (markerImage.transform.position.z < 0) {
						markerImage.enabled = false;
					} else {
						markerImage.enabled = true;
						if (targetTransform) {
							markerImage.color = Color.red;
						} else {
							markerImage.color = Color.white;
						}
					}
				} else {
					markerImage.enabled = false;
				}
			}
		}

		#if !UNITY_ANDROID && !UNITY_IPHONE
		void Desktop_Input ()
		{
			Mouse_Input ();
		}

		void Mouse_Input ()
		{
			if (idScript.aimButtonDown) {
				isTracking = true;
				Cast_Ray_Sphere (Input.mousePosition, true);
				previousMousePos = Input.mousePosition;
			} else if (idScript.aimButton) {
				if (gunCamFlag) {
					adjustAng += (Input.mousePosition - previousMousePos) * 0.03f;
					previousMousePos = Input.mousePosition;
				} else if (Mathf.Abs (targetAng) < 5.0f) {
					gunCamFlag = true;
					gunCameraScript.GunCam_On ();
				}
			} else if (idScript.aimButtonUp) {
				gunCamFlag = false;
				gunCameraScript.GunCam_Off ();
				adjustAng = Vector3.zero;
				targetTransform = null;
			} else {
				Cast_Ray_Sphere (Input.mousePosition, false);
			}
		}
		#endif

		#if UNITY_ANDROID || UNITY_IPHONE
		int fingerID;
		void Mobile_Input ()
		{
			// Set Target.
			if (CrossPlatformInputManager.GetButtonDown ("Target_Press")) {
				isTracking = true;
				#if UNITY_EDITOR
				Cast_Ray_Sphere (Input.mousePosition, true);
				#else
				Cast_Ray_Sphere (Input.touches [Input.touches.Length - 1].position, true);
				#endif
			} else if (CrossPlatformInputManager.GetButtonDown ("LockOff")) {
				isTracking = false;
				targetTransform = null;
			}
			// Control GunCam and Aiming.
			if (CrossPlatformInputManager.GetButtonDown ("GunCam_Press")) {
				isButtonDown = true ;
				#if UNITY_EDITOR
				previousMousePos = Input.mousePosition;
				#else
				fingerID = Input.touches.Length - 1;
				previousMousePos = Input.touches [fingerID].position;
				#endif
				return;
			}
			if (isButtonDown && CrossPlatformInputManager.GetButton ("GunCam_Press")) {
				if (gunCamFlag) {
					#if UNITY_EDITOR
					Vector3 currentMousePos = Input.mousePosition;
					#else
					Vector3 currentMousePos = Input.touches [fingerID].position;
					#endif
					adjustAng += (currentMousePos - previousMousePos) * 0.02f;
					previousMousePos = currentMousePos ;
				} else {
					if (Mathf.Abs (targetAng) < 5.0f) {
						gunCamFlag = true;
						gunCameraScript.GunCam_On ();
					}
				}
				return;
			}
			if (CrossPlatformInputManager.GetButtonUp ("GunCam_Press")) {
				gunCamFlag = false;
				isButtonDown = false;
				adjustAng = Vector3.zero;
				gunCameraScript.GunCam_Off ();
			}
		}
		#endif

		void Cast_Ray_Sphere (Vector3 screenPos, bool isLockOn)
		{ // Try to find the target that has a rigidbody.
			Ray ray = Camera.main.ScreenPointToRay (screenPos);
			RaycastHit [] raycastHits;
			raycastHits = Physics.SphereCastAll (ray, spherecastRadius, 1000.0f, layerMask);
			foreach (RaycastHit raycastHit in raycastHits) {
				Transform colliderTransform = raycastHit.collider.transform;
				if (colliderTransform.root != rootTransform) {
					if (raycastHit.transform.GetComponent <Rigidbody> () && colliderTransform.root.tag != "Finish") { //When 'raycastHit.collider.transform' is Turret, 'raycastHit.transform' should be the MainBody.
						targetTransform = raycastHit.transform; // Set the MainBody of the target.
						targetOffset.y = 0.5f;
						/* // Set the hit collider of the target.
						targetTransform = colliderTransform ;
						targetOffset = targetTransform.InverseTransformPoint (raycastHit.point);
						if (targetTransform.localScale != Vector3.one) { // for Armor_Collider.
							targetOffset.x *= targetTransform.localScale.x;
							targetOffset.y *= targetTransform.localScale.y;
							targetOffset.z *= targetTransform.localScale.z;
						}
						*/
						return ;
					}
				}
			}
			// The target cannot be found.
			Cast_Ray (screenPos, isLockOn);
		}

		void Cast_Ray (Vector3 screenPos, bool isLockOn)
		{ // Set the target position by casting a ray.
			Ray ray = Camera.main.ScreenPointToRay (screenPos);
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, 1000.0f, layerMask)) {
				Transform colliderTransform = raycastHit.collider.transform;
				if (colliderTransform.root != rootTransform) { // Ray hits other object.
					targetTransform = null;
					targetPos = raycastHit.point;
				} else { // Ray hits itself.
					if (isLockOn) {
						isTracking = false;
						targetTransform = null;
					} else {
						targetTransform = null;
						screenPos.z = 500.0f;
						targetPos = Camera.main.ScreenToWorldPoint (screenPos);
					}
				}
			} else { // Ray does not hit anythig.
				targetTransform = null;
				screenPos.z = 500.0f;
				targetPos = Camera.main.ScreenToWorldPoint (screenPos);
			}
		}

		void FixedUpdate ()
		{
			// Calculate Angle.
			if (isTracking) {
				if (targetTransform) {
					targetPos = targetTransform.position + (targetTransform.forward * targetOffset.z) + (targetTransform.right * targetOffset.x) + (targetTransform.up * targetOffset.y);
				}
				localTargetPos = thisTransform.InverseTransformPoint (targetPos);
				targetAng = Vector2.Angle (Vector2.up, new Vector2 (localTargetPos.x, localTargetPos.z)) * Mathf.Sign (localTargetPos.x);
				targetAng += adjustAng.x;
			} else {
				targetAng = Mathf.DeltaAngle (currentAng, 0.0f);
			}
			if (Mathf.Abs (targetAng) > 0.01f) {
				// Calculate Turn Rate.
				float targetSpeedRate = Mathf.Lerp (0.0f, 1.0f, Mathf.Abs (targetAng) / (rotationSpeed * Time.fixedDeltaTime + bufferAngle)) * Mathf.Sign (targetAng);
				// Calculate Rate
				speedRate = Mathf.MoveTowardsAngle (speedRate, targetSpeedRate, Time.fixedDeltaTime / acceleration_Time);
				// Rotate
				currentAng += rotationSpeed * speedRate * Time.fixedDeltaTime;
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, currentAng, 0.0f));
			}
		}

		void Destroy ()
		{ // Called from "Damage_Control_CS".
			// Change the hierarchy.
			thisTransform.parent = thisTransform.root;
			// Blow off the turret.
			Rigidbody tempRigidbody = GetComponent <Rigidbody> ();
			if (tempRigidbody == null) {
				tempRigidbody = gameObject.AddComponent <Rigidbody> ();
				tempRigidbody.mass = 100.0f;
			}
			Vector3 randomPos = new Vector3 (Random.Range (0.0f, 1.0f), 0.0f, Random.Range (0.0f, 1.0f));
			tempRigidbody.AddForceAtPosition (thisTransform.up * 500.0f, thisTransform.position + randomPos, ForceMode.Impulse);
			// Turn off the Marker and Gun Camera.
			if (idScript.isPlayer) {
				if (markerImage) {
					markerImage.enabled = false;
				}
				gunCameraScript.GunCam_Off ();
			}
			Destroy (this);
		}

		void Get_ID_Script (ID_Control_CS tempScript)
		{
			idScript = tempScript;
			if (idScript.isPlayer) {
				#if UNITY_ANDROID || UNITY_IPHONE
				isTracking = false;
				#endif
			} else {
				isTracking = false;
			}
			idScript.turretScript = this;
		}

		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

		public void Switch_Player (bool isPlayer)
		{
			#if !UNITY_ANDROID && !UNITY_IPHONE
			isTracking = true;
			#endif
			if (markerImage) {
				markerImage.enabled = false;
			}
		}

	}

}