<script setup lang="ts">
import { type Map } from 'leaflet';
import { LMap, LTileLayer, LControlZoom } from '@vue-leaflet/vue-leaflet';
import { LMarkerClusterGroup } from 'vue-leaflet-markercluster';
import '@geoman-io/leaflet-geoman-free';

import { MovementType, MovementTargetType } from '@/models/strategus';
import { type PartyCommon, PartyStatus } from '@/models/strategus/party';
import { type SettlementPublic } from '@/models/strategus/settlement';

import { inSettlementStatuses } from '@/services/strategus-service';

import { positionToLatLng } from '@/utils/geometry';

import useMainHeaderHeight from '@/composables/use-main-header-height';
import { useMap } from '@/composables/strategus/use-map';
import { useParty } from '@/composables/strategus/use-party';
import { useSettlements } from '@/composables/strategus/use-settlements';
import { useMove } from '@/composables/strategus/use-move';
import { useTerrain } from '@/composables/strategus/use-terrain';

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
    noFooter: true,
  },
});

const router = useRouter();

const mainHeaderHeight = useMainHeaderHeight();

// prettier-ignore
const {
  map,
  mapOptions,
  center,
  mapBounds,
  maxBounds,
  zoom,
  tileLayerOptions,
  onMapMoveEnd
} = useMap();

const {
  terrain,
  terrainVisibility,
  toggleTerrainVisibilityLayer,
  toggleEditMode,
  onTerrainUpdated,
} = useTerrain(map);

// prettier-ignore
const {
  isRegistered,
  onRegistered,
  party,
  partySpawn,
  moveParty,
  visibleParties,

  //
  toggleRecruitTroops,
  isTogglingRecruitTroops
} = useParty();

// TODO: need refactoring
// TODO: tweak routes nesting
// enter/leave settlement

// TODO: to route middleware? no
watchEffect(() => {
  if (party.value === null) return;

  // if (party.value.targetedSettlement !== null && inSettlementStatuses.has(party.value.status)) {
  //   router.push({
  //     name: 'StrategusSettlementId', // TODO: tweak routes nesting
  //     params: { id: party.value.targetedSettlement.id },
  //   });
  // }
  // else {
  //   //
  //   // await moveParty
  //   router.push({
  //     name: 'StrategusParent',
  //   });
  // }
});

const {
  settlements,
  visibleSettlements,
  loadSettlements,
  shownSearch,
  toggleSearch,
  flyToSettlement,
} = useSettlements(map, mapBounds, zoom);

const {
  applyEvents: applyMoveEvents,
  isMoveMode,
  onStartMove,

  moveTarget,
  moveTargetType,

  moveDialogCoordinates,
  moveDialogMovementTypes,

  showMoveDialog,
  closeMoveDialog,
} = useMove(map);

const onPartyClick = (targetParty: PartyCommon) => {
  if (party.value === null) return;

  showMoveDialog({
    target: targetParty,
    targetType: MovementTargetType.Party,
    movementTypes: [MovementType.Follow, MovementType.Attack],
  });
};

const onSettlementClick = (settlement: SettlementPublic) => {
  if (party.value === null) return;

  showMoveDialog({
    target: settlement,
    targetType: MovementTargetType.Settlement,
    movementTypes: [MovementType.Move, MovementType.Attack],
  });
};

const onMoveDialogConfirm = (mt: MovementType) => {
  if (moveTarget.value !== null) {
    switch (moveTargetType.value) {
      case MovementTargetType.Party:
        moveParty({
          status:
            mt === MovementType.Follow
              ? PartyStatus.FollowingParty
              : PartyStatus.MovingToAttackParty,
          targetedPartyId: moveTarget.value.id,
        });
        break;
      case MovementTargetType.Settlement:
        moveParty({
          status:
            mt === MovementType.Move
              ? PartyStatus.MovingToSettlement
              : PartyStatus.MovingToAttackSettlement,
          targetedSettlementId: moveTarget.value.id,
        });
        break;
    }
  }

  closeMoveDialog();
};

const mapIsLoading = ref<boolean>(true);
const onMapReady = async (map: Map) => {
  mapBounds.value = map.getBounds();
  await Promise.all([loadSettlements(), partySpawn()]);

  applyMoveEvents();
  mapIsLoading.value = false;

  if (party.value !== null) {
    locateToSelfParty();
  }
};

const locateToSelfParty = () => {
  map.value!.leafletObject.flyTo(positionToLatLng(party.value!.position.coordinates), 5, {
    animate: false,
  });
};
</script>

