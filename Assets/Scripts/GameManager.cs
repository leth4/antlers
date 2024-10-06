using System;
using System.Collections;
using System.Collections.Generic;
using Foundation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public bool IsInfiniteMode { get; private set; }

    [SerializeField] private GameObject _overlay;
    [SerializeField] private Text _overlayText;
    [SerializeField] private Text _healthCounter;

    private static bool _haveSeenTutorial;

    private float _newLevelAutoTimeLeft;

    private bool _isEndingLevel;

    private int _currentLevel = 0;

    private void Start()
    {
#if UNITY_EDITOR
        _haveSeenTutorial = true;
#endif

        StartCoroutine(GameIntroRoutine(() =>
        {
            if (!AudioManager.Instance.IsPlaying(SoundEnum.Wind)) AudioManager.Instance.Play(SoundEnum.Wind, 0, true);
            GenerateLevel();
            PlayerController.Instance.IsActive = true;
        }));

        StartCoroutine(RandomSoundRoutine(SoundEnum.Random, 5, 15));
    }

    private IEnumerator RandomSoundRoutine(SoundEnum sound, float minTime, float maxTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));
            AudioManager.Instance.Play(sound);
        }
    }

    private IEnumerator GameIntroRoutine(Action onFinished)
    {
        _overlay.SetActive(true);

        _overlayText.text = "";

        yield return new WaitForSeconds(.5f);

        if (!_haveSeenTutorial)
        {
            _overlayText.text = "HUMAN IN A WORLD OF MACHINES";
            yield return new WaitForSeconds(4f);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);

            _overlayText.text = "THE YEAR WAS LONG, BUT IT'S WINTER SOON";
            yield return new WaitForSeconds(4f);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);

            _overlayText.text = "MOVE WITH [WASD] or [↑←↓→]";
            yield return new WaitForSeconds(4f);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);

            _overlayText.text = "HUNT A METAL DEER TO SURVIVE A NIGHT";
            yield return new WaitForSeconds(4f);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);

            _overlayText.text = "BIOTECH IS AN AQUIRED TASTE";
            yield return new WaitForSeconds(4f);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);
        }
        else
        {
            _overlayText.text = "THE YEAR WAS LONG, BUT IT'S WINTER SOON";
            yield return new WaitForSeconds(2);
            _overlayText.text = "";
            yield return new WaitForSeconds(1);
        }

        _overlay.SetActive(false);

        _haveSeenTutorial = true;

        onFinished?.Invoke();
    }

    private IEnumerator GameEndingRoutine()
    {
        _overlay.SetActive(true);
        _overlayText.text = "";

        Time.timeScale = 0;

        AudioManager.Instance.Stop(SoundEnum.Wind, 3);
        yield return new WaitForSecondsRealtime(4);

        AudioManager.Instance.Play(SoundEnum.Ambient, 5);

        yield return new WaitForSecondsRealtime(.5f);

        _overlayText.text = "YOU SURVIVED ANOTHER FALL";
        yield return new WaitForSecondsRealtime(4);
        _overlayText.text = "";
        yield return new WaitForSecondsRealtime(1);

        _overlayText.text = "MACHINES WILL HYBERNATE NOW";
        yield return new WaitForSecondsRealtime(4);
        _overlayText.text = "";
        yield return new WaitForSecondsRealtime(1);

        _overlayText.text = "THANK YOU FOR PLAYING";
        yield return new WaitForSecondsRealtime(4);
        _overlayText.text = "";
        yield return new WaitForSecondsRealtime(1);

        _overlayText.text = "PRESS [K] FOR IDLE MODE, THEN [R] TO SWITCH";
        yield return new WaitForSecondsRealtime(4);
        _overlayText.text = "";
        yield return new WaitForSecondsRealtime(3);

        SceneDirector.RestartScene();
    }

    private void Update()
    {
        if (IsInfiniteMode)
        {
            _newLevelAutoTimeLeft -= Time.deltaTime;
            if (_newLevelAutoTimeLeft <= 0 || Input.GetKeyDown(KeyCode.R))
            {
                _newLevelAutoTimeLeft = 25;
                GenerateLevel();
            }
        }
#if UNITY_EDITOR
        else if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateLevel();
            // HandleNoDeersLeft();
        }
#endif

        if (Input.GetKeyDown(KeyCode.K))
        {
            IsInfiniteMode = !IsInfiniteMode;
            if (IsInfiniteMode)
            {
                PlayerController.Instance.transform.position = new Vector3(1000, 1000, 0);
            }
            else SceneDirector.RestartScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) Window.Close();
        if (Input.GetKeyDown(KeyCode.F)) Window.ToggleFullScreen();
    }

    private void GenerateLevel()
    {
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Ambient, 1);
        GridManager.Instance.GenerateGrid();
        FaunaManager.Instance.GenerateCreatures();
    }

    private void HandleNoDeersLeft()
    {
        if (IsInfiniteMode) return;
        if (_isEndingLevel) return;
        _isEndingLevel = true;
        _currentLevel = 0;
        AudioManager.Instance.Play(SoundEnum.Death, 0, false);
        StartCoroutine(DeathRoutine("YOU LOST YOUR LIFE TO HUNGER"));
    }

    public void HandleFoundFood()
    {
        if (IsInfiniteMode) return;
        if (_isEndingLevel) return;
        _isEndingLevel = true;
        _currentLevel++;
        if (_currentLevel == 7) StartCoroutine(GameEndingRoutine());
        else StartCoroutine(LevelChangeRoutine());
    }

    private IEnumerator LevelChangeRoutine()
    {
        _isEndingLevel = true;
        yield return new WaitForSeconds(.5f);
        UnloadEverything();
        _overlay.SetActive(true);
        _overlayText.text = $"NOVEMBER {24 + _currentLevel}";
        yield return new WaitForSeconds(1.5f);
        _overlay.SetActive(false);

        GenerateLevel();

        _isEndingLevel = false;
    }

    private IEnumerator DeathRoutine(string text)
    {
        UnloadEverything();
        _isEndingLevel = true;
        _overlay.SetActive(true);
        _overlayText.text = text + $"\nON NOVEMBER {24 + _currentLevel}.";
        yield return new WaitForSeconds(3);
        _isEndingLevel = false;
        SceneDirector.RestartScene();
    }

    private void UnloadEverything()
    {
        FaunaManager.Instance.DestroyCreatures();
    }

    public void HandlePlayerDeath(DeathReason reason)
    {
        if (IsInfiniteMode) return;
        if (_isEndingLevel) return;
        AudioManager.Instance.Play(SoundEnum.Death, 0, false);
        if (reason is DeathReason.Snake) StartCoroutine(DeathRoutine("YOU LOST YOUR LIFE TO A CARBON GRASS LURKER"));
        if (reason is DeathReason.Mine) StartCoroutine(DeathRoutine("YOU LOST YOUR LIFE TO AN ELECTRIC LANDMINE"));
        if (reason is DeathReason.Hunter) StartCoroutine(DeathRoutine("YOU LOST YOUR LIFE TO AN OVERCHARGED DOUBLE-HEADED DOG"));
        _currentLevel = 0;
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