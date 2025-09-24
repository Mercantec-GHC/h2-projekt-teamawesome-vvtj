interface NotificationSubscription {
  url: string;
  p256dh: string;
  auth: string;
}

interface Window {
  PushNotifications: {
    getNotificationPermission: () => string;
    requestPermission: () => Promise<NotificationPermission>;
    requestSubscription: () => Promise<NotificationSubscription | null>;
  };
}