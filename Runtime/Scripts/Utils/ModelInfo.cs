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
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	[Serializable]
	public class ModelInfo
	{
		public AvatarColor skin_color;
	}

	[Serializable]
	public class AvatarColor
	{
		public int red = -1;
		public int green = -1;
		public int blue = -1;

		public Color ToUnityColor()
		{
			return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
		}
	}

	public static class AvatarColorExtensions
	{
		public static bool IsNullOrNotSpecified(this AvatarColor color)
		{
			return color == null || (color.red == -1 && color.green == -1 && color.blue == -1);
		}
	}
}
