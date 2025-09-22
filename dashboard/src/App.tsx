import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Login } from './app/login/Login'
import { Dashboard } from './app/dashboard/Dashboard'
import { ProtectedRoute } from './app/login/ProtectedRoute'
import { AuthProvider } from './app/login/AuthContext'
import Rooms from './app/rooms/Room'
import Bookings from './app/bookings/Bookings'
import { RoomTypesList } from './app/rooms/RoomTypesList'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<ProtectedRoute><Dashboard /></ProtectedRoute>}>
              <Route path="rooms" element={<Rooms />} />
              <Route path="bookings" element={<Bookings />} />
              <Route path='room-types' element={<RoomTypesList />} />
          </Route>
          <Route path="/login" element={<Login />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
