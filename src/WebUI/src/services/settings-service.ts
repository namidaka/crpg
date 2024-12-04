import type { Settings } from '~/models/setting'

import { get, patch } from '~/services/crpg-client'

export const getSettings = () => get<Settings>('/settings')

export const editSettings = (setting: Partial<Settings>) => patch('/settings', setting)
