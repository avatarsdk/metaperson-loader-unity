/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2023
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSDK.MetaPerson.Sample
{
	public class MetaPersonSample : MonoBehaviour
	{
		public string modelUrl = string.Empty;
		public Button loadAvatarButton;
		public Text progressText;
		public MetaPersonLoader metaPersonLoader;

		public void OnLoadAvatarButtonClick()
		{
			if (!string.IsNullOrEmpty(modelUrl))
				LoadAvatar();
			else
				progressText.text = "Model URL isn't provided.";
		}

		public void SetModelUrl(string modelUrl)
		{
			this.modelUrl = modelUrl;
		}

		private async void LoadAvatar()
		{
			progressText.text = string.Empty;
			loadAvatarButton.interactable = false;
			bool isModelLoaded = await metaPersonLoader.LoadModelAsync(modelUrl, p => progressText.text = string.Format("Downloading avatar: {0}%", (int)(p * 100)));
			if (isModelLoaded)
			{
				loadAvatarButton.gameObject.SetActive(false);
				progressText.gameObject.SetActive(false);
			}
			else
				progressText.text = "Unable to load the model";
			loadAvatarButton.interactable = true;
		}
	}
}
