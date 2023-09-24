using Assets.Resources.Scripts.Global;
using UnityEngine;

namespace Assets.Resources.Scripts.Enemies
{
    public class EnemyClickDetection : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return; // Check if mouse has been clicked

            CheckIfEnemyClicked();
        }

        private void CheckIfEnemyClicked()
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hitInfo.collider == null) return; // Check if something has been hit

            if (!hitInfo.collider.CompareTag("Enemy")) return; // Check if enemy has been hit

            var enemy = hitInfo.collider.gameObject.GetComponent<EnemyData>();
            GameManager.Instance.EnemyHit(enemy, 10);
        }
    }
}