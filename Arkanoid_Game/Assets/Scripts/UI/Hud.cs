using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image lifeIconTemplate;

    private readonly List<Image> lifeIcons = new List<Image>();

    public void CreateLifeIcons(int count)
    {
        lifeIconTemplate.gameObject.SetActive(false);
        for (int index = lifeIcons.Count; index < count; index++)
        {
            Image icon = Instantiate(lifeIconTemplate, lifeIconTemplate.transform.parent);
            icon.gameObject.SetActive(true);
            lifeIcons.Add(icon);
        }
    }

    public void SetReserveLives(int reserveLives)
    {
        for (int index = 0; index < lifeIcons.Count; index++)
        {
            lifeIcons[index].enabled = index < reserveLives;
        }
    }

    public void SetScore(int score)
    {
        scoreText.SetText("{0}", score);
    }

    public void SetHighScore(int highScore)
    {
        highScoreText.SetText("{0}", highScore);
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.enabled = true;
    }

    public void HideMessage()
    {
        messageText.enabled = false;
    }
}
