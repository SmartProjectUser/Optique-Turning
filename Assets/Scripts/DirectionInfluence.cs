using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DirectionInfluence : MonoBehaviour
    {
        public event Action Influenced;

        [SerializeField] private Character _targetCharacter;
        [SerializeField] private float _distanceToTarget;
        [SerializeField] private float _hideDuration = 0.7f;
        [SerializeField] private float _hideDistance = 3;

        [SerializeField] private Sprite _normalHandSprite;
        [SerializeField] private Sprite _switchedHandSprite;

        private Vector2 _velocity;
        private bool _isHidden = false;
        private SpriteRenderer _renderer;
        private bool _hasStarted = false;


        public void UpdateHandSprite(Vector2Int direction)
        {
            int dir = direction == Vector2Int.right ? -1 : 1;
            
            
            
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Abs(transform.localEulerAngles.z) * dir);
        }
        
        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            if (_isHidden || _targetCharacter.IsDead)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (_hasStarted)
                {
                    OnInfluenced();
                }
                else
                {
                    _hasStarted = true;
                }
            }

            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            position = Vector2.SmoothDamp(position,
                _targetCharacter.transform.position - transform.up * _distanceToTarget, ref _velocity, 0.15f);
            
            transform.position = new Vector3(position.x, position.y, 0);


        }

        private void OnInfluenced()
        {
            Influenced?.Invoke();

            _renderer.sprite = _switchedHandSprite;
            
            StartCoroutine(Hide());
        }

        private IEnumerator Hide()
        {
            _isHidden = true;

            yield return new WaitForSeconds(0.2f);
            
            float time = _hideDuration;

            while (time > 0)
            {
                transform.position -= transform.up * _hideDistance * Time.deltaTime;
                time -= Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }
            
            _isHidden = false;

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -transform.localEulerAngles.z);
            
            _velocity = Vector2.zero;

            _renderer.sprite = _normalHandSprite;
        }
    }
}