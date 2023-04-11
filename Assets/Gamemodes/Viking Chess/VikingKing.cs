namespace Gamemodes.VikingChess
{
    public class VikingKing : VikingPiece
    {
        public VikingKing(V2 position, int team, AbstractBoard board): base(position, team, board)
        {
            AppearanceID = 101;
        }

        public override int GetUID() => 801;

        public override AbstractPiece Clone(AbstractBoard newBoard) => new VikingKing(Position, Team, newBoard);
    }

}
