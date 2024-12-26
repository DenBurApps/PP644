using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPlane : DataPlane
{
    [SerializeField] private TMP_Text _tagText;
    [SerializeField] private TMP_Text _questionText;
    
    public QuestionData QuestionData { get; private set; }

    public override void SetData(IData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data)); 
        
        QuestionData = (QuestionData)data;
        _tagText.text = QuestionData.Tag;
        _questionText.text = QuestionData.Question;
    }

    public void DeleteAnswer(AnswerData answerData)
    {
        if (QuestionData.Answers.Contains(answerData))
        {
            QuestionData.Answers.Remove(answerData);
        }
    }

    public void AddAnswer(string answer)
    {
        if(string.IsNullOrEmpty(answer) && QuestionData == null)
            return;
        
        if (QuestionData != null)
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            var answerData = new AnswerData(answer, date);
            QuestionData.Answers.Add(answerData);
        }
    }

    protected override void ReturnToDefault()
    {
        _tagText.text = string.Empty;
        _questionText.text = string.Empty;
        QuestionData = null;
    }
}

[Serializable]
public class QuestionData : IData
{
    public string Tag;
    public string Question;
    public List<AnswerData> Answers;

    public QuestionData(string tag, string question)
    {
        Tag = tag;
        Question = question;
        Answers = new List<AnswerData>();
    }
}

[Serializable]
public class AnswerData
{
    public string Answer;
    public string Date;

    public AnswerData(string answer, string date)
    {
        Answer = answer;
        Date = date;
    }
}
