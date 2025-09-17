import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Login } from './feature/auth/Login'
import { Index } from './feature/apps/Index'
import { ProtectedRoute } from './feature/auth/ProtectedRoute'


function App() {
  return (
    <BrowserRouter>
      <Routes>
         <Route
          path="/"
          element={
            <ProtectedRoute>
              <Index />
            </ProtectedRoute>
          }
        />
        <Route path="/login" element={<Login />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
