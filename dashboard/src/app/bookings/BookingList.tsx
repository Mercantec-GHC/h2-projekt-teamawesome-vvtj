"use client"

// React hooks for state and lifecycle
import { useEffect, useMemo, useState } from "react"
// UI components for table, button, input
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components/ui/table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
// Auth context for user info
import { useAuth } from "../login/AuthContext"
import type { BookingDto } from "@/types/BookingDTO"
import { ApiService } from "@/services/ApiService"

export function BookingList() {
    // Get JWT token from auth context
    const { token } = useAuth()
    // Decode JWT to get user role and department (hotel name)
    const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
    
    const userRole = decoded?.role;
    const userHotelName = decoded?.department; 

    // State for bookings, loading, editing, and hotel filter
    const [bookings, setBookings] = useState<BookingDto[]>([])
    const [loading, setLoading] = useState(true)
    const [editingId, setEditingId] = useState<number | null>(null)
    const [editValues, setEditValues] = useState<{ checkIn: string; checkOut: string }>({
        checkIn: "",
        checkOut: "",
    })
    const [hotelFilter, setHotelFilter] = useState("")

    // Fetch all bookings from API when token changes
    useEffect(() => {
        const fetchBookings = async () => {
            setLoading(true);
            try {
                const data = await ApiService.getAllBookings(); 
                if (data) {
                    setBookings(data);
                }
            } catch (error) {
                console.error("Error fetching bookings:", error);
            } finally {
                setLoading(false);
            }
        }

        if (token) {
            fetchBookings();
        } else {
            setLoading(false);
        }
    }, [token])

    // Filter bookings by user role and hotel filter
    const filteredBookings = useMemo(() => {
        let currentBookings = bookings;
        
        // Non-admins only see their own hotel's bookings
        if (userRole !== "Admin" && userHotelName) {
             currentBookings = currentBookings.filter(b => b.hotelName === userHotelName);
        }
        
        // Admins can filter by hotel
        if (hotelFilter) {
            currentBookings = currentBookings.filter(b => b.hotelName === hotelFilter);
        }

        return currentBookings;
    }, [bookings, userRole, userHotelName, hotelFilter]);
    
    // Get unique hotel names for filter dropdown
    const hotelNames = useMemo(() => {
        return Array.from(new Set(bookings.map((b) => b.hotelName)));
    }, [bookings]);

    // Start editing a booking's dates
    const startEdit = (booking: BookingDto) => {
        setEditingId(booking.id)
        setEditValues({
            checkIn: booking.checkIn.split("T")[0],
            checkOut: booking.checkOut.split("T")[0],
        })
    }

    // Cancel editing
    const cancelEdit = () => {
        setEditingId(null)
        setEditValues({ checkIn: "", checkOut: "" })
    }

    // Save edited dates to API and update state
    const saveEdit = async (id: number) => {
        if (!token) return; 

        const updatedBooking = await ApiService.updateBookingDates(
            id,
            editValues.checkIn,
            editValues.checkOut,
            token
        )

        if (updatedBooking) {
            setBookings((prev) =>
                prev.map((b) => (b.id === id ? { ...b, ...updatedBooking } : b))
            )
            setEditingId(null)
        }
    }

    // Show loading or empty state
    if (loading) return <div>Loading bookings...</div>
    if (!bookings.length) return <div>No bookings found</div>

  return (
    <div className="flex flex-col gap-4">
      {/* Hotel filter dropdown for Admins */}
      {userRole === "Admin" && (
      <select
        value={hotelFilter}
        onChange={(e) => setHotelFilter(e.target.value)}
        className="border p-2 rounded w-64"
      >
        <option value="">All Hotels</option>
        {hotelNames.map((hotel) => (
          <option key={hotel} value={hotel}>
            {hotel}
          </option>
        ))}
      </select>
      )}

      {/* Bookings table */}
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
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {filteredBookings.map((booking) => (
            <TableRow key={booking.id}>
              <TableCell>{booking.id}</TableCell>
              <TableCell>{new Date(booking.createdAt).toLocaleString()}</TableCell>
              <TableCell>
                {/* Show updated date or dash if not set */}
                {booking.updatedAt !== "0001-01-01T00:00:00"
                  ? new Date(booking.updatedAt).toLocaleString()
                  : "-"}
              </TableCell>
              <TableCell>{booking.userName}</TableCell>
              <TableCell>{booking.roomId}</TableCell>
              <TableCell>{booking.hotelName}</TableCell>

              {/* Editable check-in date */}
              <TableCell>
                {editingId === booking.id ? (
                  <Input
                    type="date"
                    value={editValues.checkIn}
                    onChange={(e) => setEditValues((v) => ({ ...v, checkIn: e.target.value }))}
                  />
                ) : (
                  new Date(booking.checkIn).toLocaleDateString()
                )}
              </TableCell>

              {/* Editable check-out date */}
              <TableCell>
                {editingId === booking.id ? (
                  <Input
                    type="date"
                    value={editValues.checkOut}
                    onChange={(e) => setEditValues((v) => ({ ...v, checkOut: e.target.value }))}
                  />
                ) : (
                  new Date(booking.checkOut).toLocaleDateString()
                )}
              </TableCell>

              {/* Edit and Delete actions */}
              <TableCell>
              {editingId === booking.id ? (
                <>
                  <Button size="sm" onClick={() => saveEdit(booking.id)}>Save</Button>
                  <Button size="sm" variant="secondary" onClick={cancelEdit}>Cancel</Button>
                </>
              ) : (
                <>
                  <Button size="sm" onClick={() => startEdit(booking)}>Edit</Button>
                  <Button
                    size="sm"
                    variant="destructive"
                    className="ml-2"
                    onClick={async () => {
                      if (!token) return
                      const confirmed = confirm("Delete this booking?")
                      if (!confirmed) return
                      const success = await ApiService.deleteBooking(booking.id, token)
                      if (success) {
                        setBookings((prev) => prev.filter((b) => b.id !== booking.id))
                      }
                    }}
                  >
                    Delete
                  </Button>
                </>
              )}
            </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
