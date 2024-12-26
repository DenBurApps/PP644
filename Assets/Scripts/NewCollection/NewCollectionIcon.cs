using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class NewCollectionIcon : MonoBehaviour
{
    [SerializeField] private CollectionType _type;
    
    private Image _image;
    private Button _button;

    public event Action<NewCollectionIcon> ButtonClicked;

    public CollectionType Type => _type;

    public Image Image => _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked() => ButtonClicked?.Invoke(this);
}
