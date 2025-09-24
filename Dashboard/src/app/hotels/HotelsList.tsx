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
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { useAuth } from "../login/AuthContext"
import type { HotelDto } from "@/types/HotelDTO"
import { ApiService } from "@/services/ApiService"

export function HotelList() {
  const { token } = useAuth()
  const [hotels, setHotels] = useState<HotelDto[]>([])
  const [loading, setLoading] = useState(true)
  const [cityFilter, setCityFilter] = useState("")
  const [editingId, setEditingId] = useState<number | null>(null)
  const [editValues, setEditValues] = useState<Partial<HotelDto>>({})

  useEffect(() => {
    const fetchHotels = async () => {
      setLoading(true)
      const hotelsData = await ApiService.getAllHotels()
      if (hotelsData) setHotels(hotelsData)
      setLoading(false)
    }

    if (token) fetchHotels()
    else setLoading(false)
  }, [token])

  if (loading) return <div>Loading hotels...</div>
  if (!hotels.length) return <div>No hotels found</div>

  const cityNames = Array.from(new Set(hotels.map((h) => h.cityName)))
  const filteredHotels = cityFilter
    ? hotels.filter((h) => h.cityName === cityFilter)
    : hotels

  const startEdit = (hotel: HotelDto) => {
    setEditingId(hotel.id)
    setEditValues({ ...hotel })
  }

  const cancelEdit = () => {
    setEditingId(null)
    setEditValues({})
  }

  const saveEdit = async (id: number) => {
    if (!editValues.id) editValues.id = id

    try {
      const isSuccess = await ApiService.updateHotel(editValues as HotelDto)
      if (isSuccess) {
        setHotels((prev) =>
          prev.map((h) => (h.id === id ? { ...h, ...editValues } : h))
        )
        setEditingId(null)
      } else {
        console.error("Failed to save changes.")
      }
    } catch (error) {
      console.error("An unexpected error occurred:", error)
    }
  }

  return (
    <div className="flex flex-col gap-4">
      <select
        value={cityFilter}
        onChange={(e) => setCityFilter(e.target.value)}
        className="border p-2 rounded w-64"
      >
        <option value="">All Cities</option>
        {cityNames.map((city) => (
          <option key={city} value={city}>
            {city}
          </option>
        ))}
      </select>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>City</TableHead>
            <TableHead>Address</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead>Weekday Time</TableHead>
            <TableHead>Saturday Time</TableHead>
            <TableHead>Holidays Time</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {filteredHotels.map((hotel) => (
            <TableRow key={hotel.id}>
              <TableCell>{hotel.id}</TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.hotelName || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, hotelName: e.target.value }))
                    }
                  />
                ) : (
                  hotel.hotelName
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.cityName || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, cityName: e.target.value }))
                    }
                  />
                ) : (
                  hotel.cityName
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.address || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, address: e.target.value }))
                    }
                  />
                ) : (
                  hotel.address
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.email || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, email: e.target.value }))
                    }
                  />
                ) : (
                  hotel.email ?? "-"
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.phone || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, phone: e.target.value }))
                    }
                  />
                ) : (
                  hotel.phone ?? "-"
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.weekdayTime || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, weekdayTime: e.target.value }))
                    }
                  />
                ) : (
                  hotel.weekdayTime ?? "-"
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.saturdayTime || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, saturdayTime: e.target.value }))
                    }
                  />
                ) : (
                  hotel.saturdayTime ?? "-"
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <Input
                    value={editValues.holidaysTime || ""}
                    onChange={(e) =>
                      setEditValues((v) => ({ ...v, holidaysTime: e.target.value }))
                    }
                  />
                ) : (
                  hotel.holidaysTime ?? "-"
                )}
              </TableCell>

              <TableCell>
                {editingId === hotel.id ? (
                  <>
                    <Button size="sm" onClick={() => saveEdit(hotel.id)}>
                      Save
                    </Button>
                    <Button size="sm" variant="secondary" onClick={cancelEdit}>
                      Cancel
                    </Button>
                  </>
                ) : (
                  <Button size="sm" onClick={() => startEdit(hotel)}>Edit</Button>
                )}
              </TableCell>

            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  )
}
