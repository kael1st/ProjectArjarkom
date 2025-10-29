using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Drag Panel-panel dari Hierarchy ke sini di Inspector
    public GameObject mainPanel;
    public GameObject slotsPanel;

    private const string LastUsedSlotKey = "LastUsedSlot";

    // Dipanggil oleh Tombol "Mulai" / "Start"
    public void OpenSlots()
    {
        // 1. Sembunyikan Panel Utama
        mainPanel.SetActive(false);

        // 2. Tampilkan Panel Slot
        slotsPanel.SetActive(true);
    }

    // Dipanggil oleh Tombol Back (Opsional)
    public void CloseSlots()
    {
        slotsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // Dipanggil oleh Tombol Slot (1, 2, atau 3)
    public void SelectSlotAndStartGame(int slotNumber)
    {
        PlayerPrefs.SetInt(LastUsedSlotKey, slotNumber);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}