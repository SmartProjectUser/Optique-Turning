using UnityEngine;

namespace OptiqueGames
{
    public interface IGameGrid
    {
        Vector3 GetNextCellPosition(Vector3 relativePosition, Vector2Int direction);
    }
}