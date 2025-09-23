import type { RoomToCleanDto } from "@/types/RoomToCleanDTO";
import type { HotelDto } from "@/types/HotelDTO";
import type { RoomDto } from "@/types/RoomDTO";
import type { RoomTypeDto } from "@/types/RoomTypesDTO";
import type { RoomTypeUpdateDto } from "@/types/RoomTypeUpdateDTO";

export const ApiService = {
  getAllRoomsToClean: async (): Promise<RoomToCleanDto[] | null> => {
    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Cleaning`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return null;
      return res.json();
    } catch (err) {
      console.error("Error fetching rooms to clean:", err);
      return null;
    }
  },

  markRoomsAsCleaned: async (rooms: RoomToCleanDto[]): Promise<boolean> => {
    if (!rooms || rooms.length === 0) return false;

    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Cleaning/MarkRoomAsCleaned`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(rooms),
      });
      return res.ok;
    } catch (err) {
      console.error("Error marking rooms as cleaned:", err);
      return false;
    }
  },

  getAllRooms: async (): Promise<RoomDto[] | null> => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.error("Authentication token is missing.");
        return null;
      }
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Rooms`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) {
        console.error("Failed to fetch rooms:", res.status, res.statusText);
        return null;
      }
      return res.json();
    } catch (err) {
      console.error("Error fetching rooms:", err);
      return null;
    }
  },

  getAllRoomTypes: async (): Promise<RoomTypeDto[] | null> => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.error("Authentication token is missing.");
        return null;
      }
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/RoomTypes`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) {
        console.error("Failed to fetch room types:", res.status, res.statusText);
        return null;
      }
      return res.json();
    } catch (err) {
      console.error("Error fetching room types:", err);
      return null;
    }
  },

   updateRoomType: async (id: number, updatedData: RoomTypeUpdateDto): Promise<boolean> => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.error("Authentication token is missing.");
        return false;
      }

      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/RoomTypes/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(updatedData),
      });

      if (!response.ok) {
        console.error("Failed to update room type:", response.status, response.statusText);
        return false;
      }

      return true;
    } catch (error) {
      console.error("Error updating room type:", error);
      return false;
    }
  },

  getAllHotels: async (): Promise<HotelDto[] | null> => {
    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Hotel`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return null;
      return res.json();
    } catch (err) {
      console.error("Error fetching hotels:", err);
      return null;
    }
  },

  

};
