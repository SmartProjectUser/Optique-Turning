using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace OptiqueGames
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private AnimationCurve _singleStepAltitudeCurve;
        [SerializeField] private float _singleStepDuration;
        [SerializeField] private float _singleStepLength;
        [SerializeField] private float _singleStepMaxAltitude;

        private Vector2 _movementDirection = new Vector2(0.8f, 0.6f);


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StepForward();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                _movementDirection.x *= -1f;
            }
        }

        private void OnLanded()
        {
            StepForward();
        }

        private IEnumerator Jump(Vector2 targetPosition, float duration, float maxAltitude, AnimationCurve verticalCurve, Action landedCallback = null)
        {
            float curveDeltaTime = 1f / duration;
            Vector2 previousPosition = transform.position;
            
            while (duration > 0)
            {
                float curveTime = 1f - duration * curveDeltaTime;
                float normalizedAltitude = verticalCurve.Evaluate(curveTime);
                float scaledAltitude = normalizedAltitude * maxAltitude;

                Vector2 resultPosition = Vector2.MoveTowards(previousPosition, targetPosition,
                    Vector2.Distance(previousPosition, targetPosition) * curveTime);

                transform.position = resultPosition;
                _body.localPosition = Vector3.up * scaledAltitude;
                
                yield return new WaitForEndOfFrame();
                duration -= Time.deltaTime;
            }
            
            transform.position = targetPosition;
            _body.localPosition = Vector3.zero;
            
            landedCallback?.Invoke();
        }

        private void StepForward()
        {
            //Vector3 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + _movementDirection * _singleStepLength;
            
            StartCoroutine(Jump(targetPosition, _singleStepDuration, _singleStepMaxAltitude, _singleStepAltitudeCurve, OnLanded));
        }
    }
}