<template>
  <div class="relative overflow-hidden" :style="{ height: `calc(100vh - ${mainHeaderHeight}px)` }">
    <OLoading v-if="mapIsLoading" fullPage active iconSize="xl" />

    <LMap
      v-model:zoom="zoom"
      ref="map"
      :center="center"
      :options="mapOptions"
      :maxBounds="maxBounds"
      @ready="onMapReady"
      @moveEnd="onMapMoveEnd"
    >
      <!-- TODO: FIXME: low res map image -->
      <!-- TODO: FIXME: zIndex -->
      <!-- <LImageOverlay
        url="https://www.printablee.com/postpic/2011/06/blank-100-square-grid-paper_405041.jpg"
        :bounds="maxBounds"
      /> -->
      <LTileLayer v-bind="tileLayerOptions" />
      <LControlZoom position="bottomleft" />

      <ControlSearchToggle position="topleft" @click="toggleSearch" />
      <ControlTerrainVisibilityToggle position="topleft" @click="toggleTerrainVisibilityLayer" />

      <!-- TODO: policy -->
      <ControlTerrainEditToggle position="topleft" @click="toggleEditMode" />

      <ControlMousePosition position="bottomright" />
      <ControlLocateParty v-if="party !== null" :party="party" position="bottomleft" />

      <LayerTerrain v-if="terrainVisibility" :data="terrain" @edit="onTerrainUpdated" />

      <MarkerParty v-if="party !== null" :party="party" isSelf @click="onStartMove" />
      <PartyMovementLine v-if="party !== null && !isMoveMode" :party="party" />

      <LMarkerClusterGroup :showCoverageOnHover="false" chunkedLoading>
        <MarkerParty
          v-for="visibleParty in visibleParties"
          :party="visibleParty"
          :key="'party-' + visibleParty.id"
          @click="onPartyClick(visibleParty)"
        />
      </LMarkerClusterGroup>

      <MarkerSettlement
        v-for="settlement in visibleSettlements"
        :settlement="settlement"
        :key="'settlement-' + settlement.id"
        @click="onSettlementClick(settlement)"
      />

      <DialogMove
        v-if="moveDialogCoordinates !== null"
        :latLng="moveDialogCoordinates"
        :movementTypes="moveDialogMovementTypes"
        @confirm="onMoveDialogConfirm"
        @cancel="closeMoveDialog"
      />
    </LMap>

    <!-- Dialogs -->
    <div class="absolute left-16 top-6 z-[1000]">
      <!-- TODO: placement, design-->
      <SettlementSearch v-if="shownSearch" :settlements="settlements" @select="flyToSettlement" />

      <DialogRegistration v-if="!isRegistered" @registered="onRegistered" />
    </div>

    <PartyProfile
      v-if="party"
      class="absolute right-10 top-12 z-[1000]"
      :party="party"
      @locate="locateToSelfParty"
    />

    <RouterView v-slot="{ Component }" class="absolute left-16 top-6 z-[1000]">
      <Suspense>
        <div>
          <component :is="Component" />
        </div>
        <template #fallback>
          <OLoading fullPage active iconSize="xl" />
        </template>
      </Suspense>
    </RouterView>
  </div>
</template>

<style>
@import 'leaflet/dist/leaflet.css';
@import 'vue-leaflet-markercluster/dist/style.css';
@import '@geoman-io/leaflet-geoman-free/dist/leaflet-geoman.css'; /* TODO: */

.leaflet-pm-toolbar .icon-river {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MTIgNTEyIiBzdHlsZT0iaGVpZ2h0OiA1MTJweDsgd2lkdGg6IDUxMnB4OyI+PGcgY2xhc3M9IiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCwwKSIgc3R5bGU9IiI+PHBhdGggZD0iTTI2My44NDQgNDAuMzQ0QzIzNC4xIDIxMy4yMDIgMTQ1LjU5NCAyNDguMDMgMTQ1LjU5NCAzNjkuMjJjMCA2MC44MDQgNjAuMTA2IDEwNS41IDExOC4yNSAxMDUuNSA1OS40NSAwIDExNS45MzctNDEuODAzIDExNS45MzctOTkuNTMzIDAtMTE2LjMzMi04NS4yLTE2Mi4zMTItMTE1LjkzNi0zMzQuODQzem0tNTguMjggMjE3LjA5NGMtMjcuOTYzIDc1LjUzLTUuMTA1IDE1NC41NjcgNTQuMjUgMTc5LjM3NSAxNS4xODUgNi4zNDggMzEuNzI0IDcuNzE0IDQ3LjkwNSA2LjI4LTExNi4xMzQgNDkuNzg3LTE4NS44MzYtNzkuODE2LTEwMi4xNTgtMTg1LjY1NnoiIGZpbGw9IiMwMDAiIGZpbGwtb3BhY2l0eT0iMSI+PC9wYXRoPjwvZz48L3N2Zz4=');
}

