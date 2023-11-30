using UnityEngine;
using UnityEngine.Events;

namespace ncn.TriggersCollisions
{
    /// <summary>
    /// Клас який містить в собі код який відповідає за
    /// те чи зайшов гравець у якийсь трігер та визиває
    /// юніті івент якшо тег об'єкта який зайшов = тегу
    /// який вказаний
    /// </summary>
    public class TriggerEnter : MonoBehaviour
    {
        /// <summary>
        /// Тег з яким звіряємо об'єкт який зайшов у трігер
        /// </summary>
        [SerializeField] private string _tag;

        /// <summary>
        /// Івент який визивається якщо об'єкт який зайшов
        /// у трігер = вказаному тегу 
        /// </summary>
        [SerializeField] private UnityEvent _action;

        /// <summary>
        /// Якщо якийсь колайдер зайшов у трігер
        /// який має цей компонент, то визветься
        /// цей метод
        /// </summary>
        /// <param name="other">Об'єкт який зайшов</param>
        private void OnTriggerEnter(Collider other) 
        {
            // Звіряємо тег об'єкта з тегом який вказали раніше
            // Якщо вони співпадають
            if(other.CompareTag(_tag))
                // То викличем юніті івент
                _action.Invoke();
        }
    }
}