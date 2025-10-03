/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { ApiService } from "@/services/ApiService";
import { useAuth } from "../login/AuthContext";

interface CleaningForm {
  selectedHotelId: string;
  roomNumbersInput: string;
}

export function Cleaning() {
  const [cleaningViewModel, setCleaningViewModel] = useState<any[]>([]);
  const [hotelDtos, setHotelDtos] = useState<any[]>([]);
  const [formModel, setFormModel] = useState<CleaningForm>({
    selectedHotelId: "",
    roomNumbersInput: "",
  });
  const [loading, setLoading] = useState(true);
  const [errorLoadData, setErrorLoadData] = useState<string | null>(null);
  const [errorMarkedRooms, setErrorMarkedRooms] = useState<string | null>(null);

  const navigate = useNavigate();
  const { token } = useAuth()
  const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;
  const role = decoded?.role;
  const hotelName = decoded?.department; 

  useEffect(() => {
    if (!token) navigate("/login");
    else loadRooms();
  }, []);

  // Loads hotels and rooms to clean, filters by user role
  const loadRooms = async () => {
    setLoading(true);
    try {
      const hotels = await ApiService.getAllHotels() ?? [];
      const roomsToClean = await ApiService.getAllRoomsToClean() ?? [];
      
      let filteredRooms = roomsToClean;

      if (role !== "Admin") {
        const hotel = hotels.find(h => h.hotelName === hotelName);
        if (!hotel) {
          console.warn("No hotel found for this user");
          setCleaningViewModel([]);
          return;
        }
        filteredRooms = roomsToClean.filter(r => parseInt(r.hotelId) === hotel.id);
        hotels.splice(0, hotels.length, hotel); // keep only this hotel in the list
      }

      // Map rooms to display model
      const mapped = filteredRooms.map((r: any) => {
        const hotelName = hotels.find((h: any) => h.id === r.hotelId)?.hotelName ?? "Unknown Hotel";
        return { hotel: hotelName, roomNumbers: r.roomNumbers };
      });

      setCleaningViewModel(mapped);
      setHotelDtos(hotels);

    } catch (err) {
      console.error(err);
      setErrorLoadData("Something went wrong. Rooms cannot be loaded.");
    } finally {
      setLoading(false);
    }
  };

  // Handles marking rooms as cleaned
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const roomNumbers = formModel.roomNumbersInput.split(",").map(r => parseInt(r.trim()));
    const success = await ApiService.markRoomsAsCleaned([{ hotelId: formModel.selectedHotelId, roomNumbers }]);
    if (success) {
      setFormModel({ selectedHotelId: "", roomNumbersInput: "" });
      loadRooms();
    } else {
      setErrorMarkedRooms("Rooms not marked as cleaned.");
    }
  };

  return (
    <div className="p-5 max-w-5xl mx-auto space-y-5">
      <h2 className="text-center text-2xl font-semibold">Rooms Pending Cleaning 🧹</h2>

      {errorLoadData && <Card className="p-4 text-red-600">{errorLoadData}</Card>}

      {loading ? (
        <Card className="p-4 text-center">Loading...</Card>
      ) : (
        <Card>
          <CardContent>
            <table className="w-full table-auto border-collapse border border-gray-300">
              <thead className="bg-gray-800 text-white text-center">
                <tr>
                  <th className="border px-2 py-1">Hotel Name</th>
                  <th className="border px-2 py-1">Room Numbers</th>
                </tr>
              </thead>
              <tbody>
                {cleaningViewModel.length === 0 ? (
                  <tr>
                    <td colSpan={2} className="text-center py-2">No rooms to clean ✅</td>
                  </tr>
                ) : (
                  cleaningViewModel.map((group, idx) => (
                    <tr key={idx}>
                      <td className="border px-2 py-1">{group.hotel}</td>
                      <td className="border px-2 py-1">
                        {group.roomNumbers.map((room: number) => (
                          <Badge key={room} variant="destructive" className="mr-1">{room}</Badge>
                        ))}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </CardContent>
        </Card>
      )}

      <h3 className="text-center text-xl mt-5">Mark Rooms as Cleaned</h3>
      {errorMarkedRooms && <Card className="p-4 text-red-600">{errorMarkedRooms}</Card>}

      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <Select
          value={formModel.selectedHotelId}
          onValueChange={val => setFormModel({ ...formModel, selectedHotelId: val })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Select a hotel">
              {formModel.selectedHotelId
                ? hotelDtos.find(h => String(h.id) === formModel.selectedHotelId)?.hotelName
                : ""}
            </SelectValue>
          </SelectTrigger>
          <SelectContent>
            {hotelDtos.map(h => (
              <SelectItem key={h.id} value={h.id}>
                {h.hotelName}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        <Input
          placeholder="Enter room numbers, comma separated"
          value={formModel.roomNumbersInput}
          onChange={e => setFormModel({ ...formModel, roomNumbersInput: e.target.value })}
        />

        <Button type="submit">Mark as Cleaned</Button>
      </form>
    </div>
  );
}
