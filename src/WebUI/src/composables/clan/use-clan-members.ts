import { getClanMembers } from '~/services/clan-service'

export const useClanMembers = () => {
  const { execute: loadClanMembers, state: clanMembers } = useAsyncState(
    ({ id }: { id: number }) => getClanMembers(id),
    [],
    {
      immediate: false,
    },
  )

  const clanMembersCount = computed(() => clanMembers.value.length)
  const isLastMember = computed(() => clanMembersCount.value <= 1)

  return {
    clanMembers,
    clanMembersCount,
    isLastMember,
    loadClanMembers,
  }
}
