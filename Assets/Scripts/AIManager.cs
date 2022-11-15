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

        public const int MAX_SEARCH_TIME = 10;

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

        private static void UpdateProgress()
        {
            Progress = Mathf.Clamp((float) (Timer.Elapsed.TotalMilliseconds / (MAX_SEARCH_TIME * 1000f)) * 100f, 0, 100);
        }

        private static void StartMoveSearch(List<Move> possible_moves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            Timer.Restart();

            Move current_best_move = possible_moves[new Random().Next(0, possible_moves.Count - 1)];

            if (possible_moves.Count == 1)
            {
                foundMove = current_best_move;
                Progress = -1f;
                return;
            }

            int depth = 0;

            while (true)
            {
                Debug.Log($"AI Depth: {depth}");

                if (IsOverTime())
                {
                    Debug.Log("Over time");
                    foundMove = current_best_move;
                    Progress = -1f;
                    return;
                }

                float best_score = float.NegativeInfinity;
                Move best_move = possible_moves[0];
                
                foreach (Move move in possible_moves)
                {
                    UpdateProgress();

                    AbstractGameManager cloned_game_manager = gameManager.Clone();
                    int next_turn = cloned_game_manager.OnMove(move, initialGameData);
                    // float score = cloned_game_manager.GetScore(initialGameData);
                    float score;

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
                    else
                    {
                        score = RecursivelySearch(cloned_game_manager, initialGameData, cloned_game_manager.GetMoves(initialGameData), 0, depth);
                    }

                    if (score > best_score)
                    {
                        best_score = score;
                        best_move = move;
                    }
                }

                if (!IsOverTime())
                {
                    current_best_move = best_move;
                    depth++;
                }
            }
        }

        
        private static float RecursivelySearch(AbstractGameManager gameManager, LiveGameData gameData, List<Move> moves, int depth, int max_depth)
        {
            if (depth == max_depth)
            {
                return gameManager.GetScore(gameData);
            }

            float best_score;
            bool maximising;
            if (gameData.CurrentPlayer == gameData.LocalPlayerID || gameData.GetPlayerList()[gameData.LocalPlayerID] == gameData.GetPlayerList()[gameData.CurrentPlayer])
            {
                best_score = float.NegativeInfinity;
                maximising = true;
            }
            else
            {
                best_score = float.PositiveInfinity;
                maximising = false;
            }

            foreach (Move m in moves)
            {
                if (IsOverTime()) return 0;

                AbstractGameManager new_manager = gameManager.Clone();
                int next_player = new_manager.OnMove(m, gameData);

                if (next_player < 0)
                {
                    if (GUtil.TurnDecodeTeam(next_player) != gameData.LocalPlayerTeam)
                    {
                        if (maximising) return float.NegativeInfinity;
                        else return float.PositiveInfinity;
                    }
                    else
                    {
                        if (maximising) return float.PositiveInfinity;
                        else return float.NegativeInfinity;
                    }
                }

                LiveGameData new_game_data = gameData.Clone();
                new_game_data.CurrentPlayer = next_player;
                List<Move> new_moves = new_manager.GetMoves(new_game_data);
                float score = RecursivelySearch(new_manager, new_game_data, new_moves, depth + 1, max_depth);

                if (maximising && score > best_score) best_score = score;
                else if (!maximising && score < best_score) best_score = score;
            }

            return best_score;
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