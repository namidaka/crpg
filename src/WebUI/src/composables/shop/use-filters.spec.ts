import { type Item, ItemType, WeaponClass } from '~/models/item'

import { useItemsFilter } from './use-filters'

const { mockedPush, mockedUseRoute } = vi.hoisted(() => ({
  mockedPush: vi.fn(),
  mockedUseRoute: vi.fn(),
}))
const { mockedUseRouter } = vi.hoisted(() => ({
  mockedUseRouter: vi.fn().mockImplementation(() => ({
    push: mockedPush,
  })),
}))
vi.mock('vue-router', () => ({
  useRoute: mockedUseRoute,
  useRouter: mockedUseRouter,
}))
vi.mock('@vueuse/core', () => ({
  useDebounceFn: vi.fn(fn => fn),
}))
const { mockedGetWeaponClassesByItemType } = vi.hoisted(() => ({
  mockedGetWeaponClassesByItemType: vi.fn().mockReturnValue([]),
}))
vi.mock('~/services/item-service', async () => ({
  ...(await vi.importActual<typeof import('~/services/item-service')>('~/services/item-service')),
  getWeaponClassesByItemType: mockedGetWeaponClassesByItemType,
}))

const {
  mockedFilterItemsByType,
  mockedFilterItemsByWeaponClass,
  mockedGenerateEmptyFiltersModel,
  mockedGetAggregationBy,
  mockedGetAggregationsConfig,
  mockedGetScopeAggregations,
  mockedGetVisibleAggregationsConfig,
} = vi.hoisted(() => ({
  mockedFilterItemsByType: vi.fn(),
  mockedFilterItemsByWeaponClass: vi.fn(),
  mockedGenerateEmptyFiltersModel: vi.fn(obj =>
    Object.keys(obj).reduce(
      (model, key) => {
        model[key] = []
        return model
      },
      {} as Record<string, any>,
    ),
  ),
  mockedGetAggregationBy: vi.fn((_arr, key) => ({ [key]: {} })),
  mockedGetAggregationsConfig: vi.fn(),
  mockedGetScopeAggregations: vi.fn((_arr, obj) => obj),
  mockedGetVisibleAggregationsConfig: vi.fn(),
}))

vi.mock('~/services/item-search-service', async () => ({
  ...(await vi.importActual<typeof import('~/services/item-search-service')>(
    '~/services/item-search-service',
  )),
  filterItemsByType: mockedFilterItemsByType,
  filterItemsByWeaponClass: mockedFilterItemsByWeaponClass,
  generateEmptyFiltersModel: mockedGenerateEmptyFiltersModel,
  getAggregationBy: mockedGetAggregationBy,
  getAggregationsConfig: mockedGetAggregationsConfig,
  getScopeAggregations: mockedGetScopeAggregations,
  getVisibleAggregationsConfig: mockedGetVisibleAggregationsConfig,
}))

vi.mock('~/services/item-search-service/indexator', () => ({
  createItemIndex: vi.fn(val => val),
}))

const items = [] as Item[]

describe('itemType model', () => {
  it('empty query - value should be default', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {},
    }))

    const { itemTypeModel } = useItemsFilter(items)

    expect(itemTypeModel.value).toEqual(ItemType.OneHandedWeapon)
  })

  it('with query', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {
        type: ItemType.TwoHandedWeapon,
      },
    }))

    const { itemTypeModel } = useItemsFilter(items)

    expect(itemTypeModel.value).toEqual(ItemType.TwoHandedWeapon)
  })

  describe('change', () => {
    it('query should be reset', () => {
      mockedUseRoute.mockImplementation(() => ({
        query: {
          filter: {
            bestPonies: ['TwilightSparkle', 'Fluttershy'],
          },
          page: 1,
          sort: 'some_asc',
        },
      }))

      const { itemTypeModel } = useItemsFilter(items)

      itemTypeModel.value = ItemType.BodyArmor

      expect(mockedPush).toBeCalledWith({
        query: {
          type: ItemType.BodyArmor,
        },
      })
    })

    it('itemType has weaponClasses - by default the first one in the list', () => {
      mockedUseRoute.mockImplementation(() => ({
        query: {},
      }))
      mockedGetWeaponClassesByItemType.mockReturnValue([
        WeaponClass.TwoHandedSword,
        WeaponClass.TwoHandedAxe,
      ])

      const { itemTypeModel } = useItemsFilter(items)

      itemTypeModel.value = ItemType.TwoHandedWeapon

      expect(mockedPush).toBeCalledWith({
        query: {
          type: ItemType.TwoHandedWeapon,
          weaponClass: WeaponClass.TwoHandedSword,
        },
      })
    })
  })
})

