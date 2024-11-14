import { useClanApplications } from './use-clan-applications'

const { CLAN_INVINTATIONS } = vi.hoisted(() => ({
  CLAN_INVINTATIONS: [{ id: 2 }, { id: 3 }],
}))
const { mockedGetClanInvitations } = vi.hoisted(() => ({
  mockedGetClanInvitations: vi.fn().mockResolvedValue(CLAN_INVINTATIONS),
}))
vi.mock('~/services/clan-service', () => ({
  getClanInvitations: mockedGetClanInvitations,
}))

it('initial state', () => {
  const { applications, applicationsCount } = useClanApplications()

  expect(applications.value).toEqual([])
  expect(applicationsCount.value).toEqual(0)
})

it('initial state TODO: change title', async () => {
  const { applications, applicationsCount, loadClanApplications } = useClanApplications()

  await loadClanApplications(0, { id: 2 })

  expect(applications.value).toEqual(CLAN_INVINTATIONS)
  expect(applicationsCount.value).toEqual(2)
  expect(mockedGetClanInvitations).toBeCalledWith(2, ['Request'], ['Pending'])
})
