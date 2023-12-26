import { getClanMembers } from '@/services/clan-service';

export const useClanMembers = (clanId?: number) => {
  const { state: clanMembers, execute: loadClanMembers } = useAsyncState(
    () => getClanMembers(clanId!),
    [],
    {
      immediate: false,
    }
  );

  const clanMembersCount = computed(() => clanMembers.value.length);
  const isLastMember = computed(() => clanMembersCount.value <= 1);

  return {
    clanMembers,
    loadClanMembers,
    clanMembersCount,
    isLastMember,
  };
};
