<script setup lang="ts">
import type { Position } from 'geojson'

import { LPolyline } from '@vue-leaflet/vue-leaflet'

import type { Party } from '~/models/strategus/party'

import { PartyStatus } from '~/models/strategus/party'
import { positionToLatLng } from '~/utils/geometry'

const { party } = defineProps<{ party: Party }>()

// TODO: colors
// TODO: to service
const attackColor = '#f14668'
const followColor = '#10b981'
const moveColor = '#485fc7'

const partyMovementLine = computed(() => {
  let color: string
  const positions: Position[] = []

  switch (party.status) {
    case PartyStatus.MovingToPoint:
      positions.push(...party.waypoints.coordinates)
      color = moveColor
      break
    case PartyStatus.FollowingParty:
      positions.push(party.targetedParty!.position.coordinates)
      color = followColor
      break
    case PartyStatus.MovingToSettlement:
      positions.push(party.targetedSettlement!.position.coordinates)
      color = moveColor
      break
    case PartyStatus.MovingToAttackParty:
      positions.push(party.targetedParty!.position.coordinates)
      color = attackColor
      break
    case PartyStatus.MovingToAttackSettlement:
      positions.push(party.targetedSettlement!.position.coordinates)
      color = attackColor
      break
    default:
      return null
  }

  return {
    // TODO: ts
    color,
    dashArray: '10, 10',
    dashOffset: '10',
    latLngs: [party.position.coordinates, ...positions].map(positionToLatLng),
    options: {
      pmIgnore: true,
    },
  }
})
</script>

<template>
  <LPolyline
    v-if="partyMovementLine !== null"
    v-bind="partyMovementLine"
  />
</template>
