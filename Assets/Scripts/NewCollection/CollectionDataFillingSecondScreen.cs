using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionDataFillingSecondScreen : MonoBehaviour
{
    [SerializeField] private CollectionDataFillingSecondScreenView _view;
    [SerializeField] private QuestionsTagScroll _questionsTagScroll;
    [SerializeField] private List<NewCollectionQuestion> _newCollectionQuestions;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private CollectionDataFillingFirstScreen _firstScreen;
    [SerializeField] private QuestionFillingScreen _questionFillingScreen;

    private List<int> _availableIndexes = new List<int>();
    private List<NewCollectionQuestion> _selectedQuestions = new List<NewCollectionQuestion>();

    public event Action BackButtonClicked;
    public event Action<CollectionData> Saved;
    public event Action AddQuestionClicked;

    private void OnEnable()
    {
        _view.SaveButtonClicked += OnSaved;
        _view.BackButtonClicked += OnBackButtonClicked;
        _firstScreen.NextButtonClicked += OpenScreen;
        _view.AddQuestionClicked += OnAddQuestionClicked;
        _questionsTagScroll.TagClicked += FilterQuestions;
    }

    private void OnDisable()
    {
        _view.SaveButtonClicked -= OnSaved;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _firstScreen.NextButtonClicked -= OpenScreen;
        _view.AddQuestionClicked -= OnAddQuestionClicked;
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

    private void OpenScreen()
    {
        DisableAllWindows();
        Validate();
        

        _view.Enable();

        foreach (var plane in _mainScreen.DataPlanes)
        {
            if (plane.IsActive && plane.QuestionData != null)
            {
                EnableDataPlane(plane.QuestionData);
            }
        }
        
        _view.ToggleEmptyPlane(_availableIndexes.Count >= _newCollectionQuestions.Count);

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
                currentFilledItemPlane.SetSelectedStatus(false);
                currentFilledItemPlane.Selected += ProcessPlaneSelected;
                currentFilledItemPlane.Unselected += ProcessPlaneUnselected;
            }
        }

        _view.ToggleEmptyPlane(false);
    }

    private void DisableAllWindows()
    {
        _availableIndexes.Clear();
        
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

        Saved?.Invoke(dataToSave);
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

    private void OnAddQuestionClicked()
    {
        AddQuestionClicked?.Invoke();
        DisableAllWindows();
        _selectedQuestions.Clear();
        _questionFillingScreen.EnableScreen();
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