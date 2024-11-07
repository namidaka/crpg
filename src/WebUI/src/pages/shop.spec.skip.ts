// TODO: FIXME:
import { createTestingPinia } from '@pinia/testing'

import type { UserItem } from '~/models/user'

import mockItems from '~/__mocks__/items.json'
import { mountWithRouter } from '~/__test__/unit/utils'
import { ItemType, WeaponClass } from '~/models/item'
import { useUserStore } from '~/stores/user'

import Page from './shop.vue'

const page = 1
const perPage = 15
const aggregationConfig = { handling: {}, price: {} }
const filterModel = { handling: [], price: [] }
const sortingConfig = {
  price_asc: {
    field: 'price_asc',
    order: 'asc',
  },
  price_desc: {
    field: 'price_desc',
    order: 'desc',
  },
}
const {
  mockAggregationsConfig,
  mockedGetCompareItemsResult,
  mockedGetItems,
  mockedGetSearchResult,
  mockResetFilter,
  mockToggleCompare,
  mockToggleToCompareList,
  mockUseItemsFilter,
  mockUseItemsSort,
  mockUsePagination,
} = vi.hoisted(() => ({
  mockAggregationsConfig: vi.fn(),
  mockedGetCompareItemsResult: vi.fn().mockReturnValue({}),
  mockedGetItems: vi.fn(),
  mockedGetSearchResult: vi.fn().mockReturnValue({
    data: {
      aggregations: {},
      items: [],
    },
    pagination: {
      page: 1,
      per_page: 1,
      total: 1,
    },
  }),
  mockResetFilter: vi.fn(),
  mockToggleCompare: vi.fn(),
  mockToggleToCompareList: vi.fn(),
  mockUseItemsFilter: vi.fn().mockImplementation(() => ({
    aggregationByClass: computed(() => ({ data: { buckets: [] } })),
    aggregationByType: computed(() => ({ data: { buckets: [] } })),
    aggregationsConfig: mockAggregationsConfig,
    aggregationsConfigVisible: computed(() => ({})),
    filteredByClassFlatItems: computed(() => []),
    filterModel: computed(() => filterModel),
    itemTypeModel: computed(() => ItemType.OneHandedWeapon),
    nameModel: computed(() => ''),
    resetFilters: mockResetFilter,
    scopeAggregations: computed(() => ({})),
    updateFilterModel: vi.fn(),
    weaponClassModel: computed(() => WeaponClass.OneHandedSword),
  })),
  mockUseItemsSort: vi.fn().mockImplementation(() => ({
    sortingConfig: computed(() => sortingConfig),
    sortingModel: computed(() => 'price_desc'),
  })),
  mockUsePagination: vi.fn().mockImplementation(() => ({
    pageModel: computed(() => 1),
    perPageConfig: computed(() => [10, 15, 20]),
    perPageModel: computed(() => 15),
  })),
}))

const { mockUseItemsCompare } = vi.hoisted(() => ({
  mockUseItemsCompare: vi.fn().mockImplementation(() => ({
    compareList: computed(() => []),
    isCompare: computed(() => false),
    toggleCompare: mockToggleCompare,
    toggleToCompareList: mockToggleToCompareList,
  })),
}))

vi.mock('~/services/item-service', () => ({
  getCompareItemsResult: mockedGetCompareItemsResult,
  getItems: mockedGetItems,
}))
vi.mock('~/services/item-search-service', () => ({
  getSearchResult: mockedGetSearchResult,
}))
vi.mock('~/composables/shop/use-filters', () => ({
  useItemsFilter: mockUseItemsFilter,
}))
vi.mock('~/composables/use-pagination', () => ({
  usePagination: mockUsePagination,
}))
vi.mock('~/composables/shop/use-sort', () => ({
  useItemsSort: mockUseItemsSort,
}))
vi.mock('~/composables/shop/use-compare', () => ({
  useItemsCompare: mockUseItemsCompare,
}))

beforeAll(() => {
  mockAggregationsConfig.mockReturnValue(computed(() => aggregationConfig))()
})

const userStore = useUserStore(createTestingPinia())

const routes = [
  {
    component: Page,
    name: 'shop',
    path: '/shop',
  },
]
const route = {
  name: 'shop',
  query: {},
}
const mountOptions = {
  global: {
    stubs: {
      OPagination: true,
      ShopGridFilters: true,
      ShopGridItem: true,
      ShopTypeSelect: true,
    },
  },
}

beforeAll(() => {
  mockedGetItems.mockResolvedValue(mockItems)
})

beforeEach(() => {
  userStore.$reset()
})

it.only('default state - empty query string', async () => {
  await mountWithRouter(mountOptions, routes, route)

  expect(mockedGetItems).toBeCalled()
  expect(userStore.fetchUserItems).toBeCalled()

  expect(mockUseItemsFilter).toBeCalledWith(mockItems)
  expect(mockUseItemsSort).toBeCalledWith(mockAggregationsConfig)
  expect(mockUsePagination).toBeCalled()
  expect(mockUseItemsCompare).toBeCalled()

  expect(mockedGetSearchResult).toBeCalledWith({
    aggregationConfig,
    filter: filterModel,
    items: [],
    page,
    perPage,
    query: '',
    sort: 'price_desc',
    sortingConfig,
  })
  expect(mockedGetCompareItemsResult).not.toBeCalled()
})

