/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2024
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class MetaPersonCache
	{
		public static string GetModelFilePathByUri(Uri uri)
		{
			if (!uri.IsFile)
			{
				List<string> directories = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				directories.Insert(0, uri.Host);
				directories.Insert(0, GetRootDirectory());
				string modelFilePath = Path.Combine(directories.ToArray());

				if (modelFilePath.ToLower().EndsWith("glb") || modelFilePath.ToLower().EndsWith("gltf"))
					return modelFilePath;

				string modelDirPath = Path.GetDirectoryName(modelFilePath);
				if (Directory.Exists(modelDirPath))
				{
					string[] glbFiles = Directory.GetFiles(modelDirPath, "*.glb");
					if (glbFiles != null && glbFiles.Length > 0)
						return glbFiles[0];

					string[] gltfFiles = Directory.GetFiles(modelDirPath, "*.gltf");
					if (gltfFiles != null && gltfFiles.Length > 0)
						return gltfFiles[0];
				}

				return string.Empty;
			}
			else
				return uri.LocalPath;
		}

		public static string GetModelDirByUri(Uri uri)
		{
			if (uri.IsFile)
				return Path.GetDirectoryName(uri.LocalPath);
			else
			{
				List<string> directories = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				directories.Insert(0, uri.Host);
				directories.Insert(0, GetRootDirectory());
				string modelFilePath = Path.Combine(directories.ToArray());
				return Path.GetDirectoryName(modelFilePath);
			}
		}

		public static void ClearCache()
		{
			string rootDirPath = GetRootDirectory();
			if (Directory.Exists(rootDirPath))
				Directory.Delete(rootDirPath, true);
		}

		private static string GetRootDirectory()
		{
			return Path.Combine(Application.persistentDataPath, "MetaPersonLoaderCache");
		}
	}
}
