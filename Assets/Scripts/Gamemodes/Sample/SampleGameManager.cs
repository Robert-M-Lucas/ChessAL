using System.Collections.Generic;
using System.Diagnostics;

public class SampleGameManagerData : AbstractGameManagerData
{
    public override AbstractGameManager Instantiate()
    {
        return new SampleGameManager(this);
    }

    public override int GetUID() => 1;

    public override string GetName() => "Sample Gamemode";

    public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 4) };
}

public class SampleGameManager : AbstractGameManager
{
    public SampleGameManager(AbstractGameManagerData d) : base(d)
    {
        Board = new SampleBoard();
    }

    public override void LoadData(byte[] data)
    {

    }

    public override List<Move> GetMoves()
    {
        return Board.GetMoves();
    }

    public override void OnForeignMove(MoveData moveData)
    {
        
    }
}