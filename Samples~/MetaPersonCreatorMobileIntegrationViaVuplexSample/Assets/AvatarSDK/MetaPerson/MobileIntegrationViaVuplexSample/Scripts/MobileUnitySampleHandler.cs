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
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

namespace AvatarSDK.MetaPerson.MobileIntegrationViaVuplexSample
{
	public class MobileUnitySampleHandler : MonoBehaviour
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
			{
				progressText.text = "Account credentials are not provided!";
				getAvatarButton.interactable = false;
			}
		}

		public async void OnGetAvatarButtonClick()
		{
			webViewPlaceholder.SetActive(true);

			if (!isWebViewInitialized)
			{
				canvasWebViewPrefab.LogConsoleMessages = true;

				await canvasWebViewPrefab.WaitUntilInitialized();

				await canvasWebViewPrefab.WebView.WaitForNextPageLoadToFinish();

				ConfigureJSApi();

				isWebViewInitialized = true;
			}
		}

		public void OnCloseButtonClick()
		{
			webViewPlaceholder.SetActive(false);
		}


		private void ConfigureJSApi()
		{
			string javaScriptCode = @"
					{
						function sendConfigurationParams() {
							console.log('sendConfigurationParams');

							const CLIENT_ID = '" + credentials.clientId + @"';
							const CLIENT_SECRET = '" + credentials.clientSecret + @"';

							let authenticationMessage = {
								'eventName': 'authenticate',
								'clientId': CLIENT_ID,
								'clientSecret': CLIENT_SECRET
							};
							window.postMessage(authenticationMessage, '*');

							let exportParametersMessage = {
								'eventName': 'set_export_parameters',
								'format': 'glb',
								'lod': 2,
								'textureProfile': '1K.jpg'
							};
							window.postMessage(exportParametersMessage, '*');

							let uiParametersMessage = {
								'eventName': 'set_ui_parameters',
								'isExportButtonVisible' : true,
								'isLoginButtonVisible': true
							};
							window.postMessage(uiParametersMessage, '*');
						}

						function onWindowMessage(evt) {
							console.log('onWindowMessage: ' + evt.data);
							if (evt.type === 'message') {
								if (evt.data?.source === 'metaperson_creator') {
									let data = evt.data;
									let evtName = data?.eventName;
									if (evtName === 'unity_loaded' ||
										evtName === 'mobile_loaded') {
										sendConfigurationParams();
									} else if (evtName === 'model_exported') {
										console.log('model url: ' + data.url);
										console.log('gender: ' + data.gender);
										console.log('avatar code: ' + data.avatarCode);
										window.vuplex.postMessage(evt.data);
									}
								}
							}
						}
						window.addEventListener('message', onWindowMessage);

						if (window.metaPersonCreator && window.metaPersonCreator.isLoaded)
							sendConfigurationParams();
					}
				";

			canvasWebViewPrefab.WebView.ExecuteJavaScript(javaScriptCode, OnJavaScriptExecuted);
			canvasWebViewPrefab.WebView.MessageEmitted += OnWebViewMessageReceived;
		}

		private void OnJavaScriptExecuted(string executionResult)
		{
			Debug.LogFormat("JS execution result: {0}", executionResult);
		}

		private async void OnWebViewMessageReceived(object sender, EventArgs<string> args)
		{
			Debug.LogFormat("Got WebView message: {0}", args.Value);

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
}
