namespace Crpg.Domain.Entities.ActivityLogs;

/// <summary>
/// Represents state of a <see cref="ActivityLog"/>.
/// </summary>
public enum ActivityLogState
{
    /// <summary>
    /// Notification is not read by user yet.
    /// </summary>
    Unread,

    /// <summary>
    /// Notification is read by user.
    /// </summary>
    Read,
}
