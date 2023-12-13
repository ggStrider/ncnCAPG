using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace ncn.Interact.Interactable
{
    /// <summary>
    /// Компонент який визначає куди та повертає (rotate) об'єкт
    /// </summary>
    public class RotateObject : MonoBehaviour
    {
        /// <summary>
        /// Об'єкт який повертаємо (rotate)
        /// </summary>
        [SerializeField] private Transform _objectToRotate;

        /// <summary>
        /// Наскільки повертаємо об'єкт
        /// </summary>
        [SerializeField] private Vector3 _rotateDelta;

        /// <summary>
        /// Час за який треба повернути об'єкт
        /// </summary>
        [SerializeField] private float _timeToRotate;

        [Space, Header("Other settings")]

        /// <summary>
        /// Чи можна буде повертати (rotate) об'єкт неодноразово
        /// </summary>
        [SerializeField] private bool _isLever;

        /// <summary>
        /// Чи повернутий вже об'єкт
        /// </summary>
        [SerializeField] private bool _isRotated;

        /// <summary>
        /// Якщо _isLever, то скільки буде затримка між використовуванням
        /// повороту об'єкту
        /// </summary>
        [SerializeField] private float _timeBetweenUsing;

        /// <summary>
        /// Чи можна повернути (rotate) об'єкт
        /// </summary>
        [SerializeField] private bool _canUse = true;

        /// <summary>
        /// Стартова позиція повороту об'єкту
        /// </summary>
        private Quaternion _startRotation;

        [Space, Header("Actions")]
        /// <summary>
        /// Івент який відбудеться коли об'єкт повернеться (rotate)
        /// </summary>
        [SerializeField] private UnityEvent _whenRotated;

        /// <summary>
        /// Івент який відбудеться коли об'єкт повернеться назад (rotate) 
        /// </summary>
        [SerializeField] private UnityEvent _whenUnRotated;

        // Виставляємо початкову позицію (поворот)
        private void Start() =>
            _startRotation = transform.rotation;

        /// <summary>
        /// Метод який запускає корутину яка визначає
        /// куди треба повернути об'єкт
        /// </summary>
        public void UseRotate() =>
            StartCoroutine(DetermineRotate());

        /// <summary>
        /// Корутина яка визначає куди та як повернеться об'єкт
        /// </summary>
        /// <returns>Затримка для _isLever using delay</returns>
        private IEnumerator DetermineRotate()
        {
            if (_canUse)
            {
                _canUse = false;
                
                if (!_isLever)
                {
                    if (!_isRotated)
                    {
                        StartCoroutine(FirstRotateCycle());
                    }
                    else
                    {
                        StartCoroutine(SecondRotateCycle());
                    }
                }

                else if (_isLever)
                {
                    if (!_isRotated)
                    {
                        StartCoroutine(FirstRotateCycle());
                    }
                    else
                    {
                        StartCoroutine(SecondRotateCycle());
                    }
                    _isRotated = !_isRotated;
                    
                    yield return new WaitForSeconds(_timeBetweenUsing);
                    _canUse = true;
                }
            }
        }

        /// <summary>
        /// Корутина яка повертає об'єкт на _rotateDelta
        /// </summary>
        /// <returns>Пропуск кадру для while</returns>
        private IEnumerator FirstRotateCycle()
        {
            var targetNextRotation = Quaternion.Euler(_rotateDelta) * _startRotation;
            var elapsedTime = 0.0f;

            while(elapsedTime < _timeToRotate)
            {
                _objectToRotate.rotation = Quaternion.Slerp(_objectToRotate.rotation, targetNextRotation, elapsedTime / _timeToRotate);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _objectToRotate.rotation = targetNextRotation;
            _whenRotated?.Invoke();
        }

        /// <summary>
        /// Корутина яка повертає об'єкт на початкову позицію
        /// </summary>
        /// <returns>Пропуск кадру для while</returns>
        private IEnumerator SecondRotateCycle()
        {
            var targetNextRotation = _startRotation;
            var elapsedTime = 0.0f;

            while(elapsedTime < _timeToRotate)
            {
                _objectToRotate.rotation = Quaternion.Slerp(_objectToRotate.rotation, targetNextRotation, elapsedTime / _timeToRotate);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _objectToRotate.rotation = targetNextRotation;
            _whenUnRotated?.Invoke();
        }

        /// <summary>
        /// Повернути об'єкт на початкову позицію
        /// </summary>
        public void SetObjectToStartRotation()
        {
            if(!_isRotated) return;
            
            StartCoroutine(SecondRotateCycle());
        }

        /// <summary>
        /// Повернути об'єкт на фінішну позицію
        /// </summary>
        public void SetObjectToEndRotation()
        {
            if(_isRotated) return;

            StartCoroutine(FirstRotateCycle());
        }
    }
}