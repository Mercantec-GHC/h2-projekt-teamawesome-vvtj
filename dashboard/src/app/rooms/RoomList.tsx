"use client"

import { useEffect, useState } from "react"
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components/ui/table"
import { useAuth } from "../login/AuthContext"
import type { RoomDto } from "@/types/RoomDTO"

export function RoomList() {
  const { token } = useAuth()
  const [rooms, setRooms] = useState<RoomDto[]>([])
  const [loading, setLoading] = useState(true)


  useEffect(() => {
    if (!token) return
    fetch(`${import.meta.env.VITE_API_URL}/api/Rooms`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((res) => res.json())
      .then((data: RoomDto[]) => setRooms(data))
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [token])

  if (loading) return <div>Loading rooms...</div>
  if (!rooms.length) return <div>No rooms found</div>

  return (
    <Table>
  <TableHeader>
    <TableRow>
      <TableHead>ID</TableHead>
      <TableHead>Room Number</TableHead>
      <TableHead>Type</TableHead>
      <TableHead>Capacity</TableHead>
      <TableHead>Price/Night</TableHead>
      <TableHead>Available</TableHead>
      <TableHead>Breakfast</TableHead>
      <TableHead>Sea View</TableHead>
      <TableHead>Balcony</TableHead>
      <TableHead>Jacuzzi</TableHead>
      <TableHead>Hotel</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    {rooms.map((room) => (
      <TableRow key={room.id}>
        <TableCell>{room.id}</TableCell>
        <TableCell>{room.roomNumber}</TableCell>
        <TableCell>{room.roomType.typeofRoom}</TableCell>
        <TableCell>{room.roomType.maxCapacity}</TableCell>
        <TableCell>{room.roomType.pricePerNight}</TableCell>
        <TableCell>{room.isAvailable ? "Yes" : "No"}</TableCell>
        <TableCell>{room.isBreakfast ? "Yes" : "No"}</TableCell>
        <TableCell>{room.roomType.hasSeaView ? "Yes" : "No"}</TableCell>
        <TableCell>{room.roomType.hasBalcony ? "Yes" : "No"}</TableCell>
        <TableCell>{room.roomType.hasJacuzzi ? "Yes" : "No"}</TableCell>
        <TableCell>{room.hotelName}</TableCell>
      </TableRow>
    ))}
  </TableBody>
</Table>

  )
}
