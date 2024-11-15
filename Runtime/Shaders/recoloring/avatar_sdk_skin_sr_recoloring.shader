// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2023

Shader "Avatar SDK/Recoloring/Skin Using SR"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SkinMask("Skin Mask", 2D) = "white" {}
        _SkinColor("Skin Color", Color) = (0, 0, 0, 1)
        _TargetSkinColor("Target Skin Color", Color) = (0, 0, 0, 1)
    }
    
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "avatar_sdk_gamma_linear_conversion.cginc"
            #include "avatar_sdk_recoloring_utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _SkinMask;
            fixed4 _SkinColor;
            fixed4 _TargetSkinColor;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed4 skinMask = tex2D(_SkinMask, i.uv);
                if (skinMask.r > 0)
                {
#if !UNITY_COLORSPACE_GAMMA
                    col = linearToGamma(col);
                    _SkinColor = linearToGamma(_SkinColor);
                    _TargetSkinColor = linearToGamma(_TargetSkinColor);
#endif

                    float3x3 colorTransform = GetColorTransformMat(_SkinColor.rgb, _TargetSkinColor.rgb);
                    col.rgb = applyColorTransformMat(col.rgb, colorTransform);
                    
#if !UNITY_COLORSPACE_GAMMA
                    col = gammaToLinear(col);
#endif
                }

                return col;
            }
            ENDCG
        }
    }
}
