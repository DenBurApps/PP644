using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : MonoBehaviour
{
    [SerializeField] private MainScreenView _view;
    [SerializeField] private RandomQuestionPlane _randomQuestionPlane;
    [SerializeField] private MainScreenDataHandler _collectionsHandler;
    [SerializeField] private MainScreenDataHandler _questionsHandler;
    [SerializeField] private QuestionsTagScroll _questionsTagScroll;
    [SerializeField] private QuestionFillingScreen _questionFillingScreen;
    [SerializeField] private OpenQuestion _openQuestion;
    [SerializeField] private Answer _answer;
    [SerializeField] private CollectionDataFillingFirstScreen _collectionDataFillingFirst;
    [SerializeField] private CollectionDataFillingSecondScreen _collectionDataFillingSecondScreen;
    [SerializeField] private CollectionIconProvider _collectionIconProvider;
    [SerializeField] private OpenCollectionScreen _openCollectionScreen;
    [SerializeField] private SettingsScreen _settingsScreen;

    public event Action AddCollectionClicked;
    public event Action<QuestionPlane> QuestionOpened;
    public event Action<CollectionPlane> CollectionOpened;
    public event Action SettingsClicked;

    public List<QuestionPlane> DataPlanes => _questionsHandler.DataPlanes;

    private void OnEnable()
    {
        _view.AddQuestionClicked += OnAddNewQuestion;
        _view.AddCollectionClicked += OnAddCollection;

        _questionsHandler.OpenedDataPlane += OnQuestionOpened;
        _questionsHandler.DataDeleted += ValidateRandomQuestionPlane;
        _randomQuestionPlane.NewQuestionRequired += ValidateRandomQuestionPlane;

        _questionFillingScreen.Saved += EnableQuestionFillingPlane;
        _questionFillingScreen.BackButtonClicked += _view.Enable;

        _openQuestion.BackButtonClicked += OpenWindow;
        _openQuestion.Edited += _questionsHandler.SaveFilledQuestionsWindowsData;

        _answer.AnswerClicked += SaveNewAnswer;
        _answer.CloseButtonClicked += _view.Enable;

        _randomQuestionPlane.AnswerButtonClicked += ProcessAnswer;

        _view.AddCollectionClicked += OnAddCollection;
        _view.SettingsClicked += OnSettingsClicked;

        _collectionDataFillingFirst.BackButtonClicked += _view.Enable;

        _collectionDataFillingSecondScreen.Saved += EnableCollectionFillingPlane;

        _collectionsHandler.OpenedDataPlane += OnCollectionOpened;

        _openCollectionScreen.BackButtonClicked += _view.Enable;

        _settingsScreen.BackButtonClicked += _view.Enable;

        _questionsTagScroll.TagClicked += FilterQuestions;

        _openCollectionScreen.Edited += _collectionsHandler.SaveFilledCollectionsWindowsData;
    }

    private void OnDisable()
    {
        _view.AddQuestionClicked -= OnAddNewQuestion;
        _view.AddCollectionClicked -= OnAddCollection;

        _questionsHandler.OpenedDataPlane -= OnQuestionOpened;
        _questionsHandler.DataDeleted -= ValidateRandomQuestionPlane;

        _questionFillingScreen.Saved -= EnableQuestionFillingPlane;
        _questionFillingScreen.BackButtonClicked -= _view.Enable;
        _randomQuestionPlane.NewQuestionRequired -= ValidateRandomQuestionPlane;

        _openQuestion.BackButtonClicked -= OpenWindow;
        _openQuestion.Edited -= _questionsHandler.SaveFilledQuestionsWindowsData;

        _answer.AnswerClicked -= SaveNewAnswer;
        _answer.CloseButtonClicked -= _view.Enable;

        _randomQuestionPlane.AnswerButtonClicked -= ProcessAnswer;

        _view.AddCollectionClicked -= OnAddCollection;
        _view.SettingsClicked -= OnSettingsClicked;

        _collectionDataFillingFirst.BackButtonClicked -= _view.Enable;

        _collectionDataFillingSecondScreen.Saved -= EnableCollectionFillingPlane;

        _collectionsHandler.OpenedDataPlane -= OnCollectionOpened;

        _openCollectionScreen.BackButtonClicked -= _view.Enable;

        _settingsScreen.BackButtonClicked -= _view.Enable;

        _questionsTagScroll.TagClicked -= FilterQuestions;

        _openCollectionScreen.Edited -= _collectionsHandler.SaveFilledCollectionsWindowsData;
    }

    private void Start()
    {
        CategoryHolder.LoadTags();
        _questionsHandler.LoadFilledQuestionsWindowsData();
        _collectionsHandler.SetIconProvider(_collectionIconProvider);
        _collectionsHandler.LoadFilledCollectionsWindowsData();
        _view.Enable();
        _answer.Disable();
        _questionsTagScroll.EnableTags();
        ValidateRandomQuestionPlane();
    }

    private void ValidateRandomQuestionPlane()
    {
        if (_questionsHandler.ArePlanesEnabled())
        {
            if (_randomQuestionPlane.HasData)
            {
                return;
            }

            _randomQuestionPlane.Enable();
            _view.ToggleEmptyRandomQuestionPlane(false);
            _randomQuestionPlane.SetQuestion(_questionsHandler.GetRandomQuestionPlane());
        }
        else
        {
            _randomQuestionPlane.Disable();
            _view.ToggleEmptyRandomQuestionPlane(true);
            _questionsTagScroll.Disable();
        }
    }

    private void OpenWindow()
    {
        _view.Enable();
        _randomQuestionPlane.UpdateValues();
    }

    private void EnableQuestionFillingPlane(QuestionData data)
    {
        _questionsHandler.EnableDataPlane(data);
        _questionsHandler.SaveFilledQuestionsWindowsData();
        _questionsTagScroll.Enable();
        _questionsTagScroll.SetNewTag(data.Tag);
        _view.ToggleEmptyRandomQuestionPlane(false);
        ValidateRandomQuestionPlane();
    }

    private void EnableCollectionFillingPlane(CollectionData data)
    {
        _view.Enable();
        _collectionsHandler.EnableDataPlane(data);
        _collectionsHandler.SaveFilledCollectionsWindowsData();
    }

    private void OnAddNewQuestion()
    {
        _questionFillingScreen.EnableScreen();
        _view.Disable();
    }

    private void OnAddCollection()
    {
        AddCollectionClicked?.Invoke();
        _view.Disable();
    }

    private void OnQuestionOpened(DataPlane plane)
    {
        _view.Disable();
        QuestionOpened?.Invoke((QuestionPlane)plane);
    }

    private void OnCollectionOpened(DataPlane plane)
    {
        _view.Disable();
        CollectionOpened?.Invoke((CollectionPlane)plane);
    }

    private void SaveNewAnswer(string answer)
    {
        _answer.CurrentPlane.AddAnswer(answer);
        _view.Enable();
    }

    private void ProcessAnswer(QuestionPlane plane)
    {
        _answer.Enable(plane);
        _view.MakeTransparent();
    }

    private void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
        _view.Disable();
    }

    private void FilterQuestions(string tag)
    {
        string filteredTag = tag.ToLower();

        foreach (var plane in _questionsHandler.DataPlanes)
        {
            if (filteredTag == "all")
            {
                if (plane.QuestionData != null)
                {
                    plane.Enable();
                }
            }
            else
            {
                if (plane.QuestionData != null && plane.QuestionData.Tag.ToLower() == filteredTag)
                {
                    if (!plane.IsActive)
                    {
                        plane.Enable();
                    }
                }
                else
                {
                    if (plane.IsActive)
                    {
                        plane.Disable();
                    }
                }
            }
        }
    }
}