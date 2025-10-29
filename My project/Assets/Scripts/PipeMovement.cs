using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    // Kecepatan pipa bergerak ke kiri (atur di Inspector)
    public float moveSpeed = 3f;

    // Batas X untuk menghapus pipa (di luar batas kiri layar)
    public float destroyX = -10f;

    void Update()
    {
        // Vector3.left adalah shorthand untuk (-1, 0, 0)
        // Time.deltaTime memastikan gerakan mulus terlepas dari frame rate
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Hapus objek pipa jika sudah melewati batas kiri
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}