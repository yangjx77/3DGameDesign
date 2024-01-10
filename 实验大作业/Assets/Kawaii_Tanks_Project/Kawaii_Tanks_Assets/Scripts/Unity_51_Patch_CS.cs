using UnityEngine;
using System.Collections;

//在 Unity 5.1 版本下修复 HingeJoint（铰链关节）的问题
namespace ChobiAssets.KTP
{
	
	public class Unity_51_Patch_CS : MonoBehaviour
	{
		void Awake ()
		{
			#if UNITY_5_1
		HingeJoint [] hingeJoints = GetComponentsInChildren < HingeJoint > () ;
		foreach ( HingeJoint hingeJoint in hingeJoints ) {
			JointSpring jointSpring = hingeJoint.spring ;
			jointSpring.targetPosition *= -1.0f ;
			hingeJoint.spring = jointSpring ;
		}
			#endif
			Destroy (this);
		}
	}

}