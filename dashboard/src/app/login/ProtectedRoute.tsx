import type { JSX } from "react"
import { Navigate } from "react-router-dom"
import { jwtDecode } from "jwt-decode"
import { Link } from "react-router-dom"

// Props for ProtectedRoute component
interface ProtectedRouteProps {
  children: JSX.Element
  allowedRoles?: string[] 
}

// JWT payload interface
interface JwtPayload {
  role?: string
}

// Protects routes based on JWT token and allowed roles
export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const token = localStorage.getItem("token")

  // Redirect to login if no token
  if (!token) {
    return <Navigate to="/login" replace />
  }

  const decoded = jwtDecode<JwtPayload>(token)
  const role = decoded.role

  // Deny access for Guest role
  if (role === "Guest") {
    return (
      <div className="flex flex-col items-center justify-center h-screen gap-4 text-red-500">
        <p className="text-xl font-semibold">Access for guest denied</p>
        <Link to="/login" className="text-blue-500 underline hover:text-blue-700">
          Go back to login
        </Link>
      </div>
    )
  }

  // Deny access if role is not allowed
  if (allowedRoles && !allowedRoles.includes(role || "")) {
    return (
      <div className="flex flex-col items-center justify-center h-screen gap-4 text-red-500">
        <p className="text-xl font-semibold">Access for {role} denied</p>
      </div>
    )
  }

  // Render children if access is allowed
  return children
}
