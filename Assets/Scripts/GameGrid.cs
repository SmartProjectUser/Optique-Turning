using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    [RequireComponent(typeof(Grid))]
    public class GameGrid : MonoBehaviour, IGameGrid
    {
        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }
        
        public Vector3 GetNextCellPosition(Vector3 relativePosition, Vector2Int direction)
        {
            Vector3Int currentCell = _grid.WorldToCell(relativePosition);
            return _grid.GetCellCenterWorld(currentCell + new Vector3Int(direction.x, direction.y, 0));
        }
    }
}