.leaflet-pm-toolbar .icon-forest {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MTIgNTEyIiBzdHlsZT0iaGVpZ2h0OiA1MTJweDsgd2lkdGg6IDUxMnB4OyI+PGcgY2xhc3M9IiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCwwKSIgc3R5bGU9IiI+PHBhdGggZD0iTTI0OS4yOCAxOS4xODh2LjI1Yy0xOC4xMTQgMzguNjM0LTQ1LjA2NSA3Mi4zNi03Ny42ODYgMTAyLjkzN2wzNy43Mi0zLjkzOC01MS4zNDUgNjUuMDMyIDI0LjgxLTcuOTA3LTMzLjYyNCA1NC44NzUgMTYuNTMgOS44NDMtNjUuMjUgOTIuMTU3IDM2LjA5NS4xODgtNTEuNjg2IDgzLjU5NCA2My41NjItOC4xMjYgMTIgMzIuMDk0IDY2LjQzOC0yNS4yODJMMjE1LjUgNDkzLjI4aDUyLjkzOGwtNi41MzItNjguMjE3IDM4LjE4OCAxNi40MDYgMTAuMTg3LTI0Ljc4MyA0NC4yODMgMjAuOTcgNTYuNDA2LTIwLjc1LTM3LjA2NC02NC4wOTQtMTIuNDM3LTIuMjgyIDYuNzggMTcuMTkgNy44NDQgMTkuOTA1LTE5LjkzOC03Ljc4LTUwLjkwNi0xOS45MDhWMzk1LjY4OGwtMTQuMTU2LTguNTk0LTY5LjM3NS00Mi0yMS41OTUgMjEuMjUtMTguMDMgMTcuNzUgMi4xNTUtMjUuMjIgMi4xMjUtMjQuNjU1IDE4LjE4OCAxLjU2IDkuMjE4LTkuMDkyIDUuMTktNS4wOTQgNi4yMTggMy43NSA2MS4zNzUgMzcuMTU2di0yOS45MDZsMTIuNzUgNC45NyA0My43MTggMTcuMDkyLTUuMDkyLTEyLjkwNi02LjE1Ny0xNS42NTYgMTYuNTMzIDMuMDMgNDUuNDY4IDguMzQ1LTM0LjUzLTM4Ljk0LTIzLjYyNSAxNC4wMzMtNi42ODggMy45NjgtNS4xMjUtNS44NzQtMTQuMjgtMTYuNDM3LjIxOCAxLjIxNy0xOC40MDYgMy4yMi01Ljk3LTM0LjMxMy01Ljc1LTMzLjA2MyAyMiAyNS4zNDUgMzEuMTg4IDM1Ljg3NSA0My45MDctMjYuMDNjLTI0LjY3LTE5LjU0My0zOS41MDctMzMuODctNDkuNjU4LTQ4LjgxNGwuODEzIDEyLjY1NiAxLjk3IDMxLTE4Ljc1LTI0Ljc1LTM0LjQ3LTQ1LjQzNy0yMi4yNSA0Ni44MTMtMTMuODQ0IDI5LjEyNS0zLjg0My0zMi4wMzItMy41LTI4Ljg0MyAxNi41MzItMS45NjggMTYuNjI0LTM0Ljk3IDYuNTk0LTEzLjg3NSA5LjI4IDEyLjIyIDI1IDMyLjkzNi0uNzUtMTEuNTMtLjkwNi0xNC4yOCAxMy40NyA0LjkzNkwzNDEuODEgMTg4bC0yNi4xMjUtMzUuMTU2LTU1Ljg0My0yOC44NzUtOC45MzggMjAuMjE4LTkuNjU2IDIxLjkzNy03LjcyLTIyLjY4OC03LjQ2OC0yMS44NzUgMTYuOTctNS43OCAzLjcxOC04LjQzOCA0LTkuMTI1IDguODQ0IDQuNTkzIDQ5LjM3NSAyNS41MyAxNi40NjctNS41NjJjLTQzLjQyLTM0LjMxLTY0LjYzLTY4Ljg4Ni03Ni4xNTYtMTAzLjU5M3oiIGZpbGw9IiMwMDAiIGZpbGwtb3BhY2l0eT0iMSI+PC9wYXRoPjwvZz48L3N2Zz4=');
}

