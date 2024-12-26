using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestionsTagScroll : MonoBehaviour
{
    [SerializeField] private QuestionTag[] _questionTags;

    private List<string> _tags = new List<string>();
    private QuestionTag _currentSelectedTag;

    public event Action<string> TagClicked;

    private void OnEnable()
    {
        DisableAllTags();
    }

    private void Start()
    {
        _tags = new List<string>(CategoryHolder.Tags);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void EnableTags()
    {
        _tags.Clear();
        InitializeFirstTag();

        foreach (var tag in CategoryHolder.Tags)
        {
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                var questionTag = _questionTags.FirstOrDefault(t => !t.IsActive && t.Tag != tag);

                if (questionTag != null)
                {
                    questionTag.Enable(tag);
                    questionTag.SetDefault();
                    questionTag.ButtonClicked += ProcessTagClicked;
                }
            }
        }
    }

    public void SetNewTag(string tag)
    {
        InitializeFirstTag();

        if (_tags.Contains(tag))
            return;

        _tags.Add(tag);

        if (!CategoryHolder.Tags.Contains(tag))
        {
            CategoryHolder.Tags.Add(tag);
            CategoryHolder.SaveTags();
        }

        var selectedTag = _questionTags.FirstOrDefault(t => !t.IsActive);

        if (selectedTag == null)
            throw new Exception(nameof(selectedTag));

        selectedTag.Enable(tag);
        selectedTag.SetDefault();
        selectedTag.ButtonClicked += ProcessTagClicked;
    }
    
    private void InitializeFirstTag()
    {
        if (_tags.Count > 0)
        {
            return;
        }
        
        _questionTags[0].Enable("All");
        _questionTags[0].SetSelected();
        _currentSelectedTag = _questionTags[0];
        _currentSelectedTag.ButtonClicked += ProcessTagClicked;
    }

    public void DisableAllTags()
    {
        foreach (var tag in _questionTags)
        {
            tag.Disable();
            tag.ButtonClicked -= ProcessTagClicked;
        }
    }

    private void ProcessTagClicked(QuestionTag questionTag)
    {
        if (_currentSelectedTag != null)
        {
            _currentSelectedTag.SetDefault();
        }
        _currentSelectedTag = questionTag;
        _currentSelectedTag.SetSelected();

        TagClicked?.Invoke(questionTag.Tag);
    }
}