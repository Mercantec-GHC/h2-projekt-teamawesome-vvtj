"use client"

import { useEffect, useState, useMemo } from "react"
import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableHead,
    TableCell,
} from "@/components/ui/table"
import { useAuth } from "../login/AuthContext"
import type { GetNotificationsDto } from "@/types/GetNotificationsDTO"
import { ApiService } from "@/services/ApiService"
import { Dialog, DialogContent, DialogTitle, DialogTrigger } from "@radix-ui/react-dialog"

const availableStatuses: { [key: number]: string } = {
    1: "New",
    2: "Processed",
    3: "Done",
}
const statusOptions = Object.values(availableStatuses) 

export function NotificationsPage() {
    const { token } = useAuth()
    const [notificationsDto, setNotificationsDto] = useState<GetNotificationsDto>({ newCount: 0, notifications: [] }) 
    const [loading, setLoading] = useState(true)
    const [statusFilter, setStatusFilter] = useState("") 
    const [savingNotificationId, setSavingNotificationId] = useState<number | null>(null) 

    const notifications = notificationsDto.notifications; 

    useEffect(() => {
        const fetchNotifications = async () => {
            setLoading(true)
            const data = await ApiService.getAllNotifications() 
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

    const handleStatusChange = async (id: number, newStatus: string) => {
        setSavingNotificationId(id)
        
        try {
            const success = await ApiService.updateNotificationStatus(id, newStatus)
            
            if (success) {
                setNotificationsDto((prev) => ({
                    ...prev,
                    notifications: prev.notifications.map((n) =>
                        n.id === id ? { ...n, status: newStatus } : n 
                    ),
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

    if (loading) return <div className="p-4 text-lg font-medium">Loading notifications...</div>
    if (!notifications.length) return <div className="p-4 text-lg font-medium">No notifications found</div>

    return (
        <div className="flex flex-col gap-4 p-4">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold">Notifications ({notificationsDto.newCount} new)</h1>
                
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
                            <TableCell>
                                {n.createdAt
                                    ? new Date(n.createdAt).toLocaleString("da-DK") 
                                    : "N/A"}
                                </TableCell>

                                <TableCell>
                                {n.updatedAt
                                    ? new Date(n.createdAt).toLocaleString("da-DK") 
                                    : "N/A"}
                                </TableCell>
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