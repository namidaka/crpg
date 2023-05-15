import { LMap } from '@vue-leaflet/vue-leaflet';
import L, { type Map } from 'leaflet';
import { useStorage } from '@vueuse/core';
import { v4 as uuidv4 } from 'uuid';

import { type TerrainFeatureCollection, Terrain } from '@/models/strategus/terrain';
import { TerrainColorByType } from '@/services/strategus-service/terrain';

export const useTerrain = (map: Ref<typeof LMap | null>) => {
  // FIXME: TODO: migrate to useAsyncState
  const terrain = useStorage<TerrainFeatureCollection>('terrainGeoJSON', {
    type: 'FeatureCollection',
    features: [
      {
        type: 'Feature',
        id: '1',
        properties: {
          type: Terrain.Forest,
        },
        geometry: {
          type: 'Polygon',
          coordinates: [
            [
              [117.84375, -115.69750123365006],
              [119.5, -112.69662115645465],
              [120.28125, -116.44772125294892],
            ],
          ],
        },
      },
      {
        type: 'Feature',
        id: '2',
        properties: {
          type: Terrain.River,
        },
        geometry: {
          type: 'Polygon',
          coordinates: [
            [
              [122, -116.07251381470937],
              [128.34375, -116.32257744801623],
              [126.5625, -120.35485353508943],
              [123.4375, -120.29233762676272],
            ],
          ],
        },
      },
    ],
  });

  const terrainVisibility = ref<boolean>(false);
  const toggleTerrainVisibilityLayer = () => {
    terrainVisibility.value = !terrainVisibility.value;
  };

  const saveTerrain = () => {
    if (map.value === null) return;

    const _geoJSON: TerrainFeatureCollection = {
      type: 'FeatureCollection',
      features: [],
    };

    (map.value.leafletObject as Map).eachLayer(layer => {
      // TODO: ts-ignore
      // @ts-ignore
      if (layer.internalId && layer instanceof L.Polygon) {
        const geoJSONShape = layer.toGeoJSON(); // to precise geo shape!
        // @ts-ignore
        geoJSONShape.properties = layer.properties;
        // @ts-ignore
        geoJSONShape.id = layer.internalId;
        // @ts-ignore
        _geoJSON.features.push(geoJSONShape);
      }
    });

    terrain.value = { ..._geoJSON };
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

  const editType = ref<Terrain | null>(null);

  const setEditType = (type: Terrain) => {
    editType.value = type;

    if (map.value === null) return;

    const color = TerrainColorByType[editType.value];

    (map.value.leafletObject as Map).pm.setPathOptions({
      color: color,
      fillColor: color,
    });
  };

  const onTerrainUpdated = (event: any) => {
    if (event.type === 'pm:create') {
      event.layer.properties = {
        type: event.shape,
      };
      event.layer.internalId = uuidv4();
      saveTerrain();
      event.layer.removeFrom(map.value!.leafletObject as Map);
    }

    if (event.type === 'pm:remove') {
      event.layer.off();
      saveTerrain();
    }

    if (event.type === 'pm:edit') {
      saveTerrain();
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
      name: Terrain.River,
      block: 'draw',
      title: 'River',
      className: 'icon-river',
      onClick: () => setEditType(Terrain.River),
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      name: Terrain.Forest,
      block: 'draw',
      title: 'Forest',
      className: 'icon-forest',
      onClick: () => setEditType(Terrain.Forest),
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      name: Terrain.Mountain,
      block: 'draw',
      title: 'Mountains',
      className: 'icon-mountains',
      onClick: () => setEditType(Terrain.Mountain),
    });

    (map.value!.leafletObject as Map).on('pm:create', onTerrainUpdated);
    (map.value!.leafletObject as Map).on('pm:remove', onTerrainUpdated);

    L.PM.reInitLayer(map.value!.leafletObject);

    isEditorInit.value = true;
  };

  return {
    terrain,
    terrainVisibility,
    toggleTerrainVisibilityLayer,

    editMode,
    toggleEditMode,

    onTerrainUpdated,
    createEditControls,
  };
};
