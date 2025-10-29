using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Setup")]
    public GameObject playerPrefab;     // assign prefab Player di Inspector
    public Transform spawnPoint;        // titik spawn awal

    private GameObject currentPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab belum diassign di GameManager!");
            return;
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }
}
