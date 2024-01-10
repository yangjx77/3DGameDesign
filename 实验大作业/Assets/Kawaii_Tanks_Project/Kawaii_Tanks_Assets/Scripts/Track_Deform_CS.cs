using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This script must be attached to Tracks.
//用于控制履带变形
namespace ChobiAssets.KTP
{
	
	[System.Serializable]
	public class IntArray
	{
		public int[] intArray;
		public IntArray (int[] newIntArray)
		{
			intArray = newIntArray;
		}
	}

	public class Track_Deform_CS : MonoBehaviour
	{
	
		public int anchorNum;   //锚点数量
		public Transform[] anchorArray;   //锚点的Transform组件数组
		public float[] widthArray;        //每个锚点的宽度数组
 		public float[] heightArray;       //每个锚点的高度数组
		public float[] offsetArray;       //每个锚点的偏移量数组

		Mesh thisMesh;                    //履带的Mesh组件 
 		public float[] initialPosArray;   //初始位置数组
 		public Vector3[] initialVertices; //初始顶点数组
		public IntArray[] movableVerticesList;  //可移动顶点的数组

		Vector3[] currentVertices;              //当前顶点数组

		//在脚本启动时调用
		//获取MeshFilter组件的Mesh引用
		//将Mesh的MarkDynamic属性设置为true
		//检查锚点数组中的锚点是否已分配，如果有未分配的锚点，则输出错误信息并销毁脚本。
		//检查顶点数组是否已准备好，如果没有，则调用Set_Vertices()方法
		void Awake ()
		{
			thisMesh = GetComponent < MeshFilter > ().mesh;
			thisMesh.MarkDynamic ();
			// Check Anchor wheels.
			for (int i = 0; i < anchorArray.Length; i++) {
				if (anchorArray [i] == null) {
					Debug.LogError ("Anchor Wheel is not assigned in " + this.name);
					Destroy (this);
				}
			}
			// Check vertices list.
			if (initialPosArray == null || initialPosArray.Length == 0 || initialVertices == null || initialVertices.Length == 0 || movableVerticesList == null || movableVerticesList.Length == 0) {
				Set_Vertices ();
			}
			//
			currentVertices = new Vector3 [initialVertices.Length];
		}

		//用于旧版本的坦克
		//在Awake()中调用，如果顶点列表未准备好
		//初始化初始位置数组、初始顶点数组和可移动顶点数组
		//根据锚点位置和指定的范围，获取在范围内的顶点列表
		//创建IntArray对象，并将顶点列表赋给可移动顶点数组
		void Set_Vertices ()
		{ // for old version tanks.
			Debug.Log ("Vertices Lists are not prepared in the prefab.");
			initialPosArray = new float [anchorArray.Length];
			initialVertices = thisMesh.vertices;
			movableVerticesList = new IntArray [anchorArray.Length];
			// Get vertices in the range.
			for (int i = 0; i < anchorArray.Length; i++) {
				if (anchorArray [i] != null) {
					Transform anchorTransform = anchorArray [i];
					initialPosArray [i] = anchorTransform.localPosition.x;
					Vector3 anchorPos = transform.InverseTransformPoint (anchorTransform.position);
					List <int> withinVerticesList = new List <int> ();
					for (int j = 0; j < thisMesh.vertices.Length; j++) {
						float distZ = Mathf.Abs (anchorPos.z - thisMesh.vertices [j].z);
						float distY = Mathf.Abs (anchorPos.y - thisMesh.vertices [j].y);
						if (distZ <= widthArray [i] * 0.5f && distY <= heightArray [i] * 0.5f) {
							withinVerticesList.Add (j);
						}
					}
					IntArray withinVerticesArray = new IntArray (withinVerticesList.ToArray ());
					movableVerticesList [i] = withinVerticesArray;
				}
			}
		}

		//每帧调用一次
		//将初始顶点数组复制到当前顶点数组
		//遍历锚点数组，计算锚点位置相对于初始位置的差值
		//遍历可移动顶点数组，根据差值调整顶点的Y坐标
		//将当前顶点数组应用于Mesh的vertices属性
		void Update ()
		{
			initialVertices.CopyTo (currentVertices, 0);
			for (int i = 0; i < anchorArray.Length; i++) {
				float tempDist = anchorArray [i].localPosition.x - initialPosArray [i];
				for (int j = 0; j < movableVerticesList [i].intArray.Length; j++) {
					currentVertices [movableVerticesList [i].intArray [j]].y += tempDist;
				}
			}
			thisMesh.vertices = currentVertices;
		}

		//在Scene视图中绘制Gizmos
		//根据锚点数组和偏移量数组绘制矩形Gizmos
		void OnDrawGizmos ()
		{
			if (anchorArray != null && anchorArray.Length != 0 && offsetArray != null && offsetArray.Length != 0) {
				Gizmos.color = Color.green;
				for (int i = 0; i < anchorArray.Length; i++) {
					if (anchorArray [i] != null) {
						Vector3 tempSize = new Vector3 (0.0f, heightArray [i], widthArray [i]);
						Vector3 tempCenter = anchorArray [i].position;
						tempCenter.y += offsetArray [i];
						Gizmos.DrawWireCube (tempCenter, tempSize);
					}
				}
			}
		}

		//从"Game_Controller_CS"脚本调用
		//根据传入的isPaused参数启用或禁用脚本
		void Pause (bool isPaused)
		{ // Called from "Game_Controller_CS".
			this.enabled = !isPaused;
		}

	}

}