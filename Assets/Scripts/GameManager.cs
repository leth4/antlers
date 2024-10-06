using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject _overlay;
    [SerializeField] private Text _healthCounter;

    private int _healthLeft = 3;

    private void Start()
    {
        _overlay.SetActive(true);
        UpdateHealthCounter();

        Tween.Delay(this, 0f, () =>
        {
            _overlay.SetActive(false);
            AudioManager.Instance.Play(SoundEnum.Wind, 2, true);
            GenerateLevel();
            PlayerController.Instance.IsActive = true;
        });
    }

    public void AddHealth()
    {
        _healthLeft++;
        UpdateHealthCounter();
    }

    public void RemoveHealth()
    {
        _healthLeft--;
        UpdateHealthCounter();
    }

    private void UpdateHealthCounter()
    {
        var counterWord = _healthLeft != 1 ? "NIGHTS" : "NIGHT";
        _healthCounter.text = $"{_healthLeft.ToWord()} {counterWord} TO LIVE.";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveHealth();
            GenerateLevel();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Window.Close();
        if (Input.GetKeyDown(KeyCode.F)) Window.ToggleFullScreen();
    }

    private void GenerateLevel()
    {
        GridManager.Instance.GenerateGrid();
        FaunaManager.Instance.GenerateCreatures();
    }
}
