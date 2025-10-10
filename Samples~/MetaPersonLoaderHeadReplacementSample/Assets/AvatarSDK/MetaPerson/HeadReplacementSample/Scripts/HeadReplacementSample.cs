/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2024
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSDK.MetaPerson.Sample
{
	public class HeadReplacementSample : MonoBehaviour
	{
		public GameObject controls;
		public Text progressText;

		public Toggle maleLod1Toggle;
		public Toggle maleLod2Toggle;
		public Toggle femaleLod1Toggle;
		public Toggle femaleLod2Toggle;

		public HeadReplacementAvatarConfig maleLod1Config;
		public HeadReplacementAvatarConfig maleLod2Config;
		public HeadReplacementAvatarConfig femaleLod1Config;
		public HeadReplacementAvatarConfig femaleLod2Config;

		public async void OnLoadAvatarButtonClick()
		{
			controls.gameObject.SetActive(false);

			HeadReplacementAvatarConfig selectedConfig = GetSelectedConfig();

			string headModelPath = selectedConfig.GetHeadModelPath();
			string modelJsonPath = selectedConfig.GetModelJsonPath();

			if (string.IsNullOrEmpty(headModelPath))
			{
				progressText.text = "Head model path isn't specified!";
				return;
			}

			MetaPersonInstantiator metaPersonInstantiator = new MetaPersonInstantiator();
			GameObject avatarModel = await metaPersonInstantiator.LoadModelWithHeadReplacement(selectedConfig.templateModelPrefab, headModelPath, modelJsonPath);
			if (avatarModel == null)
			{
				progressText.text = "Unable to load model!";
				return;
			}

			MoveByMouse moveByMouse = avatarModel.AddComponent<MoveByMouse>();
			moveByMouse.detectMovementsOverOtherGameObjects = true;

		}

		private HeadReplacementAvatarConfig GetSelectedConfig()
		{
			if (maleLod1Toggle.isOn)
				return maleLod1Config;
			else if (maleLod2Toggle.isOn)
				return maleLod2Config;
			else if (femaleLod1Toggle.isOn)
				return femaleLod1Config;
			else if (femaleLod2Toggle.isOn)
				return femaleLod2Config;
			else
				return null;
		}
	}
}
