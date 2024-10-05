using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _overlay;

    private void Start()
    {
        _overlay.SetActive(true);

        Tween.Delay(this, 0f, () =>
        {
            _overlay.SetActive(false);
            AudioManager.Instance.Play(SoundEnum.Wind, 2, true);
            GenerateLevel();
            PlayerController.Instance.IsActive = true;
        });

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
