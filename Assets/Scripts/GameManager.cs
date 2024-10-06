using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject _overlay;
    [SerializeField] private Text _overlayText;
    [SerializeField] private Text _healthCounter;

    private int _healthLeft = 3;

    private void Start()
    {
        UpdateHealthCounter();
        // StartCoroutine(GameIntroRoutine(() =>
        // {
        AudioManager.Instance.Play(SoundEnum.Wind, 0, true);
        GenerateLevel();
        PlayerController.Instance.IsActive = true;
        // }));
    }

    private IEnumerator GameIntroRoutine(Action onFinished)
    {
        _overlay.SetActive(true);

        yield return new WaitForSeconds(.5f);

        _overlayText.text = "A HUMAN IN THE WORLD OF MACHINES";
        yield return new WaitForSeconds(2);
        _overlayText.text = "";
        yield return new WaitForSeconds(1);

        _overlayText.text = "THERE'S 15 DAYS LEFT UNTIL WINTER";
        yield return new WaitForSeconds(2);
        _overlayText.text = "";
        yield return new WaitForSeconds(1);

        _overlayText.text = "MOVE WITH [WASD] or [↑←↓→]";
        yield return new WaitForSeconds(2);
        _overlayText.text = "";
        yield return new WaitForSeconds(1);

        _overlay.SetActive(false);

        onFinished?.Invoke();
    }

    private IEnumerator GameEndingRoutine(Action onFinished)
    {
        _overlay.SetActive(true);

        yield return new WaitForSeconds(.5f);

        _overlayText.text = "YOUR SURVIVED ANOTHER FALL";
        yield return new WaitForSeconds(2);
        _overlayText.text = "";
        yield return new WaitForSeconds(1);

        _overlayText.text = "MACHINES WILL HYBERNATE NOW";
        yield return new WaitForSeconds(2);
        _overlayText.text = "";
        yield return new WaitForSeconds(1);

        _overlay.SetActive(false);

        onFinished?.Invoke();
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
