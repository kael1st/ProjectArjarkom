using UnityEngine;

public class CoinMovement : MonoBehaviour
{
    // Kecepatan harus SAMA dengan moveSpeed pipa
    public float moveSpeed = 3f;
    public float destroyX = -10f; // Batas hapus di kiri layar

    void Update()
    {
        // Gerakkan koin ke kiri
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Hapus koin jika sudah melewati batas kiri layar
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    // Dipanggil saat objek lain melewati Collider (Is Trigger = true)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pastikan burung kamu memiliki Tag "Player"
        if (other.CompareTag("Player"))
        {
            // 1. Tambah Koin (panggil fungsi di GameData)
            // Asumsi: Skrip GameData ada di scene
            FindObjectOfType<GameData>().AddCoin(1);

            // 2. Hapus Koin setelah diambil
            Destroy(gameObject);
        }
    }
}