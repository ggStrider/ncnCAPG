using UnityEngine;
using Cinemachine;

using ncn.PlayerSettings.Interact;
using ncn.Tools;
using ncn.Mechanics;

namespace ncn.PlayerSettings
{
    /// <summary>
    /// Вся система гравця (Ходьба, інтеракції, керування камерою)
    /// </summary>
    public class PlayerSystem : MonoBehaviour
    {
        /// <summary>
        /// Швидкість гравця
        /// </summary>
        [SerializeField] private float _speed;

        /// <summary>
        /// Камера яка прикріплена до гравця
        /// </summary>
        [SerializeField] private Camera _camera;

        /// <summary>
        /// Компонент який кастить луч та повертає об'єкт з яким
        /// заколайдився луч
        /// </summary>
        [SerializeField] private CheckObjectsInRay _checkInteractRay;

        /// <summary>
        /// Булеан який відповідає чи може гравець рухатись
        /// </summary>
        private bool _canMove = true;

        /// <summary>
        /// Мозок Cinemachine який прикріплений до камери
        /// в цьому коді використовується як функція яка вимикає
        /// можливість крутити камеру
        /// </summary>
        [SerializeField] private CinemachineBrain _cameraCinemachineBrain;

        /// <summary>
        /// Rigidbody (фізика) гравця
        /// </summary>
        private Rigidbody _rigidbody;

        /// <summary>
        /// Напрям сторони в яку йдемо
        /// </summary>
        private Vector2 _direction;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            // Блокуємо зміну позиції курсору  
            Cursor.lockState = CursorLockMode.Locked;

            // Вимикаємо видимість курсору
            Cursor.visible = false;
        }

        /// <summary>
        /// Код зчитування клавіш передає сюди напрям в який
        /// ми повинні йти (залежить від кнопки на яку нажимаємо)
        /// </summary>
        /// <param name="direction">Вектор який містить напрям в який повинні йти</param>
        public void SetDirection(Vector2 direction)
        {
            _direction = direction;

            // Якщо в метод передали нульовий вектор то
            // гравець різко зупиниться
            // Або якщо булеан _canMove = false
            if (_direction == Vector2.zero)
                _rigidbody.velocity = Vector2.zero;
        }

        private void FixedUpdate()
        {
            // Отримуємо напрям в який дивиться камера
            Vector3 cameraForward = _camera.transform.forward;

            // Ігноруємо Y координату щоб персонаж не перевертався
            cameraForward.y = 0f;

            // Нормалізуємо вектор перед тим як використати
            cameraForward.Normalize();

            // Вектор який каже в яку сторону підемо враховуючи
            // кут в який дивиться камера
            Vector3 movementDirection = cameraForward * _direction.y + _camera.transform.right * _direction.x;
            movementDirection.Normalize();

            // Використовуємо фізику для того щоб двигати
            // гравця, використовуючи значення які отримали вище
            if (_canMove)
                _rigidbody.MovePosition(_rigidbody.position + movementDirection * _speed * Time.fixedDeltaTime);

#if UNITY_EDITOR
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 3);
#endif
        }

        /// <summary>
        /// Метод для того щоб кастимо луч за допомогою 
        /// допоміжних скриптів
        /// </summary>
        public void OnCheckObjectsInRay()
        {
            // Значення з яких буде каститься луч
            var cameraPosition = _camera.transform.position;
            var cameraDirection = _camera.transform.forward;

            // Зберігаємо в змінну з тим чим
            // заколайдився луч (а також 
            // передаємо йому значення з яких позиції він
            // має кастити свій луч)
            var go = _checkInteractRay.GetObjectsInRange(cameraPosition, cameraDirection);

            // Якщо луч заколайдився з об'єктом у якого є тег
            // який записаний в окремому скрипту
            // та луч заколайдився з якимось об'єктом (!= null)
            if (go != null && go.CompareTag(TagsInGame.InteractableTag))
            {
                // То ми пошукаємо у об'єкта компонент DoAction
                var goDoAction = go.GetComponent<DoAction>();

                // Викликаємо метод OnDoAction у об'єкта
                goDoAction.OnDoAction();
            }
        }

        /// <summary>
        /// Метод для забезпечення "гнучкості" коду
        /// а саме, тут ми можемо "заборонити" гравцю
        /// ходити
        /// </summary>
        /// <param name="canMove">Булеан який передаємо, чи може гравець ходити</param>
        public void SetMoveBool(bool canMove) =>
            _canMove = canMove;


        /// <summary>
        /// Метод який дозволяє керувати функцією
        /// чи можна вертіти камеру
        /// </summary>
        /// <param name="canCameraMove">Булеан який передаємо, чи може гравець крутити камеру</param>
        public void SetCameraMove(bool canCameraMove) =>
            _cameraCinemachineBrain.enabled = canCameraMove;
    }
}