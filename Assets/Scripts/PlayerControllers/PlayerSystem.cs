using UnityEngine;
using Cinemachine;
using System.Collections;

using ncn.PlayerSettings.Interact;
using ncn.Tools;
using ncn.Mechanics;
using ncn.Model;

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

        [Space, Header("Sprint settings")]
        [SerializeField] private bool _canSprint;
        [SerializeField] private bool _isUsingSprintButton;
        [SerializeField] private float _stamina;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _decreaseDeltaStamina;
        [SerializeField] private float _decreaseDelayStamina;
        [SerializeField] private float _increaseDeltaStamina;
        [SerializeField] private float _increaseDelayStamina;
        [SerializeField] private float _delayBetweenUsingSprint;

        private bool _isSprinting;

        private float _maxStamina;

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

        [SerializeField] private float _interactCheckDistance = 3f;

        /// <summary>
        /// Rigidbody (фізика) гравця
        /// </summary>
        private Rigidbody _rigidbody;

        /// <summary>
        /// Напрям сторони в яку йдемо
        /// </summary>
        private Vector2 _direction;

        private GameSession _session;

        private float _commonSpeed;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _session = FindObjectOfType<GameSession>();
            _session.Data.Inventory.OnChanged += OnInventoryChanged;

            _commonSpeed = _speed;
            _maxStamina = _stamina;

            // Блокуємо зміну позиції курсору  
            Cursor.lockState = CursorLockMode.Locked;

            // Вимикаємо видимість курсору
            Cursor.visible = false;
        }

        private void OnInventoryChanged(string id, int value)
        {
            if (id == "Item1")
                Debug.Log("collected item1");
        }

        private void OnDestroy()
        {
            _session.Data.Inventory.OnChanged -= OnInventoryChanged;
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
            {
                var stopPlayer = new Vector3(0f, _rigidbody.velocity.y, 0f);
                _rigidbody.velocity = stopPlayer;
            }
        }

        private void FixedUpdate()
        {
            if (_isUsingSprintButton)
                _speed = _sprintSpeed;
            else _speed = _commonSpeed;
            // Отримуємо напрям в який дивиться камера
            var cameraForward = _camera.transform.forward;

            // Ігноруємо Y координату щоб персонаж не перевертався
            cameraForward.y = 0f;

            // Нормалізуємо вектор перед тим як використати
            cameraForward.Normalize();

            // Вектор який каже в яку сторону підемо враховуючи
            // кут в який дивиться камера
            var movementDirection = cameraForward * _direction.y + _camera.transform.right * _direction.x;
            movementDirection.Normalize();

            // Використовуємо фізику для того щоб двигати
            // гравця, використовуючи значення які отримали вище
            if (_canMove)
                _rigidbody.MovePosition(_rigidbody.position + movementDirection * _speed * Time.fixedDeltaTime);

#if UNITY_EDITOR
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 3, Color.green);
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
            var go = _checkInteractRay.GetObjectsInRange(cameraPosition, cameraDirection, _interactCheckDistance);

            // Якщо луч заколайдився з об'єктом у якого є тег
            // який записаний в окремому скрипту
            // та луч заколайдився з якимось об'єктом (!= null)
            if (go != null && go.CompareTag(KeyStrings.InteractableTag))
            {
                // То ми пошукаємо у об'єкта компонент DoAction
                var goDoAction = go.GetComponent<DoAction>();

                // Викликаємо метод OnDoAction у об'єкта
                goDoAction.OnDoAction();
            }
        }

        public void Sprint(bool onUsing)
        {
            if (!_canSprint) return;
            _isUsingSprintButton = onUsing;

            StartCoroutine(SprintSystem());
        }

        private IEnumerator SprintSystem()
        {
            if (_isSprinting) yield return null;
            _isSprinting = true;

            while (_stamina > 0.1f && _canSprint && _isUsingSprintButton)
            {
                if (_direction != Vector2.zero)
                {
                    _stamina = Mathf.Clamp(_stamina - _decreaseDeltaStamina, 0, _maxStamina);
                    yield return new WaitForSeconds(_decreaseDelayStamina);
                }
                yield return null;
            }

            _isSprinting = false;
            if (_stamina < _maxStamina)
            {
                _canSprint = false;
                _isUsingSprintButton = false;

                StartCoroutine(RestoreStamina());

                yield return new WaitForSeconds(_delayBetweenUsingSprint);
                _canSprint = true;
            }
        }

        private IEnumerator RestoreStamina()
        {
            while (!_isUsingSprintButton && _stamina < _maxStamina)
            {
                _stamina = Mathf.Clamp(_stamina + _increaseDeltaStamina, 0, _maxStamina);
                yield return new WaitForSeconds(_increaseDelayStamina);
            }
        }

        public void OnSpeedChange(float delta)
        {
            _speed += delta;
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

        public void AddInInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }
    }
}