using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCollectionSecondScreen : MonoBehaviour
{
    [SerializeField] private CollectionDataFillingSecondScreenView _view;
    [SerializeField] private QuestionsTagScroll _questionsTagScroll;
    [SerializeField] private List<NewCollectionQuestion> _newCollectionQuestions;
    [SerializeField] private EditCollectionFirstScreen _firstScreen;

    private List<int> _availableIndexes = new List<int>();
    private List<NewCollectionQuestion> _selectedQuestions = new List<NewCollectionQuestion>();

    private CollectionPlane _collectionPlane;
    
    public event Action BackButtonClicked;
    public event Action Saved;

    private void OnEnable()
    {
        _view.SaveButtonClicked += OnSaved;
        _view.BackButtonClicked += OnBackButtonClicked;
        _firstScreen.NextButtonClicked += OpenScreen;
        _questionsTagScroll.TagClicked += FilterQuestions;
    }

    private void OnDisable()
    {
        _view.SaveButtonClicked -= OnSaved;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _firstScreen.NextButtonClicked -= OpenScreen;
        _questionsTagScroll.TagClicked -= FilterQuestions;

        foreach (var plane in _newCollectionQuestions)
        {
            plane.Selected -= ProcessPlaneSelected;
            plane.Unselected -= ProcessPlaneUnselected;
        }
    }

    private void Start()
    {
        _view.Disable();
    }

    private void OpenScreen(CollectionPlane collectionPlane)
    {
        DisableAllWindows();
        _view.Enable();

        _collectionPlane = collectionPlane;

        foreach (var plane in collectionPlane.CollectionData.Questions)
        {
            if (plane != null)
            {
                EnableDataPlane(plane);
            }
        }

        _view.ToggleEmptyPlane(_availableIndexes.Count >= _newCollectionQuestions.Count);
        Validate();
        
        if (_availableIndexes.Count >= _newCollectionQuestions.Count)
        {
            _questionsTagScroll.Disable();
        }
        else
        {
            _questionsTagScroll.EnableTags();
        }
    }

    private void EnableDataPlane(QuestionData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _newCollectionQuestions[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable();
                currentFilledItemPlane.SetQuestionData(data);
                currentFilledItemPlane.SetSelectedStatus(true);
                currentFilledItemPlane.Selected += ProcessPlaneSelected;
                currentFilledItemPlane.Unselected += ProcessPlaneUnselected;

                if (data.Question == currentFilledItemPlane.Data.Question)
                {
                    ProcessPlaneSelected(currentFilledItemPlane);
                }
            }
        }

        _view.ToggleEmptyPlane(_availableIndexes.Count >= _newCollectionQuestions.Count);
    }

    private void DisableAllWindows()
    {
        for (int i = 0; i < _newCollectionQuestions.Count; i++)
        {
            _newCollectionQuestions[i].Disable();
            _availableIndexes.Add(i);
        }

        _view.ToggleEmptyPlane(_availableIndexes.Count >= _newCollectionQuestions.Count);
    }

    private void ProcessPlaneSelected(NewCollectionQuestion newCollectionQuestion)
    {
        if (!_selectedQuestions.Contains(newCollectionQuestion))
        {
            _selectedQuestions.Add(newCollectionQuestion);
        }

        Validate();
    }

    private void ProcessPlaneUnselected(NewCollectionQuestion newCollectionQuestion)
    {
        if (_selectedQuestions.Contains(newCollectionQuestion))
        {
            _selectedQuestions.Remove(newCollectionQuestion);
        }

        Validate();
    }

    private void Validate()
    {
        bool isValid = _selectedQuestions.Count >= 2;

        _view.ToggleSaveButton(isValid);
    }

    private void OnSaved()
    {
        List<QuestionData> questions = new List<QuestionData>();

        foreach (var selectedQuestion in _selectedQuestions)
        {
            if (selectedQuestion.Data != null)
                questions.Add(selectedQuestion.Data);
        }

        CollectionData dataToSave = new CollectionData(_firstScreen.CurrentIcon.Type, _firstScreen.Title,
            _firstScreen.Description, questions.ToArray());

        _collectionPlane.SetData(dataToSave);
        Saved?.Invoke();
        _selectedQuestions.Clear();
        _view.Disable();
        DisableAllWindows();
    }

    private void OnBackButtonClicked()
    {
        DisableAllWindows();
        _selectedQuestions.Clear();
        BackButtonClicked?.Invoke();
        _view.Disable();
    }
    
    private void FilterQuestions(string tag)
    {
        string filteredTag = tag.ToLower();

        foreach (var plane in _newCollectionQuestions)
        {
            if (filteredTag == "all")
            {
                if (plane.Data != null)
                {
                    plane.Enable();
                }
            }
            else
            {
                if (plane.Data != null && plane.Data.Tag.ToLower() == filteredTag)
                {
                    if (!plane.IsActive)
                    {
                        plane.Enable();
                    }
                }
                else
                {
                    if (plane.IsActive)
                    {
                        plane.Disable();
                    }
                }
            }
        }
    }
    
}
