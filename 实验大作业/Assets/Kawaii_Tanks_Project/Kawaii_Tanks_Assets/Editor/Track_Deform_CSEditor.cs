using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ChobiAssets.KTP
{
	
	[ CustomEditor (typeof(Track_Deform_CS))]
	public class Track_Deform_CSEditor : Editor
	{
		SerializedProperty anchorNumProp;
		SerializedProperty anchorArrayProp;
		SerializedProperty widthArrayProp;
		SerializedProperty heightArrayProp;
		SerializedProperty offsetArrayProp;

		void OnEnable ()
		{
			anchorNumProp = serializedObject.FindProperty ("anchorNum");
			anchorArrayProp = serializedObject.FindProperty ("anchorArray");
			widthArrayProp = serializedObject.FindProperty ("widthArray");
			heightArrayProp = serializedObject.FindProperty ("heightArray");
			offsetArrayProp = serializedObject.FindProperty ("offsetArray");
		}

		public override void OnInspectorGUI ()
		{
			Set_Inspector ();
			if (GUI.changed) {
				Set_Vertices ();
			}
			if (Event.current.commandName == "UndoRedoPerformed") {
				Set_Vertices ();
			}
		}

		void Set_Inspector ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();
				EditorGUILayout.Space ();

				if (GUILayout.Button ("Find RoadWheels [ Left ]", GUILayout.Width (200))) {
					Find_RoadWheels (true);
				}
				if (GUILayout.Button ("Find RoadWheels [ Right ]", GUILayout.Width (200))) {
					Find_RoadWheels (false);
				}

				EditorGUILayout.IntSlider (anchorNumProp, 1, 64, "Number of Anchor Wheels");
				EditorGUILayout.Space ();

				anchorArrayProp.arraySize = anchorNumProp.intValue;
				widthArrayProp.arraySize = anchorNumProp.intValue;
				heightArrayProp.arraySize = anchorNumProp.intValue;
				offsetArrayProp.arraySize = anchorNumProp.intValue;
				for (int i = 0; i < anchorArrayProp.arraySize; i++) {
					anchorArrayProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Anchor Wheel", anchorArrayProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
					EditorGUILayout.Slider (widthArrayProp.GetArrayElementAtIndex (i), 0.0f, 10.0f, "Weight Width");
					EditorGUILayout.Slider (heightArrayProp.GetArrayElementAtIndex (i), 0.0f, 10.0f, "Weight Height");
					EditorGUILayout.Slider (offsetArrayProp.GetArrayElementAtIndex (i), -10.0f, 10.0f, "Offset");
					EditorGUILayout.Space ();
				}

				// Update Value
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Update Values")) {
					Set_Vertices ();
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				//
				serializedObject.ApplyModifiedProperties ();
			}
		}

		void Set_Vertices ()
		{
			GameObject thisGameObject = Selection.activeGameObject;
			if (thisGameObject.GetComponent <MeshFilter> ().sharedMesh == null) {
				Debug.LogError ("Mesh is not assigned in the Mesh Filter.");
				return;
			}
			PrefabUtility.DisconnectPrefabInstance (thisGameObject); // Break prefab connection.
			Mesh thisMesh = thisGameObject.GetComponent <MeshFilter> ().sharedMesh;
			float[] initialPosArray = new float [ anchorArrayProp.arraySize ];
			IntArray[] movableVerticesList = new IntArray [ anchorArrayProp.arraySize ];
			// Get vertices in the range.
			for (int i = 0; i < anchorArrayProp.arraySize; i++) {
				if (anchorArrayProp.GetArrayElementAtIndex (i).objectReferenceValue != null) {
					Transform anchorTransform = anchorArrayProp.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
					initialPosArray [i] = anchorTransform.localPosition.x;
					Vector3 anchorPos = thisGameObject.transform.InverseTransformPoint (anchorTransform.position);
					List < int > withinVerticesList = new List < int > ();
					for (int j = 0; j < thisMesh.vertices.Length; j++) {
						float distZ = Mathf.Abs (anchorPos.z - thisMesh.vertices [j].z);
						float distY = Mathf.Abs ((anchorPos.y + offsetArrayProp.GetArrayElementAtIndex (i).floatValue) - thisMesh.vertices [j].y);
						if (distZ <= widthArrayProp.GetArrayElementAtIndex (i).floatValue * 0.5f && distY <= heightArrayProp.GetArrayElementAtIndex (i).floatValue * 0.5f) {
							withinVerticesList.Add (j);
						}
					}
					IntArray withinVerticesArray = new IntArray (withinVerticesList.ToArray ());
					movableVerticesList [i] = withinVerticesArray;
				}
			}
			// Set values.
			Track_Deform_CS deformScript = thisGameObject.GetComponent < Track_Deform_CS > ();
			deformScript.initialPosArray = initialPosArray;
			deformScript.initialVertices = thisMesh.vertices;
			deformScript.movableVerticesList = movableVerticesList;
		}

		void Find_RoadWheels (bool isLeft)
		{
			// Find RoadWheels.
			List <Transform> roadWheelsList = new List <Transform> ();
			List <Transform> tempList = new List <Transform> ();
			Transform bodyTransform = Selection.activeGameObject.transform.parent;
			Wheel_Rotate_CS [] wheelScripts = bodyTransform.GetComponentsInChildren <Wheel_Rotate_CS> ();
			foreach (Wheel_Rotate_CS wheelScript in wheelScripts) {
				Transform connectedTransform = wheelScript.GetComponent <HingeJoint> ().connectedBody.transform;
				MeshFilter meshFilter = wheelScript.GetComponent <MeshFilter> ();
				if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
					if (isLeft) { // Left.
						if (wheelScript.transform.localEulerAngles.z == 0.0f) { // Left.
							tempList.Add (wheelScript.transform);
						}
					} else { // Right.
						if (wheelScript.transform.localEulerAngles.z != 0.0f) { // Right.
							tempList.Add (wheelScript.transform);
						}
					}
				}
			}
			// Sort (rear >> front)
			int tempCount = tempList.Count;
			for (int i = 0; i < tempCount; i++) {
				float maxPosZ = Mathf.Infinity;
				int targetIndex = 0;
				for (int j = 0; j < tempList.Count; j++) {
					if (tempList [j].position.z < maxPosZ) {
						maxPosZ = tempList [j].position.z;
						targetIndex = j;
					}
				}
				roadWheelsList.Add (tempList [targetIndex]);
				tempList.RemoveAt (targetIndex);
			}
			// Set
			anchorNumProp.intValue = roadWheelsList.Count;
			anchorArrayProp.arraySize = anchorNumProp.intValue;
			widthArrayProp.arraySize = anchorNumProp.intValue;
			heightArrayProp.arraySize = anchorNumProp.intValue;
			offsetArrayProp.arraySize = anchorNumProp.intValue;
			for (int i = 0; i < anchorArrayProp.arraySize; i++) {
				anchorArrayProp.GetArrayElementAtIndex (i).objectReferenceValue = roadWheelsList [i];
				if (widthArrayProp.GetArrayElementAtIndex (i).floatValue == 0.0f) {
					widthArrayProp.GetArrayElementAtIndex (i).floatValue = 0.5f;
				}
				if (heightArrayProp.GetArrayElementAtIndex (i).floatValue == 0.0f) {
					heightArrayProp.GetArrayElementAtIndex (i).floatValue = 1.0f;
				}
			}
		}

	}

}