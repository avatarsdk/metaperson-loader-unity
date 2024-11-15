/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2024
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public class MetaPersonInstantiator
	{
		public GameObject InstantiateModel(GameObject avatarModel, bool makeActive = true)
		{
			GameObject instantiatedModel = UnityEngine.Object.Instantiate(avatarModel);
			instantiatedModel.SetActive(makeActive);
			return instantiatedModel;
		}

		public async Task<GameObject> LoadModelFromPointCloud(GameObject srcAvatarModel, string pointCloudModelPath, bool makeActive = true)
		{
			GameObject dstAvatarModel = InstantiateModel(srcAvatarModel, false);
			var meshRenderersMap = FindAllMeshRenderers(dstAvatarModel);
			var meshTextureMap = PlyAvatarFiles.FindAvailableMeshesAndTextures(pointCloudModelPath);
			foreach(var pair in meshRenderersMap)
			{
				AvatarPart avatarPart = pair.Key;
				MeshTexturePair meshTexturePair = meshTextureMap[avatarPart];
				if (meshTexturePair != null)
				{
					if (!string.IsNullOrEmpty(meshTexturePair.meshFilePath) && File.Exists(meshTexturePair.meshFilePath))
						await ReplaceVertices(pair.Value, meshTexturePair.meshFilePath);

					if (!string.IsNullOrEmpty(meshTexturePair.textureFilePath) && File.Exists(meshTexturePair.meshFilePath))
						await ReplaceTexture(pair.Value, meshTexturePair.textureFilePath);
				}
			}

			if (meshRenderersMap.ContainsKey(AvatarPart.Body))
				RecolorBodyTexture(meshRenderersMap[AvatarPart.Body], pointCloudModelPath);

			dstAvatarModel.SetActive(makeActive);
			return dstAvatarModel;
		}

		public void AddOutfit(GameObject avatarModel, GameObject outfitPrefab, bool makeActive = true)
		{
			GameObject outfit = UnityEngine.Object.Instantiate(outfitPrefab);
			MetaPersonUtils.MergeModels(outfit, avatarModel);
			avatarModel.SetActive(makeActive);
		}

		private async Task ReplaceVertices(SkinnedMeshRenderer meshRenderer, string plyModelPath)
		{
			Vector3[] vertices = await Task.Run(() => PlyReader.ReadVerticesFromPly(plyModelPath));

			Mesh mesh = meshRenderer.sharedMesh;
			if (mesh.vertexCount != vertices.Length)
			{
				Debug.LogErrorFormat("Unexpected number of vertices: {0} vs {1}", mesh.vertexCount, vertices.Length);
				return;
			}

			Mesh updatedMesh = UnityEngine.Object.Instantiate(mesh);
			updatedMesh.vertices = vertices;
			await AddBlendshapes(updatedMesh, meshRenderer.name, Path.GetDirectoryName(plyModelPath));
			updatedMesh.RecalculateNormals();
			updatedMesh.UploadMeshData(true);
			meshRenderer.sharedMesh = updatedMesh;
		}

		private async Task ReplaceTexture(SkinnedMeshRenderer meshRenderer, string texturePath)
		{
			Texture2D texture = await ImageUtils.LoadTexture(texturePath, true, true);
			if (texture != null)
			{
				Material material = meshRenderer.material;
				material.mainTexture = texture;
			}
		}

		private void RecolorBodyTexture(SkinnedMeshRenderer bodyMeshRenderer, string avatarDirPath)
		{
			string modelInfoFilePath = Path.Combine(avatarDirPath, "model.json");
			if (!File.Exists(modelInfoFilePath))
			{
				Debug.LogErrorFormat("Unable to recolor body texture. Model info file not found: {0}", modelInfoFilePath);
				return;
			}

			ModelInfo modelInfo = JsonUtility.FromJson<ModelInfo>(File.ReadAllText(modelInfoFilePath));
			if (modelInfo == null || modelInfo.skin_color.IsNullOrNotSpecified())
			{
				Debug.LogError("Unable to recolor body texture. There is no skin color info");
				return;
			}

			BodyTextureRecolorer.RecolorBodyTexture(bodyMeshRenderer, modelInfo.skin_color);
		}

		private Dictionary<AvatarPart, SkinnedMeshRenderer> FindAllMeshRenderers(GameObject model)
		{
			Dictionary<AvatarPart, SkinnedMeshRenderer> meshRenderersDict = new Dictionary<AvatarPart, SkinnedMeshRenderer>();

			var meshRenderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (var meshRenderer in meshRenderers)
			{
				AvatarPart avatarPart = AvatarStructureUtils.GetAvatarPartByName(meshRenderer.name);
				if (avatarPart != AvatarPart.Unknown)
					meshRenderersDict.Add(avatarPart, meshRenderer);
			}
			return meshRenderersDict;
		}

		private async Task AddBlendshapes(Mesh mesh, string meshName, string avatarDir)
		{
			mesh.ClearBlendShapes();

			var blendshapesDirs = Path.Combine(avatarDir, "blendshapes", meshName);
			if (!Directory.Exists(blendshapesDirs))
				return;

			Dictionary<string, Vector3[]> blendshapesDict = await Task.Run(() => 
			{
				var blendshapes = new Dictionary<string, Vector3[]>();
				string[] blendshapeFiles = Directory.GetFiles(blendshapesDirs, "*.bin", SearchOption.AllDirectories);

				var blendshapeReader = new BlendshapeReader();

				for (int i = 0; i < blendshapeFiles.Length; ++i)
				{
					var blendshapePath = blendshapeFiles[i];
					var filename = Path.GetFileName(blendshapePath);

					// crude parsing of filenames
					if (!filename.EndsWith(".bin"))
						continue;
					var tokens = filename.Split(new[] { ".bin" }, StringSplitOptions.None);
					if (tokens.Length != 2)
						continue;

					var blendshapeName = tokens[0];
					blendshapes[blendshapeName] = blendshapeReader.ReadVerticesDeltas(blendshapePath);
				}

				return blendshapes;
			});

			var addBlendshapesTimer = DateTime.Now;
			float targetFps = 30.0f;

			int numBlendshapes = 0, loadedSinceLastPause = 0;
			foreach (var blendshape in blendshapesDict)
			{
				mesh.AddBlendShapeFrame(blendshape.Key, 100.0f, blendshape.Value, null, null);
				++numBlendshapes;
				++loadedSinceLastPause;

				if ((DateTime.Now - addBlendshapesTimer).TotalMilliseconds > 1000.0f / targetFps && loadedSinceLastPause >= 5)
				{
					// Debug.LogFormat ("Pause after {0} blendshapes to avoid blocking the main thread", numBlendshapes);
					await Task.Yield();
					addBlendshapesTimer = DateTime.Now;
					loadedSinceLastPause = 0;
				}
			}
		}
	}
}
