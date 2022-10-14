using System.Collections.Generic;

public class SampleGameManagerData : AbstractGameManagerData
{
    public override AbstractGameManager Instantiate()
    {
        return new SampleGameManager(this);
    }

    public override int GetUID() => 1;

    public override string GetName() => "Sample Gamemode";

    public override TeamSize[] GetTeamSizes() => new TeamSize[0];
}

public class SampleGameManager : AbstractGameManager
{
    public SampleGameManager(AbstractGameManagerData d) : base(d)
    {
    }

    public override void LoadData(byte[] data)
    {

    }

    public override List<Move> GetMoves()
    {
        return new List<Move>();
    }

    public override void OnForeignMove(MoveData moveData)
    {
        
    }
}