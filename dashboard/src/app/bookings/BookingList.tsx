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
import type { BookingDto } from "@/types/BookingDTO"

export function BookingList() {
  const { token } = useAuth()
  const [bookings, setBookings] = useState<BookingDto[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!token) return

    fetch(`${import.meta.env.VITE_API_URL}/api/Booking`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((res) => res.json())
      .then((data: BookingDto[]) => setBookings(data))
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [token])

  if (loading) return <div>Loading bookings...</div>
  if (!bookings.length) return <div>No bookings found</div>

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>ID</TableHead>
          <TableHead>Created</TableHead>
          <TableHead>Updated</TableHead>
          <TableHead>User</TableHead>
          <TableHead>Room ID</TableHead>
          <TableHead>Hotel</TableHead>
          <TableHead>Check-In</TableHead>
          <TableHead>Check-Out</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {bookings.map((booking) => (
          <TableRow key={booking.id}>
            <TableCell>{booking.id}</TableCell>
            <TableCell>{new Date(booking.createdAt).toLocaleString()}</TableCell>
            <TableCell>
              {booking.updatedAt !== "0001-01-01T00:00:00"
                ? new Date(booking.updatedAt).toLocaleString()
                : "-"}
            </TableCell>
            <TableCell>{booking.userName}</TableCell>
            <TableCell>{booking.roomId}</TableCell>
            <TableCell>{booking.hotelName}</TableCell>
            <TableCell>{new Date(booking.checkIn).toLocaleDateString()}</TableCell>
            <TableCell>{new Date(booking.checkOut).toLocaleDateString()}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
