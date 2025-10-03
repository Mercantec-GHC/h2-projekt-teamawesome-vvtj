"use client"

// React hooks for state and lifecycle
import { useEffect, useState } from "react"
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
// Auth context to get user token and role
import { useAuth } from "../login/AuthContext"
import type { RoomTypeDto } from "@/types/RoomTypesDTO"
// API service for backend calls
import { ApiService } from "@/services/ApiService"

// Main component for listing and editing room types
export function RoomTypesList() {
  // Get JWT token from auth context
  const { token } = useAuth()
  // Decode token to get user role
  const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
  const role = decoded?.role;
  // State for room types data
  const [roomTypes, setRoomTypes] = useState<RoomTypeDto[]>([])
  // State for loading indicator
  const [loading, setLoading] = useState(true)
  // State for currently editing row id
  const [editingId, setEditingId] = useState<number | null>(null)
  // State for edit form values
  const [editValues, setEditValues] = useState<{ description: string; price: string }>({
    description: "",
    price: "",
  })

  // Fetch room types from API when token changes
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

  // Show loading or empty state
  if (loading) return <div>Loading room types...</div>
  if (!roomTypes.length) return <div>No room types found</div>

  // Start editing a room type row
  const startEdit = (rt: RoomTypeDto) => {
    setEditingId(rt.id)
    setEditValues({
      description: rt.description ?? "",
      price: rt.pricePerNight?.toString() ?? "",
    })
  }

  // Cancel editing
  const cancelEdit = () => {
    setEditingId(null)
    setEditValues({ description: "", price: "" })
  }

  // Save edited room type to API
  const saveEdit = async (id: number) => {
    const updatedData = {
      description: editValues.description,
      price: editValues.price ? parseFloat(editValues.price) : null,
    };

    try {
      // Update room type via API
      const isSuccess = await ApiService.updateRoomType(id, updatedData);

      if (isSuccess) {
        console.log("Room type updated successfully!");
        // Refresh room types list after update
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

  // Render table of room types
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
                // Editable price input
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
                // Editable description input
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
                // Show Save/Cancel for Admin, View Only for others
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
                // Show Edit button for Admin only
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
