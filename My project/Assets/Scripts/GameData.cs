using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class GameData : MonoBehaviour
{
    // --- Konfigurasi API ---
    private const string ApiBaseUrl = "http://localhost:5182/api/Game";

    // --- UI Variable ---
    public TextMeshProUGUI coinText;

    // --- Data Variables ---
    public string playerName;
    public int score;
    public int coins;

    private int currentSlot;

    // --- Data Keys (Konstanta Kunci Penyimpanan) ---
    private const string ScoreKey = "Score";
    private const string CoinsKey = "Coins"; // Kunci untuk penyimpanan koin lokal
    private const string HasDataKey = "HasData";
    private const string PlayerNameKey = "PlayerName";
    // -----------------------------------------------------------------

    void Awake()
    {
        currentSlot = PlayerPrefs.GetInt("LastUsedSlot", 1);
        Debug.Log("Game dimuat menggunakan Slot: " + currentSlot);

        StartCoroutine(LoadGame());
    }

    // -----------------------------------------------------------------
    // FUNGSI LOAD DARI SERVER (Coroutine)
    // -----------------------------------------------------------------
    public IEnumerator LoadGame()
    {
        string hasDataUniqueKey = HasDataKey + currentSlot;
        string url = ApiBaseUrl + "/load/" + currentSlot;

        if (PlayerPrefs.HasKey(hasDataUniqueKey))
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                Debug.Log("[LOAD] Mengirim permintaan ke: " + url);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    SaveData loadedData = JsonUtility.FromJson<SaveData>(jsonResponse);

                    // Muat Data dari Server
                    playerName = loadedData.PlayerName;
                    score = loadedData.Score;
                    coins = loadedData.Coins;
                }
                else
                {
                    Debug.LogError("[LOAD] Gagal koneksi/data dari Server: " + webRequest.error);
                    score = 0; coins = 0;
                    playerName = PlayerPrefs.GetString(PlayerNameKey + currentSlot, "Player");
                }
            }
        }
        else
        {
            score = 0; coins = 0;
            playerName = PlayerPrefs.GetString(PlayerNameKey + currentSlot, "Player");
            Debug.Log("[NEW GAME] Memulai baru dengan nama: " + playerName);
        }

        UpdateCoinUI();
    }

    // -----------------------------------------------------------------
    // FUNGSI SAVE KE SERVER (Coroutine)
    // -----------------------------------------------------------------
    public IEnumerator SaveGame()
    {
        string hasDataUniqueKey = HasDataKey + currentSlot;
        string coinsUniqueKey = CoinsKey + currentSlot; // Kunci lokal untuk UI
        string url = ApiBaseUrl + "/save";

        // 1. Siapkan data yang mau dikirim dalam bentuk objek
        SaveData dataToSave = new SaveData
        {
            SlotId = this.currentSlot,
            PlayerName = this.playerName,
            Score = this.score,
            Coins = this.coins
        };

        // 2. Ubah objek C# ke string JSON
        string jsonToSend = JsonUtility.ToJson(dataToSave);

        // Menggunakan konstruktor UnityWebRequest murni untuk POST
        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonToSend);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // ❗ SOLUSI UI/MENU: Simpan Koin dan HasData ke PlayerPrefs
                // Ini penting agar MainMenuManager bisa melihat data terbaru tanpa memanggil API
                PlayerPrefs.SetInt(hasDataUniqueKey, 1);
                PlayerPrefs.SetInt(coinsUniqueKey, this.coins); // SIMPAN KOIN LOKAL
                PlayerPrefs.Save();

                Debug.Log("[SAVE] Data berhasil dikirim ke Server DAN disimpan lokal.");
            }
            else
            {
                Debug.LogError("[SAVE] Gagal mengirim data ke Server: " + webRequest.error);
            }
        }
    }

    // -----------------------------------------------------------------
    // FUNGSI LAIN
    // -----------------------------------------------------------------

    public void AddCoin(int amount)
    {
        coins += amount;

        StartCoroutine(SaveGame());

        UpdateCoinUI();
    }

    // FUNGSI WAJIB: Simpan dan Muat Scene dengan Aman
    public IEnumerator SaveAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(SaveGame());
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }
}

// -----------------------------------------------------------------
// STRUKTUR DATA UNTUK PARSING JSON (WAJIB ADA!)
// -----------------------------------------------------------------
[System.Serializable]
public class SaveData
{
    public int SlotId;
    public string PlayerName;
    public int Score;
    public int Coins;
}