import qs from 'qs';
import {
  ActivityLogMetadataDicts,
  ActivityLogType,
  type ActivityLog,
} from '@/models/activity-logs';
import { get } from '@/services/crpg-client';

export interface ActivityLogsPayload {
  from: Date;
  to: Date;
  userId: number[];
  type?: ActivityLogType[];
}

export const getActivityLogs = async (payload: ActivityLogsPayload) =>
  get<{ activityLogs: ActivityLog[]; dict: ActivityLogMetadataDicts }>(
    `/activity-logs?${qs.stringify(payload, {
      strictNullHandling: true,
      arrayFormat: 'brackets',
      skipNulls: true,
    })}`
  );
