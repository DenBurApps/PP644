using System;
using System.Collections.Generic;
using UnityEngine;

public class OpenQuestion : MonoBehaviour
{
    [SerializeField] private OpenQuestionView _view;
    [SerializeField] private List<AnswerPlane> _answerPlanes;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private Answer _answer;
    [SerializeField] private QuestionFillingScreen _editQuestionScreen;
    
    private List<int> _availableIndexes = new List<int>();

    private QuestionPlane _currentQuestion;
    
    public event Action BackButtonClicked;
    public event Action Edited;

    private void OnEnable()
    {
        _view.AnswerButtonClicked += OnAnswerButtonClicked;
        _view.BackButtonClicked += OnBackButtonClicked;
        _view.EditButtonClicked += OnEditButtonClicked;

        _mainScreen.QuestionOpened += OpenScreen;
        _answer.AnswerClicked += EnableDataPlane;
        _answer.CloseButtonClicked += _view.Enable;

        _editQuestionScreen.NewDataSaved += OpenScreen;
        _editQuestionScreen.BackButtonClicked += _view.Enable;
    }

    private void OnDisable()
    {
        _view.AnswerButtonClicked -= OnAnswerButtonClicked;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _view.EditButtonClicked -= OnEditButtonClicked;
        
        _mainScreen.QuestionOpened -= OpenScreen;
        _answer.AnswerClicked -= EnableDataPlane;
        _answer.CloseButtonClicked -= _view.Enable;
        
        _editQuestionScreen.NewDataSaved -= OpenScreen;
        _editQuestionScreen.BackButtonClicked -= _view.Enable;
    }

    private void Start()
    {
        _view.Disable();
        DisableAllWindows();
        _answer.Disable();
    }

    private void OpenScreen(QuestionPlane questionPlane)
    {
        _view.Enable();
        
        if (questionPlane == null)
            throw new ArgumentNullException(nameof(questionPlane));
        
        _currentQuestion = questionPlane;
        
        _view.SetCategory(_currentQuestion.QuestionData.Tag);
        _view.SetQuestion(_currentQuestion.QuestionData.Question);

        if (_currentQuestion.QuestionData.Answers.Count > 0)
        {
            foreach (var answer in _currentQuestion.QuestionData.Answers)
            {
                EnableDataPlane(answer.Answer, answer.Date);
            }
        }
        
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);
    }
    
    public void EnableDataPlane(string answer)
    {
        _view.Enable();
        
        if (string.IsNullOrEmpty(answer))
            return;
        
        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _answerPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable(answer);
                _currentQuestion.AddAnswer(answer);
                currentFilledItemPlane.AnswerDeleted += DeleteAnswer;
            }
        }

        Edited?.Invoke();
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);

    }

    private void DeletePlane(AnswerPlane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        int index = _answerPlanes.IndexOf(plane);

        if (index >= 0 && !_availableIndexes.Contains(index))
        {
            _availableIndexes.Add(index);
        }

        plane.AnswerDeleted -= DeleteAnswer;
        plane.Disable();

        if (_answerPlanes.Contains(plane))
        {
            _answerPlanes.Remove(plane);
        }
        
        _currentQuestion.DeleteAnswer(plane.AnswerData);

        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);

        Edited?.Invoke();
    }
    
    public void EnableDataPlane(string answer, string date)
    {
        if (string.IsNullOrEmpty(answer) && string.IsNullOrEmpty(date))
            return;
        
        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _answerPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable(answer, date);
            }
        }

        Edited?.Invoke();
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);

    }
    
    private void DisableAllWindows()
    {
        for (int i = 0; i < _answerPlanes.Count; i++)
        {
            _answerPlanes[i].Disable();
            _availableIndexes.Add(i);
        }
        
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);
    }
    
    private void ToggleEmptyPlane(bool status)
    {
        _emptyPlane.gameObject.SetActive(status);
    }

    private void DeleteAnswer(AnswerPlane answerPlane)
    {
        if (answerPlane == null)
            throw new ArgumentNullException(nameof(answerPlane));
        
        DeletePlane(answerPlane);
        Edited?.Invoke();
    }

    private void OnBackButtonClicked()
    {
        _view.Disable();
        _view.SetCategory(string.Empty);
        _view.SetQuestion(string.Empty);
        _currentQuestion = null;
        BackButtonClicked?.Invoke();
    }

    private void OnEditButtonClicked()
    {
        _view.Disable();
        _editQuestionScreen.OpenScreen(_currentQuestion);
    }

    private void OnAnswerButtonClicked()
    {
        _view.MakeTransparent();
        _answer.Enable(_currentQuestion);
    }
}
