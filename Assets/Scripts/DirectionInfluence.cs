using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    public class DirectionInfluence : MonoBehaviour
    {
        public event Action Influenced;
        
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _camera;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Influenced?.Invoke();
            }
        }
    }
}