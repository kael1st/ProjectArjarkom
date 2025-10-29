using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    // --- Variabel Pipa ---
    [SerializeField] private GameObject _pipe;
    [SerializeField] private float float_maxTime = 2f; // Waktu antar spawn pipa (Spawn Rate)

    // --- Variabel Koin ---
    public GameObject coinPrefab; // Drag Prefab Koin ke sini
    public float coinSpawnChance = 0.7f; // Peluang muncul koin (70%)

    // --- Variabel Pengaturan Batas Spawn Y ---
    [Header("Pengaturan Batas Spawn Y")]
    public float minY_Center = 0.5f;
    public float maxY_Center = 3.5f;

    // Variabel internal untuk timer
    private float timer = 0f;

    private void Start()
    {
        SpawnPipe();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > float_maxTime)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    private void SpawnPipe()
    {
        // 1. Tentukan Y-Posisi PUSAT PIPA
        float randomYCenter = Random.Range(minY_Center, maxY_Center);

        // Z = 0 agar pipa tidak maju/mundur
        Vector3 spawnPos = new Vector3(transform.position.x, randomYCenter, 0);

        // Membuat objek pipa
        GameObject pipe = Instantiate(_pipe, spawnPos, Quaternion.identity);

        // Menghapus pipa setelah 14 detik
        Destroy(pipe, 14f);

        // 2. Logika Spawn Koin
        if (Random.value < coinSpawnChance)
        {
            SpawnCoin(randomYCenter, pipe);
        }
    }

    // Fungsi Spawn Koin
    private void SpawnCoin(float pipeCenterY, GameObject pipeObject)
    {
        // Posisi Koin Awal: Sama dengan posisi X Spawner, Y adalah pusat celah.
        // Z = 0. Koin harus diletakkan di Z=0 agar berada di lapisan yang sama dengan pipa.
        Vector3 coinSpawnPos = new Vector3(transform.position.x, pipeCenterY, 0);

        GameObject coin = Instantiate(coinPrefab, coinSpawnPos, Quaternion.identity);

        // Kunci Solusi: Jadikan Koin Anak dari Pipa
        // Koin sekarang bergerak bersama pipa induknya.
        coin.transform.SetParent(pipeObject.transform);

        // Hapus: Logika Destroy(coin, 14f) karena koin akan terhapus bersama induknya.
    }
}