using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CategoryHolder
{
    public static List<string> Tags = new List<string>();

    private static readonly string _saveFilePath = Path.Combine(Application.persistentDataPath, "tags.json");
    
    public static void SaveTags()
    {
        QuestionTags tagsToSave = new QuestionTags(Tags);

        try
        {
            string json = JsonUtility.ToJson(tagsToSave, true);
            File.WriteAllText(_saveFilePath, json);
            Debug.Log("Tags saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save tags: " + e.Message);
        }
    }

    public static void LoadTags()
    {
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                QuestionTags loadedTags = JsonUtility.FromJson<QuestionTags>(json);
                
                Tags = new List<string>(new HashSet<string>(loadedTags.Tags));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load tags: " + e.Message);
            }
        }
        else
        {
            Tags = new List<string>();
        }
    }
}

[Serializable]
public class QuestionTags
{
    public List<string> Tags;

    public QuestionTags(IEnumerable<string> tags)
    {
        Tags = new List<string>(tags);
    }
}