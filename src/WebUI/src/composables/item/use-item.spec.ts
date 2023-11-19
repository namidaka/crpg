import { type Item } from '@/models/item';
import { useItem } from './use-item';

vi.mock('@/services/item-service', () => ({
  getItemImage: vi.fn(() => 'image1.webp'),
  getRankColor: vi.fn(() => '#fff'),
}));

it('useItem:', () => {
  const item = ref({
    rank: 1,
    baseId: '1',
  } as Item);

  const { rankColor, thumb } = useItem(item);

  expect(rankColor.value).toEqual('#fff');
  expect(thumb.value).toEqual('image1.webp');
});
