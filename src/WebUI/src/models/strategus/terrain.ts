import { type FeatureCollection, type Feature, type Polygon } from 'geojson';

export enum TerrainType {
  'Forest' = 'Forest',
  'River' = 'River',
  'Mountain' = 'Mountain', // to Mountain

  // TODO:
  // Roads?
  // Road top tier
  // Road low tier ?)))
  // No
}

export interface Terrain {
  id: number;
  type: TerrainType;
  boundary: Polygon;
}

export interface TerrainProperties {
  type: TerrainType;
}

export type TerrainFeatureCollection = FeatureCollection<Polygon, TerrainProperties>;

export type TerrainFeature = Feature<Polygon, TerrainProperties>;

export interface TerrainCreation {
  type: TerrainType;
  boundary: Polygon;
}

export interface TerrainUpdate {
  boundary: Polygon;
}
