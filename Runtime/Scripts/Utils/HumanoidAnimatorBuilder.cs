/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, December 2023
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public class HumanoidAnimatorBuilder
	{
		private readonly Dictionary<string, string> metaPersonToHumanoidBoneMap = new Dictionary<string, string>()
		{
			{"Hips", "Hips"},
			{"LeftUpLeg", "LeftUpperLeg"},
			{"RightUpLeg", "RightUpperLeg"},
			{"LeftLeg", "LeftLowerLeg"},
			{"RightLeg", "RightLowerLeg"},
			{"LeftFoot", "LeftFoot"},
			{"RightFoot", "RightFoot"},
			{"Spine", "Spine"},
			{"Spine1", "Chest"},
			{"Neck", "Neck"},
			{"Head", "Head"},
			{"LeftShoulder", "LeftShoulder"},
			{"RightShoulder", "RightShoulder"},
			{"LeftArm", "LeftUpperArm"},
			{"RightArm", "RightUpperArm"},
			{"LeftForeArm", "LeftLowerArm"},
			{"RightForeArm", "RightLowerArm"},
			{"LeftHand", "LeftHand"},
			{"RightHand", "RightHand"},
			{"LeftToeBase", "LeftToes"},
			{"RightToeBase", "RightToes"},
			{"LeftHandThumb1", "Left Thumb Proximal"},
			{"LeftHandThumb2", "Left Thumb Intermediate"},
			{"LeftHandThumb3", "Left Thumb Distal"},
			{"LeftHandIndex1", "Left Index Proximal"},
			{"LeftHandIndex2", "Left Index Intermediate"},
			{"LeftHandIndex3", "Left Index Distal"},
			{"LeftHandMiddle1", "Left Middle Proximal"},
			{"LeftHandMiddle2", "Left Middle Intermediate"},
			{"LeftHandMiddle3", "Left Middle Distal"},
			{"LeftHandRing1", "Left Ring Proximal"},
			{"LeftHandRing2", "Left Ring Intermediate"},
			{"LeftHandRing3", "Left Ring Distal"},
			{"LeftHandPinky1", "Left Little Proximal"},
			{"LeftHandPinky2", "Left Little Intermediate"},
			{"LeftHandPinky3", "Left Little Distal"},
			{"RightHandThumb1", "Right Thumb Proximal"},
			{"RightHandThumb2", "Right Thumb Intermediate"},
			{"RightHandThumb3", "Right Thumb Distal"},
			{"RightHandIndex1", "Right Index Proximal"},
			{"RightHandIndex2", "Right Index Intermediate"},
			{"RightHandIndex3", "Right Index Distal"},
			{"RightHandMiddle1", "Right Middle Proximal"},
			{"RightHandMiddle2", "Right Middle Intermediate"},
			{"RightHandMiddle3", "Right Middle Distal"},
			{"RightHandRing1", "Right Ring Proximal"},
			{"RightHandRing2", "Right Ring Intermediate"},
			{"RightHandRing3", "Right Ring Distal"},
			{"RightHandPinky1", "Right Little Proximal"},
			{"RightHandPinky2", "Right Little Intermediate"},
			{"RightHandPinky3", "Right Little Distal"},
			{"Spine2", "UpperChest"}
		};

		public void MakeAvatarHumanoid(GameObject metaPersonAvatar)
		{
			SkinnedMeshRenderer meshRenderer = FindBodyMeshRenderer(metaPersonAvatar);
			if (meshRenderer == null)
			{
				Debug.LogError("Unable to find body mesh renderer.");
				return;
			}

			GameObject root = meshRenderer.transform.parent.gameObject;

			Animator animator = root.GetComponent<Animator>();
			if (animator == null)
				animator = root.AddComponent<Animator>();
			animator.avatar = AvatarBuilder.BuildHumanAvatar(root, BuildHumanDescription(meshRenderer, root));
		}

		public void SetAnimatorController(RuntimeAnimatorController animatorController, GameObject metaPersonAvatar)
		{
			Animator animator = metaPersonAvatar.GetComponentInChildren<Animator>();
			if (animator != null)
				animator.runtimeAnimatorController = animatorController;
			else
				Debug.LogError("Animator component was not found.");
		}

		private SkinnedMeshRenderer FindBodyMeshRenderer(GameObject metaPersonAvatar)
		{
			var meshRenderers = metaPersonAvatar.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			if (meshRenderers == null || meshRenderers.Length == 0)
				return null;

			SkinnedMeshRenderer bodyMeshRenderer = meshRenderers.FirstOrDefault(r => r.name == "mesh" || r.name == "AvatarBody");
			if (bodyMeshRenderer != null)
				return bodyMeshRenderer;

			Debug.LogWarning("Unable to find body mesh renderer with name \"mesh\". Return the first in a list");
			return meshRenderers[0];
		}

		private HumanDescription BuildHumanDescription(SkinnedMeshRenderer meshRenderer, GameObject rootBone)
		{
			HumanDescription description = new HumanDescription();
			description.armStretch = 0.05f;
			description.legStretch = 0.05f;
			description.upperArmTwist = 0.5f;
			description.lowerArmTwist = 0.5f;
			description.upperLegTwist = 0.5f;
			description.lowerLegTwist = 0.5f;
			description.feetSpacing = 0;

			List<HumanBone> humanBones = new List<HumanBone>();
			foreach(var boneNamePair in metaPersonToHumanoidBoneMap)
				humanBones.Add(new HumanBone() { boneName = boneNamePair.Key, humanName = boneNamePair.Value, limit = new HumanLimit() { useDefaultValues = true } });
			description.human = humanBones.ToArray();

			List<Transform> bones = meshRenderer.bones.ToList();
			Matrix4x4[] bindPoses = meshRenderer.sharedMesh.bindposes;
			List<SkeletonBone> skeletonBones = new List<SkeletonBone>();
			for (int i = 0; i < bones.Count; i++)
			{
				Matrix4x4 boneLocalPosition = bindPoses[i].inverse;
				int parentIdx = bones.FindIndex(b => b.name == bones[i].parent.name);
				if (parentIdx >= 0)
					boneLocalPosition = bindPoses[parentIdx] * boneLocalPosition;

				SkeletonBone bone = new SkeletonBone()
				{
					name = bones[i].name,
					position = boneLocalPosition.GetPosition(),
					rotation = boneLocalPosition.GetRotation(),
					scale = boneLocalPosition.GetScale()
				};

				skeletonBones.Add(bone);
			}
			
			// add root bone
			skeletonBones.Insert(0, new SkeletonBone()
			{
				name = rootBone.name,
				position = rootBone.transform.localPosition,
				rotation = rootBone.transform.localRotation,
				scale = rootBone.transform.localScale
			});

			description.skeleton = skeletonBones.ToArray();

			return description;
		}
	}
}
