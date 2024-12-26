using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTag : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor;
    [SerializeField] private Color _selectedColorText;
    [SerializeField] private Color _unselectedColorText;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _tagText;
    [SerializeField] private Image _image;

    public string Tag { get; private set; }
    public bool IsActive { get; private set; }

    public event Action<QuestionTag> ButtonClicked;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    public void Enable(string tag)
    {
        gameObject.SetActive(true);
        Tag = tag;
        _tagText.text = Tag;
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    public void SetSelected()
    {
        _image.color = _selectedColor;
        _tagText.color = _selectedColorText;
    }

    public void SetDefault()
    {
        _image.color = _unselectedColor;
        _tagText.color = _unselectedColorText;
    }

    private void OnButtonClicked() => ButtonClicked?.Invoke(this);
}