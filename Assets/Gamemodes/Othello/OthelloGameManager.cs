using System.Collections.Generic;
using Game;
using System.Linq;

namespace Gamemodes.Othello
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this, initialData);
        }
        public override int GetUID() => 1000;
        public override string GetName() => "Othello";
        public override string GetDescription()
        {
            return @"Othello

Supports 2, 3 and 4 teams with one player on each

Classic Othello";
        }
        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1), new TeamSize(0, 1), new TeamSize(0, 1) };
        public override string[] TeamAliases() => new string[] { "Black", "White", "Red", "Yellow" };
    }

    public class GameManager : AbstractGameManager
    {
        private int noMoves = 0;

        public GameManager(AbstractGameManagerData d, LiveGameData initialData) : base(d)
        {
            Board = new Board(this, initialData, initialData != null);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            noMoves = 0;
            base.OnMove(move, gameData);
            var team = gameData.CurrentTeam;
            while (true)
            {
                team++;
                if (team > 5) { team = 0; }
                var player = gameData.GetPlayerByTeam(team, 0);
                if (player != null) return (int)player;
            }
        }

        private int CountSquares()
        {
            var counts = new List<int>();

            for (var _ = 0; _ < 4; _++) { counts.Add(0); }

            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    if (Board.PieceBoard[x, y] is null) continue;
                    counts[Board.PieceBoard[x, y].Team]++;
                }
            }

            return GUtil.TurnEncodeTeam(counts.IndexOf(counts.Max()));
        }

        public override int OnNoMoves(LiveGameData gameData)
        {
            if (noMoves == gameData.GetPlayerList().Count) return CountSquares();

            noMoves += 1;
            var team = gameData.CurrentTeam;
            while (true)
            {
                team++;
                if (team > 5) { team = 0; }
                var player = gameData.GetPlayerByTeam(team, 0);
                if (player != null) return (int)player;
            }
        }

        public override AbstractGameManager Clone()
        {
            var new_game_manager = new GameManager(GameManagerData, null);
            new_game_manager.Board = ((Board) Board).Clone(new_game_manager);

            return new_game_manager;
        }
    }
}