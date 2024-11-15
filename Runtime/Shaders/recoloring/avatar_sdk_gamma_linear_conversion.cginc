// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, October 2022

#ifndef AVATAR_SDK_GAMMA_LINEAR_CONVERSION_INCLUDED
#define AVATAR_SDK_GAMMA_LINEAR_CONVERSION_INCLUDED

fixed4 linearToGamma(fixed4 c)
{
    half gamma = 1 / 2.2;
    return pow(c, gamma);
}

fixed4 gammaToLinear(fixed4 c)
{
    half gamma = 2.2;
    return pow(c, gamma);
}

fixed4 tex2DInLinear(sampler2D tex, float2 uv)
{
    fixed4 col = tex2D(tex, uv);
#if UNITY_COLORSPACE_GAMMA
    col = gammaToLinear(col);
#endif
    return col;
}

#endif //AVATAR_SDK_GAMMA_LINEAR_CONVERSION_INCLUDED