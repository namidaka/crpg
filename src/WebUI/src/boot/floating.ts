import FloatingVue from 'floating-vue';
import { type BootModule } from '@/types/boot-module';

export const install: BootModule = app => {
  app.use(FloatingVue, {
    distance: 16,
    disposeTimeout: 100,
  });
};
