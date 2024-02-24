import { type User } from '@/models/user';
import { getCaptain, getFormations, assignCharacterToFormation } from '@/services/captain-service'

export interface Captain {
    id: number;
}

export interface CaptainFormation {
    id: number,
    characterId: number | null,
    weight: number,
}
