using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace ncn.Waxwork.AI
{
    /// <summary>
    /// Компонент який керує переслідування агента до цілі
    /// </summary>
    public class WaxworkChaseAI : MonoBehaviour
    {
        /// <summary>
        /// Агент яким будемо керувати (заставляти йти його до цілі)
        /// </summary>
        [SerializeField] private NavMeshAgent _agent;

        /// <summary>
        /// Ціль яку переслідує агент
        /// </summary>3
        [SerializeField] private Transform _target;

        /// <summary>
        /// Калібровка передньої сторони агента
        /// </summary>
        [SerializeField] private Vector3 _rotationOffset;

        /// <summary>
        /// Перевірка чи переслідує агент ціль
        /// </summary>
        private bool _chasingPlayer;

        [ContextMenu("Start Chasing")]
        /// <summary>
        /// Метод через який запускається корутина
        /// переслідування цілі
        /// </summary>
        public void StartChase()
        {
            // Ставимо булеант який відповідає
            // за переслідування на true
            _chasingPlayer = true;

            // Запускаємо корутину переслідування гравця
            StartCoroutine(ChasePlayer());
        }

        /// <summary>
        /// Визначення та надавання агенту шлях до гравця
        /// </summary>
        /// <returns>Пропуск кадра для while циклу</returns>
        private IEnumerator ChasePlayer()
        {
            // Вимикаємо автоматичний поворот (так як використовуємо калібровку)
            _agent.updateRotation = false;

            while (_chasingPlayer)
            {
                // Отримуємо напрямок до цілі об'єкта
                var direction = (_target.position - transform.position).normalized;

                // Визначаємо поворот до цільового об'єкта
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

                // Додати зміщення до повороту обличчя
                lookRotation *= Quaternion.Euler(_rotationOffset);

                // Встановити шлях для _agent
                _agent.SetDestination(_target.position);

                // Встановлюємо поворот об'єкта
                transform.rotation = lookRotation;

                // Пропускаємо кадр щоб не крашнулась гра
                yield return null;
            }
        }

        /// <summary>
        ///  Метод який зупиняє переслідування цілі
        /// </summary>
        public void StopChasing()
        {
            // Вимикаємо булеан переслідування
            _chasingPlayer = false;

            // Одразу зупиняємо агента
            _agent.velocity = Vector3.zero;
        }
    }
}