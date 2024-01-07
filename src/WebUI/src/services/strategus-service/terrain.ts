import {
  type Terrain,
  TerrainType,
  type TerrainCreation,
  type TerrainUpdate,
} from '@/models/strategus/terrain';
import { get, post, put, del } from '@/services/crpg-client';

export const TerrainColorByType: Record<TerrainType, string> = {
  [TerrainType.Forest]: '#047857',
  [TerrainType.River]: '#0284c7',
  [TerrainType.Mountain]: '#d1d5db ',
};

export const getTerrains = () => get<Terrain[]>('/terrains');

export const addTerrain = (payload: TerrainCreation) => post<Terrain>('/terrains', payload);

export const updateTerrain = (id: number, payload: TerrainUpdate) =>
  put<Terrain>(`/terrains/${id}`, payload);

export const deleteTerrain = (id: number) => del(`/terrains/${id}`);
