using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using Random = System.Random;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.SocialPlatforms.Impl;
using JetBrains.Annotations;
using System.Linq;

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

        public static int MAX_SEARCH_TIME = 12;

        private static Thread searchThread = null;

        // private static float?[] threadResults;

        /// <summary>
        /// Finds the best move for the AI to play next
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if move finding is already in progress</exception>
        public static void SearchMove(List<Move> possible_moves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            if (Progress >= 0f) throw new Exception("Move finding already in progress!");

            Progress = 0;

            searchThread = new Thread(() => StartMoveSearch(possible_moves, initialGameData, gameManager));
            searchThread.Start();
        }

        /// <summary>
        /// Resets the AI Manager
        /// </summary>
        public static void Reset()
        {
            Progress = -1f;
            foundMove = null;
            Timer.Reset();
            if (searchThread is not null && searchThread.IsAlive) searchThread.Abort();
            searchThread = null;
        }

        /// <summary>
        /// Returns true if the AI has exceeded the MAX_SEARCH_TIME
        /// </summary>
        /// <returns></returns>
        private static bool IsOverTime()
        {
            bool result = Timer.Elapsed.Seconds > MAX_SEARCH_TIME;
            return result;
        }

        /// <summary>
        /// Updates the progress tracker
        /// </summary>
        private static void UpdateProgress()
        {
            // Convert to percentage
            Progress = Mathf.Clamp((float) (Timer.Elapsed.TotalMilliseconds / (MAX_SEARCH_TIME * 1000f)) * 100f, 0, 100);
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
        /// <param name="possible_moves"></param>
        /// <param name="initialGameData"></param>
        /// <param name="gameManager"></param>
        private static void StartMoveSearch(List<Move> possible_moves, LiveGameData initialGameData, AbstractGameManager gameManager)
        {
            try
            {
                Debug.Log($"Starting with search time {MAX_SEARCH_TIME}");

                Timer.Restart();

                // Select random move by default
                Move current_best_move = possible_moves[new Random().Next(0, possible_moves.Count - 1)];

                // Only one possible move
                if (possible_moves.Count == 1)
                {
                    foundMove = current_best_move;
                    Progress = -1f;
                    return;
                }

                int max_depth = 2;

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

                    float best_score = float.MinValue;
                    Move best_move = possible_moves[0];
                    bool cancelled = false;

                    // threadResults = new float?[possible_moves.Count];
                    // for (int i = 0; i < possible_moves.Count; i++) { threadResults[i] = null; }
                    // int id = -1;

                    foreach (Move move in possible_moves)
                    {
                        // id++;
                        UpdateProgress();
                        
                        
                        // Test new move
                        AbstractGameManager new_manager = gameManager.Clone();
                        int next_turn = new_manager.OnMove(move, initialGameData);
                        float score;

                        if (next_turn < 0) // If a team has won
                        {
                            if (GUtil.TurnDecodeTeam(next_turn) != initialGameData.LocalPlayerTeam)
                            {
                                score = float.MinValue;
                            }
                            else
                            {
                                best_score = float.MaxValue;
                                best_move = move;
                                break;
                            }
                        }
                        else
                        {
                            // Clone game state
                            LiveGameData new_game_data = initialGameData.Clone();
                            new_game_data.CurrentPlayer = next_turn; // Set new player

                            // ThreadPool.QueueUserWorkItem(new WaitCallback(RunMiniMax),
                            //          new MiniMaxStruct(new_manager, new_game_data, new_manager.GetMoves(new_game_data), 1, max_depth, best_score, true, id));
                            // continue;

                            // Recursively search for moves
                            score = MiniMax(new_manager, new_game_data, new_manager.GetMoves(new_game_data), 1, max_depth, best_score, true);

                            // AI terminated early
                            if (score is float.NaN)
                            {
                                cancelled = true;
                                break;
                            }
                        }
                        // continue;

                        if (score >= best_score)
                        {
                            best_score = score;
                            best_move = move;
                        }
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
        /// <param name="current_depth"></param>
        /// <param name="max_depth"></param>
        /// <param name="prev_best"></param>
        /// <param name="prev_maximising"></param>
        /// <returns></returns>
        private static float MiniMax(AbstractGameManager gameManager, LiveGameData gameData, List<Move> moves, int current_depth, int max_depth, float prev_best, bool prev_maximising)
        {
            // Return if at max depth
            if (current_depth == max_depth) return gameManager.GetScore(gameData);

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

            foreach (Move move in moves)
            {
                // Exit if over time
                if ((current_depth == max_depth - 1 || current_depth == max_depth) && IsOverTime()) return float.NaN;

                // Apply move
                AbstractGameManager new_manager = gameManager.Clone();       
                int next_player = new_manager.OnMove(move, gameData);

                float score;

                if (next_player < 0) // A team has won
                {
                    if (GUtil.TurnDecodeTeam(next_player) != gameData.LocalPlayerTeam)
                    {
                        if (maximising) score = float.MinValue;
                        else return float.MinValue; // Best possible value for minimiser so instantly return
                    }
                    else
                    {
                        if (maximising) return float.MaxValue; // Best possible value for maximiser so instantly return
                        else score = float.MinValue;
                    }
                }
                else
                {
                    // Set move to next player
                    LiveGameData new_game_data = gameData.Clone();
                    new_game_data.CurrentPlayer = next_player;

                    List<Move> new_moves = new_manager.GetMoves(new_game_data);

                    // Recursion
                    score = MiniMax(new_manager, new_game_data, new_moves, current_depth + 1, max_depth, best_score, maximising);
                }

                

                // Return if algorithm is terminating
                if (score is float.NaN) return float.NaN;

                if (maximising && score > best_score) best_score = score;
                else if (!maximising && score < best_score) best_score = score;

                
                // AB Pruning
                if ((best_score > prev_best && !prev_maximising && maximising) || (best_score <  prev_best && prev_maximising && !maximising))
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

                Move new_move = new Move(((Move)foundMove).From, ((Move)foundMove).To); // Copy move
                foundMove = null;
                return new_move;
            }

            return null;
        }
    }
}