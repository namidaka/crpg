import type { Feature, FeatureCollection, Polygon } from 'geojson'

export enum Terrain {
  Forest = 'Forest',
  River = 'River',
  Mountain = 'Mountain', // to Mountain

  // TODO:
  // Roads?
  // Road top tier
  // Road low tier ?)))
  // No
}

export interface TerrainProperties {
  type: Terrain
}

export type TerrainFeatureCollection = FeatureCollection<Polygon, TerrainProperties>

export type TerrainFeature = Feature<Polygon, TerrainProperties>
