/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2023
*/

using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class ZipUtils
	{
		static ZipUtils()
		{
			ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 0;
		}

		public static Dictionary<string, byte[]> Unzip(byte[] bytes)
		{
			Dictionary<string, byte[]> extractedFiles = new Dictionary<string, byte[]>();
			using (var s = new ZipInputStream(new MemoryStream(bytes)))
			{
				ZipEntry theEntry;
				while ((theEntry = s.GetNextEntry()) != null)
				{
					string fileName = Path.GetFileName(theEntry.Name);
					if (string.IsNullOrEmpty(fileName))
						continue;


					byte[] data = new byte[s.Length];
					int size = s.Read(data, 0, data.Length);
					if (size != data.Length)
					{
						Debug.LogErrorFormat("Extracted unexpected data size. Extracted: {0}, expected: {1}", size, data.Length);
						return null;
					}
					extractedFiles.Add(fileName, data);
				}
			}
			return extractedFiles;
		}
	}
}