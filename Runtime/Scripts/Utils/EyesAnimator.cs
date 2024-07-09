/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2024
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public class EyesAnimator : MonoBehaviour
	{
		private class BlendshapesInfo
		{
			public float maxWeight = 0.0f;

			public int eyeLookUpLeftIdx = -1;
			public int eyeLookUpRightIdx = -1;
			public int eyeLookDownLeftIdx = -1;
			public int eyeLookDownRightIdx = -1;
			public int eyeLookOutLeftIdx = -1;
			public int eyeLookInRightIdx = -1;
			public int eyeLookInLeftIdx = -1;
			public int eyeLookOutRightIdx = -1;
		}

		public Transform leftEye;

		public Transform rightEye;

		public List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>();

		private Dictionary<SkinnedMeshRenderer, BlendshapesInfo> meshRendererBlendshapesInfoMap = new Dictionary<SkinnedMeshRenderer, BlendshapesInfo>();

		private readonly float extremeUpAngle = -23.0f;
		private readonly float extremeDownAngle = 23.0f;
		private readonly float extremeLeftOutAngle = -45.0f;
		private readonly float extremeLeftInAngle = 25.0f;
		private readonly float extremeRightOutAngle = 45.0f;
		private readonly float extremeRightInAngle = -25.0f;

		private readonly string leftEyeTransformName = "LeftEye";
		private readonly string rightEyeTransformName = "RightEye";
		private readonly string avatarHeadMeshRendererName = "AvatarHead";
		private readonly string avatarEyelashesMeshRendererName = "AvatarEyelashes";

		private void Start()
		{
			if (leftEye == null || rightEye == null)
			{
				var transforms = GetComponentsInChildren<Transform>(true);
				foreach(var t in transforms)
				{
					if (leftEye == null && t.name == leftEyeTransformName)
						leftEye = t;
					if (rightEye == null && t.name == rightEyeTransformName)
						rightEye = t;
				}
			}

			var renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach(var renderer in renderers)
			{
				if ((renderer.name == avatarHeadMeshRendererName || renderer.name == avatarEyelashesMeshRendererName) && !meshRenderers.Contains(renderer))
					meshRenderers.Add(renderer);
			}

			foreach(var meshRenderer in meshRenderers)
			{
				BlendshapesInfo blendshapesInfo = new BlendshapesInfo();
				blendshapesInfo.eyeLookUpLeftIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookUpLeft");
				blendshapesInfo.eyeLookUpRightIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookUpRight");
				blendshapesInfo.eyeLookDownLeftIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookDownLeft");
				blendshapesInfo.eyeLookDownRightIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookDownRight");
				blendshapesInfo.eyeLookOutLeftIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookOutLeft");
				blendshapesInfo.eyeLookInRightIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookInRight");
				blendshapesInfo.eyeLookInLeftIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookInLeft");
				blendshapesInfo.eyeLookOutRightIdx = meshRenderer.sharedMesh.GetBlendShapeIndex("eyeLookOutRight");
				blendshapesInfo.maxWeight = meshRenderer.sharedMesh.GetBlendShapeFrameWeight(blendshapesInfo.eyeLookUpLeftIdx, 0);

				meshRendererBlendshapesInfoMap.Add(meshRenderer, blendshapesInfo);
			}
		}

		public void SetLookUpWeight(float weight)
		{
			SetXAngle(leftEye, Mathf.Lerp(0.0f, extremeUpAngle, weight));
			SetXAngle(rightEye, Mathf.Lerp(0.0f, extremeUpAngle, weight));

			foreach(var pair in meshRendererBlendshapesInfoMap)
			{
				var meshRenderer = pair.Key;
				var blendshapesInfo = pair.Value;

				float maxWeight = blendshapesInfo.maxWeight;
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookUpLeftIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookUpRightIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookDownLeftIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookDownRightIdx, 0);
			}
		}

		public void SetLookDownWeight(float weight)
		{
			SetXAngle(leftEye, Mathf.Lerp(0.0f, extremeDownAngle, weight));
			SetXAngle(rightEye, Mathf.Lerp(0.0f, extremeDownAngle, weight));

			foreach (var pair in meshRendererBlendshapesInfoMap)
			{
				var meshRenderer = pair.Key;
				var blendshapesInfo = pair.Value;

				float maxWeight = blendshapesInfo.maxWeight;
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookUpLeftIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookUpRightIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookDownLeftIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookDownRightIdx, Mathf.Lerp(0, maxWeight, weight));
			}
		}

		public void SetLookLeftWeight(float weight)
		{
			SetYAngle(leftEye, Mathf.Lerp(0.0f, extremeLeftOutAngle, weight));
			SetYAngle(rightEye, Mathf.Lerp(0.0f, extremeRightInAngle, weight));

			foreach (var pair in meshRendererBlendshapesInfoMap)
			{
				var meshRenderer = pair.Key;
				var blendshapesInfo = pair.Value;

				float maxWeight = blendshapesInfo.maxWeight;
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookOutLeftIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookInRightIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookInLeftIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookOutRightIdx, 0);
			}
		}

		public void SetLookRightWeight(float weight)
		{
			SetYAngle(leftEye, Mathf.Lerp(0.0f, extremeLeftInAngle, weight));
			SetYAngle(rightEye, Mathf.Lerp(0.0f, extremeRightOutAngle, weight));

			foreach (var pair in meshRendererBlendshapesInfoMap)
			{
				var meshRenderer = pair.Key;
				var blendshapesInfo = pair.Value;

				float maxWeight = blendshapesInfo.maxWeight;
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookOutLeftIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookInRightIdx, 0);
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookInLeftIdx, Mathf.Lerp(0, maxWeight, weight));
				meshRenderer.SetBlendShapeWeight(blendshapesInfo.eyeLookOutRightIdx, Mathf.Lerp(0, maxWeight, weight));
			}
		}

		private void SetXAngle(Transform eye, float angle)
		{
			if (eye != null)
			{
				Vector3 euler = eye.localEulerAngles;
				euler.x = angle;
				eye.localEulerAngles = euler;
			}
		}

		private void SetYAngle(Transform eye, float angle)
		{
			if (eye != null)
			{
				Vector3 euler = eye.localEulerAngles;
				euler.y = angle;
				eye.localEulerAngles = euler;
			}
		}
	}
}
