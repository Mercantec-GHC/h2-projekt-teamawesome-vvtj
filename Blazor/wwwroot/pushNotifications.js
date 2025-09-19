(function () {
    const applicationServerPublicKey = 'BMYoZFBvNMHivrknZKC3pUG3YrVmrd2yUpTe9A4efdEUVfHxAJcqv8Yi3tZ8Tpn2mumZACCKu3trYI6gTHfQF9E';

    function arrayBufferToBase64(buffer) {
        let binary = '';
        const bytes = new Uint8Array(buffer);
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    function urlBase64ToUint8Array(base64String) {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/');
        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);
        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }

    window.blazorPushNotifications = {
        getNotificationPermission: function () {
            return Notification.permission;
        },
        requestPermission: function () {
            return Notification.requestPermission();
        },
        requestSubscription: async function () {
            const worker = await navigator.serviceWorker.getRegistration();
            if (!worker) {
                console.error("Service worker is not registered.");
                return null;
            }

            let subscription = await worker.pushManager.getSubscription();

            if (!subscription) {
                subscription = await worker.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: urlBase64ToUint8Array(applicationServerPublicKey)
                });
            }

            return {
                url: subscription.endpoint,
                p256dh: arrayBufferToBase64(subscription.getKey('p256dh')),
                auth: arrayBufferToBase64(subscription.getKey('auth'))
            };
        }
    };
})();
