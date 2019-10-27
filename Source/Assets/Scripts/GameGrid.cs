using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OptiqueGames
{
    [RequireComponent(typeof(Grid))]
    public class GameGrid : MonoBehaviour, IGameGrid
    {
        private Grid _grid;
        private static GameGrid _instance;

        private Stack<StepData> _progressStack;

        public struct StepData
        {
            public Vector3Int CellCoordinates;
            public Vector2Int MovementDirection;

            public Vector3 GetCellWorldPosition()
            {
                return _instance._grid.GetCellCenterWorld(CellCoordinates);
            }
        }

        private void Awake()
        {
            _instance = this;
            _grid = GetComponent<Grid>();
            _progressStack = new Stack<StepData>();
        }

        public StepData GetDataFromProgressStack(int depth)
        {
            for (int i = 0; i < depth; ++i)
            {
                _progressStack.Pop();
            }
            
            return _progressStack.Pop();
        }

//        public Vector3 GetCellPositionFromProgressStack(int depth)
//        {
//            for (int i = 0; i < depth; ++i)
//            {
//                _progressStack.Pop();
//            }
//            
//            return _grid.GetCellCenterWorld(_progressStack.Pop().CellCoordinates);
//        }
        
        public Vector3 GetNextCellPosition(Vector3 relativePosition, Vector2Int direction)
        {
            Vector3Int currentCell = _grid.WorldToCell(relativePosition);
            Vector3Int nextCell = currentCell + new Vector3Int(direction.x, direction.y, 0);

            _progressStack.Push(new StepData {CellCoordinates = nextCell, MovementDirection = direction});
            
            return _grid.GetCellCenterWorld(nextCell);
        }
    }
}

