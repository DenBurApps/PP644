using TMPro;
using UnityEngine;

public class OpenAnswersPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _question;
    [SerializeField] private TMP_Text _answer;

    public void Enable(string question, string answer)
    {
        gameObject.SetActive(true);
        _question.text = question;
        _answer.text = answer;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
