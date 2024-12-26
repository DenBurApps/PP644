using System;
using UnityEngine;
using UnityEngine.UI;

public class NewCollectionQuestion : MonoBehaviour
{
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private QuestionPlane _plane;
    [SerializeField] private Button _selectButton;

    public event Action<NewCollectionQuestion> Selected;
    public event Action<NewCollectionQuestion> Unselected;

    public bool IsActive { get; private set; }
    public bool IsSelected { get; private set; }
    public QuestionData Data => _plane.QuestionData;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(OnSelectedButtonClicked);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(OnSelectedButtonClicked);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
        SetSelectedStatus(false);
    }

    public void SetSelectedStatus(bool status)
    {
        IsSelected = status;

        if (IsSelected)
        {
            _selectButton.image.sprite = _selectedSprite;
            IsSelected = true;
        }
        else
        {
            _selectButton.image.sprite = _unselectedSprite;
            IsSelected = false;
        }
    }

    public void SetQuestionData(QuestionData data)
    {
        _plane.SetData(data);
    }

    private void OnSelectedButtonClicked()
    {
        if (IsSelected)
        {
            _selectButton.image.sprite = _unselectedSprite;
            IsSelected = false;
            Unselected?.Invoke(this);
        }
        else
        {
            _selectButton.image.sprite = _selectedSprite;
            IsSelected = true;
            Selected?.Invoke(this);
        }
    }
}