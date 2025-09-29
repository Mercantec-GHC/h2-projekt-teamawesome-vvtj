export type HotelDto = {
  id: number;
  hotelName: string;
  cityName: string;
  address: string;
  description?: string | null;
  email?: string | null;
  phone?: string | null;
  weekdayTime?: string | null;
  saturdayTime?: string | null;
  holidaysTime?: string | null;
}