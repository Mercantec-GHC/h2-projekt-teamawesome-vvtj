export interface RoomTypeDto {
  id: number
  typeofRoom: number | null
  maxCapacity: number
  description?: string | null
  pricePerNight?: number | null

  // rules
  hasKitchenette?: boolean | null
  hasExtraTowels?: boolean | null
  hasBalcony?: boolean | null
  hasJacuzzi?: boolean | null
  hasSeaView?: boolean | null
  hasGardenView?: boolean | null
  hasAirCondition?: boolean | null
  hasTV?: boolean | null
  hasKettle?: boolean | null
  hasMiniFridge?: boolean | null
  area?: number | null
  hasVault?: boolean | null
}
