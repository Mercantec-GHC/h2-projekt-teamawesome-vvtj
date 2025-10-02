import { createContext, useContext, useState, type PropsWithChildren } from "react"
import type { UserDTO } from "../../types/UserDTO"

type AuthContextType = {
  user: UserDTO | null
  token: string | null
  setToken: (token: string | null) => void
  setUser: (user: UserDTO | null) => void
   logout: () => void 
}

const AuthContext = createContext<AuthContextType | null>(null)

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<UserDTO | null>(null)
  const [token, setToken] = useState<string | null>(localStorage.getItem("token"))
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

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error("useAuth must be used within AuthProvider")
  return context
}
