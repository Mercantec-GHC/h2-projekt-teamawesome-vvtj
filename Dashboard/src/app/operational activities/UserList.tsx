"use client"

import { useEffect, useState } from "react"
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components/ui/table"
import { useAuth } from "../login/AuthContext"
import type { UserDTO } from "@/types/UserDTO"
import { ApiService } from "@/services/ApiService"
import { Button } from "@/components/ui/button"

export function UserList() {
  const { token } = useAuth()
  const [users, setUsers] = useState<UserDTO[]>([])
  const [loading, setLoading] = useState(true)
  const [roleFilter, setRoleFilter] = useState("")
  const [savingUserId, setSavingUserId] = useState<number | null>(null)

  useEffect(() => {
    const fetchUsers = async () => {
      setLoading(true)
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

  if (loading) return <div>Loading users...</div>
  if (!users.length) return <div>No users found</div>

  const availableRoles = Array.from(
    new Set(users.map((u) => u.userRole).filter(Boolean))
  )

  const filteredUsers = roleFilter
    ? users.filter((u) => u.userRole === roleFilter)
    : users

  const handleRoleChange = async (userId: number, newRole: string) => {
    setSavingUserId(userId)
    try {
      const success = await ApiService.updateUserRole(userId, newRole)
      if (success) {
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

  return (
    <div className="flex flex-col gap-4">
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
             
              <TableCell>{user.userInfo?.firstName ?? "-"}</TableCell>
              <TableCell>{user.userInfo?.lastName ?? "-"}</TableCell>
              <TableCell>{user.userInfo?.city ?? "-"}</TableCell>
              <TableCell>{user.userInfo?.country ?? "-"}</TableCell>
              <TableCell>{user.userInfo?.phoneNumber ?? "-"}</TableCell>
              <TableCell>{user.userInfo?.dateOfBirth ?? "-"}</TableCell>
              <TableCell>
                {new Date(user.createdAt).toLocaleDateString()}
              </TableCell>
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
