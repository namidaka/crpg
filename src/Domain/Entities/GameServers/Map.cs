namespace Crpg.Domain.Entities.GameServers;
public class Map
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Name { get; set; }
    public int GameServerConfigId { get; set; }
    public GameServerConfig GameServerConfig { get; set; } = new GameServerConfig();
}
