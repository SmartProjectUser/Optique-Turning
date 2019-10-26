using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace OptiqueGames
{
    public class Character : MonoBehaviour
    {

        [SerializeField] private AnimationCurve _singleStepAltitudeCurve;
        [SerializeField] private float _singleStepDuration;
        [SerializeField] private float _singleStepMaxAltitude;
        
        [SerializeField] private GameGrid _gameGrid;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StepForward();
            }
        }

        private IEnumerator Jump(Vector2 targetPosition, float duration, float maxAltitude, AnimationCurve verticalCurve)
        {
            float curveDeltaTime = 1f / duration;
            Vector2 previousPosition = transform.position;
            
            while (duration > 0)
            {
                float curveTime = 1f - duration * curveDeltaTime;
                float normalizedAltitude = verticalCurve.Evaluate(curveTime);
                float scaledAltitude = normalizedAltitude * maxAltitude;

                Vector2 resultPosition = Vector2.MoveTowards(previousPosition, targetPosition, Vector2.Distance(previousPosition, targetPosition) * curveTime) +
                                         Vector2.up * scaledAltitude;

                transform.position = resultPosition;
                
                yield return new WaitForEndOfFrame();
                duration -= Time.deltaTime;
            }
        }

        private void StepForward()
        {
            Vector3 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartCoroutine(Jump(new Vector2(tapPosition.x, tapPosition.y), _singleStepDuration, _singleStepMaxAltitude,
                _singleStepAltitudeCurve));
        }
    }
}

