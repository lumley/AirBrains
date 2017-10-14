using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(PlayerToGameStateControllerBridge))]
    public class AirConsoleMoveProvider : MoveProvider
    {
        
        public override void StartCollectingMoves()
        {
            // TODO (slumley): Ask player to clean their movements (turn start)
        }

        public override bool FinishedPlanningMoves()
        {
            // TODO (slumley): Is ready map!
            return false;
        }

        public override List<Move> GetPlannedMoves()
        {
            return new List<Move>(0);
        }

        // TODO (slumley): Assign itself to PlayerToGameStateControllerBridge callbacks
    }
}