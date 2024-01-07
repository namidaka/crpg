import {
  type Terrain,
  TerrainType,
  type TerrainCreation,
  type TerrainUpdate,
} from '@/models/strategus/terrain';
import { get, post, put, del } from '@/services/crpg-client';

export const TerrainColorByType: Record<TerrainType, string> = {
  [TerrainType.Barrier]: '#d03c3c ', // TODO: color

  [TerrainType.SparseForest]: '#22c55e',
  [TerrainType.ThickForest]: '#166534',

  [TerrainType.ShallowWater]: '#60a5fa',
  [TerrainType.DeepWater]: '#1e40af',
};

export const getTerrains = () => get<Terrain[]>('/terrains');

export const addTerrain = (payload: TerrainCreation) => post<Terrain>('/terrains', payload);

export const updateTerrain = (id: number, payload: TerrainUpdate) =>
  put<Terrain>(`/terrains/${id}`, payload);

export const deleteTerrain = (id: number) => del(`/terrains/${id}`);
