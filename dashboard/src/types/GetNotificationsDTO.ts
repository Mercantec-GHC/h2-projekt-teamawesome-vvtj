export interface GetNotificationsDto {
    newCount: number; 
    notifications: UserNotification[];
}
interface UserNotification {
    id: number;
    status: string; 
    resource: string;
    name: string;
    email: string;
    message: string;
    createdAt: Date; 
    updatedAt?: Date;
}