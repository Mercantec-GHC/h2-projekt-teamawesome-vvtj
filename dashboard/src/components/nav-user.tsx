import {
  ChevronsUpDown,
  LogOut,
} from "lucide-react"

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from "@/components/ui/sidebar"
import { useAuth } from "@/app/login/AuthContext"
import { useNavigate } from "react-router-dom"

export function NavUser() {
  const { isMobile } = useSidebar()
 const { logout } = useAuth()
 const navigate = useNavigate() 
  const token = localStorage.getItem("token");
  const decoded = token ? JSON.parse(atob(token.split(".")[1])) : null;

  const name = decoded?.unique_name || "Loading..."
  const email = decoded?.email || ""
  const role = decoded?.role
  const department = decoded?.department || ""
  const handleLogout = () => {
      logout()
      navigate("/login") 
    }

  return (
   <SidebarMenu>
    <SidebarMenuItem>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <SidebarMenuButton
            size="lg"
            className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
          >
            <div className="grid flex-1 text-left text-sm leading-tight">
              <span className="truncate font-medium">{name}</span>
              <span className="truncate text-xs">{email}</span>
              <span className="truncate text-xs font-light">{role} - {department}</span>
            </div>
            <ChevronsUpDown className="ml-auto size-4" />
          </SidebarMenuButton>
        </DropdownMenuTrigger>

        <DropdownMenuContent
          className="w-(--radix-dropdown-menu-trigger-width) min-w-56 rounded-lg"
          side={isMobile ? "bottom" : "right"}
          align="end"
          sideOffset={4}
        >
          <DropdownMenuLabel className="p-0 font-normal">
            <div className="flex flex-col gap-1 px-2 py-2 text-left text-sm">
              <span className="truncate font-medium">{name}</span>
              <span className="truncate text-xs">{email}</span>
              <span className="truncate text-xs font-light">{role} - {department}</span>
            </div>
          </DropdownMenuLabel>

          <DropdownMenuSeparator />

          <DropdownMenuGroup>
          </DropdownMenuGroup>

          <DropdownMenuSeparator />

            <DropdownMenuItem onClick={handleLogout}>
            <LogOut />
            Log out
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu> 
    </SidebarMenuItem>
  </SidebarMenu>
  )
}
