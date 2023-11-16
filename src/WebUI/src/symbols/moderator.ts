import type { UserPublic } from '@/models/user';

export const moderationUserKey: InjectionKey<Ref<UserPublic | null>> = Symbol('ModerationUser');
