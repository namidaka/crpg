import { Position } from 'geojson';
import { LatLng, type LatLngExpression } from 'leaflet';

export const positionToLatLng = (p: Position) => new LatLng(p[1], p[0]);

export const coordinatesToLatLngs = (coordinates: Position[][]): LatLngExpression[][] => [
  coordinates[0].map(positionToLatLng),
];
