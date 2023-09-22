/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2020
*/

using UnityEngine;

namespace AvatarSDK.MetaPerson.Sample
{
    public class MoveByMouse : RotateByMouse
    {
        float lastDoubleTouchMangnitude;
        Vector2 lastDoubleTouchCenter = Vector2.zero;

        void Update()
        {
#if !UNITY_WEBGL
            if (!Input.mousePresent)
            {
                if (Input.touches.Length == 1)
                {
                    Touch t = Input.touches[0];
                    if (t.phase == TouchPhase.Moved)
                    {
                        Vector2 delta = t.position - lastPosition;
                        transform.Rotate(Vector3.up, -0.5f * delta.x);
                    }
                    lastPosition = t.position;
                }
                else if (Input.touches.Length == 2)
                {
                    Touch t1 = Input.touches[0];
                    Touch t2 = Input.touches[1];
                    Vector2 doubleTouchCenter = t1.position + 0.5f * (t2.position - t1.position);
                    float magnitude = (t1.position - t2.position).magnitude;
                    if (t1.phase == TouchPhase.Moved && t2.phase == TouchPhase.Moved)
                    {
                        float magnitudeDelta = magnitude - lastDoubleTouchMangnitude;
                        transform.Translate(0, 0, magnitudeDelta * 0.01f, Space.World);

                        if (lastDoubleTouchCenter != Vector2.zero)
                        {
                            Vector2 centerDelta = doubleTouchCenter - lastDoubleTouchCenter;
                            transform.Translate(0, centerDelta.y * 0.01f, 0);
                        }

                    }
                    lastDoubleTouchMangnitude = magnitude;
                    lastDoubleTouchCenter = doubleTouchCenter;
                }
            }
            else
#endif
            {

                bool isLeftButtonPressed = Input.GetMouseButton(0);
                bool isRightButtonPressed = Input.GetMouseButton(1);
                if (isLeftButtonPressed || isRightButtonPressed)
                {
                    bool isPointerOverUI = IsPointerOverUIObject();
                    if (!isMovementInProgress)
                    {
                        isMovementInProgress = true;
                        isStartPointOverUI = isPointerOverUI;
                    }

                    if (!isPointerOverUI && !isStartPointOverUI)
                    {
                        if (isLeftButtonPressed)
                        {
                            var dx = Input.GetAxis("Mouse X");
                            transform.Rotate(Vector3.up, -dx * 5);
                        }

                        if (isRightButtonPressed)
                        {
                            var dy = Input.GetAxis("Mouse Y");
                            transform.Translate(0, dy * 0.1f, 0);
                        }
                    }
                }
                else
                {
                    isMovementInProgress = false;
                }

                Vector2 scrollDelta = Input.mouseScrollDelta;
                if (scrollDelta != Vector2.zero && IsMouseOverGameWindow())
                {
                    transform.Translate(0, 0, 0.1f * scrollDelta.y, Space.World);
                }
            }
        }

        bool IsMouseOverGameWindow()
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }
    }
}
