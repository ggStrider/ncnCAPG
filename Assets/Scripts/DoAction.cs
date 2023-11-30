using UnityEngine;
using UnityEngine.Events;

namespace ncn.Mechanics
{
    public class DoAction : MonoBehaviour
    {
        /// <summary>
        /// Івент який будемо викликати
        /// </summary>
        [SerializeField] private UnityEvent _action;

        /// <summary>
        /// Метод через який викликаємо кастомний івент
        /// </summary>
        public void OnDoAction() =>
            _action?.Invoke();
    }
}