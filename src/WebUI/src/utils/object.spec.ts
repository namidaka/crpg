import { mergeObjectWithSum, pick, omit, omitPredicate } from './object';

it('mergeObjectWithSum', () => {
  const obj1 = {
    applejack: 10,
    rarity: 1,
  };

  const obj2 = {
    applejack: 1,
    rarity: 0,
  };

  const obj3 = {
    applejack: 11,
    rarity: 1,
  };

  expect(mergeObjectWithSum(obj1, obj2)).toEqual(obj3);
});

it('pick', () => {
  expect(pick({ id: 1, name: 2 }, ['id'])).toEqual({ id: 1 });
});

it('omit', () => {
  expect(omit({ id: 1, name: 2 }, ['id'])).toEqual({ name: 2 });
});

it('omitPredicate', () => {
  expect(omitPredicate({ id: 1, name: 2 }, key => key !== 'name')).toEqual({ id: 1 });
});
