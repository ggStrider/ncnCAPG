using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace ncn.PlayerSettings
{
    /// <summary>
    /// Клас де зчитуються всі кнопки які вказані
    /// в ActionMap'і
    /// </summary>
    public class InputReader : MonoBehaviour
    {
        /// <summary>
        /// Action map'а для зчитування клавіш
        /// </summary>
        [SerializeField] private PlayerController _controller;
        
        /// <summary>
        /// Система де знаходяться налаштування гравця,
        /// саме в неї ми передаємо direction куди треба
        /// йти
        /// </summary>
        [SerializeField] private PlayerSystem _playerSystem;

        private void Awake()
        {
            // Створюємо екземпляр Action Map'и
            _controller = new PlayerController();

            // Підписуємо нажимання клавіши на івент, а точніше
            // на метод OnMove
            _controller.Controller.Movement.performed += OnMove;
            _controller.Controller.Movement.canceled += OnMove;

            // Підписка на метод OnInteract
            _controller.Controller.Interact.started += OnInteract;

            _controller.Controller.Sprint.performed += OnSprint;
            _controller.Controller.Sprint.canceled += OnSprint;

#if UNITY_EDITOR
            _controller.Controller.RestartLevel.started += Restart;
#endif

            // Вмикаємо контроллер
            _controller.Enable();
        }
        
        // Очищаємо контроллер на OnDisable
        private void OnDisable() =>
            _controller.Dispose();

        /// <summary>
        /// Метод де записуємо вектор та відправляємо його
        /// в систему гравця
        /// </summary>
        /// <param name="context">Передані дані які містять
        /// значення нажаття кнопки</param>
        private void OnMove(InputAction.CallbackContext context)
        {
            // Зчитуємо дані саме Vector2
            var direction = context.ReadValue<Vector2>();

            // Відсилаємо дані Vector2 які отримали
            // в систему гравця
            _playerSystem.SetDirection(direction);
        }

        /// <summary>
        /// Метод з якого запускаємо інший метод в системі гравця
        /// </summary>
        /// <param name="context">Дані про те що ми нажали на кнопку</param>
        private void OnInteract(InputAction.CallbackContext context) =>
            _playerSystem.OnCheckObjectsInRay();

        private void OnSprint(InputAction.CallbackContext context)
        {
            if(context.performed) _playerSystem.Sprint(true);
            if(context.canceled) _playerSystem.Sprint(false);
        }


#if UNITY_EDITOR
        private void Restart(InputAction.CallbackContext context)
        {
            var currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
#endif
    }
}