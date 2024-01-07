import { LMap } from '@vue-leaflet/vue-leaflet';
import L, { type Map } from 'leaflet';

import { type TerrainFeatureCollection, TerrainType } from '@/models/strategus/terrain';
import {
  TerrainColorByType,
  getTerrains,
  updateTerrain,
  addTerrain,
  deleteTerrain,
} from '@/services/strategus-service/terrain';

export const useTerrains = (map: Ref<typeof LMap | null>) => {
  const { state: terrains, execute: loadTerrains } = useAsyncState(() => getTerrains(), [], {
    immediate: false,
    resetOnExecute: false,
  });

  const terrainsFeatureCollection = computed<TerrainFeatureCollection>(() => ({
    type: 'FeatureCollection',
    features: terrains.value.map(t => ({
      type: 'Feature',
      id: t.id,
      geometry: t.boundary,
      properties: {
        type: t.type,
      },
    })),
  }));

  const terrainVisibility = ref<boolean>(true); // TODO:
  const toggleTerrainVisibilityLayer = () => {
    terrainVisibility.value = !terrainVisibility.value;
  };

  const editMode = ref<boolean>(false);
  const isEditorInit = ref<boolean>(false);
  const toggleEditMode = () => {
    editMode.value = !editMode.value;

    if (!isEditorInit.value) {
      return createEditControls();
    }

    (map.value!.leafletObject as Map).pm.toggleControls();
  };

  const editType = ref<TerrainType | null>(null);

  const setEditType = (type: TerrainType) => {
    editType.value = type;

    if (map.value === null) return;

    const color = TerrainColorByType[editType.value];

    (map.value.leafletObject as Map).pm.setPathOptions({
      color: color,
      fillColor: color,
    });
  };

  // TODO: event - ts
  const onTerrainUpdated = async (event: any) => {
    if (event.type === 'pm:create') {
      await addTerrain({
        type: event.shape,
        boundary: event.layer.toGeoJSON().geometry,
      });
      event.layer.removeFrom(map.value!.leafletObject as Map);
      await loadTerrains();
    }

    if (event.type === 'pm:update') {
      await updateTerrain(event.layer.feature.id as number, {
        boundary: event.layer.toGeoJSON().geometry,
      });
      await loadTerrains();
    }

    if (event.type === 'pm:remove') {
      event.layer.off();
      await deleteTerrain(event.layer.feature.id as number);
      await loadTerrains();
    }
  };

  const createEditControls = () => {
    (map.value!.leafletObject as Map).pm.addControls({
      position: 'topleft',
      drawCircle: false,
      drawMarker: false,
      drawCircleMarker: false,
      drawPolyline: false,
      drawRectangle: false,
      drawText: false,
      drawPolygon: false,
      rotateMode: false,
      cutPolygon: false,
    });

    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      name: TerrainType.River,
      block: 'draw',
      title: 'River',
      className: 'icon-river',
      onClick: () => setEditType(TerrainType.River),
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      name: TerrainType.Forest,
      block: 'draw',
      title: 'Forest',
      className: 'icon-forest',
      onClick: () => setEditType(TerrainType.Forest),
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      name: TerrainType.Mountain,
      block: 'draw',
      title: 'Mountains',
      className: 'icon-mountains',
      onClick: () => setEditType(TerrainType.Mountain),
    });

    (map.value!.leafletObject as Map).on('pm:create', onTerrainUpdated);
    (map.value!.leafletObject as Map).on('pm:remove', onTerrainUpdated);

    L.PM.reInitLayer(map.value!.leafletObject);

    isEditorInit.value = true;
  };

  return {
    // terrains,
    terrainsFeatureCollection,
    loadTerrains,
    terrainVisibility,
    toggleTerrainVisibilityLayer,

    editMode,
    toggleEditMode,

    onTerrainUpdated,
    createEditControls,
  };
};
