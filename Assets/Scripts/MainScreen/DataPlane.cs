using System;
using UnityEngine;
using UnityEngine.UI;

public class DataPlane : MonoBehaviour
{
    [SerializeField] private Button _openButton;

    public IData Data { get; private set; }
    public bool IsActive { get; private set; }

    public event Action<DataPlane> OpenButtonClicked;

    private void OnEnable()
    {
        if (_openButton != null)
            _openButton.onClick.AddListener(OpenPlane);
    }

    private void OnDisable()
    {
        if (_openButton != null)
            _openButton.onClick.RemoveListener(OpenPlane);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
        //ReturnToDefault();
    }

    protected virtual void ReturnToDefault()
    {
    }

    public virtual void SetData(IData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        Data = data;
    }

    private void OpenPlane() => OpenButtonClicked?.Invoke(this);
}