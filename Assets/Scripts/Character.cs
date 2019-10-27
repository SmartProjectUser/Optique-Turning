using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    public class Character : MonoBehaviour
    {
        public bool IsDead => _isDead;
        public Vector2Int MovementDirection => _movementDirection;
        
        [SerializeField] private DirectionInfluence _directionInfluence;
        
        [SerializeField] private GameGrid _gameGrid;
        [SerializeField] private Transform _body;
        [SerializeField] private AnimationCurve _singleStepAltitudeCurve;
        [SerializeField] private AnimationCurve _influenceAltitudeCurve;
        [SerializeField] private AnimationCurve _respawnFallingSpeedCurve;
        [SerializeField] private float _respawnFallingDuration;
        [SerializeField] private float _singleStepDuration;
        [SerializeField] private float _influenceFlyDuration;
        [SerializeField] private float _singleStepMaxAltitude;
        [SerializeField] private int _influenceTilesCount = 2;

        [SerializeField] private int _respawnBackward = 5;

        private Vector2Int _movementDirection = Vector2Int.right;

        

        private Coroutine _jumpCoroutine = null;
        private bool _insideWater = false;
        private bool _isDead = false;

        

        private void Awake()
        {
            _directionInfluence.Influenced += OnReceiveInfluence;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StepForward();
            }
        }

        private void OnReceiveInfluence()
        {
            if (_isDead)
            {
                return;
            }
            
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

            Vector3 targetPosition = _gameGrid.GetNextCellPosition(transform.position, _movementDirection * _influenceTilesCount);

            StartCoroutine(Jump(targetPosition, _influenceFlyDuration, _singleStepMaxAltitude, _influenceAltitudeCurve, OnLanded));
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                _insideWater = false;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Ground"))
            {
                _insideWater = true;
            }
        }

        private void OnLanded()
        {
            if (_insideWater)
            {
                StartCoroutine(Death());
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

        private IEnumerator Death()
        {
            _isDead = true;
                
            yield return new WaitForSeconds(1f);


            StartCoroutine(Respawn());
        }

        private IEnumerator Respawn()
        {
            GameGrid.StepData stepData = _gameGrid.GetDataFromProgressStack(_respawnBackward);
            
            Vector3 respawnPosition = stepData.GetCellWorldPosition();
            _movementDirection = stepData.MovementDirection;
            _directionInfluence.UpdateHandSprite(_movementDirection);

            transform.position = respawnPosition;
            float startAltitude = 7;

            float time = _respawnFallingDuration;

            while (time > 0)
            {
                float curveTime = 1f - (1f / _respawnFallingDuration) * time;

                float currentAltitude = _respawnFallingSpeedCurve.Evaluate(curveTime) * startAltitude;

                _body.localPosition = Vector3.up * currentAltitude;
                
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
            }
            
            _body.localPosition = Vector3.zero;
            
            yield return new WaitForSeconds(.5f);

            _isDead = false;
            
            StepForward();
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
