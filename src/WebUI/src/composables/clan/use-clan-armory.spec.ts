import { useClanArmory } from './use-clan-armory'

const {
  mockedAddItemToClanArmory,
  mockedBorrowItemFromClanArmory,
  mockedGetClanArmory,
  mockedRemoveItemFromClanArmory,
  mockedReturnItemToClanArmory,
} = vi.hoisted(() => ({
  mockedAddItemToClanArmory: vi.fn(),
  mockedBorrowItemFromClanArmory: vi.fn(),
  mockedGetClanArmory: vi.fn().mockResolvedValue([{ id: 1 }]),
  mockedRemoveItemFromClanArmory: vi.fn(),
  mockedReturnItemToClanArmory: vi.fn(),
}))
vi.mock('~/services/clan-service', () => ({
  addItemToClanArmory: mockedAddItemToClanArmory,
  borrowItemFromClanArmory: mockedBorrowItemFromClanArmory,
  getClanArmory: mockedGetClanArmory,
  removeItemFromClanArmory: mockedRemoveItemFromClanArmory,
  returnItemToClanArmory: mockedReturnItemToClanArmory,
}))

const clanId = 1

it('clanArmory', async () => {
  const { clanArmory, isLoadingClanArmory, loadClanArmory } = useClanArmory(clanId)

  expect(mockedGetClanArmory).not.toBeCalled()
  expect(clanArmory.value).toEqual([])
  expect(isLoadingClanArmory.value).toEqual(false)

  await loadClanArmory()

  expect(mockedGetClanArmory).toBeCalled()
  expect(clanArmory.value).toEqual([{ id: 1 }])
})

it('addItem', async () => {
  const itemId = 1
  const { addItem } = useClanArmory(clanId)

  await addItem(itemId)

  expect(mockedAddItemToClanArmory).toBeCalledWith(clanId, itemId)
})

it('removeItem', async () => {
  const itemId = 1
  const { removeItem } = useClanArmory(clanId)

  await removeItem(itemId)

  expect(mockedRemoveItemFromClanArmory).toBeCalledWith(clanId, itemId)
})

it('borrowItem', async () => {
  const itemId = 1
  const { borrowItem } = useClanArmory(clanId)

  await borrowItem(itemId)

  expect(mockedBorrowItemFromClanArmory).toBeCalledWith(clanId, itemId)
})

it('returnItem', async () => {
  const itemId = 1
  const { returnItem } = useClanArmory(clanId)

  await returnItem(itemId)

  expect(mockedReturnItemToClanArmory).toBeCalledWith(clanId, itemId)
})
