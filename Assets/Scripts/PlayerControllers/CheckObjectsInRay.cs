using UnityEngine;

namespace ncn.PlayerSettings.Interact
{
    public class CheckObjectsInRay : MonoBehaviour
    {
        /// <summary>
        /// Метод який касте луч.
        /// </summary>
        /// <param name="position">Позиція з якої каститься луч</param>
        /// <param name="direction">Напрямок в який каститься луч</param>
        /// <param name="distance">Дистанція на яку каститься луч</param> 
        /// <returns>Повертає об'єкт з яким пересікся луч</returns>
        public GameObject GetObjectsInRange(Vector3 position, Vector3 direction, float distance)
        {
            // Змінна в яку помістимо інформацію з пересіченим
            // об'єктом
            RaycastHit hitInfo;

            // Перевіряємо чи з чимось пересікся луч
            if(Physics.Raycast(position, direction, out hitInfo, distance))
                // Якщо так то повертаємо об'єкт з яким перетнувся луч
                return hitInfo.collider.gameObject; 

            // Нічого не повертаємо якшо не пересіклись
            return null;
        }
    }
}