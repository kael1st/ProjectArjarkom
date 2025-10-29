using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // PENTING: Tambahkan ini untuk menggunakan TextMeshPro

public class GameData : MonoBehaviour
{
    //Variabel Baru: Drag objek TextMeshProUGUI ke sini di Inspector
    public TextMeshProUGUI coinText;

    // Data yang ingin kita simpan
    public int score;
    public int coins;

    // Nomor Slot saat ini (diambil dari Main Menu)
    private int currentSlot;

    // Kunci Dasar untuk Data
    private const string ScoreKey = "Score";
    private const string CoinsKey = "Coins";
    private const string HasDataKey = "HasData";

    void Awake()
    {
        currentSlot = PlayerPrefs.GetInt("LastUsedSlot", 1);
        Debug.Log("Game dimuat menggunakan Slot: " + currentSlot);

        // 2. Muat Data Game (atau Mulai Baru)
        LoadGame();

        // Panggil Update UI saat game dimulai
        UpdateCoinUI();
    }

    public void LoadGame()
    {
        string hasDataUniqueKey = HasDataKey + currentSlot;

        if (PlayerPrefs.HasKey(hasDataUniqueKey))
        {
            // SLOT SUDAH ADA (LOAD GAME)
            string scoreUniqueKey = ScoreKey + currentSlot;
            string coinsUniqueKey = CoinsKey + currentSlot;

            score = PlayerPrefs.GetInt(scoreUniqueKey, 0);
            coins = PlayerPrefs.GetInt(coinsUniqueKey, 0);

            Debug.Log($"[LOAD] Slot {currentSlot} ditemukan. Skor: {score}, Koin: {coins}");
        }
        else
        {
            // SLOT KOSONG (NEW GAME)
            score = 0;
            coins = 0;

            Debug.Log($"[NEW GAME] Slot {currentSlot} masih kosong. Memulai permainan baru.");
        }
    }

    public void SaveGame()
    {
        string scoreUniqueKey = ScoreKey + currentSlot;
        string coinsUniqueKey = CoinsKey + currentSlot;
        string hasDataUniqueKey = HasDataKey + currentSlot;

        PlayerPrefs.SetInt(scoreUniqueKey, score);
        PlayerPrefs.SetInt(coinsUniqueKey, coins);
        PlayerPrefs.SetInt(hasDataUniqueKey, 1);

        PlayerPrefs.Save();

        Debug.Log($"Data Slot {currentSlot} berhasil disimpan dan ditandai sebagai terisi.");
    }

    // Fungsi Contoh: Menambahkan Koin (Dipanggil oleh CoinMovement.cs)
    public void AddCoin(int amount)
    {
        coins += amount;

        // Panggil SaveGame untuk menyimpan data baru
        SaveGame();

        //  Panggil fungsi update UI setelah data berubah dan tersimpan
        UpdateCoinUI();
    }

    //  FUNGSI BARU: Memperbarui Tampilan Text UI
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            // Set teks di UI sama dengan jumlah koin saat ini
            coinText.text = coins.ToString();
        }
        else
        {
            // Debugging jika kamu lupa drag objek
            Debug.LogError("Coin Text UI (TextMeshProUGUI) belum di-drag ke slot 'Coin Text' di GameData.cs!");
        }
    }
}