describe('weaponClass model', () => {
  it('empty query - value should be default - exact', () => {
    mockedGetWeaponClassesByItemType.mockReturnValue([
      WeaponClass.TwoHandedPolearm,
      WeaponClass.OneHandedPolearm,
    ])
    mockedUseRoute.mockImplementation(() => ({
      query: {
        type: ItemType.Polearm,
      },
    }))

    const { weaponClassModel } = useItemsFilter(items)

    expect(weaponClassModel.value).toEqual(WeaponClass.TwoHandedPolearm)
  })

  it('empty query - value should be default - null', () => {
    mockedGetWeaponClassesByItemType.mockReturnValue([])
    mockedUseRoute.mockImplementation(() => ({
      query: {
        type: ItemType.Bow,
      },
    }))

    const { weaponClassModel } = useItemsFilter(items)

    expect(weaponClassModel.value).toEqual(null)
  })

  it('with query', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {
        type: ItemType.TwoHandedWeapon,
        weaponClass: WeaponClass.TwoHandedSword,
      },
    }))

    const { weaponClassModel } = useItemsFilter(items)

    expect(weaponClassModel.value).toEqual(WeaponClass.TwoHandedSword)
  })

  describe('change', () => {
    it('query should be reset, except itemType', () => {
      mockedUseRoute.mockImplementation(() => ({
        query: {
          filter: {
            bestPonies: ['TwilightSparkle', 'Fluttershy'],
          },
          page: 1,
          sort: 'some_asc',
          type: ItemType.Polearm,
          WeaponClass: WeaponClass.TwoHandedPolearm,
        },
      }))

      const { weaponClassModel } = useItemsFilter(items)

      weaponClassModel.value = WeaponClass.OneHandedPolearm

      expect(mockedPush).toBeCalledWith({
        query: {
          type: ItemType.Polearm,
          weaponClass: WeaponClass.OneHandedPolearm,
        },
      })
    })

    it('empty value', () => {
      mockedUseRoute.mockImplementation(() => ({
        query: {
          type: ItemType.Polearm,
          WeaponClass: WeaponClass.TwoHandedPolearm,
        },
      }))

      const { weaponClassModel } = useItemsFilter(items)

      weaponClassModel.value = null

      expect(mockedPush).toBeCalledWith({
        query: {
          type: ItemType.Polearm,
        },
      })
    })
  })
})

describe('filter model', () => {
  it('empty query & empty aggregations', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {},
    }))

    mockedGetAggregationsConfig.mockReturnValue({})

    const { filterModel } = useItemsFilter(items)

    expect(filterModel.value).toEqual({})

    expect(mockedGenerateEmptyFiltersModel).toBeCalledWith({})
  })

  it('empty query', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {},
    }))

    mockedGetAggregationsConfig.mockReturnValue({
      handling: {},
      length: {},
    })

    const { filterModel } = useItemsFilter(items)

    expect(filterModel.value).toEqual({
      handling: [],
      length: [],
    })

    expect(mockedGenerateEmptyFiltersModel).toBeCalledWith({
      handling: {},
      length: {},
    })
  })

  it('with query', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {
        filter: {
          length: [1, 2],
        },
      },
    }))

    mockedGetAggregationsConfig.mockReturnValue({
      handling: {},
      length: {},
    })

    const { filterModel } = useItemsFilter(items)

    expect(filterModel.value).toEqual({
      handling: [],
      length: [1, 2],
    })

    expect(mockedGenerateEmptyFiltersModel).toBeCalledWith({
      handling: {},
      length: {},
    })
  })

  it('update filter model method', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {
        filter: {
          length: [1, 2],
        },
        page: 1,
        sort: 'some_asc',
      },
    }))

    mockedGetAggregationsConfig.mockReturnValue({})

    const { updateFilter } = useItemsFilter(items)

    updateFilter('length', [1, 5])

    expect(mockedPush).toBeCalledWith({
      query: {
        filter: {
          length: [1, 5],
        },
        page: 1,
        sort: 'some_asc',
      },
    })
  })

  it('reset filter method', () => {
    mockedUseRoute.mockImplementation(() => ({
      query: {
        compareList: ['1', '2'],
        filter: {
          handling: [1, 5],
          length: [1, 2],
        },
        isCompareActive: true,
        page: 3,
        perPage: 25,
        sort: 'some_asc',
        type: ItemType.OneHandedWeapon,
        weaponClass: WeaponClass.OneHandedAxe,
      },
    }))

    mockedGetAggregationsConfig.mockReturnValue({})

    const { resetFilters } = useItemsFilter(items)

    resetFilters()

    expect(mockedPush).toBeCalledWith({
      query: {
        compareList: ['1', '2'],
        isCompareActive: true,
        perPage: 25,
        sort: 'some_asc',
        type: ItemType.OneHandedWeapon,
        weaponClass: WeaponClass.OneHandedAxe,
      },
    })
  })
})

