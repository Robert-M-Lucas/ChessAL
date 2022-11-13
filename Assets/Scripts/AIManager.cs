using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Random = System.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace AI
{
    public static class AIManager
    {
        /// <summary>
        /// Progress of finding a move. -1 indicates completion.
        /// </summary>
        public static float Progress { private set; get; } = -1f;

        private static Move? foundMove = null;

        private static Stopwatch Timer = new Stopwatch();

        public const int MAX_SEARCH_TIME = 25;

        /// <summary>
        /// Finds the best move for the AI to play next
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if move finding is already in progress</exception>
        public static void SearchMove(List<Move> possible_moves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            if (Progress >= 0f) throw new Exception("Move finding already in progress!");

            Progress = 0;

            new Thread(() => StartMoveSearch(possible_moves, initialGameData, gameManager)).Start();
        }

        private static bool IsOverTime()
        {
            return Timer.Elapsed.Seconds > MAX_SEARCH_TIME;
        }

        private static void StartMoveSearch(List<Move> possible_moves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            Timer.Restart();

            Move current_best_move = possible_moves[new Random().Next(0, possible_moves.Count - 1)];

            int depth = 0;

            Debug.Log(initialGameData.LocalPlayerTeam);

            while (true)
            {
                if (IsOverTime())
                {
                    foundMove = current_best_move;
                    Progress = -1f;
                    return;
                }

                float best_score = float.NegativeInfinity;
                Move best_move = possible_moves[0];

                
                foreach (Move move in possible_moves)
                {
                    AbstractGameManager cloned_game_manager = gameManager.Clone();
                    int next_turn = cloned_game_manager.OnMove(move, initialGameData);
                    float score = cloned_game_manager.GetScore(initialGameData);

                    if (next_turn < 0)
                    {
                        if (GUtil.TurnDecodeTeam(next_turn) != initialGameData.LocalPlayerTeam)
                        {
                            score = float.NegativeInfinity;
                        }
                        else
                        {
                            score = float.PositiveInfinity;
                        }
                    }

                    if (score > best_score)
                    {
                        best_move = move;
                    }
                }

                foundMove = best_move;
                Progress = -1f;
                return;

                depth++;
            }
        }

        public static Move? GetMove()
        {
            if (foundMove is not null)
            {
                Move new_move = new Move(((Move)foundMove).From, ((Move)foundMove).To);
                foundMove = null;
                return new_move;
            }

            return null;
        }
    }
}