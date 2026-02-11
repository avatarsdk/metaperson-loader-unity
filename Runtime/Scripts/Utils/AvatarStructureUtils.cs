/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2024
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Loader
{
	public static class AvatarStructureUtils
	{
		private static Dictionary<AvatarPart, string> avatarPartNames = new Dictionary<AvatarPart, string>()
		{
			{ AvatarPart.Body, "AvatarBody" },
			{ AvatarPart.Head, "AvatarHead" },
			{ AvatarPart.Eyelashes, "AvatarEyelashes" },
			{ AvatarPart.LeftCornea, "AvatarLeftCornea" },
			{ AvatarPart.RightCornea, "AvatarRightCornea" },
			{ AvatarPart.LeftEyeball, "AvatarLeftEyeball" },
			{ AvatarPart.RightEyeball, "AvatarRightEyeball" },
			{ AvatarPart.TeethLower, "AvatarTeethLower" },
			{ AvatarPart.TeethUpper, "AvatarTeethUpper" },
			{ AvatarPart.Glasses, "glasses" },
			{ AvatarPart.Haircut, "haircut" },
			{ AvatarPart.Outfit, "outfit" },
			{ AvatarPart.OutfitTop, "outfit_top" },
			{ AvatarPart.OutfitBottom, "outfit_bottom" },
			{ AvatarPart.OutfitShoes, "outfit_shoes" },
			{ AvatarPart.Hat, "hat" },
			{ AvatarPart.Earrings, "earring" },
			{ AvatarPart.Necklace, "necklace" },
			{ AvatarPart.Beard, "beard"},
			{ AvatarPart.Props, "props"}
		};

		public static string GetAvatarPartName(AvatarPart part)
		{
			return avatarPartNames[part];
		}

		public static AvatarPart GetAvatarPartByName(string name)
		{
			foreach (var pair in avatarPartNames)
			{
				if (pair.Value == name)
					return pair.Key;
			}
			return AvatarPart.Unknown;
		}

		public static bool IsCornea(string name)
		{
			return name == GetAvatarPartName(AvatarPart.LeftCornea) || name == GetAvatarPartName(AvatarPart.RightCornea);
		}

		public static bool IsEyeball(string name)
		{
			return name == GetAvatarPartName(AvatarPart.LeftEyeball) || name == GetAvatarPartName(AvatarPart.RightEyeball);
		}

		public static bool IsTeeth(string name)
		{
			return name == GetAvatarPartName(AvatarPart.TeethLower) || name == GetAvatarPartName(AvatarPart.TeethUpper);
		}

		public static bool IsOutfit(string name) 
		{
			return name == GetAvatarPartName(AvatarPart.Outfit) || 
				name == GetAvatarPartName(AvatarPart.OutfitTop) ||
				name == GetAvatarPartName(AvatarPart.OutfitBottom) ||
				name == GetAvatarPartName(AvatarPart.OutfitShoes) ||
				name == GetAvatarPartName(AvatarPart.Hat);
		}

		public static bool IsJewelry(string name)
		{
			return name == GetAvatarPartName(AvatarPart.Earrings) || name == GetAvatarPartName(AvatarPart.Necklace);
		}

		public static bool IsBeard(string name)
		{
			return name == GetAvatarPartName(AvatarPart.Beard);
		}

		public static bool IsProps(string name)
		{
			return name == GetAvatarPartName(AvatarPart.Props);
		}
	}
}
