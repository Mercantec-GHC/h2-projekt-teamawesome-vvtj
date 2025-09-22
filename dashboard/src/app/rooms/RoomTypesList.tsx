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
import type { RoomTypeDto } from "@/types/RoomTypesDTO"

export function RoomTypesList() {
  const { token } = useAuth()
  const [roomTypes, setRoomTypes] = useState<RoomTypeDto[]>([])
  const [loading, setLoading] = useState(true)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [editValues, setEditValues] = useState<{ description: string; price: string }>({
    description: "",
    price: "",
  })

  useEffect(() => {
    if (!token) return
    fetch(`${import.meta.env.VITE_API_URL}/api/RoomTypes`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((res) => res.json())
      .then((data: RoomTypeDto[]) => setRoomTypes(data))
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [token])

  if (loading) return <div>Loading room types...</div>
  if (!roomTypes.length) return <div>No room types found</div>

  const startEdit = (rt: RoomTypeDto) => {
    setEditingId(rt.id)
    setEditValues({
      description: rt.description ?? "",
      price: rt.pricePerNight?.toString() ?? "",
    })
  }

  const cancelEdit = () => {
    setEditingId(null)
    setEditValues({ description: "", price: "" })
  }

  const saveEdit = async (id: number) => {
    try {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/RoomTypes/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          description: editValues.description,
          price: editValues.price ? parseFloat(editValues.price) : null,
        }),
      })

      if (!response.ok) throw new Error("Failed to update room type")

      // update the local state after a successful update.
      setRoomTypes((prev) =>
        prev.map((rt) =>
          rt.id === id
            ? { ...rt, description: editValues.description, pricePerNight: parseFloat(editValues.price) }
            : rt
        )
      )
      cancelEdit()
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>ID</TableHead>
          <TableHead>Type</TableHead>
          <TableHead>Max Capacity</TableHead>
          <TableHead>Price/Night</TableHead>
          <TableHead>Description</TableHead>
          <TableHead>Actions</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {roomTypes.map((rt) => (
          <TableRow key={rt.id}>
            <TableCell>{rt.id}</TableCell>
            <TableCell>{rt.typeofRoom}</TableCell>
            <TableCell>{rt.maxCapacity}</TableCell>

            <TableCell>
              {editingId === rt.id ? (
                <Input
                  type="number"
                  value={editValues.price}
                  onChange={(e) => setEditValues((v) => ({ ...v, price: e.target.value }))}
                />
              ) : (
                rt.pricePerNight ?? "-"
              )}
            </TableCell>

            <TableCell>
              {editingId === rt.id ? (
                <Input
                  type="text"
                  value={editValues.description}
                  onChange={(e) => setEditValues((v) => ({ ...v, description: e.target.value }))}
                />
              ) : (
                rt.description ?? "-"
              )}
            </TableCell>

            <TableCell>
              {editingId === rt.id ? (
                <>
                  <Button size="sm" onClick={() => saveEdit(rt.id)}>
                    Save
                  </Button>
                  <Button size="sm" variant="secondary" onClick={cancelEdit}>
                    Cancel
                  </Button>
                </>
              ) : (
                <Button size="sm" onClick={() => startEdit(rt)}>
                  Edit
                </Button>
              )}
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
