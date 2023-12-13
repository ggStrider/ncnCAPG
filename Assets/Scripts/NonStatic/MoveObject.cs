using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace ncn.Interact.Interactable
{
    public class MoveObject : MonoBehaviour
    {
        /// <summary>
        /// Об'єкт який будемо рухати
        /// </summary>
        [SerializeField] private Transform _objectToMove;

        /// <summary>
        /// Напрямок у який рухаємо об'єкт
        /// </summary>
        [SerializeField] private Vector3 _moveDirection;

        /// <summary>
        /// Час за який об'єкт повинен стати в позицію яку треба
        /// </summary>
        [SerializeField] private float _moveDuration;

        [Space, Header("Lever settings")]
        /// <summary>
        /// Перевірка, чи є об'єкт важелем (можна використовувати не один раз) 
        /// </summary>
        [SerializeField] private bool _isLever;

        /// <summary>
        /// Затримка між взаємодією якщо _isLever
        /// </summary>
        [SerializeField] private float _delayBetweenPressing;

        /// <summary>
        /// Перевірка чи нажата вже кнопка 
        /// </summary>
        [SerializeField] private bool _isPressed;

        /// <summary>
        /// Чи можна використовувати взаємодію на рух
        /// </summary>
        [SerializeField] private bool _canUse = true;

        [Space, Header("Actions")]
        [SerializeField] private UnityEvent _pressedAction;

        [SerializeField] private UnityEvent _unPressedAction;
        /// <summary>
        /// Стартова позиція об'єкту (ставиться у методу Start)
        /// </summary>
        private Vector3 _startPosition;

        private void Start() =>
            // Виставляємо _startPosition на позицію на якій знаходиться об'єкт 
            _startPosition = _objectToMove.position;

        /// <summary>
        /// Метод який запускає корутину яка визначає куди піде об'єкт
        /// </summary>
        public void UseMove() =>
            StartCoroutine(DetermineMove());

        /// <summary>
        /// Метод який визначає куди піде об'єкт
        /// </summary>
        /// <returns>Затримка, якщо важіль</returns>
        private IEnumerator DetermineMove()
        {
            if (_canUse)
            {
                _canUse = false;
                if (!_isLever)
                {
                    StartCoroutine(FirstMoveCycle());
                }

                else if (_isLever)
                {
                    // Якщо важіль не використаний (нажатий)
                    if (!_isPressed)
                    {
                        StartCoroutine(FirstMoveCycle());
                    }

                    // Якщо важіль використаний (нажатий)
                    else
                    {
                        StartCoroutine(SecondMoveCycle());
                    }
                    _isPressed = !_isPressed;
                    
                    // Затримка перед тим як нажати (використати) важіль ще раз
                    yield return new WaitForSeconds(_delayBetweenPressing);
                    _canUse = true;
                }
            }
        }

        /// <summary>
        /// Корутина яка рухає об'єкт на _moveDirection
        /// </summary>
        /// <returns>Нічого</returns>
        private IEnumerator FirstMoveCycle()
        {
            // Визначаємо наступну позицію об'єкта
            var targetNextPosition = _objectToMove.position + _moveDirection;

            // Таймер для перевірки скільки часу рухається об'єкт
            var elapsedTime = 0.0f;

            while (elapsedTime < _moveDuration)
            {
                // М'якенько рухаємо об'єкт через 
                // лінійну інтерполяцію
                _objectToMove.position = Vector3.Lerp(_objectToMove.position, targetNextPosition, elapsedTime / _moveDuration);

                // Додаємо час у таймер
                elapsedTime += Time.deltaTime;

                // Пропускаємо кадр щоб гра не крашнулась
                yield return null;
            }
            // Про всяк випадок виставляємо позицію на ту, яку задали
            _objectToMove.position = targetNextPosition;

            // Виконуємо юніті івент коли воно завершило свій рух
            _pressedAction?.Invoke();
        }

        /// <summary>
        /// Корутина яка рухає об'єкт назад
        /// </summary>
        /// <returns>Нічого</returns>
        private IEnumerator SecondMoveCycle()
        {
            //Визначаємо наступну позицію об'єкта
            var targetNextPosition = _objectToMove.position - _moveDirection;

            // Таймер для перевірки скільки часу рухається об'єкт
            var elapsedTime = 0.0f;

            while (elapsedTime < _moveDuration)
            {
                // М'якенько рухаємо об'єкт через 
                // лінійну інтерполяцію
                _objectToMove.position = Vector3.Lerp(_objectToMove.position, targetNextPosition, elapsedTime / _moveDuration);

                // Додаємо час у таймер
                elapsedTime += Time.deltaTime;

                // Пропускаємо кадр щоб гра не крашнулась
                yield return null;
            }
            _objectToMove.position = targetNextPosition;

            _unPressedAction?.Invoke();
        }

        /// <summary>
        /// Метод який перевіряє чи стоїть об'єкт на стартовій
        /// позиції, і якщо ні, то виставе об'єкт на стартову
        /// позицію
        /// </summary>
        public void SetObjectToStartPosition()
        {
            // Перевірка чи стоїть на стартовій
            // позиції, якщо так то виходимо з методу
            if (!_isPressed) return;

            // Якщо не вийшли з методу, то запустимо корутину
            // щоб поставити на місце
            StartCoroutine(SecondMoveCycle());
        }

        /// <summary>
        /// Метод який перевіряє чи стоїть об'єкт на фінішній
        /// позиції, і якщо ні, то виставе об'єкт на фінішну
        /// позицію
        /// </summary>
        public void SetObjectToEndPosition()
        {
            // Перевірка чи стоїть на фінішній
            // позиції, якщо так то виходимо з методу
            if (_isPressed) return;

            // Якщо не вийшли з методу, то запустимо корутину
            // щоб поставити на місце
            StartCoroutine(FirstMoveCycle());
        }
    }
}