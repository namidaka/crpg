namespace Crpg.Domain.Entities.Notification;

/// <summary>
/// Represents state of a <see cref="Notification"/>.
/// </summary>
public enum NotificationState
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
