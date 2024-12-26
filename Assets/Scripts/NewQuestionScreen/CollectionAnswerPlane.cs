using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionAnswerPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _button;

    private List<string> _answers;
    private string _date;
    
    public CollectionAnswers CollectionAnswers { get; private set; }
    
    public event Action<CollectionAnswerPlane> AnswerOpened;
    public event Action<CollectionAnswers> AnswersInputed;
    public event Action<CollectionAnswerPlane> AnswersDeleted;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnOpened);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnOpened);
    }
    
    public void Enable(List<string> answers)
    {
        gameObject.SetActive(true);
        _answers = new List<string>(answers);
        _date = DateTime.Now.ToString("dd.MM.yyyy");
        _dateText.text = _date;
        
        CollectionAnswers = new CollectionAnswers(_answers, _date);
        AnswersInputed?.Invoke(CollectionAnswers);
    }
    
    public void Enable(List<string> answers, string date)
    {
        gameObject.SetActive(true);
        _answers = answers;

        _date = date;
        _dateText.text = _date;
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void OnDeleted() => AnswersDeleted?.Invoke(this);
    private void OnOpened() => AnswerOpened?.Invoke(this);
}