it('Reset filters', async () => {
  const { wrapper } = await mountWithRouter(mountOptions, routes, route)

  await wrapper.find('[data-aq-shop-handler="reset-filters"]').trigger('click')

  expect(mockResetFilter).toBeCalled()
})

describe('Toggle compare mode', () => {
  it('nothing to compare, the button is unavailable ;(', async () => {
    const originalMock = mockUseItemsCompare.getMockImplementation()!()
    mockUseItemsCompare.mockImplementation(() => ({
      ...originalMock,
      compareList: computed(() => []),
    }))

    const { wrapper } = await mountWithRouter(mountOptions, routes, route)
    expect(wrapper.find('[data-aq-shop-handler="toggle-compare"]').exists()).toBeFalsy()
  })

  it('there are a couple of items for comparison :)', async () => {
    const originalMock = mockUseItemsCompare.getMockImplementation()!()
    mockUseItemsCompare.mockImplementation(() => ({
      ...originalMock,
      compareList: computed(() => ['1', '2']),
    }))

    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    await wrapper.find('[data-aq-shop-handler="toggle-compare"]').trigger('click')

    expect(mockToggleCompare).toBeCalled()
  })
})

describe('shop item', () => {
  mockedGetSearchResult.mockReturnValue({
    data: {
      aggregations: {},
      items: [{ id: '1' }, { id: '2' }],
    },
    pagination: {
      page: 1,
      per_page: 1,
      total: 2,
    },
  })

  it('emit - buy', async () => {
    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const itemsComponents = wrapper.findAllComponents({ name: 'ShopGridItem' })

    itemsComponents.at(1)!.vm.$emit('buy')

    expect(userStore.buyItem).toBeCalledWith('2')
  })

  it('emit - select ', async () => {
    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const itemsComponents = wrapper.findAllComponents({ name: 'ShopGridItem' })

    itemsComponents.at(0)!.vm.$emit('select')

    expect(mockToggleToCompareList).toBeCalledWith('1')
  })

  it('pass prop - disabled', async () => {
    userStore.userItems = [{ item: { id: '2' } } as UserItem]
    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const itemsComponents = wrapper.findAllComponents({ name: 'ShopGridItem' })

    expect(itemsComponents.at(0)!.attributes('disabled')).toEqual('false')
    expect(itemsComponents.at(1)!.attributes('disabled')).toEqual('true')
  })

  it('pass prop - selected', async () => {
    const originalMock = mockUseItemsCompare.getMockImplementation()!()
    mockUseItemsCompare.mockImplementation(() => ({
      ...originalMock,
      compareList: computed(() => ['1']),
    }))

    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const itemsComponents = wrapper.findAllComponents({ name: 'ShopGridItem' })

    expect(itemsComponents.at(0)!.attributes('selected')).toEqual('true')
    expect(itemsComponents.at(1)!.attributes('selected')).toEqual('false')
  })

  it('pass prop - colsCount, fields', async () => {
    const originalMock = mockUseItemsFilter.getMockImplementation()!()
    mockUseItemsFilter.mockImplementation(() => ({
      ...originalMock,
      aggregationsConfigVisible: computed(() => ({
        handling: {},
        price: {},
      })),
    }))

    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const itemsComponent = wrapper.findComponent({ name: 'ShopGridItem' })

    expect(itemsComponent.attributes('colscount')).toEqual('2')
    expect(itemsComponent.attributes('fields')).toEqual('price,handling')
  })

  describe('pass prop - isCompare, comparedResult', () => {
    it('isCompare:false', async () => {
      const { wrapper } = await mountWithRouter(mountOptions, routes, route)

      const itemComponent = wrapper.findComponent({ name: 'ShopGridItem' })

      expect(itemComponent.attributes('iscompare')).toEqual('false')
      expect(itemComponent.attributes('comparedresult')).not.toBeDefined()
    })

    it('isCompare:true', async () => {
      const originalMock = mockUseItemsCompare.getMockImplementation()!()
      mockUseItemsCompare.mockImplementation(() => ({
        ...originalMock,
        isCompare: computed(() => true),
      }))

      const { wrapper } = await mountWithRouter(mountOptions, routes, route)

      const itemComponent = wrapper.findComponent({ name: 'ShopGridItem' })

      expect(itemComponent.attributes('iscompare')).toEqual('true')
      expect(itemComponent.attributes('comparedresult')).toEqual('[object Object]')
    })
  })
})

describe('pagination', () => {
  it('pass props', async () => {
    mockedGetSearchResult.mockReturnValue({
      data: {
        aggregations: {},
        items: [],
      },
      pagination: {
        page: 1,
        per_page: 25,
        total: 12,
      },
    })

    const { wrapper } = await mountWithRouter(mountOptions, routes, route)

    const paginationComponent = wrapper.findComponent({ name: 'OPagination' })

    expect(paginationComponent.attributes('total')).toEqual('12')
    expect(paginationComponent.attributes('perpage')).toEqual('25')
  })
})

it.todo('v-if="isPrimaryUsage in filterModel', () => {})