.leaflet-pm-toolbar .icon-mountains {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MTIgNTEyIiBzdHlsZT0iaGVpZ2h0OiA1MTJweDsgd2lkdGg6IDUxMnB4OyI+PGcgY2xhc3M9IiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCwwKSIgc3R5bGU9IiI+PHBhdGggZD0iTTI0NS43OTUgMTkuMTJsLTUyLjM2MyAxNTMuNTEzIDI2LjY3IDYxLjkzNyAzOC44ODQtNTIuMzcgNTMuMjE3IDY3LjQ5MyAxMS42ODItNDAuNDg2LTc4LjA5LTE5MC4wODZ6TTEwMS4xNzIgMTkzLjY5bC0yOS4wNiA4MC4yMjIgMjQuNTQtMTIuNzE1IDI0LjgwMyAxNC4zMyAxMS42NC00OC4wMTMtMzEuOTIzLTMzLjgyNXptODMuMjY3IDUuMzA4bC0yMC43NzYgNjAuOTA0LTE1LjI3LTE2LjE3Ny0xNC42NjIgNjAuNDgtMzcuNTY4LTIxLjcwNy0zMy40NCAxNy4zMjRMMTkuMDQgNDIwLjQybDg0Ljg4NCAzMC45MzcgNzMuNDE4LTIyLjQzNyA3My45MzUgMTkuNDcgNzEuNjYtMjEuNTM2IDkxLjk3MyAyNS4yMjYgNzcuMjgtMzEuNjYtNDguNDQtODkuMDA2LTM5LjA0NSAyNi42NjQtMzguODkyLTI3LjU3Ni0yNy4xNTMgNDIuNzktMTUuNzgtMTAuMDEzIDM5LjAzMi02MS41MS0yNi42LTY0Ljc1Mi0xNS4yNDYgNTIuODMtNjAuNjM0LTc2LjktNDMuNjY0IDU4LjgxLTMxLjMzLTcyLjc2em0yMjMuMDYgNjUuODFMMzc1Ljg0IDMxNC43bDI5LjA2NiAyMC42MSAyOS44NjUtMjAuMzk0LTI3LjI3LTUwLjExeiIgZmlsbD0iIzAwMCIgZmlsbC1vcGFjaXR5PSIxIj48L3BhdGg+PC9nPjwvc3ZnPg==');
}

.leaflet-container.mode-create {
  @apply cursor-crosshair;
}

.leaflet-right .leaflet-control {
  @apply mb-2;
}

.leaflet-container .leaflet-control-attribution {
  @apply mb-0;
}

/* TODO: colors */
.marker-cluster-small {
  @apply bg-primary;
}

.marker-cluster-small div {
  @apply bg-primary-hover text-content-100;
}

.marker-cluster-medium {
  @apply bg-primary;
}

.marker-cluster-medium div {
  @apply bg-primary-hover text-content-100;
}

.marker-cluster-large {
  @apply bg-primary;
}

.marker-cluster-large div {
  @apply bg-primary-hover text-content-100;
}

/* Popup */
.leaflet-popup-content-wrapper,
.leaflet-popup-tip {
  @apply border-transparent !bg-base-100/80 !text-content-100 !shadow-none;
}

.leaflet-container a.leaflet-popup-close-button {
  @apply !text-content-200;
}

.leaflet-container a.leaflet-popup-close-button:hover,
.leaflet-container a.leaflet-popup-close-button:focus {
  @apply !text-content-100;
}

.leaflet-oldie .leaflet-control-zoom,
.leaflet-oldie .leaflet-control-layers,
.leaflet-oldie .leaflet-popup-content-wrapper,
.leaflet-oldie .leaflet-popup-tip {
  @apply !border-base-100/80;
}

/* Tooltip */
.leaflet-tooltip {
  @apply !rounded-xl border-transparent !bg-base-100/80 !text-content-100 !shadow-none;
}

.leaflet-tooltip-top:before {
  @apply !border-t-base-100/80;
}

.leaflet-tooltip-bottom:before {
  @apply !border-b-base-100/80;
}

.leaflet-tooltip-left:before {
  @apply !border-l-base-100/80;
}

.leaflet-tooltip-right:before {
  @apply !border-r-base-100/80;
}
</style>
