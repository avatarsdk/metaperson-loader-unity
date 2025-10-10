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
using UnityEngine;

namespace AvatarSDK.MetaPerson.Sample
{
	[Serializable]
	public class HeadReplacementAvatarConfig
	{
		public GameObject templateModelPrefab;

		public UnityEngine.Object headModelAsset;
		public string headModelPath;

		public UnityEngine.Object modelJsonAsset;
		public string modelJsonPath;

		public string GetHeadModelPath()
		{
			if (!string.IsNullOrEmpty(headModelPath))
				return headModelPath;

#if UNITY_EDITOR
			if (headModelAsset != null)
				return GetAssetFullPath(headModelAsset);
#endif

			return string.Empty;
		}

		public string GetModelJsonPath()
		{
			if (!string.IsNullOrEmpty(modelJsonPath))
				return modelJsonPath;

#if UNITY_EDITOR
			if (modelJsonAsset != null)
				return GetAssetFullPath(modelJsonAsset);
#endif

			return string.Empty;
		}

#if UNITY_EDITOR
		private string GetAssetFullPath(UnityEngine.Object assetObject)
		{
			if (assetObject != null)
			{
				string assetPath = UnityEditor.AssetDatabase.GetAssetPath(assetObject);
				if (!string.IsNullOrEmpty(assetPath))
				{
					if (assetPath.StartsWith("Assets"))
						assetPath = assetPath.Substring("Assets".Length + 1);
					return Path.Combine(Application.dataPath, assetPath);
				}
			}
			return string.Empty;
		}
#endif
	}
}
