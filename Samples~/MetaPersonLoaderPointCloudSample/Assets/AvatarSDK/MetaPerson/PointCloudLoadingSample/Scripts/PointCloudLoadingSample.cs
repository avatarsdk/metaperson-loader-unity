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
	public class PointCloudLoadingSample : MonoBehaviour
	{
		public GameObject controls;
		public Text progressText;

		public Toggle maleLod1Toggle;
		public Toggle maleLod2Toggle;
		public Toggle femaleLod1Toggle;
		public Toggle femaleLod2Toggle;

		public AvatarConfig maleLod1Config;
		public AvatarConfig maleLod2Config;
		public AvatarConfig femaleLod1Config;
		public AvatarConfig femaleLod2Config;

		public async void OnLoadAvatarButtonClick()
		{
			controls.gameObject.SetActive(false);

			AvatarConfig selectedConfig = GetSelectedConfig();

			string pointCloudModelPath = string.Empty;

#if UNITY_EDITOR
			string pointCloudsDirPath = FindPointCloudDirInAssets();
			pointCloudModelPath = Path.Combine(pointCloudsDirPath, selectedConfig.pointCloudDirInAssets);
#endif

			if (!Directory.Exists(pointCloudModelPath))
			{
				progressText.text = "Point cloud directory not found!";
				return;
			}

			DateTime startTime = DateTime.Now;
			int startFrame = Time.frameCount;

			MetaPersonInstantiator metaPersonInstantiator = new MetaPersonInstantiator();
			GameObject avatarModel = await metaPersonInstantiator.LoadModelFromPointCloud(selectedConfig.templateModelPrefab, pointCloudModelPath);
			metaPersonInstantiator.AddOutfit(avatarModel, selectedConfig.outfitPrefab);

			MoveByMouse moveByMouse = avatarModel.AddComponent<MoveByMouse>();
			moveByMouse.detectMovementsOverOtherGameObjects = true;

			Debug.LogFormat("Loading time: {0} sec, {1} frames", (DateTime.Now - startTime).TotalSeconds, Time.frameCount - startFrame);

		}

		private AvatarConfig GetSelectedConfig()
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

#if UNITY_EDITOR
		private string FindPointCloudDirInAssets()
		{
			string pointCloudsDirName = "MetaPersonPointClouds";

			string[] dirs = Directory.GetDirectories(Application.dataPath, pointCloudsDirName, SearchOption.AllDirectories);
			if (dirs.Length == 0)
				return string.Empty;

			return dirs[0];
		}
#endif
	}
}
