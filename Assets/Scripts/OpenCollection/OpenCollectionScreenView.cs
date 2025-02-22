using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenCollectionScreenView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _answerButton;
    [SerializeField] private Image _logo;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action BackButtonClicked;
    public event Action EditButtonClicked;
    public event Action AnswerButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }
    
    private void OnEnable()
    {
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _editButton.onClick.AddListener(OnEditButtonClicked);
        _answerButton.onClick.AddListener(OnAnswerButtonClicked);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _editButton.onClick.RemoveListener(OnEditButtonClicked);
        _answerButton.onClick.RemoveListener(OnAnswerButtonClicked);
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
    
    public void SetQuestion(string text)
    {
        _title.text = text;
    }

    public void SetDescription(string text)
    {
        _description.text = text;
    }

    public void SetLogo(Sprite sprite)
    {
        _logo.sprite = sprite;
    }

    public void MakeTransparent()
    {
        _screenVisabilityHandler.SetTransperent();
    }
    
    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
    private void OnEditButtonClicked() => EditButtonClicked?.Invoke();
    private void OnAnswerButtonClicked() => AnswerButtonClicked?.Invoke();
}
