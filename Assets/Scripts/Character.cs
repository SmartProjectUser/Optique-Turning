using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private DirectionInfluence _directionInfluence;
        
        [SerializeField] private GameGrid _gameGrid;
        [SerializeField] private Transform _body;
        [SerializeField] private AnimationCurve _singleStepAltitudeCurve;
        [SerializeField] private AnimationCurve _influenceAltitudeCurve;
        [SerializeField] private float _singleStepDuration;
        [SerializeField] private float _influenceFlyDuration;
        [SerializeField] private float _singleStepMaxAltitude;

        private Vector2Int _movementDirection = Vector2Int.right;

        private Coroutine _jumpCoroutine = null;
        private bool _insideWater = false;

        private void Awake()
        {
            _directionInfluence.Influenced += OnReceiveInfluence;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StepForward();
            }
        }

        private void OnReceiveInfluence()
        {
            if (_jumpCoroutine != null)
            {
                StopCoroutine(_jumpCoroutine);
                _jumpCoroutine = null;
            }

            if (_movementDirection.Equals(Vector2Int.right))
            {
                _movementDirection = Vector2Int.up;
            }
            else
            {
                _movementDirection = Vector2Int.right;
            }

            Vector3 targetPosition = _gameGrid.GetNextCellPosition(transform.position, _movementDirection * 2);

            StartCoroutine(Jump(targetPosition, _influenceFlyDuration, _singleStepMaxAltitude, _influenceAltitudeCurve, OnLanded));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Respawn"))
            {
                _insideWater = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Respawn"))
            {
                _insideWater = false;
            }
        }

        private void OnLanded()
        {
            if (_insideWater)
            {
                
            }
            else
            {
                StepForward();
            }
        }
        
        private IEnumerator Jump(Vector2 targetPosition, float duration, float maxAltitude, AnimationCurve verticalCurve, Action landedCallback = null)
        {
            float curveDeltaTime = 1f / duration;
            Vector2 previousPosition = transform.position;
            float startAltitude = _body.localPosition.y;
            
            while (duration > 0)
            {
                float curveTime = 1f - duration * curveDeltaTime;
                
                float normalizedAltitude = verticalCurve.Evaluate(curveTime);
                float scaledAltitude = normalizedAltitude * maxAltitude;

                Vector2 resultPosition = Vector2.MoveTowards(previousPosition, targetPosition,
                    Vector2.Distance(previousPosition, targetPosition) * curveTime);

                transform.position = resultPosition;
                _body.localPosition = Vector3.up * (scaledAltitude + Mathf.Lerp(0f, startAltitude, 1f - curveTime));
                
                yield return new WaitForEndOfFrame();
                duration -= Time.deltaTime;
            }
            
            transform.position = targetPosition;
            _body.localPosition = Vector3.zero;
            
            landedCallback?.Invoke();
        }

        private Vector3 GetNextDestinationPoint()
        {
            Vector3 targetPosition = _gameGrid.GetNextCellPosition(transform.position, _movementDirection);
            return targetPosition;
        }

        private void StepForward()
        {
            _jumpCoroutine = StartCoroutine(Jump(GetNextDestinationPoint(), _singleStepDuration, _singleStepMaxAltitude, _singleStepAltitudeCurve,
                OnLanded));
        }
    }
}
