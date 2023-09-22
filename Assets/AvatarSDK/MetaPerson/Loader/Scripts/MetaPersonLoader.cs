/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2023
*/

using GLTFast;
using GLTFast.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AvatarSDK.MetaPerson.Loader
{
	public class MetaPersonLoader : MonoBehaviour
	{
		public string modelUri = string.Empty;

		public GameObject avatarObject = null;

		public MetaPersonMaterialGenerator materialGenerator = null;

		public async void LoadModel(string uri, Action<float> downloadProgressCallback = null)
		{
			await LoadModelAsync(uri, downloadProgressCallback);
		}

		public async Task<bool> LoadModelAsync(string uri, Action<float> downloadProgressCallback = null)
		{
			try
			{
				byte[] modelBytes = null;
				Uri modelUri = new Uri(uri);
				if (modelUri.IsFile)
					modelBytes = File.ReadAllBytes(modelUri.AbsolutePath);
				else
					modelBytes = await DownloadFileAsync(modelUri.AbsoluteUri, downloadProgressCallback);
			

				if (uri.EndsWith("zip"))
					modelBytes = Unzip(modelBytes);

				ImportSettings importSettings = new ImportSettings();
				importSettings.GenerateMipMaps = true;
				importSettings.AnisotropicFilterLevel = 1;
				importSettings.DefaultMagFilterMode = GLTFast.Schema.Sampler.MagFilterMode.Linear;
				importSettings.DefaultMinFilterMode = GLTFast.Schema.Sampler.MinFilterMode.Linear;

				var gltfImporter = new GltfImport(materialGenerator: materialGenerator);
				var success = await gltfImporter.Load(modelBytes, importSettings: importSettings);
				if (materialGenerator != null)
					materialGenerator.DestroyUnusedTextures();

				foreach (Mesh mesh in gltfImporter.GetMeshes())
					mesh.Optimize();

				if (success)
				{
					if (avatarObject == null)
						avatarObject = new GameObject("MetaPerson");
					success = await gltfImporter.InstantiateMainSceneAsync(avatarObject.transform);
				}
				else
				{
					Debug.LogError("Loading glTF failed!");
				}

				return success;
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Exception during avatar loading: {0}", exc.Message);
				return false;
			}
		}

		private void Start()
		{
			if (!string.IsNullOrEmpty(modelUri))
				LoadModel(modelUri);
		}

		private async Task<byte[]> DownloadFileAsync(string url, Action<float> progressCallback = null)
		{
			UnityWebRequest request = UnityWebRequest.Get(url);
			UnityWebRequestAsyncOperation requestAsyncOperation = request.SendWebRequest();
			while (!requestAsyncOperation.isDone)
			{
				progressCallback?.Invoke(request.downloadProgress);
				await Task.Yield();
			}

			if (request.result == UnityWebRequest.Result.Success)
			{
				return request.downloadHandler.data;
			}
			else
			{
				Debug.LogErrorFormat("Unable to download model: {0}, {1}", request.responseCode, request.error);
				return null;
			}
		}

		private byte[] Unzip(byte[] zipBytes)
		{
			Dictionary<string, byte[]> extractedFiles = ZipUtils.Unzip(zipBytes);
			foreach(var pair in extractedFiles)
			{
				if (pair.Key.EndsWith("glb"))
					return pair.Value;
			}
			return null;
		}
	}
}
