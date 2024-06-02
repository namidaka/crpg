using System;
using System.Collections.Generic;
using System.Text;

namespace Crpg.Domain.Entities.GameServers;

public class IdempotencyKey
{
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserUpdateStatus Status { get; set; }
}
