using C2SProtoInterface;

namespace GameServer;

public class MatchManager:Single<MatchManager>
{
    private int matchId = 1;
    Dictionary<int,MatchAgent> matchAgents = new Dictionary<int,MatchAgent>();
    private Dictionary<int, MatchAgent> playerMatchAgents = new Dictionary<int, MatchAgent>();

    public int StartMatch(MatchInfo matchInfo)
    {
        var newMatch = new MatchAgent(matchInfo);
        newMatch.StartMatch(matchId++);
        matchAgents.Add(newMatch.matchGuid,newMatch);
        foreach (var plInfo in matchInfo.players)
        {
            playerMatchAgents.Add(plInfo.guid,newMatch);
        }
        return newMatch.matchGuid;
    }

    public void Update()
    {
        foreach (var agent in matchAgents.Values)
        {
            agent.Update();
        }
    }

    public void ReceiveUDPData(int guid,C2SFrameUpdate data)
    {
        if (playerMatchAgents.TryGetValue(guid,out var matchAgent))
        {
            matchAgent.ReceiveC2SFrameData(guid, data);
        }
    }
    
    public void ReqEndMatch(PlayerAgent player)
    {
        if (playerMatchAgents.TryGetValue(player.guid, out MatchAgent agent))
        {
            agent.EndMatch();
            matchAgents.Remove(agent.matchGuid);
            foreach (var plInfo in agent.matchInfo.players)
            {
                playerMatchAgents.Remove(plInfo.guid);
            }
        }
    }
    
    public MatchAgent GetMatchAgentByPlayerId(int guid)
    {
        if (playerMatchAgents.TryGetValue(guid,out MatchAgent agent))
            return agent;
        return null;
    }
}