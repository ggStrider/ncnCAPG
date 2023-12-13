using UnityEngine;
using System.Collections;
using ncn.PlayerSettings.Interact;

namespace ncn.Waxwork.AI
{
    public class WaxWorkStatueAI : MonoBehaviour
    {
        [SerializeField] private WaxworkChaseAI _agentWhoChasePlayer;
        [SerializeField] private Camera _targetTransform;
        [SerializeField] private CheckObjectsInRay _checkRay;
        [SerializeField] private GameObject _bannedCollider;
        [SerializeField] private bool _activated;
        public bool test;

        [ContextMenu("check")]
        public void StartCheckTargetCameraAngle()
        {
            _activated = true;
            StartCoroutine(CheckTargetCameraAngle());
        }

        private IEnumerator CheckTargetCameraAngle()
        {
            while (_activated)
            {
                var position = _targetTransform.transform.position;
                var direction = _targetTransform.transform.forward;
                var distance = 180;

                Debug.DrawRay(position, direction * distance, Color.red);

                var check = _checkRay.GetObjectsInRange(position, direction, distance);

                if (check != null)
                {
                    Debug.Log(check.name, check);
                    if (check != _bannedCollider && test)
                    {
                        test = false;
                        _agentWhoChasePlayer.StartChase();
                    }

                    else if (check == _bannedCollider)
                    {
                        _agentWhoChasePlayer.StopChasing();
                        test = true;
                    }
                }
                yield return null;
            }
        }

        public void StopCheckingPlayer()
        {
            _activated = false;
        }
    }
}