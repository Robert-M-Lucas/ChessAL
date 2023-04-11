using Gamemodes;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Random = System.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Game;

namespace AI
{
    public static class AIManager
    {
        /// <summary>
        /// Progress of finding a move. -1 indicates completion.
        /// </summary>
        public static float Progress { private set; get; } = -1f;

        private static Move? foundMove = null;

        private static readonly Stopwatch TIMER = new Stopwatch();

        private const int MAX_SEARCH_TIME = 10;

        private static Thread searchThread = null;

        // private static float?[] threadResults;

        /// <summary>
        /// Finds the best move for the AI to play next
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if move finding is already in progress</exception>
        public static void SearchMove(List<Move> possibleMoves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            if (Progress >= 0f) throw new Exception("Move finding already in progress!");

            Progress = 0;

            searchThread = new Thread(() => StartMoveSearch(possibleMoves, initialGameData, gameManager));
            searchThread.Start();
        }

        /// <summary>
        /// Resets the AI Manager
        /// </summary>
        public static void Reset()
        {
            Progress = -1f;
            foundMove = null;
            TIMER.Reset();
            if (searchThread is not null && searchThread.IsAlive) searchThread.Abort();
            searchThread = null;
        }

        /// <summary>
        /// Returns true if the AI has exceeded the MAX_SEARCH_TIME
        /// </summary>
        /// <returns></returns>
        private static bool IsOverTime()
        {
            var result = TIMER.Elapsed.Seconds > MAX_SEARCH_TIME;
            return result;
        }

        /// <summary>
        /// Updates the progress tracker
        /// </summary>
        private static void UpdateProgress()
        {
            // Convert to percentage
            Progress = Mathf.Clamp((float) (TIMER.Elapsed.TotalMilliseconds / (MAX_SEARCH_TIME * 1000f)) * 100f, 0, 100);
        }

        /*
        struct MiniMaxStruct
        {
            public AbstractGameManager gameManager;
            public LiveGameData gameData;
            public List<Move> moves;
            public int current_depth;
            public int max_depth;
            public float prev_best;
            public bool prev_maximising;
            public int id;

            public MiniMaxStruct(AbstractGameManager gameManager, LiveGameData gameData, List<Move> moves, int current_depth, int max_depth, float prev_best, bool prev_maximising, int id)
            {
                this.gameManager = gameManager;
                this.gameData = gameData;
                this.moves = moves;
                this.current_depth = current_depth;
                this.max_depth = max_depth;
                this.prev_best = prev_best;
                this.prev_maximising = prev_maximising;
                this.id = id;
            }
        }
        */

        /// <summary>
        /// Starts looking for a move
        /// </summary>
        /// <param name="possibleMoves"></param>
        /// <param name="initialGameData"></param>
        /// <param name="gameManager"></param>
        private static void StartMoveSearch(List<Move> possibleMoves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            try
            {
                Debug.Log($"Starting with search time {MAX_SEARCH_TIME}");

                TIMER.Restart();

                // Select random move by default
                var current_best_move = possibleMoves[new Random().Next(0, possibleMoves.Count - 1)];

                // Only one possible move
                if (possibleMoves.Count == 1)
                {
                    foundMove = current_best_move;
                    Progress = -1f;
                    return;
                }

                var max_depth = 2;

                while (true)
                {
                    Debug.Log($"AI Depth: {max_depth + 1}");

                    if (IsOverTime())
                    {
                        Debug.Log("Over time");
                        foundMove = current_best_move;
                        Progress = -1f;
                        return;
                    }

                    var best_score = float.MinValue;
                    var best_move = possibleMoves[0];
                    var cancelled = false;

                    // threadResults = new float?[possible_moves.Count];
                    // for (int i = 0; i < possible_moves.Count; i++) { threadResults[i] = null; }
                    // int id = -1;

                    foreach (var move in possibleMoves)
                    {
                        // id++;
                        UpdateProgress();
                        
                        
                        // Test new move
                        var new_manager = gameManager.Clone();
                        var next_turn = new_manager.OnMove(move, initialGameData);
                        float score;

                        if (next_turn < 0) // If a team has won
                        {
                            if (GUtil.TurnDecodeTeam(next_turn) != initialGameData.LocalPlayerTeam)
                            {
                                score = float.MinValue;
                            }
                            else
                            {
                                // ReSharper disable once RedundantAssignment
                                best_score = float.MaxValue;
                                best_move = move;
                                break;
                            }
                        }
                        else
                        {
                            // Clone game state
                            var new_game_data = initialGameData.Clone();
                            new_game_data.CurrentPlayer = next_turn; // Set new player

                            // ThreadPool.QueueUserWorkItem(new WaitCallback(RunMiniMax),
                            //          new MiniMaxStruct(new_manager, new_game_data, new_manager.GetMoves(new_game_data), 1, max_depth, best_score, true, id));
                            // continue;

                            // Recursively search for moves
                            score = MiniMax(new_manager, new_game_data, new_manager.GetMoves(new_game_data, fastMode: true), 1, max_depth, best_score, true);

                            // AI terminated early
                            if (score is float.NaN)
                            {
                                cancelled = true;
                                break;
                            }
                        }
                        // continue;

                        if (!(score >= best_score)) continue;
                        best_score = score;
                        best_move = move;
                    }

                    /*
                    bool complete = false;
                    while (!complete)
                    {
                        complete = true;
                        for (int i = 0; i < possible_moves.Count; i++)
                        {
                            if (threadResults[i] == null)
                            {
                                complete = false;
                                break;
                            }
                        }
                    }

                    float max = float.NegativeInfinity;
                    int max_pos = 0;
                    for (int i = 0; i< possible_moves.Count; i++)
                    {
                        if (threadResults[i] is float.NaN)
                        {
                            cancelled = true;
                            break;
                        }
                        else if (threadResults[i] > max)
                        {
                            max = (float) threadResults[i];
                            max_pos = i;
                        }
                    }
                    */

                    // Update current best if search has not been cancelled
                    if (!cancelled)
                    {
                        current_best_move = best_move;
                        // current_best_move = possible_moves[max_pos];
                        max_depth++;
                    }
                        
                    }
                }
            catch (Exception e) { Debug.LogException(e); }
        }

