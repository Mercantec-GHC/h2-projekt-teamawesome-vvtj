import { createContext, useContext, useState, type PropsWithChildren } from "react"
import type { UserDTO } from "../../types/UserDTO"

// Auth context type definition
type AuthContextType = {
  user: UserDTO | null
  token: string | null
  setToken: (token: string | null) => void
  setUser: (user: UserDTO | null) => void
  logout: () => void
}

// Create AuthContext
const AuthContext = createContext<AuthContextType | null>(null)

export function AuthProvider({ children }: PropsWithChildren) {
  // User state
  const [user, setUser] = useState<UserDTO | null>(null)
  // Token state, initialized from localStorage
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"))
  // Logout function clears user and token
  const logout = () => {
    setUser(null)
    setToken(null)
    localStorage.removeItem("token")
  }

  return (
    <AuthContext.Provider value={{ user, token, setToken, setUser, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

// Custom hook to use AuthContext
// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error("useAuth must be used within AuthProvider")
  return context
}
