using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerCollection : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_InputField _answerInput;
    [SerializeField] private Button _answerButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;

    private List<string> _answers;
    private int _currentQuestionIndex;

    public event Action<List<string>> AnswerClicked;

    public CollectionPlane CurrentPlane { get; private set; }

    private void OnEnable()
    {
        _answerInput.onValueChanged.AddListener(OnAnswerInputed);
        _nextButton.onClick.AddListener(OnNextButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _answerButton.onClick.AddListener(OnAnswerButtonClicked);
    }

    private void OnDisable()
    {
        _answerInput.onValueChanged.RemoveListener(OnAnswerInputed);
        _nextButton.onClick.RemoveListener(OnNextButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        _answerButton.onClick.RemoveListener(OnAnswerButtonClicked);
    }
    
    public void Enable(CollectionPlane collectionPlane)
    {
        gameObject.SetActive(true);
        
        CurrentPlane = collectionPlane;

        if (CurrentPlane?.CollectionData?.Questions == null || CurrentPlane.CollectionData.Questions.Length == 0)
        {
            Debug.LogWarning("No questions available.");
            return;
        }
        
        _answers = new List<string>();
        _answers.Clear();

        for (int i = 0; i < CurrentPlane.CollectionData.Questions.Length; i++)
        {
            _answers.Add(string.Empty);
        }
        
        _currentQuestionIndex = 0;

        _answerButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(true);
        _nextButton.interactable = false;

        _questionText.text = CurrentPlane.CollectionData.Questions[_currentQuestionIndex].Question;
       
       
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        CurrentPlane = null;
    }

    private void OnAnswerInputed(string text)
    {
        if (_currentQuestionIndex >= 0 && _currentQuestionIndex < CurrentPlane.CollectionData.Questions.Length)
        {
            _answers[_currentQuestionIndex] = text;
            _nextButton.interactable = _nextButton.IsActive();
            _answerButton.interactable = _answerButton.IsActive();
        }
    }

    private void OnNextButtonClicked()
    {
        _currentQuestionIndex++;
        if (_currentQuestionIndex < CurrentPlane.CollectionData.Questions.Length)
        {
            _questionText.text = CurrentPlane.CollectionData.Questions[_currentQuestionIndex].Question;
            _nextButton.interactable = false;
            _backButton.gameObject.SetActive(true);
            _answerInput.text = string.Empty;

            if (_currentQuestionIndex == CurrentPlane.CollectionData.Questions.Length - 1)
            {
                _nextButton.gameObject.SetActive(false);
                _answerButton.gameObject.SetActive(true);
                _answerButton.interactable = false;
            }
        }
    }

    private void OnBackButtonClicked()
    {
        if (_currentQuestionIndex > 0)
        {
            _currentQuestionIndex--;
            _questionText.text = CurrentPlane.CollectionData.Questions[_currentQuestionIndex].Question;
            _answerInput.text = _answers[_currentQuestionIndex];
            _answerButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(true);
            _nextButton.interactable = !string.IsNullOrEmpty(_answers[_currentQuestionIndex]);
            _backButton.gameObject.SetActive(_currentQuestionIndex > 0);
        }
    }

    private void OnCloseButtonClicked()
    {
        _answerInput.text = string.Empty;
        Disable();
    }

    private void OnAnswerButtonClicked()
    {
        AnswerClicked?.Invoke(_answers);
        OnCloseButtonClicked();
    }
}