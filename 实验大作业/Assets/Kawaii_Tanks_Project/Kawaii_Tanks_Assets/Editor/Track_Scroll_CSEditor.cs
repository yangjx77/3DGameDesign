using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.KTP
{

	[ CustomEditor (typeof(Track_Scroll_CS))]
	public class Track_Scroll_CSEditor : Editor
	{
		SerializedProperty referenceWheelProp;
		SerializedProperty scrollRateProp;
		SerializedProperty textureNameProp;

		void OnEnable ()
		{
			referenceWheelProp = serializedObject.FindProperty ("referenceWheel");
			scrollRateProp = serializedObject.FindProperty ("scrollRate");
			textureNameProp = serializedObject.FindProperty ("textureName");
		}

		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Find RoadWheel [ Left ]", GUILayout.Width (200))) {
					Find_RoadWheel (true);
				}
				if (GUILayout.Button ("Find RoadWheel [ Right ]", GUILayout.Width (200))) {
					Find_RoadWheel (false);
				}
				EditorGUILayout.Space ();

				referenceWheelProp.objectReferenceValue = EditorGUILayout.ObjectField ("Reference Wheel", referenceWheelProp.objectReferenceValue, typeof(Transform), true);
				EditorGUILayout.Slider (scrollRateProp, -0.01f, 0.01f, "Scroll Rate");
				textureNameProp.stringValue = EditorGUILayout.TextField ("Texture Name in Shader", textureNameProp.stringValue);

				serializedObject.ApplyModifiedProperties ();
			}
		}

		void Find_RoadWheel (bool isLeft)
		{
			Transform bodyTransform = Selection.activeGameObject.transform.parent;
			Wheel_Rotate_CS [] wheelScripts = bodyTransform.GetComponentsInChildren <Wheel_Rotate_CS> ();
			float minDist = Mathf.Infinity;
			Transform closestWheel = null;
			foreach (Wheel_Rotate_CS wheelScript in wheelScripts) {
				Transform connectedTransform = wheelScript.GetComponent <HingeJoint> ().connectedBody.transform;
				MeshFilter meshFilter = wheelScript.GetComponent <MeshFilter> ();
				if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
					float tempDist = Vector3.Distance (bodyTransform.position, wheelScript.transform.position); // Distance to the MainBody.
					if (isLeft) { // Left.
						if (wheelScript.transform.localEulerAngles.z == 0.0f) { // Left.
							if (tempDist < minDist) {
								closestWheel = wheelScript.transform;
								minDist = tempDist;
							}
						}
					} else { // Right.
						if (wheelScript.transform.localEulerAngles.z != 0.0f) { // Right.
							if (tempDist < minDist) {
								closestWheel = wheelScript.transform;
								minDist = tempDist;
							}
						}
					}
				}
			}
			referenceWheelProp.objectReferenceValue = closestWheel;
		}
	}

}
