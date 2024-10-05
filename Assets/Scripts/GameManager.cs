using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        AudioReceiver.HandleStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneDirector.RestartScene();
    }
}
