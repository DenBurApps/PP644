using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class QuestionFillingScreenView : MonoBehaviour
{
    [SerializeField] private TMP_Text _saveButtonText;
    [SerializeField] private Color _selectedButtonColor;
    [SerializeField] private Color _unselectedButtonColor;
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private Color _unselectedTextColor;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _questionInput;
    [SerializeField] private TMP_InputField _categoryInput;
    [SerializeField] private Button _saveButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    
    public event Action SaveClicked;
    public event Action<string> QuestionInputed;
    public event Action<string> CategoryInputed;
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _saveButton.onClick.AddListener(OnSaveClicked);
        _questionInput.onValueChanged.AddListener(OnQuestionInputed);
        _categoryInput.onEndEdit.AddListener(OnCategoryInputed);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _saveButton.onClick.RemoveListener(OnSaveClicked);
        _questionInput.onValueChanged.RemoveListener(OnQuestionInputed);
        _categoryInput.onEndEdit.RemoveListener(OnCategoryInputed);
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void SetQuestinText(string text)
    {
        _questionInput.text = text;
    }

    public void SetCategoryText(string text)
    {
        _categoryInput.text = text;
    }

    public void ToggleSaveButton(bool status)
    {
        _saveButton.enabled = status;

        _saveButtonText.color = status ? _selectedTextColor : _unselectedTextColor;
        _saveButton.image.color = status ? _selectedButtonColor : _unselectedButtonColor;
    }

    private void OnBackButtonClicked() => BackClicked?.Invoke();
    private void OnSaveClicked() => SaveClicked?.Invoke();
    private void OnQuestionInputed(string text) => QuestionInputed?.Invoke(text);
    private void OnCategoryInputed(string text) => CategoryInputed?.Invoke(text);
}
