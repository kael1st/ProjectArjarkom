using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Wajib untuk TextMeshPro dan InputField

public class MainMenuManager : MonoBehaviour
{
    // --- Referensi Panel UI (Drag dari Hierarchy) ---
    public GameObject mainPanel;
    public GameObject slotsPanel;
    public GameObject nameInputPanel; // Panel Input Nama
    public TMP_InputField nameInputField; // Field tempat pemain mengetik nama

    // --- VARIABEL BARU UNTUK KONFIRMASI ---
    public GameObject deleteConfirmPanel; // Layar Konfirmasi Hapus
    public GameObject nameReviewPanel;    // Layar Review Nama
    public TextMeshProUGUI reviewNameText; // Teks untuk menampilkan nama yang diinput

    // --- Referensi Tombol Slot (Array size 3) ---
    public SlotButtonData[] slotButtons = new SlotButtonData[3];

    // --- Variabel Internal & Konstanta ---
    private int selectedSlot = 0; // Menyimpan slot yang baru saja di-klik
    private int slotToDelete = 0; // Slot yang lagi mau dihapus
    private string currentInputName = ""; // Nama yang lagi di-review

    private const string LastUsedSlotKey = "LastUsedSlot";
    private const string HasDataKey = "HasData";
    private const string PlayerNameKey = "PlayerName";
    private const string CoinsKey = "Coins";
    private const string ScoreKey = "Score";

    // Struktur Data untuk Tombol di Inspector
    [System.Serializable]
    public class SlotButtonData
    {
        public TextMeshProUGUI slotText;
        public GameObject deleteButton; // Tombol Hapus kecil
    }

    void Start()
    {
        // Pastikan hanya Main Panel yang aktif saat menu dimulai, sisanya nonaktif
        mainPanel.SetActive(true);
        slotsPanel.SetActive(false);
        nameInputPanel.SetActive(false);
        // Set nonaktif panel konfirmasi baru
        deleteConfirmPanel.SetActive(false);
        nameReviewPanel.SetActive(false);

        // Update tampilan slot (memuat data koin/nama)
        UpdateSlotDisplay();
    }

    // Dipanggil oleh Tombol "Mulai"
    public void OpenSlots()
    {
        mainPanel.SetActive(false);
        slotsPanel.SetActive(true);
    }

    // Dipanggil oleh Tombol "Back" (Jika ada)
    public void CloseSlots()
    {
        slotsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    // ----------------------------------------------------
    // --- LOGIKA UTAMA: PENENTUAN SLOTS ---

    // Dipanggil saat Tombol Slot (1, 2, atau 3) diklik
    public void SelectSlot(int slotNumber)
    {
        selectedSlot = slotNumber;
        string hasDataKey = HasDataKey + slotNumber;

        if (PlayerPrefs.HasKey(hasDataKey))
        {
            // Slot TERISI: Langsung muat game
            StartGame();
        }
        else
        {
            // Slot KOSONG: Minta input nama

            // Tampilkan panel input nama
            slotsPanel.SetActive(false);
            nameInputPanel.SetActive(true);

            // Kosongkan input field untuk nama baru
            nameInputField.text = "";
        }
    }

    // Dipanggil oleh Tombol Submit di NameInputPanel
    public void SubmitNameInput()
    {
        if (selectedSlot == 0) return;

        // 1. Ambil Nama
        string inputName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(inputName))
        {
            currentInputName = "Player"; // Default jika kosong
        }
        else
        {
            currentInputName = inputName;
        }

        // 2. Tampilkan Nama di Panel Review untuk Konfirmasi Akhir
        reviewNameText.text = "Kamu mau pakai nama " + currentInputName + ", sudah yakin?";

        nameInputPanel.SetActive(false);
        nameReviewPanel.SetActive(true);
    }

    // Dipanggil oleh Tombol "YAKIN" di NameReviewPanel
    public void FinalConfirmName()
    {
        // 1. Simpan Nama ke PlayerPrefs
        string nameKey = PlayerNameKey + selectedSlot;
        PlayerPrefs.SetString(nameKey, currentInputName);
        PlayerPrefs.Save();

        // 2. Muat Game
        StartGame();
    }

    // Dipanggil oleh Tombol "GANTI NAMA" di NameReviewPanel
    public void CancelNameReview()
    {
        nameReviewPanel.SetActive(false);
        nameInputPanel.SetActive(true); // Kembali ke Layar Ketik Nama
    }

    // ----------------------------------------------------
    // --- LOGIKA DELETE DENGAN KONFIRMASI ---

    // Dipanggil saat Tombol Delete kecil diklik
    public void ShowDeleteConfirm(int slotNumber)
    {
        slotToDelete = slotNumber; // Ingat slot mana yang mau dihapus
        slotsPanel.SetActive(false);
        deleteConfirmPanel.SetActive(true); // Tampilkan Panel Konfirmasi Hapus

        // Opsional: Perbarui teks di panel konfirmasi (misalnya: Yakin hapus data Slot X?)
    }

    // Dipanggil oleh Tombol "YA, HAPUS"
    public void ExecuteDelete()
    {
        if (slotToDelete == 0) return; // Kalau gak ada slot, batal

        // Proses Hapus Data
        PlayerPrefs.DeleteKey(PlayerNameKey + slotToDelete);
        PlayerPrefs.DeleteKey(CoinsKey + slotToDelete);
        PlayerPrefs.DeleteKey(HasDataKey + slotToDelete);
        PlayerPrefs.DeleteKey(ScoreKey + slotToDelete);

        PlayerPrefs.Save();

        Debug.Log("Data Slot " + slotToDelete + " berhasil dihapus.");

        // Reset dan kembali ke SlotsPanel
        slotToDelete = 0;
        deleteConfirmPanel.SetActive(false);
        slotsPanel.SetActive(true);
        UpdateSlotDisplay();
    }

    // Dipanggil oleh Tombol "TIDAK, BATAL"
    public void CancelDelete()
    {
        slotToDelete = 0;
        deleteConfirmPanel.SetActive(false);
        slotsPanel.SetActive(true); // Kembali ke Layar Slot
    }
    // ----------------------------------------------------

    private void StartGame()
    {
        // 1. Simpan Pilihan Slot Akhir (untuk GameData)
        PlayerPrefs.SetInt(LastUsedSlotKey, selectedSlot);
        PlayerPrefs.Save();

        // 2. Muat Scene Game
        SceneManager.LoadScene("SampleScene");
    }

    // FUNGSI UTILITY: Memperbarui tampilan semua slot
    public void UpdateSlotDisplay()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotId = i + 1;
            string hasDataKey = HasDataKey + slotId;

            if (PlayerPrefs.HasKey(hasDataKey))
            {
                // SLOT TERISI: Tampilkan Nama dan Koin
                string name = PlayerPrefs.GetString(PlayerNameKey + slotId, "Player");
                int coins = PlayerPrefs.GetInt(CoinsKey + slotId, 0);

                slotButtons[i].slotText.text = "Slot " + slotId + "\n" + name + " (" + coins + " Koin)";
                slotButtons[i].deleteButton.SetActive(true);
            }
            else
            {
                // SLOT KOSONG: Tampilkan status EMPTY
                slotButtons[i].slotText.text = "Slot " + slotId + "\n[EMPTY - NEW GAME]";
                slotButtons[i].deleteButton.SetActive(false);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}