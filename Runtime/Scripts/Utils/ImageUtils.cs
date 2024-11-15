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
using UnityEngine.Experimental.Rendering;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class ImageUtils
	{
		public static async Task<Texture2D> LoadTexture(string textureFilename, bool compress, bool makeNonReadable)
		{
			if (!File.Exists(textureFilename))
			{
				Debug.LogErrorFormat("Unable to load texture. File not found: {0}", textureFilename);
				return null;
			}

			byte[] textureBytes = await Task.Run(() => File.ReadAllBytes(textureFilename));

			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(textureBytes);

			if (compress)
			{
				CompressTextureIfPossible(texture);
			}

			if (makeNonReadable)
				texture.Apply(true, true);

			return texture;
		}

		public static void CompressTextureIfPossible(Texture2D texture)
		{
			if (IsPowOfTwo(texture.width) && IsPowOfTwo(texture.height))
				texture.Compress(true);
		}


		public static void DrawTextureByMaterial(RenderTexture dstTexture, Texture srcTexture, Material material)
		{
			int textureWidth = srcTexture.width;
			int textureHeight = srcTexture.height;

			RenderTexture backup = RenderTexture.active;
			RenderTexture.active = dstTexture;

			GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f));
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, textureWidth, textureHeight, 0);

			Graphics.DrawTexture(new Rect(0, 0, textureWidth, textureHeight), srcTexture, material);
			if (dstTexture.useMipMap)
				dstTexture.GenerateMips();

			GL.PopMatrix();
			RenderTexture.active = backup;
		}

		public static Texture2D DrawTextureByMaterial(Texture srcTexture, Material material, bool compress = true, bool makeUnreadable = true, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
		{
			int textureWidth = srcTexture.width;
			int textureHeight = srcTexture.height;

			RenderTexture renderTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32,
				QualitySettings.activeColorSpace == ColorSpace.Linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB, 8);
			renderTexture.filterMode = FilterMode.Point;

			RenderTexture backup = RenderTexture.active;
			RenderTexture.active = renderTexture;

			GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f));
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, textureWidth, textureHeight, 0);

			Graphics.DrawTexture(new Rect(0, 0, textureWidth, textureHeight), srcTexture, material);

			var resultTexture = new Texture2D(srcTexture.width, srcTexture.height, GraphicsFormat.R8G8B8A8_UNorm, srcTexture.mipmapCount, TextureCreationFlags.MipChain);
			resultTexture.ReadPixels(new Rect(0, 0, srcTexture.width, srcTexture.height), 0, 0);
			resultTexture.Apply();

			GL.PopMatrix();
			RenderTexture.active = backup;

			RenderTexture.ReleaseTemporary(renderTexture);

			if (compress)
				CompressTextureIfPossible(resultTexture);
			if (makeUnreadable)
				resultTexture.Apply(true, true);

			resultTexture.wrapMode = wrapMode;

			return resultTexture;
		}

		public static RenderTexture CreateRenderTexture(int width, int height)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			renderTexture.filterMode = FilterMode.Point;
			renderTexture.autoGenerateMips = false;
			renderTexture.useMipMap = true;
			renderTexture.antiAliasing = 8;
			return renderTexture;
		}

#if UNITY_EDITOR
		public static void SaveRenderTexture(RenderTexture renderTexture)
		{
			RenderTexture backup = RenderTexture.active;
			RenderTexture.active = renderTexture;

			var exportTexture = new Texture2D(renderTexture.width, renderTexture.height, GraphicsFormat.B8G8R8A8_UNorm, TextureCreationFlags.MipChain);
			exportTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			exportTexture.Apply();

			var textureFilePath = UnityEditor.EditorUtility.SaveFilePanel("Save texture as PNG", "", "", "png");

			if (textureFilePath.Length != 0)
			{
				byte[] textureBytes = exportTexture.EncodeToPNG();
				File.WriteAllBytes(textureFilePath, textureBytes);
			}

			RenderTexture.active = backup;
		}
#endif

		private static bool IsPowOfTwo(int size)
		{
			return (size > 0) && (size & (size - 1)) == 0;
		}
	}
}
