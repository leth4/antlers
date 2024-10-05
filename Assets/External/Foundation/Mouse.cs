using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Foundation
{
    public static class Mouse
    {
        public static Vector3 ScreenPosition => Input.mousePosition;

        public static Vector3 WorldPosition => Service.MainCamera.ScreenToWorldPoint(ScreenPosition);

        public static GameObject UITarget
        {
            get
            {
                if (!EventSystem.current.IsPointerOverGameObject()) return null;
                return EventSystem.current.currentSelectedGameObject;
            }
        }

        public static GameObject RaycastTarget2D
        {
            get
            {
                var hit = Physics2D.Raycast(WorldPosition, Vector3.zero);
                return hit.collider?.gameObject;
            }
        }

        public static GameObject RaycastTarget
        {
            get
            {
                var hasHit = Physics.Raycast(Service.MainCamera.ScreenPointToRay(ScreenPosition), out var hit);
                return hasHit ? hit.transform.gameObject : null;
            }
        }

        public static Vector3 GetWorldPosition(Camera camera)
        {
            return camera.ScreenToWorldPoint(ScreenPosition);
        }
    }
}

