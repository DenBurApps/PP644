using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditCollectionFirstScreen : MonoBehaviour
{
    [SerializeField] private EditCollectionFirstScreenView _view;
    [SerializeField] private CollectionIconProvider _iconProvider;
    [SerializeField] private NewCollectionIcon[] _icons;
    [SerializeField] private OpenCollectionScreen _collectionScreen;
    [SerializeField] private EditCollectionSecondScreen _secondScreen;

    private NewCollectionIcon _currentIcon;
    private string _title;
    private string _description;
    private CollectionPlane _collectionPlane;
    
    
    public event Action BackButtonClicked;
    public event Action<CollectionPlane> NextButtonClicked;

    public NewCollectionIcon CurrentIcon => _currentIcon;

    public string Title => _title;

    public string Description => _description;

    private void OnEnable()
    {
        _view.BackButtonClicked += OnBackButtonClicked;
        _view.NextButtonClicked += OnNextButtonClicked;

        _view.TitleInputed += OnTitleInputed;
        _view.DescriptionInputed += OnDescriptionInputed;

        _collectionScreen.EditButtonClicked += EnableScreen;
        _secondScreen.BackButtonClicked += _view.Enable;
    }

    private void OnDisable()
    {
        _view.BackButtonClicked -= OnBackButtonClicked;
        _view.NextButtonClicked -= OnNextButtonClicked;
        
        _view.TitleInputed -= OnTitleInputed;
        _view.DescriptionInputed -= OnDescriptionInputed;
        
        _collectionScreen.EditButtonClicked -= EnableScreen;
        
        _secondScreen.BackButtonClicked -= _view.Enable;
    }

    private void Start()
    {
        _view.Disable();

        foreach (var icon in _icons)
        {
            icon.Image.sprite = _iconProvider.GetUnselectedSprite(icon.Type);
            icon.ButtonClicked += OnIconClicked;
        }

        _currentIcon = null;
        ValidateInput();
    }

    private void EnableScreen(CollectionPlane collectionPlane)
    {
        _view.Enable();
        ReturnToDefault();

        _collectionPlane = collectionPlane;
        _view.SetDescription(_collectionPlane.CollectionData.Description);
        _view.SetTitle(_collectionPlane.CollectionData.Title);

        var icon = _icons.FirstOrDefault(i => i.Type == _collectionPlane.CollectionData.Type);

        if (icon != null)
        {
            OnIconClicked(icon);
        }
        
        ValidateInput();
    }

    private void OnTitleInputed(string text)
    {
        _title = text;
        ValidateInput();
    }

    private void OnDescriptionInputed(string text)
    {
        _description = text;
        ValidateInput();
    }

    private void OnIconClicked(NewCollectionIcon icon)
    {
        if (_currentIcon != null)
        {
            _currentIcon.Image.sprite = _iconProvider.GetUnselectedSprite(_currentIcon.Type);
        }

        _currentIcon = icon;
        _currentIcon.Image.sprite = _iconProvider.GetIconSprite(_currentIcon.Type);
        ValidateInput();
    }

    private void ValidateInput()
    {
        bool isValid = !string.IsNullOrEmpty(_title) && !string.IsNullOrEmpty(_description) && _currentIcon != null;
        
        _view.ToggleNextButton(isValid);
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        _view.Disable();
        ReturnToDefault();
    }

    private void OnNextButtonClicked()
    {
        NextButtonClicked?.Invoke(_collectionPlane);
        _view.Disable();
    }

    private void ReturnToDefault()
    {
        _title = string.Empty;
        _description = string.Empty;
        _currentIcon = null;
        
        foreach (var icon in _icons)
        {
            icon.Image.sprite = _iconProvider.GetUnselectedSprite(icon.Type);
        }
    }
}
