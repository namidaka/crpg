using Crpg.Domain.Entities.Battles;

namespace Crpg.Application.Common.Services;

internal interface IBattleScheduler
{
    Task ScheduleBattle(Battle battle);
}

internal class BattleScheduler : IBattleScheduler
{
    public Task ScheduleBattle(Battle battle)
    {
        // TODO

        // get the available periode of the 2 commanders
        List<BattleFighter> commanders = battle.Fighters.Where(f => f.Commander == true).ToList();

        TimeRange battleTimeRange = new TimeRange(
            new[] { commanders.First().Party.User.AvailablePeriode.Start, commanders.Last().Party.User..AvailablePeriode.Start }.Max()),
        new[] { commanders.First()!.Party!.User!.AvailablePeriode!.End, commanders.Last().Party.User..AvailablePeriode.End }.Min()
            );

        // get the non schudled battles
        List<Battle> SchudledBattles = new List<Battle>();

        return Task.CompletedTask;
    }
}

public struct Server
{
    public int ServerID;
    public List<Battle> SchudledBattles;
    public Dictionary<int, TimeRange> WeekSchudle;

    public Server(int serverID, Dictionary<int, TimeRange> weekSchudle)
    {
        ServerID = serverID;
        SchudledBattles = new List<Battle>();
        WeekSchudle = weekSchudle;
    }

    public override string ToString()
    {
        string serverString = ServerID.ToString() + " - ";
        foreach (KeyValuePair<int, TimeRange> day in WeekSchudle)
        {
            serverString += day.ToString() + " ";
        }

        return serverString;
    }
}
