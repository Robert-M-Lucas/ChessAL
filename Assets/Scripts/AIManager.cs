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
        /// <summary>
        /// Progress of finding a move. -1 indicates completion.
        /// </summary>
        public static float Progress = -1f;

        /// <summary>
        /// Finds the best move for the AI to play next
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if move finding is already in progress</exception>
        public static Move GetMove(List<Move> possible_moves, LiveGameData initialGameData)
        {
            if (Progress >= 0f) throw new Exception("Move finding already in progress!");

            Progress = 0;

            Random r = new Random();

            Progress = -1f;

            return possible_moves[r.Next(0, possible_moves.Count)];
        }
    }
}