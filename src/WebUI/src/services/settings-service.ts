import type { Setting, SettingEdition } from '~/models/setting'

import { del, get, post } from '~/services/crpg-client'

export const getSettings = () => get<Setting[]>('/settings')

export const setSetting = (setting: SettingEdition) => post('/settings', setting)

export const deleteSetting = (id: number) => del(`/settings/${id}`)
