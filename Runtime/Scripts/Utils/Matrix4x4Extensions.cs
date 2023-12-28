/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2021
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class Matrix4x4Extensions
	{
		public static Vector3 GetPosition(this Matrix4x4 mat)
		{
			return mat.GetColumn(3);
		}

		public static Quaternion GetRotation(this Matrix4x4 mat)
		{
			return Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
		}

		public static Vector3 GetScale(this Matrix4x4 mat)
		{
			return new Vector3(mat.GetColumn(0).magnitude, mat.GetColumn(1).magnitude, mat.GetColumn(2).magnitude);
		}
	}
}
