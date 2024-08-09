export const range = (start: number, end: number) =>
  Array(end - start + 1)
    .fill(null)
    .map((_, idx) => start + idx);

// TODO: SPEC
export const groupBy = <T>(arr: T[], fn: (item: T) => any) =>
  arr.reduce<Record<string, T[]>>((prev, curr) => {
    const groupKey = fn(curr);
    const group = prev[groupKey] || [];
    group.push(curr);
    return { ...prev, [groupKey]: group };
  }, {});

// TODO: SPEC
export const getIndexToIns = (arr: number[], num: number) => {
  let index = arr.findIndex(currentNum => num <= currentNum);
  return index === -1 ? arr.length : index;
};
