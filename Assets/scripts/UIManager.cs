using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI killsText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateWave(int waveNum)
    {
        if (waveText != null)
            waveText.text = $"Wave: {waveNum}";
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = $"Lives: {lives}";
    }

    public void UpdateKills(int kills)
    {
        if (killsText != null)
            killsText.text = $"Kills: {kills}";
    }
}
