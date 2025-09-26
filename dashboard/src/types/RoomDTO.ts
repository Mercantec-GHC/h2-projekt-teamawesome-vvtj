export type RoomTypeDto = {
  id: number
  createdAt: string
  updatedAt: string
  typeofRoom: string 
  maxCapacity: number
  description: string
  pricePerNight: number
  hasKitchenette: boolean
  hasExtraTowels: boolean
  hasSeaView: boolean
  hasGardenView: boolean
  hasBalcony: boolean
  hasJacuzzi: boolean
  hasAirCondition: boolean
  hasTV: boolean
  hasKettle: boolean
  hasMiniFridge: boolean
  area: number
  hasVault: boolean
}

export type RoomDto = {
  id: number
  isAvailable: boolean
  isBreakfast: boolean
  availableFrom: string
  roomNumber: number
  roomType: RoomTypeDto
  hotelName: string
  hotelId: number
}
