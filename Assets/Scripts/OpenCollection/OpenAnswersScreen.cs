using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenAnswersScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private List<OpenAnswersPlane> _openAnswersPlanes;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private OpenCollectionScreen _collectionScreen;

    private List<int> _availableIndexes = new List<int>();

    private CollectionAnswerPlane _currentPlane;
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<CollectionAnswerPlane> DeleteClicked;
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _collectionScreen.OpenAnswersClicked += EnableScreen;
        _deleteButton.onClick.AddListener(OnDeleteClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDisable()
    {
        _collectionScreen.OpenAnswersClicked -= EnableScreen;
        _deleteButton.onClick.RemoveListener(OnDeleteClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void EnableScreen(CollectionAnswerPlane plane)
    {
        DisableAllWindows();
        _screenVisabilityHandler.EnableScreen();

        if (plane == null)
            return;

        _currentPlane = plane;
        _dateText.text = _currentPlane.CollectionAnswers.Date;

        var questions = _collectionScreen.Plane.CollectionData.Questions;
        
        for (int i = 0; i < _currentPlane.CollectionAnswers.Answers.Count; i++)
        {
            EnableWindows(questions[i].Question,
                _currentPlane.CollectionAnswers.Answers[i]);
        }
    }

    private void EnableWindows(string question, string answer)
    {
        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _openAnswersPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable(question, answer);
            }
        }
    }

    private void DisableAllWindows()
    {
        for (int i = 0; i < _openAnswersPlanes.Count; i++)
        {
            _openAnswersPlanes[i].Disable();
            _availableIndexes.Add(i);
        }
    }

    private void OnBackButtonClicked()
    {
        _screenVisabilityHandler.DisableScreen();
        DisableAllWindows();
        BackClicked?.Invoke();
    }

    private void OnDeleteClicked()
    {
        DeleteClicked?.Invoke(_currentPlane);
        _screenVisabilityHandler.DisableScreen();
        DisableAllWindows();
    }
}