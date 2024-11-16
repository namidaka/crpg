using System;
using System.Collections.Generic;
using System.Text;
using Crpg.Domain.Common;

namespace Crpg.Domain.Entities.GameServers;

public class IdempotencyKey : AuditableEntity
{
    public Guid Key { get; set; } = Guid.NewGuid();
    public UserUpdateStatus Status { get; set; }
}
