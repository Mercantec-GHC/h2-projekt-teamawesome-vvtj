// src/types/UserDTO.ts

export type UserInfoDTO = {
  userId: number
  firstName: string
  lastName: string
  address: string
  postalCode: string
  city: string
  country: string
  phoneNumber: string
  dateOfBirth: string
  specialRequests: string
  createdAt: string
  updatedAt: string
}

export type UserDTO = {
  id: number
  email: string
  userName: string
  userRole: string
  hashedPasword: string
  userInfo: UserInfoDTO
  lastLogin: string
  createdAt: string
  updatedAt: string
}
