import type { Point } from 'geojson'

export enum SettlementType {
  Village = 'Village',
  Castle = 'Castle',
  Town = 'Town',
}

export interface SettlementPublic {
  id: number
  name: string
  scene: string
  region: string
  culture: string
  position: Point
  type: SettlementType
}
