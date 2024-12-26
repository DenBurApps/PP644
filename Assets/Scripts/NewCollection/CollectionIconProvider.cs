using System;
using UnityEngine;

public class CollectionIconProvider : MonoBehaviour
{
    [SerializeField] private IconType[] _iconTypes;


    public Sprite GetIconSprite(CollectionType type)
    {
        foreach (var iconType in _iconTypes)
        {
            if (iconType.Type == type)
                return iconType.IconSprite;
        }

        return null;
    }

    public Sprite GetUnselectedSprite(CollectionType type)
    {
        foreach (var iconType in _iconTypes)
        {
            if (iconType.Type == type)
                return iconType.UnselectedSprite;
        }

        return null;
    }
}

[Serializable]
public class IconType
{
    public Sprite IconSprite;
    public Sprite UnselectedSprite;
    public CollectionType Type;
}