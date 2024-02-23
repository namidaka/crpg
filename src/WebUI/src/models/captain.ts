import { type User } from '@/models/user';
import { type Character } from '@/models/character';
import { getCaptain, getFormations, assignCharacterToFormation } from '@/services/captain-service'

export interface Captain {
    id: number;
}

export interface CaptainFormation {
    id: number,
    troop: Character,
    weight: number;
}
