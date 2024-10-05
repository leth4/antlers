using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioReceiver
{
    public static void HandleStart()
    {
        AudioManager.Instance.Play(SoundEnum.Wind, 2, true);
    }
}
