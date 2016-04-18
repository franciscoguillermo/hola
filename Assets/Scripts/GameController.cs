using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public delegate void GameControllerEventHandler();
    public static event GameControllerEventHandler OnGameStart = delegate { };
    public Material InkEffectMaterial;
    public Animator LogoAnim;

    public Text CollectionText;
    public CanvasGroup PostGameGroup;
    public CanvasGroup PreGameGroup;

    private int _fadeHash = Animator.StringToHash("Fade");
    private float _menuScreenEffectMag = 0.002f;
    private float _gameScreenEffectMag = 0.005f;
    private bool _isPlaying;
    private int _ringsCollected;
    private bool _postGame;

    public void OnLevelWasLoaded(int level)
    {
        _isPlaying = false;
        InkEffectMaterial.SetFloat("_InkBleedMag", _menuScreenEffectMag);
    }

    void Awake()
    {
        Instance = this;
        InkEffectMaterial.SetFloat("_InkBleedMag", _menuScreenEffectMag);
    }

    void Update()
    {
        if (!_isPlaying && Input.GetButtonDown("Primary Action"))
            StartCoroutine(StartGameAsync());
        if (_postGame && Input.GetButtonDown("Primary Action"))
            Application.LoadLevel(0);
    }

    IEnumerator StartGameAsync()
    {
        _isPlaying = true;
        LogoAnim.SetTrigger(_fadeHash);
        yield return new WaitForSeconds(0.1f);
        OnGameStart();
        InkEffectMaterial.SetFloat("_InkBleedMag", _gameScreenEffectMag);

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float normalT = t * 2;
            PreGameGroup.alpha = 1 - normalT;
            yield return null;
        }

        PreGameGroup.alpha = 0;
    }

    public void CollectRing()
    {
        _ringsCollected++;
    }

    public void PostGameScreen()
    {
        StartCoroutine(PostGameAsync());
    }

    IEnumerator PostGameAsync()
    {
        CollectionText.text = string.Format("You collected <color=#ffff00ff>{0}</color> rings.", _ringsCollected.ToString());

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float normalT = t * 2;
            PostGameGroup.alpha = normalT;
            yield return null;
        }

        _postGame = true;
        PostGameGroup.alpha = 1;
    }
}