using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainScreenDataHandler : MonoBehaviour
{
    [SerializeField] private string _saveFileName;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private List<DataPlane> _dataPlanes;
    [SerializeField] private Button _addButton;

    private List<int> _availableIndexes = new List<int>();
    private List<DataPlane> _activePlanes = new List<DataPlane>();

    public event Action<DataPlane> OpenedDataPlane;
    public event Action DataOpened;
    public event Action DataDeleted;

    private string _saveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);

    public List<QuestionPlane> DataPlanes => _activePlanes.OfType<QuestionPlane>().ToList();
    public List<CollectionPlane> CollectionPlanes => _activePlanes.OfType<CollectionPlane>().ToList();

    public void EnableDataPlane(IData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (_availableIndexes.Count > 0)
        {
            int availableIndex = _availableIndexes[0];
            _availableIndexes.RemoveAt(0);

            var currentFilledItemPlane = _dataPlanes[availableIndex];

            if (currentFilledItemPlane != null)
            {
                currentFilledItemPlane.Enable();
                currentFilledItemPlane.SetData(data);
                currentFilledItemPlane.OpenButtonClicked += OnOpenFilledPlane;
                _activePlanes.Add(currentFilledItemPlane);
            }
        }
        
        ToggleEmptyPlane(_availableIndexes.Count >= _dataPlanes.Count);
    }

    public void DeletePlane(DataPlane plane)
    {
        if (plane == null)
            throw new ArgumentNullException(nameof(plane));

        int index = _dataPlanes.IndexOf(plane);

        if (index >= 0 && !_availableIndexes.Contains(index))
        {
            _availableIndexes.Add(index);
        }

        plane.OpenButtonClicked -= OnOpenFilledPlane;
        plane.Disable();

        if (_activePlanes.Contains(plane))
        {
            _activePlanes.Remove(plane);
        }

        ToggleEmptyPlane(_availableIndexes.Count >= _dataPlanes.Count);
        DataDeleted?.Invoke();
    }

    public void DisableAllWindows()
    {
        _availableIndexes.Clear();
        
        for (int i = 0; i < _dataPlanes.Count; i++)
        {
            _dataPlanes[i].Disable();
            _availableIndexes.Add(i);
        }

        ToggleEmptyPlane(_availableIndexes.Count >= _dataPlanes.Count);
    }

    public bool ArePlanesEnabled()
    {
        return DataPlanes.Count > 0 ;
    }

    public DataPlane GetRandomQuestionPlane()
    {
        int randomPlaneIndex = Random.Range(0, DataPlanes.Count);

        if (DataPlanes[randomPlaneIndex].IsActive)
        {
            var plane = DataPlanes[randomPlaneIndex];
            return plane;
        }

        return null;
    }

    public void SetIconProvider(CollectionIconProvider iconProvider)
    {
        foreach (var plane in _dataPlanes.OfType<CollectionPlane>())
        {
            plane.SetIconProvider(iconProvider);
        }
    }

    private void ToggleEmptyPlane(bool status)
    {
        _emptyPlane.gameObject.SetActive(status);

        if (status)
        {
            _addButton.gameObject.SetActive(false);
        }
        else
        {
            _addButton.gameObject.SetActive(true);
        }
    }

    private void OnOpenFilledPlane(DataPlane plane)
    {
        OpenedDataPlane?.Invoke(plane);
        DataOpened?.Invoke();
    }
    
    public void SaveFilledQuestionsWindowsData()
    {
        List<QuestionData> questionsToSave = DataPlanes
            .Select(plane => plane.QuestionData)
            .ToList();

        if (questionsToSave.Count > 0)
        {
            SaveData(questionsToSave);
        }
        else
        {
            Debug.LogWarning("No QuestionData to save.");
        }
    }

    public void SaveFilledCollectionsWindowsData()
    {
        List<CollectionData> questionsToSave = CollectionPlanes
            .Select(plane => plane.CollectionData)
            .ToList();

        if (questionsToSave.Count > 0)
        {
            SaveData(questionsToSave);
        }
        else
        {
            Debug.LogWarning("No QuestionData to save.");
        }
    }
    
    public void LoadFilledQuestionsWindowsData()
    {
        LoadData<QuestionData>((data) => {
        });
    }

    public void LoadFilledCollectionsWindowsData()
    {
        LoadData<CollectionData>((data) => {
        });
    }

    public void SaveData<T>(List<T> itemsToSave)
    {
        string json = JsonUtility.ToJson(new ActiveDataList<T>(itemsToSave), true);
        try
        {
            File.WriteAllText(_saveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public void LoadData<T>(Action<T> onDataLoaded) where T : IData
    {
        DisableAllWindows();
        
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                ActiveDataList<T> loadedDataList = JsonUtility.FromJson<ActiveDataList<T>>(json);
                foreach (var data in loadedDataList.Data)
                {
                    EnableDataPlane(data);
                    onDataLoaded?.Invoke(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
            }
        }
    }
}

[Serializable]
public class ActiveDataList<T>
{
    public List<T> Data;

    public ActiveDataList(List<T> data)
    {
        Data = data;
    }
}
