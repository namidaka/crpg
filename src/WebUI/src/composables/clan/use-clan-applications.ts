import { useAsyncState } from '@vueuse/core'

import { ClanInvitationStatus, ClanInvitationType } from '~/models/clan'
import { getClanInvitations } from '~/services/clan-service'

export const useClanApplications = () => {
  const { execute: loadClanApplications, state: applications } = useAsyncState(
    ({ id }: { id: number }) =>
      getClanInvitations(id, [ClanInvitationType.Request], [ClanInvitationStatus.Pending]),
    [],
    {
      immediate: false,
    },
  )

  const applicationsCount = computed(() => applications.value.length)

  return {
    applications,
    applicationsCount,
    loadClanApplications,
  }
}
