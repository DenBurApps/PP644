using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestionFillingScreen : MonoBehaviour
{
    [SerializeField] private QuestionFillingScreenView _view;
    [SerializeField] private List<QuestionTag> _questionTags;

    private List<string> _inputedTags = new List<string>();
    private QuestionTag _chosenTag;
    private string _question;
    private string _tag;

    private QuestionPlane _questionPlane;

    public event Action<QuestionData> Saved;

    public event Action<QuestionPlane> NewDataSaved;
    public event Action BackButtonClicked;

    private void OnEnable()
    {
        _view.QuestionInputed += OnQuestionInputed;
        _view.CategoryInputed += CategoryInputed;
        _view.SaveClicked += SaveData;
        _view.BackClicked += OnBackButtonClicked;
        
        foreach (var tag in _questionTags)
        {
            tag.ButtonClicked += OnTagChosen;
        }
    }

    private void OnDisable()
    {
        _view.QuestionInputed -= OnQuestionInputed;
        _view.CategoryInputed -= CategoryInputed;
        _view.SaveClicked -= SaveData;
        _view.BackClicked -= OnBackButtonClicked;

        foreach (var tag in _questionTags)
        {
            tag.ButtonClicked -= OnTagChosen;
        }
    }

    private void Start()
    {
        _view.Disable();
        DiactivateAllTags();
        SetDefault();
    }

    public void OpenScreen(QuestionPlane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        _questionPlane = plane;
        
        if (CategoryHolder.Tags.Count > 0)
        {
            _inputedTags = CategoryHolder.Tags;
            FillCategorys();
        }
        
        _view.SetQuestinText(_questionPlane.QuestionData.Question);
        _question = _questionPlane.QuestionData.Question;
        ChooseTag(_questionPlane.QuestionData.Tag);
        _view.Enable();
        ValidateInput();
    }

    public void EnableScreen()
    {
        _view.Enable();
        ValidateInput();

        if (CategoryHolder.Tags.Count > 0)
        {
            _inputedTags = CategoryHolder.Tags;
            FillCategorys();
        }
    }

    private void SetDefault()
    {
        _question = string.Empty;
        _tag = string.Empty;
        _view.SetCategoryText(_tag);
        _view.SetQuestinText(_question);
    }

    private void OnQuestionInputed(string text)
    {
        _question = text;
        ValidateInput();
    }

    private void FillCategorys()
    {
        int tagCount = Mathf.Min(_inputedTags.Count, _questionTags.Count); 

        for (int i = 0; i < tagCount; i++)
        {
            if (_questionTags[i].IsActive && _questionTags[i].Tag == _inputedTags[i])
                continue;

            if (!_questionTags[i].IsActive && _questionTags[i].Tag != _inputedTags[i])
            {
                _questionTags[i].Enable(_inputedTags[i]);
            }
        }
    }

    private void ChooseTag(string category)
    {
        foreach (var tag in _questionTags)
        {
            if (tag.IsActive && tag.Tag == category)
            {
                OnTagChosen(tag);
                return;
            }
        }
    }

    private void CategoryInputed(string input)
    {
        _tag = input;
        var tagToEnable = _questionTags.FirstOrDefault(t => !t.IsActive);

        if (tagToEnable != null && !_inputedTags.Contains(_tag) && !string.IsNullOrEmpty(_tag))
        {
            _inputedTags.Add(_tag);
            CategoryHolder.Tags.Add(_tag);
            CategoryHolder.SaveTags();
            tagToEnable.Enable(_tag);
        }

        ValidateInput();
    }

    private void OnTagChosen(QuestionTag tag)
    {
        if (_chosenTag != null)
        {
            _chosenTag.SetDefault();
        }

        _chosenTag = tag;
        _chosenTag.SetSelected();
        ValidateInput();
    }

    private void SaveData()
    {
        if (_chosenTag == null)
            return;

        QuestionData data = new QuestionData(_chosenTag.Tag, _question);

        if (_questionPlane == null)
        {
            Saved?.Invoke(data);
            OnBackButtonClicked();
        }
        else
        {
            _questionPlane.SetData(data);
            NewDataSaved?.Invoke(_questionPlane);
            _view.Disable();
        }
    }

    private void DiactivateAllTags()
    {
        foreach (var tag in _questionTags)
        {
            tag.SetDefault();
            tag.Disable();
        }
    }

    private void OnBackButtonClicked()
    {
        SetDefault();
        _view.Disable();
        BackButtonClicked?.Invoke();
    }

    private void ValidateInput()
    {
        bool isValid = !string.IsNullOrEmpty(_question) && _chosenTag != null;

        _view.ToggleSaveButton(isValid);
    }
}