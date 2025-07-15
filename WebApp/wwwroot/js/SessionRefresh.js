// Add this to your main layout or create a separate JS file
(function () {
    let lastActivity = Date.now();
    let sessionTimeout = 60 * 60 * 1000; // 60 minutes in milliseconds
    let warningTime = 5 * 60 * 1000; // 5 minutes before expiry
    let refreshInterval = 10 * 60 * 1000; // Check every 10 minutes

    // Track user activity
    function updateActivity() {
        lastActivity = Date.now();
    }

    // Add event listeners for user activity
    ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click'].forEach(event => {
        document.addEventListener(event, updateActivity, { passive: true });
    });

    // Function to refresh session
    function refreshSession() {
        fetch('/Account/RefreshSession', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            }
        }).catch(err => console.log('Session refresh failed:', err));
    }

    // Check session status periodically
    setInterval(() => {
        const timeSinceActivity = Date.now() - lastActivity;
        const timeUntilExpiry = sessionTimeout - timeSinceActivity;

        // If user has been active and session is getting close to expiry, refresh it
        if (timeSinceActivity < refreshInterval && timeUntilExpiry < warningTime) {
            refreshSession();
        }

        // Optional: Show warning when session is about to expire
        if (timeUntilExpiry < warningTime && timeUntilExpiry > 0) {
            console.log('Session will expire in', Math.ceil(timeUntilExpiry / 60000), 'minutes');
            // You could show a modal warning here
        }
    }, 30000); // Check every 30 seconds
})();