import type { JSX } from "react"
import { Navigate } from "react-router-dom"
import { jwtDecode } from "jwt-decode"
import { Link } from "react-router-dom"


interface ProtectedRouteProps {
  children: JSX.Element
}

interface JwtPayload {
  role?: string
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const token = localStorage.getItem("token")

  if (!token) {
    return <Navigate to="/login" replace />
  }

  const decoded = jwtDecode<JwtPayload>(token)
  const role = decoded.role

  // if role = Guest → don't let in
  if (role === "Guest") {
    return (
      <div className="flex flex-col items-center justify-center h-screen gap-4 text-red-500">
        <p className="text-xl font-semibold">Access denied</p>
        <Link
          to="/login"
          className="text-blue-500 underline hover:text-blue-700"
        >
          Go back to login
        </Link>
      </div>
    )
  }

  return children
}
