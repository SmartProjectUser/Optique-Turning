using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    public class CameraMotor : MonoBehaviour
    {
        [SerializeField] private Vector2 _positionOffset;
        [SerializeField] private Transform _target;

        private Vector2 _velocity;

        private void Update()
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y) - _positionOffset;
            position = Vector2.SmoothDamp(position, _target.position, ref _velocity, 0.5f);
            //Vector3 position = Vector3.SmoothDamp(transform.position - _positionOffset, _target.position, ref _velocity, 0.5f);
            position += _positionOffset;

            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
}

