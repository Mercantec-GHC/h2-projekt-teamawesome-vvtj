"use client"

// React hooks for state and lifecycle
import { useEffect, useState, useMemo } from "react"
// UI table components
import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableHead,
    TableCell,
} from "@/components/ui/table"
// Auth context for user info
import { useAuth } from "../login/AuthContext"
// Room DTO type
import type { RoomDto } from "@/types/RoomDTO"
// API service for fetching rooms
import { ApiService } from "@/services/ApiService";

export function RoomList() {
    // Get JWT token from auth context
    const { token } = useAuth()
    // Decode JWT payload to get user info
    const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
    
    // Extract user role and hotel name from token
    const userRole = decoded?.role;
    const userHotelName = decoded?.department; 

    // State for all rooms, loading status, and hotel filter
    const [rooms, setRooms] = useState<RoomDto[]>([])
    const [loading, setLoading] = useState(true)
    const [hotelFilter, setHotelFilter] = useState<string>("")

    // Fetch rooms from API when token changes
    useEffect(() => {
        const fetchRooms = async () => {
            setLoading(true);
            try {
                const roomsData = await ApiService.getAllRooms();
                if (roomsData) {
                    setRooms(roomsData);
                }
            } catch (error) {
                // Log error if fetch fails
                console.error("Error fetching rooms:", error);
            } finally {
                setLoading(false);
            }
        };

        if (token) {
            fetchRooms();
        } else {
            setLoading(false);
        }
    }, [token]);

    // Filter rooms based on user role and selected hotel
    const filteredRooms = useMemo(() => {
        let currentRooms = rooms;
        
        // Non-admins only see rooms for their hotel
        if (userRole !== "Admin" && userHotelName) {
            currentRooms = currentRooms.filter(r => r.hotelName === userHotelName);
        }
        
        // Apply hotel filter if selected
        if (hotelFilter) {
            currentRooms = currentRooms.filter(r => r.hotelName === hotelFilter);
        }

        return currentRooms;
    }, [rooms, userRole, userHotelName, hotelFilter]); 
    

    // Get list of hotel names for filter dropdown
    const hotelNames = useMemo(() => {
        // Non-admins only see their hotel
        if (userRole !== "Admin" && userHotelName) {
             return [userHotelName]; 
        }
        // Admins see all hotels
        return Array.from(new Set(rooms.map((r) => r.hotelName)));
    }, [rooms, userRole, userHotelName]);

    // Show loading indicator
    if (loading) return <div>Loading rooms...</div>

    // Show message if no rooms exist
    if (!rooms.length) return <div>No rooms found in the system.</div>

    // Show message if no rooms match filter
    if (!filteredRooms.length && rooms.length > 0) return <div>No rooms match the current filter.</div>

    // Render room list and hotel filter
    return (
        <div className="flex flex-col gap-4">
            {/* Show hotel filter dropdown for admins or if multiple hotels */}
            {(userRole === "Admin" || hotelNames.length > 1) && ( 
                <select
                    value={hotelFilter}
                    onChange={(e) => setHotelFilter(e.target.value)}
                    className="border p-2 rounded w-64"
                >
                    <option value="">All Hotels</option>
                    {hotelNames.map((name) => (
                        <option key={name} value={name}>
                            {name}
                        </option>
                    ))}
                </select>
            )}

            {/* Table of rooms */}
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
                    {filteredRooms.map((room) => (
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
        </div>
    );
}