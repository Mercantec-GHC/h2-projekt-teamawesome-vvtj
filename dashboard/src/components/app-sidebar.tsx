"use client"

import * as React from "react"
import {
  Frame,
  Hotel,
  House,
  IdCardLanyard,
  Kanban,
  Map,
  NotebookPen,
  PieChart,
  User,
} from "lucide-react"

import { NavMain } from "@/components/nav-main"
import { NavUser } from "@/components/nav-user"
import { TeamSwitcher } from "@/components/team-switcher"
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarRail,
} from "@/components/ui/sidebar"

import { useAuth } from "../app/login/AuthContext"

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { user } = useAuth()

  const data = {
    user: {
      name: user?.userName || "",
      email: user?.email || "",
      role: user?.userRole || "",
    },
    teams: [
      {
        name: "Nova Hotels A/S",
        logo: Hotel,
        plan: "Enterprise",
      },
    ],
    navMain: [
      {
        title: "Overview",
        url: "#",
        icon: Kanban,
        isActive: true,
        items: [
          {
            title: "History",
            url: "#",
          },
        ],
      },
      {
        title: "Bookings",
        url: "#",
        icon: NotebookPen,
        items: [
          {
            title: "New Bookings",
            url: "#",
          },
          { 
            title: "All Bookings",
             url: "/bookings" 
          },
          { title: "Calendar", url: "#" },
        ],
      },
      {
        title: "Guests",
        url: "#",
        icon: User,
        items: [
          {
            title: "List of Guests",
            url: "#",
          },
        ],
      },
      {
        title: "Rooms",
        url: "/rooms",
        icon: House,
        items: [
          {
            title: "Room list",
            url: "/rooms",
          },
          {
            title: "Room types",
            url: "#",
          },
        ],
      },
      {
        title: "Hotels",
        url: "#",
        icon: Hotel,
        items: [
          {
            title: "All Hotels",
            url: "#",
          },
        ],
      },
      {
        title: "Operational activities",
        url: "#",
        icon: IdCardLanyard,
        items: [
          {
            title: "Cleaning",
            url: "#",
          },
          {
            title: "Team",
            url: "#",
          },
        ],
      },
    ],
    projects: [
      {
        name: "Design Engineering",
        url: "#",
        icon: Frame,
      },
      {
        name: "Sales & Marketing",
        url: "#",
        icon: PieChart,
      },
      {
        name: "Travel",
        url: "#",
        icon: Map,
      },
    ],
  }

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <TeamSwitcher teams={data.teams} />
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={data.navMain} />
      </SidebarContent>
      <SidebarFooter>
        <NavUser user={data.user} />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  )
}
