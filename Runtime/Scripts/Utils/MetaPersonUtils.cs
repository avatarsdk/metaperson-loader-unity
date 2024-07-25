/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2024
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class MetaPersonUtils
	{
		public static void ReplaceAvatar(GameObject srcAvatarObject, GameObject dstAvatarObject)
		{
			Animator srcAnimator = srcAvatarObject.GetComponentInChildren<Animator>();
			Animator dstAnimator = dstAvatarObject.GetComponentInChildren<Animator>();

			float time = 0;
			int nameHash = 0;
			if (dstAnimator != null)
			{
				time = dstAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				nameHash = dstAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
				if (srcAnimator != null)
					dstAnimator.avatar = srcAnimator.avatar;
			}

			Transform rootBone = dstAvatarObject.transform;
			Transform rootTransform = dstAvatarObject.transform;
			SkinnedMeshRenderer[] dstMeshRenderes = dstAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (dstMeshRenderes != null && dstMeshRenderes.Length > 0)
			{
				rootBone = dstMeshRenderes[0].rootBone;
				rootTransform = dstMeshRenderes[0].transform.parent;
				foreach (SkinnedMeshRenderer meshRenderer in dstMeshRenderes)
					Object.DestroyImmediate(meshRenderer.gameObject);
			}

			var dstTransformsMap = GetTransformsDictionary(rootBone.gameObject);
			SkinnedMeshRenderer[] srcMeshRenderers = srcAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (srcMeshRenderers != null)
			{
				foreach (SkinnedMeshRenderer meshRenderer in srcMeshRenderers)
				{
					Transform[] currentBones = meshRenderer.bones;
					Transform[] newBones = new Transform[currentBones.Length];
					for (int i = 0; i < currentBones.Length; i++)
					{
						dstTransformsMap.TryGetValue(currentBones[i].name, out newBones[i]);
					}
					meshRenderer.bones = newBones;
					meshRenderer.rootBone = rootBone;
					meshRenderer.transform.SetParent(rootTransform, false);
				}
			}

			if (dstAnimator != null)
			{
				dstAnimator.Rebind();
				dstAnimator.Play(nameHash, 0, time);
			}

			var srcTransforms = srcAvatarObject.GetComponentsInChildren<Transform>();
			foreach (Transform srcTransform in srcTransforms)
			{
				Transform dstTransform = null;
				if (dstTransformsMap.TryGetValue(srcTransform.name, out dstTransform))
				{
					dstTransform.localPosition = srcTransform.localPosition;
					dstTransform.localRotation = srcTransform.localRotation;
				}
			}

			Object.DestroyImmediate(srcAvatarObject);
		}

		private static Dictionary<string, Transform> GetTransformsDictionary(GameObject gameObject)
		{
			Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
			Dictionary<string, Transform> transformsDictionary = new Dictionary<string, Transform>();
			foreach (Transform t in transforms)
				if (!transformsDictionary.ContainsKey(t.name))
					transformsDictionary.Add(t.name, t);
			return transformsDictionary;
		}
	}
}
