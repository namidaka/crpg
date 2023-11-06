import { type Position } from 'geojson';
import { type LatLngBounds, type Map } from 'leaflet';
import { LMap } from '@vue-leaflet/vue-leaflet';
import { getSettlements } from '@/services/strategus-service';
import { shouldDisplaySettlement } from '@/services/strategus-service/map';
import { positionToLatLng } from '@/utils/geometry';

export const useSettlements = (
  map: Ref<typeof LMap | null>,
  mapBounds: Ref<LatLngBounds | null>,
  zoom: Ref<number>
) => {
  const { state: settlements, execute: loadSettlements } = useAsyncState(
    () => getSettlements(),
    [],
    {
      immediate: false,
    }
  );

  const visibleSettlements = computed(() => {
    if (mapBounds.value === null) {
      return [];
    }

    return settlements.value.filter(s => shouldDisplaySettlement(s, mapBounds.value!, zoom.value));
  });

  const shownSearch = ref<boolean>(false);
  const toggleSearch = () => {
    shownSearch.value = !shownSearch.value;
  };

  const flyToSettlement = (coordinates: Position) => {
    (map.value!.leafletObject as Map)!.flyTo(positionToLatLng(coordinates), 5, {
      animate: false,
    });
  };

  return {
    settlements,
    visibleSettlements,
    loadSettlements,

    //
    shownSearch,
    toggleSearch,

    flyToSettlement,
  };
};
