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
using UnityEngine;
using UnityEngine.UI;

#if UNITY_WEBGL && !UNITY_EDITOR
using Vuplex.WebView;
using static System.Net.WebRequestMethods;
#endif

namespace AvatarSDK.MetaPerson.WebGLWebViewIntegrationSample
{
#if UNITY_WEBGL && !UNITY_EDITOR
	public class WebglUnitySampleHandler : MonoBehaviour
	{

		public AccountCredentials credentials;

		public MetaPersonLoader metaPersonLoader;

		public GameObject webViewPlaceholder;

		public GameObject importControls;

		public Button getAvatarButton;

		public Text progressText;

		private CanvasWebViewPrefab canvasWebViewPrefab;

		private bool isWebViewInitialized = false;

		private void Start()
		{
			canvasWebViewPrefab = webViewPlaceholder.GetComponentInChildren<CanvasWebViewPrefab>();

			if (credentials.IsEmpty())
				progressText.text = "Account credentials are not provided!";
		}

		public async void OnGetAvatarButtonClick()
		{
			webViewPlaceholder.SetActive(true);


			if (!isWebViewInitialized)
			{
				string url = "https://metaperson.avatarsdk.com/iframe_vuplex.html";

				canvasWebViewPrefab.LogConsoleMessages = true;

				await canvasWebViewPrefab.WaitUntilInitialized();
				canvasWebViewPrefab.WebView.LoadUrl(url);
				await canvasWebViewPrefab.WebView.WaitForNextPageLoadToFinish();

				canvasWebViewPrefab.WebView.MessageEmitted += OnWebViewMessageReceived;
				isWebViewInitialized = true;
			}
		}

		public void OnCloseButtonClick()
		{
			webViewPlaceholder.SetActive(false);
		}

		private string GetAuthenticateMessage()
		{
			return string.Format("{{ \"eventName\" : \"authenticate\", \"clientId\" : \"{0}\", \"clientSecret\" : \"{1}\" }}"
						, credentials.clientId, credentials.clientSecret);
		}
		private string GetExportParametersMessage()
		{
			string format = "glb";
			string lod = "1";
			string textureProfile = "1K.jpg";
			return string.Format("{{ \"eventName\" : \"set_export_parameters\", \"format\" : \"{0}\", \"lod\" : \"{1}\", \"textureProfile\" : \"{2}\" }}"
						, format, lod, textureProfile);
		}
		private string GetUiParametersMessage()
		{
			string isExportButtonVisible = "true";
			string closeExportDialogWhenExportComlpeted = "true";
			return string.Format("{{ \"eventName\" : \"set_ui_parameters\", \"isExportButtonVisible\" : \"{0}\", \"closeExportDialogWhenExportComlpeted\" : \"{1}\" }}"
						, isExportButtonVisible, closeExportDialogWhenExportComlpeted);
		}
		private async void OnWebViewMessageReceived(object sender, EventArgs<string> args)
		{
			Debug.LogFormat("Got WebView message: {0}", args.Value);

			UnityLoadedEvent simpleEvent = JsonUtility.FromJson<UnityLoadedEvent>(args.Value);
			{
				if (simpleEvent.eventName == "unity_loaded")
				{
					string authMessage = GetAuthenticateMessage();
					Debug.Log("Auth: " + authMessage);
					canvasWebViewPrefab.WebView.PostMessage(authMessage);
					string exportParamsMessage = GetExportParametersMessage();
					Debug.Log("Export: " + exportParamsMessage);
					canvasWebViewPrefab.WebView.PostMessage(exportParamsMessage);
					string uiParamsMessage = GetUiParametersMessage();
					Debug.Log("Ui: " + uiParamsMessage);
					canvasWebViewPrefab.WebView.PostMessage(uiParamsMessage);
					return;
				}
			}

			try
			{
				ModelExportedEvent modelExportedEvent = JsonUtility.FromJson<ModelExportedEvent>(args.Value);
				if (modelExportedEvent.eventName == "model_exported" && modelExportedEvent.source == "metaperson_creator")
				{
					Debug.LogFormat("Model exported: {0}", modelExportedEvent.url);
					webViewPlaceholder.SetActive(false);
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
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to parse message: {0}. Exception: {1}", args.Value, exc);
				progressText.text = "Unable to load the model";
				getAvatarButton.interactable = true;
				importControls.SetActive(true);
			}
		}
	}
#else

	public class WebglUnitySampleHandler : MonoBehaviour
	{
		public AccountCredentials credentials;

		public MetaPersonLoader metaPersonLoader;

		public GameObject webViewPlaceholder;

		public GameObject importControls;

		public Button getAvatarButton;

		public Text progressText;

		private void Start()
		{
			progressText.text = "This sample requires Vuplex WebView for WebGL: https://store.vuplex.com/webview/webgl";
		}

		public void OnGetAvatarButtonClick()
		{
			Application.OpenURL("https://store.vuplex.com/webview/webgl");
		}
	}
#endif

}