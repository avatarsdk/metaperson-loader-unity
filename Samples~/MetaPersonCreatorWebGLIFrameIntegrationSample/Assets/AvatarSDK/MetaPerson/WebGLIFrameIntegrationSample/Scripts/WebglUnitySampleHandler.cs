/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, October 2023
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSDK.MetaPerson.WebGLIFrameIntegrationSample
{
	public class WebglUnitySampleHandler : MonoBehaviour
	{
		public AccountCredentials credentials;

		public MetaPersonLoader metaPersonLoader;

		public GameObject importControls;

		public Button getAvatarButton;

		public Text progressText;

		[DllImport("__Internal")]
		private static extern void showMetaPersonCreator(string clientId, string clientSecret, string modelUrlReceiverObjectName, string modelUrlReceiverMethodName);

		private void Start()
		{
			if (credentials.IsEmpty())
			{
				progressText.text = "Account credentials are not provided!";
				getAvatarButton.interactable = false;
			}
		}

		public void OnGetAvatarButtonClick()
		{
			showMetaPersonCreator(credentials.clientId, credentials.clientSecret, "SceneHandler", "HandleModelExportData");
		}

		public async void HandleModelExportData(string json)
		{
			try
			{
				ModelExportedEvent modelExportedEvent = JsonUtility.FromJson<ModelExportedEvent>(json);
				getAvatarButton.interactable = false;
				bool isLoaded = await metaPersonLoader.LoadModelAsync(modelExportedEvent.url, p => progressText.text = string.Format("Downloading avatar: {0}%", (int)(p * 100)));

				if (isLoaded)
				{
					progressText.text = string.Empty;
					importControls.SetActive(false);
				}
				else
				{
					getAvatarButton.interactable = true;
					progressText.text = "Unable to load the model";
					importControls.SetActive(true);
				}
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to parse message: {0}. Exception: {1}", json, exc);
				progressText.text = "Unable to load the model";
				getAvatarButton.interactable = true;
				importControls.SetActive(true);
			}
		}
	}
}