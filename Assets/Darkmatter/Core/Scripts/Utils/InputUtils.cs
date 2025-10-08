using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Darkmatter.Core.Utils
{
    public static class InputUtils
    {
        public static bool IsPointerOverUI()
        {
            Vector2 mousePosition = new Vector2();
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                mousePosition = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            }
#else
            if (Input.touchCount > 0)
            {
                mousePosition = Input.GetTouch(0).position;
            }
            else
            {
                mousePosition = Input.mousePosition;
            }
#endif
            return IsOverUI(mousePosition);
        }

        public static bool IsOverUI(Vector2 screenPosition)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;

        }
    }
}