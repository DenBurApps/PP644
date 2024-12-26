using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPlane : DataPlane
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _logo;
    [SerializeField] private TMP_Text _desription;

    private CollectionIconProvider _collectionIconProvider;

    public CollectionData CollectionData { get; private set; }

    public void SetIconProvider(CollectionIconProvider iconProvider)
    {
        _collectionIconProvider = iconProvider;
    }

    public void DeleteAnswer(CollectionAnswers answers)
    {
        if (CollectionData.CollectionAnswers.Contains(answers))
        {
            CollectionData.CollectionAnswers.Remove(answers);
        }
    }

    public void AddAnswer(List<string> answers)
    {
        if (answers == null && CollectionData == null)
            return;
        
        string date = DateTime.Now.ToString("dd.MM.yyyy");
        var answerData = new CollectionAnswers(answers, date);
        CollectionData.CollectionAnswers.Add(answerData);
    }

    public override void SetData(IData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        CollectionData = (CollectionData)data;
        _desription.text = CollectionData.Description;
        _logo.sprite = _collectionIconProvider.GetIconSprite(CollectionData.Type);
        _title.text = CollectionData.Title;
    }

    protected override void ReturnToDefault()
    {
        _title.text = string.Empty;
        _desription.text = string.Empty;
        CollectionData = null;
    }
}

[Serializable]
public class CollectionData : IData
{
    public CollectionType Type;
    public string Title;
    public string Description;
    public QuestionData[] Questions;
    public List<CollectionAnswers> CollectionAnswers;

    public CollectionData(CollectionType type, string title, string description, QuestionData[] questions)
    {
        Type = type;
        Title = title;
        Description = description;
        Questions = questions;
        CollectionAnswers = new List<CollectionAnswers>();
    }

}

[Serializable]
public class CollectionAnswers
{
    public List<string> Answers;
    public string Date;

    public CollectionAnswers(List<string> answers, string date)
    {
        Answers = answers;
        Date = date;
    }
}

public enum CollectionType
{
    Moon,
    Sun,
    Star,
    Books,
    BlackHole,
    Bookmark,
    FallStar,
    Lotus
}