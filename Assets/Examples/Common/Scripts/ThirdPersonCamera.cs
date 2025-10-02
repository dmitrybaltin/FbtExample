namespace Baltin.Examples.UnitaskFbt
{
    using UnityEngine;

    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;   // объект, за которым следим
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);
        [SerializeField] private float followSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            // Целевая позиция камеры
            Vector3 desiredPosition = target.position + offset;

            // Плавное перемещение камеры
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Смотрим на цель
            transform.LookAt(target);
        }
    }

}