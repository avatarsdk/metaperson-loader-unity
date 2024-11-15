/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2024
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public class MeshTexturePair
	{
		public string meshFilePath;

		public string textureFilePath;
	}

	public static class PlyAvatarFiles
	{
		private static Dictionary<AvatarPart, string> avatarPartToPlyNameMap = new Dictionary<AvatarPart, string>()
		{
			{ AvatarPart.Head, "AvatarHead.ply" },
			{ AvatarPart.Body, "AvatarBody.ply" },
			{ AvatarPart.Eyelashes, "AvatarEyelashes.ply" },
			{ AvatarPart.LeftCornea, "AvatarLeftCornea.ply" },
			{ AvatarPart.RightCornea, "AvatarRightCornea.ply" },
			{ AvatarPart.LeftEyeball, "AvatarLeftEyeball.ply" },
			{ AvatarPart.RightEyeball, "AvatarRightEyeball.ply" },
			{ AvatarPart.TeethLower, "AvatarTeethLower.ply" },
			{ AvatarPart.TeethUpper, "AvatarTeethUpper.ply" },
		};

		private static Dictionary<AvatarPart, string> avatarPartToTexturePrefixMap = new Dictionary<AvatarPart, string>()
		{
			{ AvatarPart.Head, "AvatarHead" },
			{ AvatarPart.Body, "AvatarBody" },
			{ AvatarPart.Eyelashes, "AvatarEyelashes" },
			{ AvatarPart.LeftCornea, "AvatarLeftCornea" },
			{ AvatarPart.RightCornea, "AvatarRightCornea" },
			{ AvatarPart.LeftEyeball, "AvatarEyes" },
			{ AvatarPart.RightEyeball, "AvatarEyes" },
			{ AvatarPart.TeethLower, "AvatarTeeth" },
			{ AvatarPart.TeethUpper, "AvatarTeeth" },
		};

		public static Dictionary<AvatarPart, MeshTexturePair> FindAvailableMeshesAndTextures(string avatarDir)
		{
			Dictionary<AvatarPart, MeshTexturePair> meshTextureDict = new Dictionary<AvatarPart, MeshTexturePair>()
			{
				{ AvatarPart.Head, new MeshTexturePair() },
				{ AvatarPart.Body, new MeshTexturePair() },
				{ AvatarPart.Eyelashes, new MeshTexturePair() },
				{ AvatarPart.LeftCornea, new MeshTexturePair() },
				{ AvatarPart.RightCornea, new MeshTexturePair() },
				{ AvatarPart.LeftEyeball, new MeshTexturePair() },
				{ AvatarPart.RightEyeball, new MeshTexturePair() },
				{ AvatarPart.TeethLower, new MeshTexturePair() },
				{ AvatarPart.TeethUpper, new MeshTexturePair() },
			};

			var existingPlyFiles = Directory.GetFiles(avatarDir, "*.ply");
			foreach(var pair in meshTextureDict)
			{
				string targetPlyName = avatarPartToPlyNameMap[pair.Key];
				foreach(string plyFilePath in existingPlyFiles)
				{
					if (Path.GetFileName(plyFilePath) == targetPlyName)
					{
						meshTextureDict[pair.Key].meshFilePath = plyFilePath;
						break;
					}
				}
			}

			var existingTextureFiles = Directory.GetFiles(avatarDir, "*.png").Concat(Directory.GetFiles(avatarDir, "*.jpg")).ToArray();
			foreach (var pair in meshTextureDict)
			{
				string targetTexturePrefix = avatarPartToTexturePrefixMap[pair.Key];
				foreach (string textureFilePath in existingTextureFiles)
				{
					string textureFileName = Path.GetFileName(textureFilePath);
					if (textureFileName.StartsWith(targetTexturePrefix) && textureFileName.Contains("_Color_"))
					{
						meshTextureDict[pair.Key].textureFilePath = textureFilePath;
						break;
					}
				}
			}

			return meshTextureDict;
		}
	}
}
