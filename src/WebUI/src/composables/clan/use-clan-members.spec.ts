import { useClanMembers } from './use-clan-members'

const { mockedGetClanMembers } = vi.hoisted(() => ({
  mockedGetClanMembers: vi.fn().mockResolvedValue([{ user: { id: 1 } }, { user: { id: 11 } }]),
}))
vi.mock('~/services/clan-service', () => ({
  getClanMembers: mockedGetClanMembers,
}))

it('initial state', () => {
  const { clanMembers, clanMembersCount, isLastMember } = useClanMembers()

  expect(clanMembers.value).toEqual([])
  expect(clanMembersCount.value).toEqual(0)
  expect(isLastMember.value).toEqual(true)
})

it('load clan', async () => {
  const { clanMembers, clanMembersCount, isLastMember, loadClanMembers } = useClanMembers()

  await loadClanMembers(0, { id: 1 })
  expect(clanMembers.value).toEqual([{ user: { id: 1 } }, { user: { id: 11 } }])
  expect(clanMembersCount.value).toEqual(2)
  expect(isLastMember.value).toEqual(false)
})
