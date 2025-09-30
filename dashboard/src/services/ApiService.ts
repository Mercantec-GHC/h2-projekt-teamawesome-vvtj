import type { RoomToCleanDto } from "@/types/RoomToCleanDTO";
import type { HotelDto } from "@/types/HotelDTO";
import type { RoomDto } from "@/types/RoomDTO";
import type { RoomTypeDto } from "@/types/RoomTypesDTO";
import type { RoomTypeUpdateDto } from "@/types/RoomTypeUpdateDTO";
import type { UserDTO } from "@/types/UserDTO";
import type { BookingDto } from "@/types/BookingDTO";
import type { GetNotificationsDto } from "@/types/GetNotificationsDTO";

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
    updateUserRole: async (userId: number, newRole: string): Promise<boolean> => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.error("Authentication token is missing.");
        return false;
      }

      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/api/Roles/${userId}/assign-role-to-user?newRole=${newRole}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        console.error("Failed to update user role:", response.status, response.statusText);
        return false;
      }

      return true;
    } catch (error) {
      console.error("Error updating user role:", error);
      return false;
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

   updateHotel: async (updatedData: HotelDto): Promise<boolean> => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.error("Authentication token is missing.");
        return false;
      }

      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Hotel/`, {
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

  updateBookingDates: async (
      id: number,
      checkIn: string,
      checkOut: string,
      token: string
    ): Promise<BookingDto | null> => {
      try {
        const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Booking?id=${id}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ checkIn, checkOut }),
        })

        if (!res.ok) {
          const errorText = await res.text()
          console.error("Failed to update booking dates:", errorText)
          return null
        }

        const updatedBooking: BookingDto = await res.json()
        return updatedBooking
      } catch (error) {
        console.error("Error updating booking:", error)
        return null
      }
    },
  deleteBooking: async (id: number, token: string) => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Booking/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })

      if (!res.ok) {
        const errorText = await res.text()
        console.error("Failed to delete booking:", errorText)
        return false
      }

      return true
    } catch (error) {
      console.error(error)
      return false
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

  getAllUsers: async (): Promise<UserDTO[] | null> => {
     try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Users`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return null;
      return res.json();
    } catch (err) {
      console.error("Error fetching hotels:", err);
      return null;
    }
  },

  getAllNotifications: async (): Promise<GetNotificationsDto[] | null> => {
     try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Notifications/all-notifications`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!res.ok) return null;
      return res.json();
    } catch (err) {
      console.error("Error fetching hotels:", err);
      return null;
    }
  },

  updateNotificationStatus: async (id: number, newStatus: string ): Promise<boolean> => {
    try {
      const token = localStorage.getItem("token");
      
      if (!token) {
        console.error("Authentication token is missing.");
        return false;
      }
      const payload = {
        Id: id,
        Status: newStatus
      };

      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Notifications/update-notification-status`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(payload), 
      });

      if (response.ok) {
        return true;
      } 
      
      const errorText = await response.text();
      console.error(`API Error ${response.status}: Failed to update notification ${id}. Response: ${errorText}`);
      return false;

    } catch (error) {
      console.error(`[Network Error]: Error updating notification ${id}:`, error);
      return false;
    }
  },
};
