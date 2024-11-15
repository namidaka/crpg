import { coordinatesToLatLngs, positionToLatLng } from './geometry'

it('positionToLatLng', () => {
  expect(positionToLatLng([1, 2])).toEqual({ lat: 2, lng: 1 })
})

it('coordinatesToLatLngs', () => {
  expect(coordinatesToLatLngs([[[1, 2]]])).toEqual([[{ lat: 2, lng: 1 }]])
})
