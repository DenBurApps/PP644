using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CollectionDataFillingSecondScreenView : MonoBehaviour
{
    [SerializeField] private TMP_Text _saveButtonText;
    [SerializeField] private Color _selectedButtonColor;
    [SerializeField] private Color _unselectedButtonColor;
    [SerializeField] private Color _selectedTextColor;
    [SerializeField] private Color _unselectedTextColor;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private Button _addQuestionButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackButtonClicked;
    public event Action SaveButtonClicked;
    public event Action AddQuestionClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _saveButton.onClick.AddListener(OnSaveButtonClicked);
        _addQuestionButton.onClick.AddListener(OnAddQuestionClicked);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        _addQuestionButton.onClick.RemoveListener(OnAddQuestionClicked);
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void ToggleEmptyPlane(bool status)
    {
        _emptyPlane.gameObject.SetActive(status);
    }

    public void ToggleSaveButton(bool status)
    {
        _saveButton.enabled = status;
        
        _saveButtonText.color = status ? _selectedTextColor : _unselectedTextColor;
        _saveButton.image.color = status ? _selectedButtonColor : _unselectedButtonColor;
    }

    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
    private void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();
    private void OnAddQuestionClicked() => AddQuestionClicked?.Invoke();
}
