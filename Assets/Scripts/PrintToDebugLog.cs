using UnityEngine;

namespace Tools.DebugClasses
{
    public class PrintToDebugLog : MonoBehaviour
    {
        [SerializeField] private string _text;

        public void OnText() =>
            Debug.Log(_text);
    }
}