it('aggregations configs', () => {
  mockedUseRoute.mockImplementation(() => ({
    query: {
      type: ItemType.TwoHandedWeapon,
      weaponClass: WeaponClass.TwoHandedMace,
    },
  }))

  mockedGetAggregationsConfig.mockReturnValue({ handling: {}, length: {} })
  mockedGetVisibleAggregationsConfig.mockReturnValue({ handling: {}, length: {} })

  const { aggregationsConfig, aggregationsConfigVisible } = useItemsFilter(items)

  expect(aggregationsConfig.value).toEqual({ handling: {}, length: {} })
  expect(mockedGetAggregationsConfig).toBeCalledWith(
    ItemType.TwoHandedWeapon,
    WeaponClass.TwoHandedMace,
  )

  expect(aggregationsConfigVisible.value).toEqual({ handling: {}, length: {} })
  expect(mockedGetVisibleAggregationsConfig).toBeCalledWith({ handling: {}, length: {} })
})

describe('filters & aggregations', () => {
  mockedUseRoute.mockImplementation(() => ({
    query: {
      type: ItemType.TwoHandedWeapon,
      weaponClass: WeaponClass.TwoHandedMace,
    },
  }))

  const items = [{ id: '1' }, { id: '2' }, { id: '3' }] as Item[]
  const filteredByItemTypeItems = [{ id: '1' }, { id: '2' }] as Item[]
  const filteredByWeaponClassItems = [{ id: '1' }] as Item[]

  mockedFilterItemsByType.mockReturnValue(filteredByItemTypeItems)
  mockedFilterItemsByWeaponClass.mockReturnValue(filteredByWeaponClassItems)
  mockedGetAggregationsConfig.mockReturnValue({ handling: {}, length: {} })

  it('filtered by type && weaponClass', () => {
    const { filteredByClassFlatItems, filteredByTypeFlatItems } = useItemsFilter(items)

    expect(filteredByTypeFlatItems.value).toEqual(filteredByItemTypeItems)
    expect(filteredByClassFlatItems.value).toEqual(filteredByWeaponClassItems)

    expect(mockedFilterItemsByType).toBeCalledWith(items, ItemType.TwoHandedWeapon)
    expect(mockedFilterItemsByWeaponClass).toBeCalledWith(
      filteredByItemTypeItems,
      WeaponClass.TwoHandedMace,
    )
  })

  it('aggregations', () => {
    const { aggregationByClass, aggregationByType, scopeAggregations } = useItemsFilter(items)

    expect(aggregationByType.value).toEqual({ type: {} })
    expect(mockedGetAggregationBy).nthCalledWith(1, items, 'type')

    expect(aggregationByClass.value).toEqual({ weaponClass: {} })
    expect(mockedGetAggregationBy).nthCalledWith(2, filteredByItemTypeItems, 'weaponClass')
    expect(scopeAggregations.value).toEqual({ handling: {}, length: {} })

    expect(mockedGetScopeAggregations).toBeCalledWith(filteredByWeaponClassItems, {
      handling: {},
      length: {},
    })
  })
})
