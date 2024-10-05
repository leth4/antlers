using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public static class Service
    {
        private static Camera _mainCamera;
        public static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }
    }
}
