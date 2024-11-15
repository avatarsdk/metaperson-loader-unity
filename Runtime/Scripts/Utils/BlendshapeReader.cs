/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2024
*/

using UnityEngine;
using System.IO;

namespace AvatarSDK.MetaPerson.Loader
{
	public class BlendshapeReader
	{
		public Vector3[] ReadVerticesDeltas(string blendshapeFilename, bool leftHandedCoordinates = true)
		{
			var buffer = File.ReadAllBytes(blendshapeFilename);

			Vector3[] deltas = null;
			unsafe
			{
				int vecSize = sizeof(Vector3);
				int numDeltas = buffer.Length / vecSize;
				deltas = new Vector3[numDeltas];

				fixed (byte* bytePtr = &buffer[0])
				{
					for (int i = 0; i < numDeltas; ++i)
					{
						float* ptr = (float*)(bytePtr + i * vecSize);
						deltas[i].x = -(*ptr);
						deltas[i].y = *(ptr + 1);
						deltas[i].z = *(ptr + 2);
					}
				}
			}

			if (!leftHandedCoordinates)
				for (int i = 0; i < deltas.Length; ++i)
					deltas[i].x *= -1;

			return deltas;
		}
	}
}

