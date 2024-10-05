using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.Play(SoundEnum.Wind, 2, true);
        GenerateLevel();
        PlayerController.Instance.IsActive = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) GenerateLevel();
    }

    private void GenerateLevel()
    {
        GridManager.Instance.GenerateGrid();
        FaunaManager.Instance.GenerateCreatures();
    }
}
