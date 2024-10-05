using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation
{
    public static class Window
    {
        public static void Close()
        {
            Application.Quit();
        }

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public static void ToggleFullScreen()
        {
            if (!Screen.fullScreen)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(960, 540, true);
            }
        }

        public static void SetFullScreen(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
        }
    }
}
