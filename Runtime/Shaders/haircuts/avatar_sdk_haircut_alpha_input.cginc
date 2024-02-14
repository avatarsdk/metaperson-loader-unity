// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, August 2022

#ifndef AVATAR_SDK_ALPHA_INPUT_INCLUDED
#define AVATAR_SDK_ALPHA_INPUT_INCLUDED

float _UseAlphaTex;
sampler2D _AlphaTex;
float _RootsAlphaLevel;

#define Alpha(uv) AlphaLevel(uv)
half AlphaLevel(float2 uv)
{
#if defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
    return _Color.a;
#else
    float alpha = 1.0f;
    if (_UseAlphaTex != 0.0f)
        alpha = tex2D(_AlphaTex, uv).r;
    else
        alpha = tex2D(_MainTex, uv).a;

    float startAvgPos = 0.1 * (1 - _RootsAlphaLevel);
    if (uv.y < startAvgPos)
    {
        alpha = 0;
        int n = 0;
        float delta = 0.01;
        for (int dy = 0; dy<7; dy++)
        {
            float y = uv.y + dy * delta;
            if (y < startAvgPos)
            {
                float2 dUV = float2(uv.x, y);
                if (_UseAlphaTex != 0.0f)
                    alpha += tex2D(_AlphaTex, dUV).r;
                else
                    alpha += tex2D(_MainTex, dUV).a;
                n++;
            }
        }
        alpha /= n;
    }

    return alpha;
#endif
}

#endif // AVATAR_SDK_ALPHA_INPUT_INCLUDED