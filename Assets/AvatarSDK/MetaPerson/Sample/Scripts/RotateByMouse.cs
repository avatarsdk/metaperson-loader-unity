/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AvatarSDK.MetaPerson.Sample
{
    /// <summary>
    /// This behavior is added to avatar heads in samples to allow rotation around y-axis by mouse.
    /// Does not work well on mobile, but okay for a sample.
    /// </summary>
    public class RotateByMouse : MonoBehaviour
    {
        // UI objects that don't intercept movements
        public List<GameObject> excludedUIObjects = new List<GameObject>();

        public bool detectMovementsOverOtherGameObjects = false;

        public event Action<float> onRotated;

        protected Vector2 lastPosition;

        protected bool isMovementInProgress = false;
        protected bool isStartPointOverUI = false;

        void Update()
        {
#if !UNITY_WEBGL
            if (!Input.mousePresent)
            {
                if (IsPointerOverUIObject() || Input.touches.Length != 1)
                    return;

                Touch t = Input.touches[0];
                if (t.phase == TouchPhase.Moved)
                {
                    Vector2 delta = t.position - lastPosition;
                    transform.Rotate(Vector3.up, -0.5f * delta.x);
                    onRotated?.Invoke(transform.rotation.eulerAngles.y);
                }
                lastPosition = t.position;
            }
            else
#endif
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    bool isPointerOverUI = IsPointerOverUIObject();
                    if (!isMovementInProgress)
                    {
                        isMovementInProgress = true;
                        isStartPointOverUI = isPointerOverUI;
                    }

                    if (!isPointerOverUI && !isStartPointOverUI)
                    {
                        var dx = Input.GetAxis("Mouse X");
                        transform.Rotate(Vector3.up, -dx * 5);
                        onRotated?.Invoke(transform.rotation.eulerAngles.y);
                    }
                }
                else
                {
                    isMovementInProgress = false;
                }
            }
        }

        protected bool IsPointerOverUIObject()
        {
            if (detectMovementsOverOtherGameObjects)
                return false;

            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            foreach (RaycastResult raycastResult in results)
            {
                if (!excludedUIObjects.Contains(raycastResult.gameObject))
                    return true;
            }
            return false;
        }
    }
}
