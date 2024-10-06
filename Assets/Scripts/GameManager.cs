using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public bool IsInfiniteMode { get; private set; }

    [SerializeField] private GameObject _overlay;
    [SerializeField] private Text _overlayText;
    [SerializeField] private Text _healthCounter;

    private bool _isEndingLevel;

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

        _overlayText.text = "YOU SURVIVED ANOTHER FALL";
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

    private void UpdateHealthCounter()
    {
        _healthCounter.text = "";
        // var counterWord = _healthLeft != 1 ? "NIGHTS" : "NIGHT";
        // _healthCounter.text = $"{_healthLeft.ToWord()} {counterWord} TO LIVE.";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateLevel();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            IsInfiniteMode = !IsInfiniteMode;
            if (IsInfiniteMode) PlayerController.Instance.transform.position = new Vector3(1000, 1000, 0);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Window.Close();
        if (Input.GetKeyDown(KeyCode.F)) Window.ToggleFullScreen();
    }

    private void GenerateLevel()
    {
        GridManager.Instance.GenerateGrid();
        FaunaManager.Instance.GenerateCreatures();
    }

    private void HandleNoDeersLeft()
    {
        if (_isEndingLevel) return;
        StartCoroutine(LevelChangeRoutine());
    }

    private IEnumerator LevelChangeRoutine(string text = "", float delay = 1f)
    {
        _isEndingLevel = true;
        yield return new WaitForSeconds(delay);
        _overlay.SetActive(true);
        _overlayText.text = text;
        yield return new WaitForSeconds(text == "" ? 1f : 2.5f);
        _overlay.SetActive(false);

        GenerateLevel();

        _isEndingLevel = false;
    }

    public void HandlePlayerDeath(DeathReason reason)
    {
        AudioManager.Instance.Play(SoundEnum.Death, 0, false);
        if (reason is DeathReason.Snake) StartCoroutine(LevelChangeRoutine("YOU LOST YOUR LIFE TO A GRASS LURKER.", 0));
        if (reason is DeathReason.Mine) StartCoroutine(LevelChangeRoutine("YOU LOST YOUR LIFE TO AN ELECTRIC LANDMINE.", 0));
        if (reason is DeathReason.Hunter) StartCoroutine(LevelChangeRoutine("YOU LOST YOUR LIFE TO A DOUBLE-HEADED DOG.", 0));
    }

    private void OnEnable()
    {
        FaunaManager.OnNoDeersLeft += HandleNoDeersLeft;
    }

    private void OnDisable()
    {
        FaunaManager.OnNoDeersLeft -= HandleNoDeersLeft;
    }
}

public enum DeathReason
{
    Snake,
    Hunter,
    Hunger,
    Mine
}