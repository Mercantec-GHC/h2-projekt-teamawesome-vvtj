"use client"

import { useEffect, useState, useMemo } from "react"
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
import { ApiService } from "@/services/ApiService";

export function RoomList() {
    const { token } = useAuth()
    const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
    
    const userRole = decoded?.role;
    const userHotelName = decoded?.department; 

    const [rooms, setRooms] = useState<RoomDto[]>([])
    const [loading, setLoading] = useState(true)
    const [hotelFilter, setHotelFilter] = useState<string>("")

    useEffect(() => {
        const fetchRooms = async () => {
            setLoading(true);
            try {
                const roomsData = await ApiService.getAllRooms();
                if (roomsData) {
                    setRooms(roomsData);
                }
            } catch (error) {
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

    const filteredRooms = useMemo(() => {
        let currentRooms = rooms;
        
        if (userRole !== "Admin" && userHotelName) {
            currentRooms = currentRooms.filter(r => r.hotelName === userHotelName);
        }
        
        if (hotelFilter) {
            currentRooms = currentRooms.filter(r => r.hotelName === hotelFilter);
        }

        return currentRooms;
    }, [rooms, userRole, userHotelName, hotelFilter]); 
    

    const hotelNames = useMemo(() => {
        if (userRole !== "Admin" && userHotelName) {
             return [userHotelName]; 
        }
        return Array.from(new Set(rooms.map((r) => r.hotelName)));
    }, [rooms, userRole, userHotelName]);

    if (loading) return <div>Loading rooms...</div>

    if (!rooms.length) return <div>No rooms found in the system.</div>

    if (!filteredRooms.length && rooms.length > 0) return <div>No rooms match the current filter.</div>


    return (
        <div className="flex flex-col gap-4">
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