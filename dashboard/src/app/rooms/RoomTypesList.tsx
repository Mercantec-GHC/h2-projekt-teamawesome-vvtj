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
import { ApiService } from "@/services/ApiService"

export function RoomTypesList() {
  const { token } = useAuth()
  const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
  const role = decoded?.role;
  const [roomTypes, setRoomTypes] = useState<RoomTypeDto[]>([])
  const [loading, setLoading] = useState(true)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [editValues, setEditValues] = useState<{ description: string; price: string }>({
    description: "",
    price: "",
  })

   useEffect(() => {
    const fetchRoomTypes = async () => {
      setLoading(true);
      const typesData = await ApiService.getAllRoomTypes();
      if (typesData) {
        setRoomTypes(typesData);
      }
      setLoading(false);
    };

    if (token) {
      fetchRoomTypes();
    } else {
      setLoading(false);
    }
  }, [token]);

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
  const updatedData = {
    description: editValues.description,
    price: editValues.price ? parseFloat(editValues.price) : null,
  };

  try {
    const isSuccess = await ApiService.updateRoomType(id, updatedData);

    if (isSuccess) {
      console.log("Room type updated successfully!");
      
      const roomsData = await ApiService.getAllRoomTypes();
      if (roomsData) {
        setRoomTypes(roomsData);
        setEditingId(null);
      }
    } else {
      console.error("Failed to save changes.");
    }
  } catch (error) {
    console.error("An unexpected error occurred:", error);
  }
};

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
                  role === "Admin" ? (
                      <>
                          <Button size="sm" onClick={() => saveEdit(rt.id)}>
                              Save
                          </Button>
                          <Button size="sm" variant="secondary" onClick={cancelEdit}>
                              Cancel
                          </Button>
                      </>
                  ) : (
                      <span className="text-gray-500">View Only</span>
                  )
              ) : (
              role === "Admin" && (
                  <Button size="sm" onClick={() => startEdit(rt)}>
                      Edit
                  </Button>
        )
    )}
</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
