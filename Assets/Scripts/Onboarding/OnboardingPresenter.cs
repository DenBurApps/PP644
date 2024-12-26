using UnityEngine;
using UnityEngine.SceneManagement;

public class OnboardingPresenter : MonoBehaviour
{
    [SerializeField] private OnboardingView _firstScreenView;
    [SerializeField] private OnboardingView _secondScreenView;
    [SerializeField] private OnboardingView _thirdScreenView;

    private void Start()
    {
        _thirdScreenView.DisableScreen();
        _secondScreenView.DisableScreen();
        _firstScreenView.EnableScreen();
    }

    private void OnEnable()
    {
        _firstScreenView.InteractableButtonClicked += ProcessFirstScreenButtonClick;
        _secondScreenView.InteractableButtonClicked += ProcessSecondScreenButtonClick;
        _thirdScreenView.InteractableButtonClicked += ProcessThridScreenButtonClick;
    }

    private void OnDisable()
    {
        _firstScreenView.InteractableButtonClicked -= ProcessFirstScreenButtonClick;
        _secondScreenView.InteractableButtonClicked -= ProcessSecondScreenButtonClick;
    }

    private void ProcessFirstScreenButtonClick()
    {
        _firstScreenView.DisableScreen();
        _secondScreenView.EnableScreen();
        _thirdScreenView.DisableScreen();
    }

    private void ProcessSecondScreenButtonClick()
    {
        _firstScreenView.DisableScreen();
        _secondScreenView.DisableScreen();
        _thirdScreenView.EnableScreen();
    }

    private void ProcessThridScreenButtonClick()
    {
        PlayerPrefs.SetInt("Onboarding", 1);
        SceneManager.LoadScene("MainScreen");
    }
}