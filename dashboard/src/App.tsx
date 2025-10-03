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

// Main application component
function App() {
  return (
    // Provide authentication context to the app
    <AuthProvider>
      {/* Set up browser routing */}
      <BrowserRouter>
        <Routes>
          {/* Protected dashboard route, acts as a layout for nested routes */}
          <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>}>
                {/* Rooms page, accessible by Admin and Reception roles */}
                <Route path="rooms"  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><Rooms /></ProtectedRoute> } />
                {/* Bookings page, accessible by Admin and Reception roles */}
                <Route path="bookings"  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><Bookings /></ProtectedRoute> } />
                {/* Room types list, accessible by Admin and Reception roles */}
                <Route path='room-types'  element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><RoomTypesList /></ProtectedRoute> } />
                {/* Cleaning page, accessible by Admin, Reception, and CleaningStaff roles */}
                <Route path="cleaning" element={<ProtectedRoute allowedRoles={["Admin", "Reception", "CleaningStaff"]}><CleaningPage /></ProtectedRoute> } />
                {/* Hotels list, accessible by Admin role only */}
                <Route path="hotels"  element={<ProtectedRoute allowedRoles={["Admin"]}><HotelList /></ProtectedRoute> } />
                {/* Users list, accessible by Admin role only */}
                <Route path='users'  element={<ProtectedRoute allowedRoles={["Admin"]}><UserList /></ProtectedRoute> } />
                {/* Notifications table, accessible by Admin and Reception roles */}
                <Route path="notifications" element={<ProtectedRoute allowedRoles={["Admin", "Reception"]}><NotificationsTable /></ProtectedRoute>} />
          </Route>
          {/* Public login route */}
          <Route path="/login" element={<Login />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
