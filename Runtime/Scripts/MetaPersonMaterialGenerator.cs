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
using GLTFast.Logging;
using GLTFast.Materials;
using GLTFast.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public class MetaPersonMaterialGenerator : MonoBehaviour, IMaterialGenerator
	{
		public UnityEngine.Material defaultMaterial;

		public UnityEngine.Material eyelashesMaterial;

		public UnityEngine.Material haircutMaterial;

		public UnityEngine.Material glassesMaterial;

		private List<Texture2D> texturesToDestroy = new List<Texture2D>();

		private static bool? isDXT5Encoding = null;

        #region IMaterialGenerator
        public UnityEngine.Material GenerateMaterial(MaterialBase gltfMaterial, IGltfReadable gltf, bool pointsSupport = false)
        {
            return GenerateMaterial((GLTFast.Schema.Material)gltfMaterial, gltf, pointsSupport);
        }

        public UnityEngine.Material GenerateMaterial(GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf, bool pointsSupport = false)
		{
			if (gltfMaterial.name == "AvatarEyelashes")
				return GenerateMaterial(eyelashesMaterial, gltfMaterial, gltf, false);
			if (gltfMaterial.name == "haircut")
				return GenerateMaterial(haircutMaterial, gltfMaterial, gltf, false);
			if (gltfMaterial.name == "glasses")
				return GenerateMaterial(glassesMaterial, gltfMaterial, gltf);
			return GenerateMaterial(defaultMaterial, gltfMaterial, gltf);
		}

		public UnityEngine.Material GetDefaultMaterial(bool pointsSupport = false)
		{
			return Instantiate(defaultMaterial);
		}

		public void SetLogger(ICodeLogger logger)
		{
			
		}
		#endregion

		public void DestroyUnusedTextures()
		{
			foreach (Texture2D texture in texturesToDestroy)
				Destroy(texture);
		}

		private UnityEngine.Material GenerateMaterial(UnityEngine.Material templateMaterial, GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf, bool useMetallicRoughness = true)
		{
			UnityEngine.Material material = Instantiate(templateMaterial);
			if (gltfMaterial.pbrMetallicRoughness.baseColorTexture.index >= 0)
			{
				Texture2D colorTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.baseColorTexture.index);
				material.mainTexture = CompressTextureIfPossible(colorTexture);
			}

			if (gltfMaterial.occlusionTexture.index >= 0)
			{
				Texture2D occlusionMapTexture = gltf.GetTexture(gltfMaterial.occlusionTexture.index);
				material.SetTexture("_OcclusionMap", CompressTextureIfPossible(occlusionMapTexture));
			}

			if (gltfMaterial.normalTexture.index >= 0)
			{
				Texture2D normalMapTexture = gltf.GetTexture(gltfMaterial.normalTexture.index);
				if (IsDXT5EncodingUsedForNormalMap())
				{
					Texture2D normalMapXYZTexture = normalMapTexture;
					normalMapTexture = ConvertXYZtoDXT5NormalMapEncoding(normalMapXYZTexture);
					if (!texturesToDestroy.Contains(normalMapXYZTexture))
						texturesToDestroy.Add(normalMapXYZTexture);
				}
				material.SetTexture("_BumpMap", CompressTextureIfPossible(normalMapTexture));
			}

			if (gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index >= 0)
			{
				Texture2D metallicRoughnessTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index);
				if (useMetallicRoughness)
				{
					Texture2D unityMetallicSmoothnessTexture = ConvertGltfMetallicRoughnessToUnityMetallicSmoothness(metallicRoughnessTexture);
					material.SetTexture("_MetallicGlossMap", CompressTextureIfPossible(unityMetallicSmoothnessTexture));
				}
				if (!texturesToDestroy.Contains(metallicRoughnessTexture))
					texturesToDestroy.Add(metallicRoughnessTexture);
			}

			return material;
		}

		private Texture2D ConvertGltfMetallicRoughnessToUnityMetallicSmoothness(Texture2D gltfMetallicRougnessTexture)
		{
			RenderTexture dstRenderTexture = RenderTexture.GetTemporary(gltfMetallicRougnessTexture.width, gltfMetallicRougnessTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

			Graphics.Blit(gltfMetallicRougnessTexture, dstRenderTexture);

			var unityMetallicSmoothnessTexture = new Texture2D(gltfMetallicRougnessTexture.width, gltfMetallicRougnessTexture.height);
			unityMetallicSmoothnessTexture.ReadPixels(new Rect(0, 0, dstRenderTexture.width, dstRenderTexture.height), 0, 0);
			unityMetallicSmoothnessTexture.Apply();

			Color32[] pixels = unityMetallicSmoothnessTexture.GetPixels32();
			for(int i=0; i < pixels.Length; i++)
			{
				pixels[i].a = (byte)(255 - pixels[i].g);
				pixels[i].r = pixels[i].b;
				pixels[i].g = pixels[i].b;
			}
			unityMetallicSmoothnessTexture.SetPixels32(pixels);
			unityMetallicSmoothnessTexture.Apply(true, true);

			RenderTexture.ReleaseTemporary(dstRenderTexture);

			return unityMetallicSmoothnessTexture;
		}

		private Texture2D ConvertXYZtoDXT5NormalMapEncoding(Texture2D normalMapDXT5Texture)
		{
			RenderTexture dstRenderTexture = RenderTexture.GetTemporary(normalMapDXT5Texture.width, normalMapDXT5Texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

			Graphics.Blit(normalMapDXT5Texture, dstRenderTexture);

			var normalMapXYZTexture = new Texture2D(normalMapDXT5Texture.width, normalMapDXT5Texture.height, TextureFormat.RGBA32, true, true);
			normalMapXYZTexture.ReadPixels(new Rect(0, 0, dstRenderTexture.width, dstRenderTexture.height), 0, 0);
			normalMapXYZTexture.Apply();

			Color32[] pixels = normalMapXYZTexture.GetPixels32();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = pixels[i].r;
				pixels[i].r = 255;
				pixels[i].b = pixels[i].g;
			}
			normalMapXYZTexture.SetPixels32(pixels);
			normalMapXYZTexture.Apply(true, true);

			RenderTexture.ReleaseTemporary(dstRenderTexture);

			return normalMapXYZTexture;
		}

		private bool IsDXT5EncodingUsedForNormalMap()
		{
			if (isDXT5Encoding != null)
				return isDXT5Encoding.Value;

			Texture2D sampleTexture = Resources.Load<Texture2D>("textures/avatar_sdk_sample_normal_map");
			if (sampleTexture != null)
			{
				Color32[] pixels = sampleTexture.GetPixels32();
				isDXT5Encoding = pixels[0].r == 255;
			}
			else
			{
				Debug.LogError("Unable to find sample normal map texture -> normal map encoding isn't detected");
				isDXT5Encoding = false;
			}
			return isDXT5Encoding.Value;
		}

		private Texture2D CompressTextureIfPossible(Texture2D texture)
		{
			if (IsPowOfTwo(texture.width) && IsPowOfTwo(texture.height))
			{
				if (texture.isReadable)
				{
					texture.Compress(true);
					return texture;
				}
				else
				{
					Texture2D resultTexture = CopyTexture(texture, true, true);
					if (!texturesToDestroy.Contains(texture))
						texturesToDestroy.Add(texture);
					return resultTexture;
				}
			}
			return texture;
		}

		private Texture2D CopyTexture(Texture2D srcTexture, bool compress, bool makeNonRedable)
		{
			RenderTexture dstRenderTexture = RenderTexture.GetTemporary(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

			Graphics.Blit(srcTexture, dstRenderTexture);

			var dstTexture = new Texture2D(srcTexture.width, srcTexture.height);
			dstTexture.ReadPixels(new Rect(0, 0, dstRenderTexture.width, dstRenderTexture.height), 0, 0);
			dstTexture.Apply();
			if (compress)
				dstTexture.Compress(true);
			if (makeNonRedable)
				dstTexture.Apply(true, true);

			RenderTexture.ReleaseTemporary(dstRenderTexture);

			return dstTexture;
		}

		private bool IsPowOfTwo(int size)
		{
			int val = 1;
			while (val <= size)
			{
				if (val == size)
					return true;
				val *= 2;
			}
			return false;
		}
	}
}
