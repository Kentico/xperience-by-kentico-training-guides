// Logs a "Content promotion click" custom activity when a visitor clicks a content promotion card.
// This script is only served to visitors who consented to tracking (see CustomActivityScripts), and
// the endpoint checks consent again on the server.
(function () {
    const LINK_SELECTOR = ".js-content-promotion-link";

    // One listener on the document rather than one per card, so cards rendered after this script
    // runs are tracked too. A Ctrl/Cmd or Shift click is still a "click"; a middle click is only an
    // "auxclick", so both are listened for.
    document.addEventListener("click", logPromotionClick);
    document.addEventListener("auxclick", function (event) {
        if (event.button === 1) {
            logPromotionClick(event);
        }
    });

    function logPromotionClick(event) {
        const link = event.target instanceof Element ? event.target.closest(LINK_SELECTOR) : null;

        if (!link) {
            return;
        }

        const trackingValue = link.getAttribute("data-tracking-value");
        const trackingUrl = link.getAttribute("data-tracking-url");

        if (!trackingValue || !trackingUrl) {
            return;
        }

        const body = new URLSearchParams({ TrackingValue: trackingValue });

        // The click navigates away, so the request has to outlive the page. sendBeacon is built for
        // exactly that, but it can refuse to queue a request; keepalive fetch covers both that and
        // browsers without it.
        if (navigator.sendBeacon && navigator.sendBeacon(trackingUrl, body)) {
            return;
        }

        fetch(trackingUrl, {
            method: "POST",
            body: body,
            keepalive: true
        }).catch(function (error) {
            console.log(error);
        });
    }
})();
