using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomQuestionPlane : MonoBehaviour
{
    private const string RandomQuestionKey = "RandomQuestion";

    [SerializeField] private TMP_Text _questionText;
    [SerializeField] private Button _answerButton;

    private DateTime _questionDate;
    private QuestionPlane _currentQuestionPlane;

    public event Action NewQuestionRequired;
    public event Action<QuestionPlane> AnswerButtonClicked;
    
    public bool HasData { get; private set; }

    private void OnEnable()
    {
        _answerButton.onClick.AddListener(OnAnswerClicked);
        ValidateDate();
    }

    private void OnDisable()
    {
        _answerButton.onClick.RemoveListener(OnAnswerClicked);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void SetQuestion(DataPlane plane)
    {
        _currentQuestionPlane = (QuestionPlane)plane;
        _questionText.text = _currentQuestionPlane.QuestionData.Question;
        _questionDate = DateTime.Now;
        HasData = true;
        PlayerPrefs.SetString(RandomQuestionKey, _questionDate.ToString("yyyy-MM-dd"));
    }

    public void UpdateValues()
    {
        _questionText.text = _currentQuestionPlane.QuestionData.Question;
    }

    private void ValidateDate()
    {
        if (PlayerPrefs.HasKey(RandomQuestionKey))
        {
            string savedDate = PlayerPrefs.GetString(RandomQuestionKey);
            DateTime currentDate = DateTime.Now;

            DateTime.TryParse(savedDate, out _questionDate);

            if (currentDate.Date > _questionDate.Date)
            {
                NewQuestionRequired?.Invoke();
                HasData = false;
            }
        }
    }

    private void OnAnswerClicked() => AnswerButtonClicked?.Invoke(_currentQuestionPlane);
}