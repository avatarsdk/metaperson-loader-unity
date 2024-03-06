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
		public static string GetModelPath(Uri uri)
		{
			if (!uri.IsFile)
			{
				List<string> directories = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				if (directories != null)
				{
					directories.Insert(0, uri.Host);
					directories.Insert(0, GetRootDirectory());
					return Path.Combine(directories.ToArray());
				}
			}

			return string.Empty;
		}

		public static Uri ConvertToCacheUriIfExists(Uri uri)
		{
			string modelPath = GetModelPath(uri);
			if (File.Exists(modelPath))
			{
				return new Uri(modelPath);
			}
			return uri;
		}

		public static void SaveModel(Uri uri, byte[] modelBytes)
		{
			string modelPath = GetModelPath(uri);
			string modelDir = Path.GetDirectoryName(modelPath);
			if (!Directory.Exists(modelDir))
				Directory.CreateDirectory(modelDir);
			File.WriteAllBytes(modelPath, modelBytes);
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
