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

		public bool cacheModels = false;

		public bool configureAnimator = true;

		public RuntimeAnimatorController animatorController = null;

		public async void LoadModel(string uri, Action<float> downloadProgressCallback = null)
		{
			await LoadModelAsync(uri, downloadProgressCallback);
		}

		public async Task<bool> LoadModelAsync(string uri, Action<float> downloadProgressCallback = null)
		{
			try
			{
				Uri modelUri = new Uri(uri);
				string modelLocalFilePath = string.Empty;

				if (!modelUri.IsFile)
				{
					if (cacheModels)
						modelLocalFilePath = MetaPersonCache.GetModelFilePathByUri(modelUri);

					if (!File.Exists(modelLocalFilePath))
					{
						modelLocalFilePath = await DownloadAndSaveModelAsync(modelUri.AbsoluteUri, downloadProgressCallback);
						if (string.IsNullOrEmpty(modelLocalFilePath))
							return false;
					}
				}
				else
				{
					modelLocalFilePath = modelUri.LocalPath;
				}

				if (!File.Exists(modelLocalFilePath))
				{
					Debug.LogErrorFormat("File doesn't exist: {0}", modelLocalFilePath);
					return false;
				}
				byte[] modelBytes = File.ReadAllBytes(modelLocalFilePath);

				ImportSettings importSettings = new ImportSettings();
				importSettings.GenerateMipMaps = true;
				importSettings.AnisotropicFilterLevel = 1;
				importSettings.DefaultMagFilterMode = GLTFast.Schema.Sampler.MagFilterMode.Linear;
				importSettings.DefaultMinFilterMode = GLTFast.Schema.Sampler.MinFilterMode.Linear;

				downloadProgressCallback?.Invoke(1.0f);

				var gltfImporter = new GltfImport(materialGenerator: materialGenerator);
				var success = await gltfImporter.Load(modelBytes, uri: new Uri(modelLocalFilePath), importSettings: importSettings);
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

				if (success)
				{
					if (configureAnimator)
					{
						HumanoidAnimatorBuilder humanoidAnimatorBuilder = new HumanoidAnimatorBuilder();
						humanoidAnimatorBuilder.AddHumanoidAnimator(avatarObject);
						if (animatorController != null)
							humanoidAnimatorBuilder.SetAnimatorController(animatorController, avatarObject);
					}
				}

				if (!cacheModels && !modelUri.IsFile)
				{
					string modelDirPath =  MetaPersonCache.GetModelDirByUri(modelUri);
					Directory.Delete(modelDirPath, true);
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

		private async Task<string> DownloadAndSaveModelAsync(string uri, Action<float> progressCallback = null)
		{
			UnityWebRequest request = UnityWebRequest.Get(uri);
			UnityWebRequestAsyncOperation requestAsyncOperation = request.SendWebRequest();
			while (!requestAsyncOperation.isDone)
			{
				progressCallback?.Invoke(request.downloadProgress);
				await Task.Yield();
			}

			if (request.result == UnityWebRequest.Result.Success)
			{
				byte[] downloadedModelBytes = request.downloadHandler.data;
				if (uri.ToLower().EndsWith("zip"))
				{
					string modelDirPath = MetaPersonCache.GetModelDirByUri(new Uri(uri));
					ExtractArchive(downloadedModelBytes, modelDirPath);
					return MetaPersonCache.GetModelFilePathByUri(new Uri(uri));
				}
				else
				{
					string modelFilePath = MetaPersonCache.GetModelFilePathByUri(new Uri(uri));
					string modelDirPath = Path.GetDirectoryName(modelFilePath);
					if (!Directory.Exists(modelDirPath))
						Directory.CreateDirectory(modelDirPath);
					File.WriteAllBytes(modelFilePath, downloadedModelBytes);
					return modelFilePath;
				}
			}
			else
			{
				Debug.LogErrorFormat("Unable to download model: {0}, {1}", request.responseCode, request.error);
				return null;
			}
		}

		private void ExtractArchive(byte[] zipBytes, string outputDir)
		{
			if (!Directory.Exists(outputDir))
				Directory.CreateDirectory(outputDir);

			Dictionary<string, byte[]> extractedFiles = ZipUtils.Unzip(zipBytes);
			foreach(var pair in extractedFiles)
			{
				string filePath = Path.Combine(outputDir, pair.Key);
				File.WriteAllBytes(filePath, pair.Value);
			}
		}
	}
}
