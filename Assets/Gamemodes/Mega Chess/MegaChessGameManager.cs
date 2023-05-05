using Game;

namespace Gamemodes.MegaChess
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }

        public override int GetUID() => 500;

        public override string GetName() => "Mega Chess";

        public override string GetDescription()
        {
            return @"Mega Chess

Must have one player on both the black and white team

Chess played on a 16x16 board with more pieces on each side";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }
    }

}

