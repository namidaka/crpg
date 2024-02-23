import { type User } from '@/models/user';
import { getCaptain, getFormations, assignCharacterToFormation } from '@/services/captain-service'
interface State {
    user: User | null;
    captain: Captain | null;
    formations: CaptainFormation[];
}

export interface Captain {
    id: number;
    userid: number;
}

export interface CaptainFormation {
    id: number,
    userid: number,
    weight: number;
}

export const useCaptainStore = defineStore('captain', {
    state: (): State => ({
        user: null,
        captain: null,
        formations: [],
    }),
  
    getters: {
      activeCharacterId: state => state.user?.activeCharacterId || state.characters?.[0]?.id || null,
    },
  
    actions: {

      async fetchCaptain() {
        this.captain = await getCaptain();
      },
  
      async fetchFormations() {
        this.formations = await getFormations();
      },
  
      assignCharacterToFormation(characterId: number) {
        this.userItems.push(characterId);
      },
  
      subtractGold(loss: number) {
        this.user!.gold -= loss;
      },
  
      async buyItem(itemId: string) {
        const userItem = await buyUserItem(itemId);
        this.addUserItem(userItem);
        this.subtractGold(userItem.item.price);
      },
  
      async fetchUserClanAndRole() {
        const userClanAndRole = await getUserClan();
  
        if (userClanAndRole === null) {
          this.clan = null;
          this.clanMemberRole = null;
          return;
        }
  
        this.clan = userClanAndRole.clan;
        this.clanMemberRole = userClanAndRole.role;
      },
  
      async fetchUserRestriction() {
        this.restriction = await getUserRestriction();
      },
    },
  });
