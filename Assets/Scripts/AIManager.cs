using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

namespace AI
{
    public static class AIManager
    {
        public static Move GetMove(List<Move> possible_moves)
        {
            Random r = new Random();
            return possible_moves[r.Next(0, possible_moves.Count)];
        }
    }
}