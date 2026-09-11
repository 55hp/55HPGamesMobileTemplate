using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Movement
{
    /// <summary>
    /// Seam for pathfinding, not an implementation — this phase only ships the contract.
    /// A project wires up NavMesh, A*, or anything else behind it and feeds the result into
    /// IMover.SetPath; the two stay decoupled so swapping pathfinders never touches movement.
    /// </summary>
    public interface IPathfinder
    {
        Vector3[] FindPath(Vector3 from, Vector3 to);

        void RequestPathAsync(Vector3 from, Vector3 to, Action<Vector3[]> onComplete);

        bool IsPathValid(Vector3[] path);
    }
}
