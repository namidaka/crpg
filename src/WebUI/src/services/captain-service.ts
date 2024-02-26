import {
    type Captain,
    type CaptainFormation,
  } from '@/models/captain';
  import { get, post, put, del } from '@/services/crpg-client';
  import { rgbHexColorToArgbInt, argbIntToRgbHexColor } from '@/utils/color';
  
  export const getCaptain = async () =>
  (get<Captain>(`/users/self/captain`));

  export const getFormations = async () =>
  (await get<CaptainFormation[]>(`/users/self/captain/formations`));
  
 export const assignCharacterToFormation = (formationId: number, characterId: number | null) =>
 put(`/users/self/captain/${formationId}/assign/character`, { characterId })

 export const setFormationWeight = (formationId: number, weight: number) =>
 put(`/users/self/captain/${formationId}/assign/weight`, { weight })
  
  