// Copyright (C) Itseez3D, Inc. - All Rights Reserved
// You may not use this file except in compliance with an authorized license
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
// See the License for the specific language governing permissions and limitations under the License.
// Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2022

#ifndef AVATAR_SDK_RECOLORING_UTILS_INCLUDED
#define AVATAR_SDK_RECOLORING_UTILS_INCLUDED

float calcNorm(float3 vec)
{
    return sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
}

float3 applyColorTransformMat(float3 rgb, float3x3 R)
{
    float r = R[0][0] * rgb[0] + R[0][1] * rgb[1] + R[0][2] * rgb[2];
    float g = R[1][0] * rgb[0] + R[1][1] * rgb[1] + R[1][2] * rgb[2];
    float b = R[2][0] * rgb[0] + R[2][1] * rgb[1] + R[2][2] * rgb[2];

    return float3(r, g, b);
}

float3x3 GetColorTransformMat(float3 defaultColor, float3 targetColor)
{
    float norm = calcNorm(defaultColor);
    if (norm == 0)
    {
        defaultColor = 0.01;
        norm = calcNorm(defaultColor);
    }

    float targetNorm = calcNorm(targetColor);
    if (targetNorm == 0)
    {
        targetColor = 0.01;
        targetNorm = calcNorm(targetColor);
    }

    float scale = targetNorm / norm;

    float3x3 R = float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    float3 r = cross(defaultColor, targetColor);

    float rNorm = calcNorm(r);
    if (rNorm > 0.0017 * norm * targetNorm)
    {
        float c = dot(defaultColor, targetColor) / (norm * targetNorm);
        float s = sqrt(1.0 - c * c);

        r *= 1.0f / rNorm;

        float3x3 rrt = float3x3(r.x * r.x, r.x * r.y, r.x * r.z,
            r.x * r.y, r.y * r.y, r.y * r.z,
            r.x * r.z, r.y * r.z, r.z * r.z);

        float3x3 r_x = float3x3(0, -r.z, r.y,
            r.z, 0, -r.x,
            -r.y, r.x, 0);

        R = c * R + (1.0 - c) * rrt + s * r_x;
    }

    return R * scale;
}

#endif //AVATAR_SDK_RECOLORING_UTILS_INCLUDED