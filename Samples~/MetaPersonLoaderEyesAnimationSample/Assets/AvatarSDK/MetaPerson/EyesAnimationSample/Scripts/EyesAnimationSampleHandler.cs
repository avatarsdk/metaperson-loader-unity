/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2024
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.EyesAnimationSample
{
	public class EyesAnimationSampleHandler : MonoBehaviour
	{
		private enum AnimationDirection
		{
			LookUp,
			LookDown,
			LookLeft,
			LookRight
		}

		public EyesAnimator eyesAnimator = null;

		private bool isPlaying = true;

		private float animationLengthInSec = 1.0f;

		private DateTime startTime;

		private AnimationDirection animationDirection;

		private void Update()
		{
			if (isPlaying)
			{
				float duration = (float)(DateTime.Now - startTime).TotalSeconds;
				if (duration >= animationLengthInSec)
				{
					eyesAnimator.SetLookUpWeight(0.0f);
					eyesAnimator.SetLookRightWeight(0.0f);
					isPlaying = false;
					return;
				}

				float halfDration = animationLengthInSec / 2.0f;
				float weight = 1.0f - Mathf.Abs(duration / halfDration - 1.0f);;
				switch (animationDirection)
				{
					case AnimationDirection.LookUp:
						eyesAnimator.SetLookUpWeight(weight);
						break;

					case AnimationDirection.LookDown:
						eyesAnimator.SetLookDownWeight(weight);
						break;

					case AnimationDirection.LookLeft:
						eyesAnimator.SetLookLeftWeight(weight);
						break;

					case AnimationDirection.LookRight:
						eyesAnimator.SetLookRightWeight(weight);
						break;
				}
			}
		}

		public void PlayLookUpAnimation()
		{
			PlayAnimation(AnimationDirection.LookUp);
		}

		public void PlayLookDownAnimation()
		{
			PlayAnimation(AnimationDirection.LookDown);
		}

		public void PlayLookLeftAnimation()
		{
			PlayAnimation(AnimationDirection.LookLeft);
		}

		public void PlayLookRightAnimation()
		{
			PlayAnimation(AnimationDirection.LookRight);
		}

		private void PlayAnimation(AnimationDirection direction)
		{
			if (!isPlaying)
			{
				isPlaying = true;
				startTime = DateTime.Now;
				animationDirection = direction;
			}
		}
	}
}
