import { NotificationProgrammatic } from '@oruga-ui/oruga-next'

export enum NotificationType {
  Success = 'success',
  Warning = 'warning',
  Danger = 'danger',
}

export const notify = (message: string, type: NotificationType = NotificationType.Success) => {
  NotificationProgrammatic.open({
    duration: 5000,
    message,
    position: 'top',
    queue: false,
    variant: type,
  })
}