        /*
        private static void RunMiniMax(object state)
        {
            var mmstruct = (MiniMaxStruct) state;
            Debug.Log($"Thread {mmstruct.id}");
            threadResults[mmstruct.id] = MiniMax(mmstruct.gameManager, mmstruct.gameData, mmstruct.moves, mmstruct.current_depth, mmstruct.max_depth, mmstruct.prev_best, mmstruct.prev_maximising);

        }
        */

        /// <summary>
        /// Implementation of the MiniMax algorithm with AB pruning
        /// </summary>
        /// <param name="gameManager"></param>
        /// <param name="gameData"></param>
        /// <param name="moves"></param>
        /// <param name="currentDepth"></param>
        /// <param name="maxDepth"></param>
        /// <param name="prevBest"></param>
        /// <param name="prevMaximising"></param>
        /// <returns></returns>
        private static float MiniMax(AbstractGameManager gameManager, LiveGameData gameData, List<Move> moves, int currentDepth, int maxDepth, float prevBest, bool prevMaximising)
        {
            // Return if at max depth
            if (currentDepth == maxDepth) return gameManager.GetScore(gameData);

            float best_score;
            bool maximising;

            // Set maximising or minimising
            if (gameData.CurrentTeam == gameData.LocalPlayerTeam)
            {
                best_score = float.MinValue;
                maximising = true;
            }
            else
            {
                best_score = float.MaxValue;
                maximising = false;
            }

            foreach (var move in moves)
            {
                // Exit if over time
                if ((currentDepth == maxDepth - 1 || currentDepth == maxDepth) && IsOverTime()) return float.NaN;

                // Apply move
                var new_manager = gameManager.Clone();       
                var next_player = new_manager.OnMove(move, gameData);

                float move_score;

                if (next_player < 0) // A team has won
                {
                    if (GUtil.TurnDecodeTeam(next_player) != gameData.LocalPlayerTeam)
                    {
                        if (maximising) move_score = float.MinValue + currentDepth;
                        else return float.MinValue + currentDepth; // Best possible value for minimiser so instantly return
                    }
                    else
                    {
                        if (maximising) return float.MaxValue - currentDepth; // Best possible value for maximiser so instantly return
                        else move_score = float.MaxValue - currentDepth;
                    }
                }
                else
                {
                    // Set move to next player
                    var new_game_data = gameData.Clone();
                    new_game_data.CurrentPlayer = next_player;

                    var new_moves = new_manager.GetMoves(new_game_data, fastMode: true);

                    // Recursion
                    move_score = MiniMax(new_manager, new_game_data, new_moves, currentDepth + 1, maxDepth, best_score, maximising);
                }

                

                // Return if algorithm is terminating
                if (move_score is float.NaN) return float.NaN;

                if (maximising && move_score > best_score) best_score = move_score;
                else if (!maximising && move_score < best_score) best_score = move_score;

                
                // AB Pruning
                if ((best_score > prevBest && !prevMaximising && maximising) || (best_score <  prevBest && prevMaximising && !maximising))
                {
                    return best_score;
                }
                
            }

            return best_score;
        }
        
        /// <summary>
        /// Returns a move if one has been found or null if the search is ongoing
        /// </summary>
        /// <returns></returns>
        public static Move? GetMove()
        {
            if (foundMove is not null)
            {
                Debug.Log("Found move returned");

                var new_move = new Move(((Move)foundMove).From, ((Move)foundMove).To); // Copy move
                foundMove = null;
                return new_move;
            }

            return null;
        }
    }
}