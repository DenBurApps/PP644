using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _answerText;
    [SerializeField] private Button _deleteButton;

    private string _answer;
    private string _date;

    public event Action<AnswerData> AnswerInputed;
    public event Action<AnswerPlane> AnswerDeleted;
    
    public AnswerData AnswerData { get; private set; }

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteClicked);
    }

    public void Enable(string answer)
    {
        gameObject.SetActive(true);
        _answer = answer;
        _answerText.text = _answer;

        _date = DateTime.Now.ToString("dd.MM.yyyy");
        _dateText.text = _date;
        AnswerData = new AnswerData(_answer, _date);
        AnswerInputed?.Invoke(AnswerData);
    }
    
    public void Enable(string answer, string date)
    {
        gameObject.SetActive(true);
        _answer = answer;
        _answerText.text = _answer;

        _date = date;
        _dateText.text = _date;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDeleteClicked() => AnswerDeleted?.Invoke(this);
}
