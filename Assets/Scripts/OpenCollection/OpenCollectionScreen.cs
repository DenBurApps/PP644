using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class OpenCollectionScreen : MonoBehaviour
{
    [SerializeField] private OpenCollectionScreenView _view;
    [SerializeField] private List<CollectionAnswerPlane> _answerPlanes;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AnswerCollection _answersScreen;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private CollectionIconProvider _collectionIconProvider;
    [SerializeField] private OpenAnswersScreen _openAnswersScreen;
    [SerializeField] private EditCollectionFirstScreen _editCollectionFirstScreen;
    [SerializeField] private EditCollectionSecondScreen _editCollectionSecondScreen;
    
    private List<int> _availableIndexes = new List<int>();

    private CollectionPlane _collectionPlane;

    public event Action BackButtonClicked;
    public event Action<CollectionAnswerPlane> OpenAnswersClicked;
    public event Action<CollectionPlane> EditButtonClicked;
    public event Action Edited;

    public CollectionPlane Plane => _collectionPlane;
    
    private void OnEnable()
    {
        _view.AnswerButtonClicked += OnAnswerButtonClicked;
        _view.BackButtonClicked += OnBackButtonClicked;
        _view.EditButtonClicked += OnEditButtonClicked;

        _mainScreen.CollectionOpened += OpenScreen;
        _answersScreen.AnswerClicked += EnableDataPlane;

        _openAnswersScreen.DeleteClicked += DeleteAnswer;
        _openAnswersScreen.BackClicked += _view.Enable;

        _editCollectionFirstScreen.BackButtonClicked += () => OpenScreen(_collectionPlane);
        _editCollectionSecondScreen.Saved += () => OpenScreen(_collectionPlane);
    }

    private void OnDisable()
    {
        _view.AnswerButtonClicked -= OnAnswerButtonClicked;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _view.EditButtonClicked -= OnEditButtonClicked;
        
        _mainScreen.CollectionOpened -= OpenScreen;
        _answersScreen.AnswerClicked -= EnableDataPlane;
        
        _openAnswersScreen.DeleteClicked -= DeleteAnswer;
        _openAnswersScreen.BackClicked -= _view.Enable;
        
        _editCollectionFirstScreen.BackButtonClicked -= () => OpenScreen(_collectionPlane);
        _editCollectionSecondScreen.Saved -= () => OpenScreen(_collectionPlane);

        foreach (var plane in _answerPlanes)
        {
            plane.AnswersDeleted -= DeleteAnswer;
            plane.AnswerOpened -= OnInputedAnswerClicked;
        }
    }
    
    private void Start()
    {
        _view.Disable();
        DisableAllWindows();
        _answersScreen.Disable();
    }
    
    private void OpenScreen(CollectionPlane collectionPlane)
    {
        _view.Enable();
        
        if (collectionPlane == null)
            throw new ArgumentNullException(nameof(collectionPlane));
        
        _collectionPlane = collectionPlane;
        
        _view.SetQuestion(_collectionPlane.CollectionData.Title);
        _view.SetDescription(_collectionPlane.CollectionData.Description);
        _view.SetLogo(_collectionIconProvider.GetIconSprite(_collectionPlane.CollectionData.Type));

        if (_collectionPlane.CollectionData.CollectionAnswers.Count > 0)
        {
            foreach (var answer in _collectionPlane.CollectionData.CollectionAnswers)
            {
                EnableDataPlane(answer.Answers, answer.Date);
            }
        }
        
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);
        Edited?.Invoke();
    }
    
    public void EnableDataPlane(List<string> answers)
    {
        _view.Enable();
        
        if (answers == null)
            return;
        
        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _answerPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable(answers);
                currentFilledItemPlane.AnswerOpened += OnInputedAnswerClicked;
                _collectionPlane.AddAnswer(answers);
                currentFilledItemPlane.AnswersDeleted += DeleteAnswer;
            }
        }

        Edited?.Invoke();
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);

    }
    
    private void DeletePlane(CollectionAnswerPlane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        int index = _answerPlanes.IndexOf(plane);

        if (index >= 0 && !_availableIndexes.Contains(index))
        {
            _availableIndexes.Add(index);
        }

        plane.AnswersDeleted -= DeleteAnswer;
        plane.AnswerOpened -= OnInputedAnswerClicked;
        plane.Disable();

        if (_answerPlanes.Contains(plane))
        {
            _answerPlanes.Remove(plane);
        }
        
        
        _collectionPlane.DeleteAnswer(plane.CollectionAnswers);
        Edited?.Invoke();
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);
    }
    
    public void EnableDataPlane(List<string> answers, string date)
    {
        if (answers == null && string.IsNullOrEmpty(date))
            return;
        
        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _answerPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable(answers, date);
                currentFilledItemPlane.AnswerOpened += OnInputedAnswerClicked;
                currentFilledItemPlane.AnswersDeleted += DeleteAnswer;
            }
        }
        
        Edited?.Invoke();
        ToggleEmptyPlane(_availableIndexes.Count >= _answerPlanes.Count);
    }
    
    private void DisableAllWindows()
    {
        _availableIndexes.Clear();
        
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
    
    private void DeleteAnswer(CollectionAnswerPlane answerPlane)
    {
        _view.Enable();
        
        if (answerPlane == null)
            throw new ArgumentNullException(nameof(answerPlane));
        
        DeletePlane(answerPlane);
        Edited?.Invoke();
    }
    
    private void OnBackButtonClicked()
    {
        _view.Disable();
        _view.SetDescription(string.Empty);
        _view.SetQuestion(string.Empty);
        _collectionPlane = null;
        BackButtonClicked?.Invoke();
    }

    private void OnEditButtonClicked()
    {
        _view.Disable();
        EditButtonClicked?.Invoke(_collectionPlane);
    }

    private void OnAnswerButtonClicked()
    {
        _view.MakeTransparent();
        _answersScreen.Enable(_collectionPlane);
    }

    private void OnInputedAnswerClicked(CollectionAnswerPlane answerPlane)
    {
        _view.Disable();
        OpenAnswersClicked?.Invoke(answerPlane);
    }
}
