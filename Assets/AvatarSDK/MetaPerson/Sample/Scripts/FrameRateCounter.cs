/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSDK.MetaPerson.Sample
{
    public class FrameRateCounter : MonoBehaviour
    {
        public Text fpsText;
        public Text avgFpsText;
        public Text frameTimeText;
        public Text avgFrameTimeText;

        private float avgFrameDuration = 0;

        void Update()
        {
            float frameDuration = Time.unscaledDeltaTime;
            avgFrameDuration += (Time.unscaledDeltaTime - avgFrameDuration) * 0.1f;

            if (fpsText != null)
            {
                int fps = Mathf.RoundToInt(1 / frameDuration);
                fpsText.text = string.Format("FPS: {0}", fps);
            }

            if (avgFpsText != null)
            {
                int avgFps = Mathf.RoundToInt(1 / avgFrameDuration);
                avgFpsText.text = string.Format("Avg FPS: {0}", avgFps);
            }

            if (frameTimeText != null)
            {
                int frameTimeMs = Mathf.RoundToInt(frameDuration * 1000.0f);
                frameTimeText.text = string.Format("Frame Time: {0} ms", frameTimeMs);
            }

            if (avgFrameTimeText != null)
            {
                int avgFrameTimeMs = Mathf.RoundToInt(avgFrameDuration * 1000.0f);
                avgFrameTimeText.text = string.Format("Avg Frame Time: {0} ms", avgFrameTimeMs);
            }
        }
    }
}
