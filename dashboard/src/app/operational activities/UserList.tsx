    "use client"

    // React hooks for state and lifecycle
    import { useEffect, useState } from "react"
    // UI table components
    import {
      Table,
      TableHeader,
      TableBody,
      TableRow,
      TableHead,
      TableCell,
    } from "@/components/ui/table"
    // Auth context for token access
    import { useAuth } from "../login/AuthContext"
    // User data type
    import type { UserDTO } from "@/types/UserDTO"
    // API service for user operations
    import { ApiService } from "@/services/ApiService"

    export function UserList() {
      // Get auth token from context
      const { token } = useAuth()
      // State for user list
      const [users, setUsers] = useState<UserDTO[]>([])
      // State for loading indicator
      const [loading, setLoading] = useState(true)
      // State for role filter dropdown
      const [roleFilter, setRoleFilter] = useState("")
      // State to track which user's role is being saved
      const [savingUserId, setSavingUserId] = useState<number | null>(null)

      // Fetch users when token changes
      useEffect(() => {
        const fetchUsers = async () => {
          setLoading(true)
          // Get all users from API
          const usersData = await ApiService.getAllUsers()
          if (usersData) {
            setUsers(usersData)
          }
          setLoading(false)
        }

        if (token) {
          fetchUsers()
        } else {
          setLoading(false)
        }
      }, [token])

      // Show loading or empty state
      if (loading) return <div>Loading users...</div>
      if (!users.length) return <div>No users found</div>

      // Extract unique roles from user list for dropdowns
      const availableRoles = Array.from(
        new Set(users.map((u) => u.userRole).filter(Boolean))
      )

      // Filter users by selected role
      const filteredUsers = roleFilter
        ? users.filter((u) => u.userRole === roleFilter)
        : users

      // Handle role change for a user
      const handleRoleChange = async (userId: number, newRole: string) => {
        setSavingUserId(userId)
        try {
          // Update user role via API
          const success = await ApiService.updateUserRole(userId, newRole)
          if (success) {
            // Update local state if successful
            setUsers((prev) =>
              prev.map((u) =>
                u.id === userId ? { ...u, userRole: newRole } : u
              )
            )
          } else {
            console.error("Failed to update role")
          }
        } catch (err) {
          console.error("Error updating role:", err)
        } finally {
          setSavingUserId(null)
        }
      }

      // Render user list table
      return (
        <div className="flex flex-col gap-4">
          {/* Role filter dropdown */}
          <select
            value={roleFilter}
            onChange={(e) => setRoleFilter(e.target.value)}
            className="border p-2 rounded w-64"
          >
            <option value="">All roles</option>
            {availableRoles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </select>

          {/* User table */}
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Username</TableHead>
                <TableHead>Role</TableHead>
                <TableHead>Actions</TableHead>
                <TableHead>First Name</TableHead>
                <TableHead>Last Name</TableHead>
                <TableHead>City</TableHead>
                <TableHead>Country</TableHead>
                <TableHead>Phone</TableHead>
                <TableHead>Date of Birth</TableHead>
                <TableHead>Created At</TableHead>
                <TableHead>Last Login</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredUsers.map((user) => (
                <TableRow key={user.id}>
                  <TableCell>{user.id}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.userName}</TableCell>
                  <TableCell>
                    {/* Role change dropdown for each user */}
                    <select
                      value={user.userRole}
                      onChange={(e) => handleRoleChange(user.id, e.target.value)}
                      disabled={savingUserId === user.id}
                      className="border p-1 rounded"
                    >
                      {availableRoles.map((role) => (
                        <option key={role} value={role}>
                          {role}
                        </option>
                      ))}
                    </select>
                  </TableCell>
                  {/* User info fields, fallback to "-" if missing */}
                  <TableCell>{user.userInfo?.firstName ?? "-"}</TableCell>
                  <TableCell>{user.userInfo?.lastName ?? "-"}</TableCell>
                  <TableCell>{user.userInfo?.city ?? "-"}</TableCell>
                  <TableCell>{user.userInfo?.country ?? "-"}</TableCell>
                  <TableCell>{user.userInfo?.phoneNumber ?? "-"}</TableCell>
                  <TableCell>{user.userInfo?.dateOfBirth ?? "-"}</TableCell>
                  {/* Format createdAt date */}
                  <TableCell>
                    {new Date(user.createdAt).toLocaleDateString()}
                  </TableCell>
                  {/* Format lastLogin date or show "-" */}
                  <TableCell>
                    {user.lastLogin
                      ? new Date(user.lastLogin).toLocaleString()
                      : "-"}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )
    }
