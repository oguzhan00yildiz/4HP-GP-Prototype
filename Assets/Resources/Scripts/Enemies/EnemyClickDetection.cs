using UnityEngine;

public class EnemyClickDetection : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Enemy"))
                {
                    GameObject enemy = hitInfo.collider.gameObject;
                    Destroy(enemy);
                    GameManager.Instance.EnemyKilled();
                }
            }
        }
    }
}
