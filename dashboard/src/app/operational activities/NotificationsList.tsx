"use client"

// React hooks for state and lifecycle management
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
// Custom authentication context
import { useAuth } from "../login/AuthContext"
// DTO type for notifications
import type { GetNotificationsDto } from "@/types/GetNotificationsDTO"
// API service for backend communication
import { ApiService } from "@/services/ApiService"
// Dialog components for message viewing
import { Dialog, DialogContent, DialogTrigger } from "@radix-ui/react-dialog"

// Status mapping for notifications
const availableStatuses: { [key: number]: string } = {
    1: "New",
    2: "Processed",
    3: "Done",
}
// Array of status options for dropdowns
const statusOptions = Object.values(availableStatuses) 

export function NotificationsPage() {
    // Get authentication token
    const { token } = useAuth()
    // State for notifications DTO
    const [notificationsDto, setNotificationsDto] = useState<GetNotificationsDto>({ newCount: 0, notifications: [] }) 
    // Loading state
    const [loading, setLoading] = useState(true)
    // Status filter for dropdown
    const [statusFilter, setStatusFilter] = useState("") 
    // State to track which notification is being saved
    const [savingNotificationId, setSavingNotificationId] = useState<number | null>(null) 

    // Extract notifications array from DTO
    const notifications = notificationsDto.notifications; 

    // Fetch notifications when token changes
    useEffect(() => {
        const fetchNotifications = async () => {
            setLoading(true)
            // Fetch all notifications from API
            const data = await ApiService.getAllNotifications() 
            // Handle possible array response
            const dto = Array.isArray(data) ? data[0] : data;
            if (dto && dto.notifications) {
                setNotificationsDto(dto)
            }
            setLoading(false)
        }

        if (token) {
            fetchNotifications()
        } else {
            setLoading(false)
        }
    }, [token])

    // Handle status change for a notification
    const handleStatusChange = async (id: number, newStatus: string) => {
        setSavingNotificationId(id)
            
        try {
            // Update notification status via API
            const success = await ApiService.updateNotificationStatus(id, newStatus)
                
            if (success) {
                // Update local state after successful status change
                setNotificationsDto((prev) => ({
                    ...prev,
                    notifications: prev.notifications.map((n) =>
                        n.id === id ? { ...n, status: newStatus } : n 
                    ),
                    // Decrement newCount if status changes from "New" to another
                    newCount: prev.newCount - (prev.notifications.find(n => n.id === id)?.status === availableStatuses[1] && newStatus !== availableStatuses[1] ? 1 : 0)
                }));
            } else {
                console.error("Failed to update status for notification ID:", id)
            }
        } catch (err) {
            console.error("Error updating status:", err)
        } finally {
            setSavingNotificationId(null)
        }
    }

    // Show loading indicator
    if (loading) return <div className="p-4 text-lg font-medium">Loading notifications...</div>
    // Show message if no notifications found
    if (!notifications.length) return <div className="p-4 text-lg font-medium">No notifications found</div>

    // Main notifications table UI
    return (
        <div className="flex flex-col gap-4 p-4">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold">Notifications ({notificationsDto.newCount} new)</h1>
                    
                {/* Status filter dropdown */}
                <select
                    value={statusFilter}
                    title="Notifications"
                    onChange={(e) => setStatusFilter(e.target.value)}
                    className="border p-2 rounded w-64 shadow-sm"
                >
                    <option value="">All Statuses</option>
                    {statusOptions.map((status) => (
                        <option key={status} value={status}>
                            {status}
                        </option>
                    ))}
                </select>
            </div>

            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>ID</TableHead>
                        <TableHead>Resource</TableHead>
                        <TableHead>Name</TableHead>
                        <TableHead>Email</TableHead>
                        <TableHead>Message</TableHead>
                        <TableHead>Created At</TableHead>
                        <TableHead>Updated At</TableHead>
                        <TableHead className="w-[150px]">Status</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {notifications.map((n) => (
                        <TableRow key={n.id}>
                            <TableCell className="font-medium">{n.id}</TableCell>
                            <TableCell>{n.resource}</TableCell>
                            <TableCell>{n.name}</TableCell>
                            <TableCell>{n.email}</TableCell>
                            {/* Message cell with dialog for full view */}
                            <TableCell className="max-w-xs truncate">
                                <Dialog>
                                    <DialogTrigger className="text-blue-500 underline ml-2">
                                        View
                                    </DialogTrigger>
                                    <DialogContent className="max-h-[80vh] overflow-y-auto">
                                        <div className="whitespace-pre-wrap">
                                        {n.message}
                                        </div>
                                    </DialogContent>
                                </Dialog>
                            </TableCell>
                            {/* Created date */}
                            <TableCell>
                                {n.createdAt
                                    ? new Date(n.createdAt).toLocaleString("da-DK") 
                                    : "N/A"}
                            </TableCell>
                            {/* Updated date (note: currently uses createdAt, may be a bug) */}
                            <TableCell>
                                {n.updatedAt
                                    ? new Date(n.createdAt).toLocaleString("da-DK") 
                                    : "N/A"}
                            </TableCell>
                            {/* Status dropdown */}
                            <TableCell>
                                <select
                                    aria-label="Notification status"
                                    title="Notification status"
                                    value={n.status}
                                    onChange={(e) => handleStatusChange(n.id, e.target.value)}
                                    disabled={savingNotificationId === n.id}
                                    className={`border p-1 rounded transition-colors
                                        ${n.status === availableStatuses[1] ? 'bg-yellow-100' : ''}
                                        ${n.status === availableStatuses[2] ? 'bg-blue-100' : ''}
                                        ${n.status === availableStatuses[3] ? 'bg-green-100' : ''}
                                    `}>
                                    {statusOptions.map((status) => (
                                        <option key={status} value={status}>
                                            {status}
                                        </option>
                                    ))}
                                </select>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>
    )
}
