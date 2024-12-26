using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private TMP_InputField _answerInput;
    [SerializeField] private Button _answerButton;
    [SerializeField] private Button _closeButton;

    private string _answer;

    public event Action<string> AnswerClicked;
    public event Action CloseButtonClicked;

    public QuestionPlane CurrentPlane { get; private set; }

    private void OnEnable()
    {
        _answerInput.onValueChanged.AddListener(OnAnswerInputed);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _answerButton.onClick.AddListener(OnAnswerClicked);
    }

    private void OnDisable()
    {
        _answerInput.onValueChanged.RemoveListener(OnAnswerInputed);
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        _answerButton.onClick.RemoveListener(OnAnswerClicked);
    }

    public void Enable(QuestionPlane questionPlane)
    {
        gameObject.SetActive(true);
        CurrentPlane = questionPlane;
        _questionText.text = questionPlane.QuestionData.Question;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        CurrentPlane = null;
    }

    private void OnAnswerInputed(string text)
    {
        _answer = text;
    }

    private void OnAnswerClicked()
    {
        AnswerClicked?.Invoke(_answer);
        OnCloseButtonClicked();
    }

    private void OnCloseButtonClicked()
    {
        CloseButtonClicked?.Invoke();
        _answer = string.Empty;
        _answerInput.text = string.Empty;
        Disable();
    }
}