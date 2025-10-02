import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Login } from './app/login/Login'
import { Dashboard } from './app/dashboard/Dashboard'
import { ProtectedRoute } from './app/login/ProtectedRoute'
import { AuthProvider } from './app/login/AuthContext'
import Rooms from './app/rooms/Room'
import Bookings from './app/bookings/Bookings'
import { RoomTypesList } from './app/rooms/RoomTypesList'
import CleaningPage from './app/operational activities/Cleaning'
import { HotelList } from './app/hotels/HotelsList'
import {UserList } from './app/operational activities/UserList'
import  NotificationsTable  from './app/operational activities/NotificationsPage'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>}>
                <Route path="rooms"  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><Rooms /></ProtectedRoute> } />
                <Route path="bookings"  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><Bookings /></ProtectedRoute> } />
                <Route path='room-types'  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><RoomTypesList /></ProtectedRoute> } />
                <Route path="cleaning" element={<ProtectedRoute allowedRoles={["Admin", "Reception", "CleaningStaff"]}><CleaningPage /></ProtectedRoute> } />
                <Route path="hotels"  element={<ProtectedRoute allowedRoles={["Admin"]}><HotelList /></ProtectedRoute> } />
                <Route path='users'  element={<ProtectedRoute allowedRoles={["Admin"]}><UserList /></ProtectedRoute> } />
                <Route path="notifications" element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><NotificationsTable /></ProtectedRoute>} />
          </Route>
          <Route path="/login" element={<Login />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
