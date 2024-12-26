using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenView : MonoBehaviour
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _randomQuestionEmptyPlane;
    [SerializeField] private Button[] _addCollectionButtons;
    [SerializeField] private Button[] _addQuestionButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action SettingsClicked;
    public event Action AddCollectionClicked;
    public event Action AddQuestionClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);

        foreach (var button in _addCollectionButtons)
        {
            button.onClick.AddListener(OnAddCollectionClikced);
        }
        
        foreach (var button in _addQuestionButton)
        {
            button.onClick.AddListener(OnAddQuestionClicked);
        }
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        
        foreach (var button in _addCollectionButtons)
        {
            button.onClick.RemoveListener(OnAddCollectionClikced);
        }
        
        foreach (var button in _addQuestionButton)
        {
            button.onClick.RemoveListener(OnAddQuestionClicked);
        }
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void ToggleEmptyRandomQuestionPlane(bool status)
    {
        _randomQuestionEmptyPlane.gameObject.SetActive(status);
    }

    public void MakeTransparent()
    {
        _screenVisabilityHandler.SetTransperent();
    }
    
    private void OnSettingsButtonClicked() => SettingsClicked?.Invoke();
    private void OnAddQuestionClicked() => AddQuestionClicked?.Invoke();
    
    private void OnAddCollectionClikced() => AddCollectionClicked?.Invoke();
}