import { useClan } from './use-clan'

const { mockedGetClan } = vi.hoisted(() => ({
  mockedGetClan: vi.fn().mockResolvedValue({ id: 2, tag: 'mlp' }),
}))
vi.mock('~/services/clan-service', () => ({
  getClan: mockedGetClan,
}))

it('initial state', () => {
  const { clan, clanId } = useClan('2')

  expect(clanId.value).toBe(2)
  expect(clan.value).toEqual(null)
})

it('load clan', async () => {
  const { clan, clanId, loadClan } = useClan('2')

  await loadClan(0, { id: clanId.value })
  expect(clan.value).toEqual({ id: 2, tag: 'mlp' })
  expect(mockedGetClan).toBeCalledWith(2)
})
