using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public static class AIManager
    {
        public static Move GetMove(List<Move> possible_moves)
        {
            return possible_moves[0];
        }
    }
}