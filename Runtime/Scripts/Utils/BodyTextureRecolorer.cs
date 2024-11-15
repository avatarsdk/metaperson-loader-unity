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

namespace AvatarSDK.MetaPerson.Loader
{
	public class BodyTextureRecolorer : MonoBehaviour
	{
		public SkinnedMeshRenderer bodyMeshRenderer = null;

		public bool enableRecoloring = true;

		public Color skinColor = Color.clear;

		public Color defaultSkinColor = Color.clear;

		public Material bodyRecoloringMaterial = null;

		private bool targetEnableRecoloring = true;
	
		private Color targetSkinColor = Color.clear;

		private Color targetDefaultSkinColor = Color.clear;

		private Texture initialColorTexture = null;

		private RenderTexture resultColorTexture = null;

		public void Initialize(Material bodyRecoloringMaterial, Color initialSkinColor, Color defaultSkinColor)
		{
			this.bodyRecoloringMaterial = bodyRecoloringMaterial;
			SetColors(defaultSkinColor, initialSkinColor);
			UpdateMaterialParameters();
		}

		public void RecolorTexture()
		{
			ImageUtils.DrawTextureByMaterial(resultColorTexture, initialColorTexture, bodyRecoloringMaterial);
			bodyMeshRenderer.material.mainTexture = resultColorTexture;
		}

		public RenderTexture GetRecoloredTexture()
		{
			return resultColorTexture;
		}

		public static void RecolorBodyTexture(SkinnedMeshRenderer bodyMeshRenderer, AvatarColor targetSkinColor)
		{
			Material recoloringMaterial = Resources.Load<Material>("Materials/Recoloring/body_recoloring_material");
			if (recoloringMaterial == null)
			{
				Debug.LogError("Recoloring material not found");
				return;
			}

			recoloringMaterial.SetVector("_TargetSkinColor", targetSkinColor.ToUnityColor());

			Texture bodyTexture = bodyMeshRenderer.material.mainTexture;
			Texture2D recoloredTexture = ImageUtils.DrawTextureByMaterial(bodyTexture, recoloringMaterial);
			bodyMeshRenderer.material.mainTexture = recoloredTexture;
		}

		private void Start()
		{
			if (bodyMeshRenderer != null)
			{
				initialColorTexture = bodyMeshRenderer.material.mainTexture;
				resultColorTexture = ImageUtils.CreateRenderTexture(initialColorTexture.width, initialColorTexture.height);

				defaultSkinColor = bodyRecoloringMaterial.GetVector("_SkinColor");
			}
		}

		private void Update()
		{
			bool textureUpdateRequired = false;

			if (targetSkinColor != skinColor)
			{
				targetSkinColor = skinColor;
				textureUpdateRequired = true;
			}

			if (enableRecoloring != targetEnableRecoloring)
			{
				targetEnableRecoloring = enableRecoloring;
				textureUpdateRequired = true;
			}

			if (defaultSkinColor != targetDefaultSkinColor)
			{
				targetDefaultSkinColor = defaultSkinColor;
				textureUpdateRequired = true;
			}

			if (textureUpdateRequired)
			{
				UpdateMaterialParameters();
				RecolorTexture();
			}
		}

		private void OnDestroy()
		{
			if (bodyMeshRenderer != null)
				bodyMeshRenderer.material.mainTexture = initialColorTexture;

			if (resultColorTexture != null)
			{
				resultColorTexture.Release();
				Destroy(resultColorTexture);
				resultColorTexture = null;
			}
		}

		private void SetColors(Color defaultSkinColor, Color currentSkinColor)
		{
			this.defaultSkinColor = defaultSkinColor;

			skinColor = currentSkinColor;
			targetSkinColor = currentSkinColor;
		}

		private void UpdateMaterialParameters()
		{
			bodyRecoloringMaterial.SetVector("_TargetSkinColor", targetSkinColor);
			bodyRecoloringMaterial.SetVector("_SkinColor", defaultSkinColor);
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(BodyTextureRecolorer))]
	public class BodyTextureRecolorerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var textureRecolorer = (BodyTextureRecolorer)target;
			RenderTexture recoloredTexture = textureRecolorer.GetRecoloredTexture();
			if (recoloredTexture != null)
			{
				GUILayout.Space(10);

				if (GUILayout.Button("Save recolored texture"))
				{
					ImageUtils.SaveRenderTexture(recoloredTexture);
				}
			}
		}
	}
#endif
}
