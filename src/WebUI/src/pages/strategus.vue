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
import { useTerrains } from '@/composables/strategus/use-terrains';

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
    noFooter: true,
  },
});

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
  terrainsFeatureCollection,
  loadTerrains,
  terrainVisibility,
  toggleTerrainVisibilityLayer,
  toggleEditMode,
  onTerrainUpdated,
} = useTerrains(map);

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
  await Promise.all([loadSettlements(), loadTerrains(), partySpawn()]);

  if (party.value !== null) {
    map.flyTo(positionToLatLng(party.value.position.coordinates), 5, {
      animate: false,
    });
  }

  applyMoveEvents();
  mapIsLoading.value = false;
};
</script>

<template>
  <div :style="{ height: `calc(100vh - ${mainHeaderHeight}px)` }">
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

      <ControlMousePosition />
      <ControlLocateParty v-if="party !== null" :party="party" position="bottomleft" />

      <LayerTerrain
        v-if="terrainVisibility"
        :data="terrainsFeatureCollection"
        @update="onTerrainUpdated"
      />

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

      <DialogSettlement
        v-if="
          party !== null &&
          party.targetedSettlement !== null &&
          inSettlementStatuses.has(party.status)
        "
        :party="party"
        :isTogglingRecruitTroops="isTogglingRecruitTroops"
        @toggleRecruitTroops="toggleRecruitTroops"
      />
    </div>
  </div>
</template>

<style>
@import 'leaflet/dist/leaflet.css';
@import 'vue-leaflet-markercluster/dist/style.css';
@import '@geoman-io/leaflet-geoman-free/dist/leaflet-geoman.css'; /* TODO: */

.leaflet-pm-toolbar .icon-terrain-barrier {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIzMiIgaGVpZ2h0PSIzMiIgZmlsbD0ibm9uZSI+PHBhdGggZmlsbD0iIzAwMCIgZD0ibTIxLjc2IDItMTEuNTgyLjAyNEwyIDEwLjIzN2wuMDI0IDExLjU4M0wxMC4yMzcgMzBsMTEuNTgzLS4wMjRMMzAgMjEuNzZsLS4wMjQtMTEuNTgyTDIxLjc2IDJabS0uNDc3IDEuMTcxIDcuNTIgNy40OS4wMjMgMTAuNjIzLTcuMzM0IDcuMzY0LS4xNTUuMTU0LTEwLjYyMy4wMjQtNy41Mi03LjQ4OS0uMDIzLTEwLjYyMyA3LjQ5LTcuNTIgMTAuNjIzLS4wMjNabS0uNDQ2IDEuMDczLTkuNzMuMDItNi44NjMgNi44OTcuMDIgOS43MyA2Ljg5NyA2Ljg2MyA5LjczLS4wMTggNi44NjMtNi45LS4wMTgtOS43MjktNi45LTYuODYzWk03LjIwNSAxMy4yODJjLjMxNiAwIC42NDMuMDMuOTc3LjA3Ny4zNC4wNDguNjg1LjEyIDEuMDQzLjIxNXYxLjIyMWE1LjA3MyA1LjA3MyAwIDAgMC0uOTMtLjMyMSAzLjc1NCAzLjc1NCAwIDAgMC0uODU4LS4xMDhjLS4zNTcgMC0uNjIuMDQ4LS43OS4xNWEuNDg4LjQ4OCAwIDAgMC0uMjU1LjQ1OGMwIC4xNTUuMDU3LjI3NC4xNy4zNjQuMTE3LjA4My4zMjcuMTYuNjMuMjJsLjYzMi4xMjVjLjY0NC4xMzEgMS4xMDMuMzI4IDEuMzc3LjU5LjI2OC4yNjguNDA1LjY0My40MDUgMS4xMjYgMCAuNjM3LS4xOSAxLjExNC0uNTY2IDEuNDMtLjM4MS4zMS0uOTYuNDY1LTEuNzQuNDY1LS4zNjMgMC0uNzMzLS4wMzYtMS4xMDMtLjEwOGE2LjQyOCA2LjQyOCAwIDAgMS0xLjEwOC0uMzFWMTcuNjJjLjM2OS4xOTcuNzI2LjM0NiAxLjA3LjQ0Ny4zNDYuMDk2LjY4Mi4xNDMuOTk4LjE0My4zMjggMCAuNTc4LS4wNTMuNzUtLjE2YS41MTguNTE4IDAgMCAwIC4yNjMtLjQ2NS41Mi41MiAwIDAgMC0uMTc5LS40MjNjLS4xMTktLjA5Ni0uMzUxLS4xODUtLjcwMy0uMjYzbC0uNTc4LS4xMjVjLS41OC0uMTI1LTEuMDA0LS4zMjEtMS4yNzMtLjU5Ni0uMjY2LS4yNjgtLjM5OS0uNjM3LS4zOTktMS4wOTYgMC0uNTc4LjE4Ni0xLjAxOC41NTgtMS4zMjguMzczLS4zMTYuOTA4LS40NzEgMS42MDktLjQ3MVptMTEuMTgzIDBjLjkzIDAgMS42NTYuMjY4IDIuMTg2LjgwNC41My41My43OTMgMS4yNjMuNzkzIDIuMjA1IDAgLjkzNS0uMjYyIDEuNjY4LS43OTMgMi4yMDQtLjUzLjUzLTEuMjU3Ljc5OS0yLjE4Ni43OTktLjkzIDAtMS42NTYtLjI2OS0yLjE4Ny0uNzk5LS41My0uNTM2LS43OTItMS4yNjktLjc5Mi0yLjIwNCAwLS45NDIuMjYyLTEuNjc0Ljc5Mi0yLjIwNS41My0uNTM2IDEuMjU4LS44MDQgMi4xODctLjgwNFptLTguNDU0LjEwN2g1LjMzOHYxLjEyNmgtMS45MTh2NC42NjVoLTEuNDk2di00LjY2NUg5LjkzNFYxMy4zOVptMTIuMjYxIDBoMi40NzljLjczMiAwIDEuMjk4LjE2NyAxLjY5Mi40OTUuMzk5LjMyMi41OTUuNzg2LjU5NSAxLjM5NCAwIC42MDgtLjE5NiAxLjA3OC0uNTk1IDEuNDA2LS4zOTQuMzIyLS45Ni40ODgtMS42OTIuNDg4aC0uOTl2Mi4wMDhoLTEuNDg5di01Ljc5Wm0tMy44MDcuOTc3Yy0uNDU5IDAtLjgxLjE2Ny0xLjA2LjUwNy0uMjUuMzMzLS4zNzYuODEtLjM3NiAxLjQxOCAwIC42MDcuMTI1IDEuMDc4LjM3NSAxLjQxOC4yNS4zMzMuNjAyLjUgMS4wNi41LjQ2IDAgLjgxMS0uMTY3IDEuMDYxLS41LjI1LS4zNC4zNzYtLjgxLjM3Ni0xLjQxOCAwLS42MDgtLjEyNS0xLjA4NS0uMzc2LTEuNDE4LS4yNS0uMzQtLjYwMS0uNTA3LTEuMDYtLjUwN1ptNS4yOTcuMTA4djEuNjE0aC44MjhjLjI5MiAwIC41MTItLjA3MS42NzMtLjIwOC4xNTUtLjE0My4yMzItLjM0Ni4yMzItLjYwMnMtLjA3Ny0uNDU5LS4yMzItLjU5NmMtLjE2LS4xNDMtLjM4MS0uMjA4LS42NzMtLjIwOGgtLjgyOFoiLz48L3N2Zz4=');
}

