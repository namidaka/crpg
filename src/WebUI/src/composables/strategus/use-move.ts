import L, { type LatLngLiteral, type LeafletMouseEvent, type Map } from 'leaflet';
import { LMap } from '@vue-leaflet/vue-leaflet';
import { MovementType, MovementTargetType } from '@/models/strategus';
import { type PartyCommon, PartyStatus } from '@/models/strategus/party';
import { type SettlementPublic } from '@/models/strategus/settlement';
import { useParty } from '@/composables/strategus/use-party';
import { positionToLatLng } from '@/utils/geometry';

export const useMove = (map: Ref<typeof LMap | null>) => {
  const { moveParty } = useParty();

  const moveDialogCoordinates = ref<LatLngLiteral | null>(null);
  const moveDialogMovementTypes = ref<MovementType[]>([]);

  const moveTargetType = ref<MovementTargetType | null>(null);
  const moveTarget = ref<PartyCommon | SettlementPublic | null>(null);

  const showMoveDialog = ({
    target,
    targetType,
    movementTypes,
  }: {
    target: PartyCommon | SettlementPublic;
    targetType: MovementTargetType;
    movementTypes: MovementType[];
  }) => {
    moveTarget.value = target;
    moveTargetType.value = targetType;

    moveDialogCoordinates.value = positionToLatLng(target.position.coordinates);
    moveDialogMovementTypes.value = movementTypes;
  };

  const closeMoveDialog = () => {
    moveDialogCoordinates.value = null;
    moveDialogMovementTypes.value = [];
    moveTarget.value = null;
    moveTargetType.value = null;
  };

  const isMoveMode = ref<boolean>(false);

  const onCreateMovePath = async (event: { shape: string; layer: L.Layer }) => {
    const { shape, layer } = event;

    if (shape === 'Line') {
      // @ts-ignore
      const coordinates = layer.toGeoJSON().geometry.coordinates;

      await moveParty({
        status: PartyStatus.MovingToPoint,
        waypoints: { type: 'MultiPoint', coordinates },
      });

      event.layer.removeFrom(map.value!.leafletObject as Map);
      isMoveMode.value = false;
    }
  };

  const onStartMove = (e: LeafletMouseEvent) => {
    isMoveMode.value = true;

    (map.value!.leafletObject as Map).pm.enableDraw('Line', {});
    // @ts-ignore
    (map.value!.leafletObject as Map).pm.Draw.Line._layer.addLatLng(e.latlng);
    // @ts-ignore
    (map.value!.leafletObject as Map).pm.Draw.Line._createMarker(e.latlng);
  };

  const applyEvents = () => {
    (map.value!.leafletObject as Map).on('pm:keyevent', e => {
      if (isMoveMode.value && (e.event as KeyboardEvent).code === 'Escape') {
        (map.value!.leafletObject as Map).pm.disableDraw();
        isMoveMode.value = false;
      }
    });

    (map.value!.leafletObject as Map).on('pm:create', onCreateMovePath);
  };

  return {
    applyEvents,

    isMoveMode,
    onStartMove,

    moveTarget,
    moveTargetType,
    //
    moveDialogCoordinates,
    moveDialogMovementTypes,
    //
    showMoveDialog,
    closeMoveDialog,
  };
};
