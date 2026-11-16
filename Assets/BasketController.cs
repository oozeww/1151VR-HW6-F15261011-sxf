using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BasketController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Grid Settings")]
    public float minX = -1.216f;
    public float maxX = 1.216f;
    public float minZ = -1.216f;
    public float maxZ = 1.216f;
    public float basketY = 0.5f;

    [Header("Audio Settings")]
    public AudioClip appleSound;
    public AudioClip bombSound;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource audioSource;
    private float cellSizeX;
    private float cellSizeZ;
    private bool isGameActive = true; // 控制游戏结束后锁定移动

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cellSizeX = (maxX - minX) / 3f;
        cellSizeZ = (maxZ - minZ) / 3f;
    }

    void Update()
    {
        if (!isGameActive) return; // 游戏结束则禁止移动

        if (Input.GetMouseButtonDown(0))
        {
            MoveBasketToGrid();
        }
    }

    public void SetGameActive(bool active)
    {
        isGameActive = active;
    }

    void MoveBasketToGrid()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, basketY, 0));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            if (hitPoint.x < minX || hitPoint.x > maxX ||
                hitPoint.z < minZ || hitPoint.z > maxZ)
            {
                return;
            }

            int column = Mathf.Clamp(Mathf.FloorToInt((hitPoint.x - minX) / cellSizeX), 0, 2);
            int row = Mathf.Clamp(Mathf.FloorToInt((hitPoint.z - minZ) / cellSizeZ), 0, 2);

            float targetX = minX + (column + 0.5f) * cellSizeX;
            float targetZ = minZ + (row + 0.5f) * cellSizeZ;

            transform.position = new Vector3(targetX, basketY, targetZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isGameActive) return;

        if (other.CompareTag("Apple"))
        {
            PlaySound(appleSound);
            if (GameDirector.Instance != null)
                GameDirector.Instance.NotifyAppleCaught(); // 通知导演加分
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Bomb"))
        {
            PlaySound(bombSound);
            if (GameDirector.Instance != null)
                GameDirector.Instance.NotifyBombCaught();  // 通知导演扣分
            Destroy(other.gameObject);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip, sfxVolume);
    }
}