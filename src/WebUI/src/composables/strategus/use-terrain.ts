import type { LMap } from '@vue-leaflet/vue-leaflet'
import type { Map } from 'leaflet'

import { useStorage } from '@vueuse/core'
import L from 'leaflet'
import { v4 as uuidv4 } from 'uuid'

import type { TerrainFeatureCollection } from '~/models/strategus/terrain'

import { Terrain } from '~/models/strategus/terrain'
import { TerrainColorByType } from '~/services/strategus-service/terrain'

export const useTerrain = (map: Ref<typeof LMap | null>) => {
  // FIXME: TODO: migrate to useAsyncState
  const terrain = useStorage<TerrainFeatureCollection>('terrainGeoJSON', {
    features: [
      {
        geometry: {
          coordinates: [
            [
              [117.84375, -115.69750123365006],
              [119.5, -112.69662115645465],
              [120.28125, -116.44772125294892],
            ],
          ],
          type: 'Polygon',
        },
        id: '1',
        properties: {
          type: Terrain.Forest,
        },
        type: 'Feature',
      },
      {
        geometry: {
          coordinates: [
            [
              [122, -116.07251381470937],
              [128.34375, -116.32257744801623],
              [126.5625, -120.35485353508943],
              [123.4375, -120.29233762676272],
            ],
          ],
          type: 'Polygon',
        },
        id: '2',
        properties: {
          type: Terrain.River,
        },
        type: 'Feature',
      },
    ],
    type: 'FeatureCollection',
  })

  const terrainVisibility = ref<boolean>(false)
  const toggleTerrainVisibilityLayer = () => {
    terrainVisibility.value = !terrainVisibility.value
  }

  const saveTerrain = () => {
    if (map.value === null) { return }

    const _geoJSON: TerrainFeatureCollection = {
      features: [],
      type: 'FeatureCollection',
    };

    (map.value.leafletObject as Map).eachLayer((layer) => {
      // @ts-expect-error TODO:
      if (layer.internalId && layer instanceof L.Polygon) {
        const geoJSONShape = layer.toGeoJSON() // to precise geo shape!
        // @ts-expect-error TODO:
        geoJSONShape.properties = layer.properties
        // @ts-expect-error TODO:
        geoJSONShape.id = layer.internalId
        // @ts-expect-error TODO:
        _geoJSON.features.push(geoJSONShape)
      }
    })

    terrain.value = { ..._geoJSON }
  }

  const editMode = ref<boolean>(false)
  const isEditorInit = ref<boolean>(false)
  const toggleEditMode = () => {
    editMode.value = !editMode.value

    if (!isEditorInit.value) {
      return createEditControls()
    }

    (map.value!.leafletObject as Map).pm.toggleControls()
  }

  const editType = ref<Terrain | null>(null)

  const setEditType = (type: Terrain) => {
    editType.value = type

    if (map.value === null) { return }

    const color = TerrainColorByType[editType.value];

    (map.value.leafletObject as Map).pm.setPathOptions({
      color,
      fillColor: color,
    })
  }

  const onTerrainUpdated = (event: any) => {
    if (event.type === 'pm:create') {
      event.layer.properties = {
        type: event.shape,
      }
      event.layer.internalId = uuidv4()
      saveTerrain()
      event.layer.removeFrom(map.value!.leafletObject as Map)
    }

    if (event.type === 'pm:remove') {
      event.layer.off()
      saveTerrain()
    }

    if (event.type === 'pm:edit') {
      saveTerrain()
    }
  }

  const createEditControls = () => {
    (map.value!.leafletObject as Map).pm.addControls({
      cutPolygon: false,
      drawCircle: false,
      drawCircleMarker: false,
      drawMarker: false,
      drawPolygon: false,
      drawPolyline: false,
      drawRectangle: false,
      drawText: false,
      position: 'topleft',
      rotateMode: false,
    });

    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      block: 'draw',
      className: 'icon-river',
      name: Terrain.River,
      onClick: () => setEditType(Terrain.River),
      title: 'River',
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      block: 'draw',
      className: 'icon-forest',
      name: Terrain.Forest,
      onClick: () => setEditType(Terrain.Forest),
      title: 'Forest',
    });
    (map.value!.leafletObject as Map).pm.Toolbar.copyDrawControl('Polygon', {
      block: 'draw',
      className: 'icon-mountains',
      name: Terrain.Mountain,
      onClick: () => setEditType(Terrain.Mountain),
      title: 'Mountains',
    });

    (map.value!.leafletObject as Map).on('pm:create', onTerrainUpdated);
    (map.value!.leafletObject as Map).on('pm:remove', onTerrainUpdated)

    L.PM.reInitLayer(map.value!.leafletObject)

    isEditorInit.value = true
  }

  return {
    createEditControls,
    editMode,
    onTerrainUpdated,

    terrain,
    terrainVisibility,

    toggleEditMode,
    toggleTerrainVisibilityLayer,
  }
}
