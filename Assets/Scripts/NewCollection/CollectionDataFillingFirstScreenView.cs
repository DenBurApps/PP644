using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CollectionDataFillingFirstScreenView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nextButtonText;
    [SerializeField] private Color _selectedButtonColor;
    [SerializeField] private Color _unselectedButtonColor;
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private Color _unselectedTextColor;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_InputField _titleInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<string> TitleInputed;
    public event Action<string> DescriptionInputed;
    public event Action NextButtonClicked;
    public event Action BackButtonClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _nextButton.onClick.AddListener(OnNextButtonClicked);
        _titleInput.onValueChanged.AddListener(OnTitleInputed);
        _descriptionInput.onValueChanged.AddListener(OnDescriptitonInputed);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _nextButton.onClick.RemoveListener(OnNextButtonClicked);
        _titleInput.onValueChanged.RemoveListener(OnTitleInputed);
        _descriptionInput.onValueChanged.RemoveListener(OnDescriptitonInputed);
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void ToggleNextButton(bool status)
    {
        _nextButton.enabled = status;
        
        _nextButtonText.color = status ? _selectedTextColor : _unselectedTextColor;
        _nextButton.image.color = status ? _selectedButtonColor : _unselectedButtonColor;
    }

    public void SetTitle(string text)
    {
        _titleInput.text = text;
    }

    public void SetDescription(string text)
    {
        _descriptionInput.text = text;
    }

    private void OnTitleInputed(string text) => TitleInputed?.Invoke(text);
    private void OnDescriptitonInputed(string text) => DescriptionInputed?.Invoke(text);
    private void OnNextButtonClicked() => NextButtonClicked?.Invoke();
    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
}
