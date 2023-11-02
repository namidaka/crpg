import { useMagicKeys, whenever } from '@vueuse/core';

interface ElementBound {
  x: number;
  y: number;
  width: number;
}

interface OpenedItem {
  id: string;
  userId: number; // TODO: FIXME:
  bound: ElementBound;
}

// shared state
const openedItems = ref<OpenedItem[]>([]);

export const useItemDetail = (setListeners: boolean = false) => {
  const openItemDetail = (item: OpenedItem) => {
    if (!openedItems.value.some(oi => oi.id === item.id)) {
      openedItems.value.push(item);
    }
  };

  const closeItemDetail = (id: string) => {
    if (openedItems.value.some(oi => oi.id === id)) {
      openedItems.value = openedItems.value.filter(oi => oi.id !== id);
    }
  };

  const toggleItemDetail = (target: HTMLElement, item: Omit<OpenedItem, 'bound'>) => {
    !openedItems.value.some(oi => oi.id === item.id)
      ? openItemDetail({ ...item, bound: getElementBounds(target) })
      : closeItemDetail(item.id);
  };

  const closeAll = () => {
    openedItems.value = [];
  };

  const getElementBounds = (el: HTMLElement) => {
    const { x, y, width } = el.getBoundingClientRect();
    return { x, y, width };
  };

  const computeDetailCardYPosition = (y: number) => {
    // we cannot automatically determine the height of the card, so we take the maximum possible value
    // think about it, but it's fine as it is
    const cardHeight = 700;

    const yDiff = window.innerHeight - y;
    const needOffset = yDiff < cardHeight;

    if (!needOffset) {
      return y;
    }

    return y + yDiff - cardHeight;
  };

  const { escape } = useMagicKeys();

  if (setListeners) {
    whenever(escape, () => {
      openedItems.value.length !== 0 &&
        closeItemDetail(openedItems.value[openedItems.value.length - 1].id);
    });

    onBeforeRouteLeave(() => {
      closeAll();
      return true;
    });
  }

  return {
    openedItems,
    openItemDetail,
    closeItemDetail,
    toggleItemDetail,
    closeAll,
    getElementBounds,
    computeDetailCardYPosition,
  };
};
