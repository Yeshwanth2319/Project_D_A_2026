using TMPro;
using UnityEngine;

public class ScannerUI : MonoBehaviour
{
    public static ScannerUI Instance;

    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text infoText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowInfo(string title, string info)
    {
        panel.SetActive(true);

        titleText.text = title;
        infoText.text = info;
    }

    public void HideInfo()
    {
        panel.SetActive(false);
    }
}