.leaflet-pm-toolbar .icon-terrain-thick-forest {
  background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIzMiIgaGVpZ2h0PSIzMiIgZmlsbD0ibm9uZSI+PHBhdGggZmlsbD0iIzAwMCIgZD0iTTEwLjk4MyAyYy0uNjA4Ljk2Ni0xLjI5NiAxLjk3LTIuNDEyIDIuOTJsLjA4MS4wNzggMS4zNjMtLjAyNC0uMjA0LjYyYy0uMDMuMDg4LS4wNjIuMTc2LS4wOTcuMjYzLjE2Mi4xMTIuMzMyLjIyNC41MTMuMzM1bDEuNDUuODg3LTIuNjYxLS4wNDZhNi4yODIgNi4yODIgMCAwIDEtLjM2NS40MmMuNTA5LjY2NyAxLjI3NSAxLjMxNSAyLjIzOCAxLjg5N2wtLjcxOC0uMDIxLS4wMDEuMzI4Yy40OTQuMzY4IDEuMDMuNzIgMS41NzIgMS4wNyAxLjA0Ny0uNjcgMS44MzktMS40MTggMi4yODYtMi4xNzdsLTMuMzE5LjA1NyAxLjQ1LS44ODdhOS42MjcgOS42MjcgMCAwIDAgMS4zMzUtLjk3NmMtLjI5NS0uMzE3LS41Mi0uNjM0LS42NTEtLjk2NmwtLjIzNC0uNTkyIDEuNzIxLS4xMDZjLTEuMzA5LS44MzgtMi42MjMtMS43Ny0zLjM0Ny0zLjA4Wm0xMy41NzUuNzY1Yy0uMzc4Ljc1LS44MDMgMS42NzktMS4zNjggMi41ODIuNjMyIDEuMDYzIDEuNDEzIDIuMDgzIDIuNzA2IDMuMDMxbDEuMTgyLjg2Ny0zLjAzOC0uMDYzYy40MDIuNzAyIDEuMTUzIDEuNTU0IDIgMi4zMTcgMS4xMDkuOTk4IDIuMzY5IDEuODg1IDMuMTIgMi4zMWwuNjU2LjM3di0uODA4Yy0uOTkzLS42OS0xLjcyMi0xLjQ5Mi0yLjEwMS0yLjQ0MWwtLjIzOC0uNTk2IDIuNTIzLS4xNWMtLjU3My0uMjUyLTEuMTQzLS40NzgtMS42NjItLjY5Ny0uNDkyLS4yMDgtLjk0LS40MTItMS4zMTUtLjY1LS4zNzYtLjIzOS0uNy0uNTE4LS44Ni0uOTIzbC0uMjI3LS41NzYgMi4wODgtLjE4NGMtMS40MDMtMS4xNy0yLjczMy0yLjgyMS0zLjQ2Ni00LjM4OVptLTE4LjU0NS4zMzJDNS4yOSA0LjQwOCAzLjk3NSA1LjM0IDIuNjY2IDYuMTc3bDEuNzIuMTA2LS4yMzMuNTkzYy0uMjc2LjcwMS0uOTczIDEuMzk2LTEuOTIgMi4xMzNhNzguNTkzIDc4LjU5MyAwIDAgMC0uMDIyIDEuMDI2YzAgLjAwMy4wMTEuMDY0LjAxNy4xODEuMDAyLjA0My4wMDIuMTg0LjAwNC4yNTNsLjguMDQ2di4wMzFsLS4yMjYuNTY2Yy0uMDY5LjE3MS0uMTQ1LjM1MS0uMjQxLjUzNGwuNjY3LS40MDhjMi4xNjItMS4zMjMgMi45MTctMi43ODYgMy45LTQuMzNsLS4wMTQuMzY5LjI1OS0uMTA5YTQuMTg0IDQuMTg0IDAgMCAxLS4xOTItLjQ3N2wtLjIwNC0uNjIgMS41MzguMDI3Yy0xLjE3My0uOTczLTEuODgtMi4wMDctMi41MDYtM1ptMTAuMzYgMS41MjljLS42MjYuOTkzLTEuMzM0IDIuMDI3LTIuNTA3IDNsMS41MzgtLjAyNi0uMjA0LjYyYy0uMzg1IDEuMTY3LTEuMzE3IDIuMTkyLTIuNTk2IDMuMDZsMS4wMzYuNjcgMS4wNTguMDQtLjI0LjYwOGMtLjEzMy4zMzktLjMxLjY2LS41Mi45NjNsLjUwNS0uMzU4YzEuMDM3LS43MzQgMi4xMjgtMS41OTEgMi45OTctMi40MDMuNTgtLjU0MiAxLjA0LTEuMDcyIDEuMzM2LTEuNWwtMi45MjQtLjIxNS45NTQtLjczOWMuNi0uNDY0IDEuMjE0LS45MjIgMS43OS0xLjM5OS0uODk2LS42NS0xLjcwOS0xLjM5LTIuMjI0LTIuMzIxWm01LjM4NS4wNGMtLjczOSAxLjU1LTIuMDk3IDIuNjYzLTMuNDIgMy42NzRsMS44MTYuMTMzLS4xODUuNTYzYy0uMjYuNzkzLS45ODMgMS41ODYtMS44OTcgMi40NGEyNy41MjEgMjcuNTIxIDAgMCAxLTIuMDc3IDEuNzMybDIuODUxLjIwMi0uMTg4LjU2NmMtLjY1MiAxLjk1Ni0yLjcxIDMuMjk0LTQuMzg3IDQuMzc0Ljc4LjQ2NiAxLjYwMS44OTMgMi4zMDQgMS4yOTEgMy45NTQgMS4wOSA4LjU0LjQwNCAxMS42MjUtLjkyNS0uNzYzLS40MzgtMS4zOC0uODQyLTEuODk0LTEuMzMyLS42ODQtLjY0OS0xLjE2Mi0xLjQ0NC0xLjUyNy0yLjU1NmwtLjE3NC0uNTMzIDMuMDA3LS4zODJhMjIuMTIgMjIuMTIgMCAwIDEtMi4xOTEtMS43MjZjLTEuMTY0LTEuMDQ4LTIuMjIyLTIuMTk5LTIuNTQxLTMuMzZsLS4xNjUtLjU5OCAxLjYxMy4wMzNjLTEuMjI2LTEuMTYxLTEuOTQxLTIuNC0yLjU3LTMuNTk3Wk03LjQ0NSA4LjE0M2MtLjYyNS45OTQtMS4zMzMgMi4wMjgtMi41MDYgM2wxLjUzOC0uMDI2LS4yMDQuNjJjLS40NzcgMS40NDYtMS43OTIgMi42NzItMy41NjYgMy42NTVsMy4wNjQuMTE2LS4yNC42MDdDNC44OTMgMTcuNzQyIDMuODEgMTkuMTggMiAyMC4yNThjNC4yNzUgMS4zMDYgOS4wNiAxLjgyNiAxMy40LS4yMDctLjc0Ny0uNDA4LTEuNTU3LS44NDgtMi4zMzQtMS4zNjMtMS4wNDUtLjY5My0xLjk5OC0xLjQ5LTIuNDEzLTIuNTNsLS4yMzgtLjU5NSAyLjgtLjE2NmEyNy4wNTMgMjcuMDUzIDAgMCAxLTEuOTQxLTEuMzUzYy0uOTQtLjczMy0xLjY4Mi0xLjM5NC0xLjk2OS0yLjEyMWwtLjIzNC0uNTkzIDEuNzIxLS4xMDZjLTEuMzA5LS44MzctMi42MjItMS43NjktMy4zNDctMy4wOFptLTIuMzQ0IDMuOTI0LTIuODM2LjA0OS0uMDI5LjAzOGMtLjAxNiAxLjI0NC0uMDI2IDEuOTMyLS4wMjkgMi40NTIgMS4zNS0uNzU1IDIuMzY1LTEuNjQgMi44OTQtMi41MzlabTYuMTE0LjcxOGMuMTkuMTcxLjM5OS4zNDkuNjI4LjUyOC4yMDQuMTU4LjQyLjMyLjY0NC40OC4zMTUtLjMuNTg0LS42MS43OTMtLjkzbC0yLjA2NS0uMDc4Wm0yLjQxIDEuMTgyYTYuNTA1IDYuNTA1IDAgMCAxLS4zNDMuMzc2Yy41MzYuMzU4IDEuMDk2LjcwOSAxLjY0MiAxLjAzbDEuMDIuNjA0Yy42NTctLjU0NSAxLjIyNC0xLjEyNCAxLjU3NS0xLjczNGwtMy44OTMtLjI3NlptMTYuMTkuNTk4LTMuOTU2LjUwM2MuMjcuNjYyLjU4MSAxLjE0MyAxIDEuNTYybC0uMDEyLS41NTZhMTYuODQ1IDE2Ljg0NSAwIDAgMCAyLjk3NC0uNDI2Yy0uMDA0LS40MTUtLjAwNS0uNzE4LS4wMDUtMS4wODNaTTE1LjcgMTYuMTc3bC0zLjg0Mi4yMjhjLjIyNi4zLjUyNi41OTcuODc0Ljg4Ni4zNy4wNzQuNzQ2LjE0MiAxLjEyNS4yMDJhMjguMzU3IDI4LjM1NyAwIDAgMCAxLjg0My0xLjMxNlptLTEyLjcxNS4xNTMtLjAxNyAyLjA2MWE2LjkxNiA2LjkxNiAwIDAgMCAxLjQzNy0yLjAwN2wtMS40Mi0uMDU0Wm0yMy45NTIgMy44NWMtLjgyNi4yNjUtMS43MDguNDg2LTIuNjI2LjY1bC4zMjEgNy42NDdoMi40ODdsLS4xODItOC4yOTdabS0xMC41MjEuNDA1Yy0uMTU0LjA4MS0uMzEuMTYtLjQ2NC4yMzRsLS4wNTQgNi42MTJoMy4wNDhsLS4xNC02LjM5M2ExNS42IDE1LjYgMCAwIDEtMi4zOS0uNDUzWm02Ljk3NC4zODdjLS44NzMuMTE1LTEuNzcuMTc3LTIuNjcyLjE3M2wtLjAyMiA4LjA4NmgzLjA0MWwtLjM0Ny04LjI1OVptLTIwLjQ0Ny41M0wyLjg5IDI4LjFoMi41NmwuMTE1LTYuMDRhMjcuMzc3IDI3LjM3NyAwIDAgMS0yLjYyMi0uNTU5Wm0xMC4xMS4zNjJjLS42Ny4xNjYtMS4zNDQuMjg2LTIuMDIuMzY0bC4yMjcgMy44M2gxLjkxM2wtLjEyLTQuMTk0Wm0tNi41NjQuMzI2LS4xNDUgNy42NyA0LjIwNi0uMTMyLS40NC03LjQxOWEyMC4wNDUgMjAuMDQ1IDAgMCAxLTMuNjIxLS4xMTlaIi8+PC9zdmc+');
}

/* TODO: */
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
</style>
