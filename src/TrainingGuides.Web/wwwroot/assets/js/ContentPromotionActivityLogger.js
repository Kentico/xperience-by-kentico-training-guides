// Logs a "Content promotion click" custom activity when a visitor clicks a content promotion card.
// This script is only served to visitors who consented to tracking (see CustomActivityScripts), and
// the endpoint checks consent again on the server.
window.addEventListener("load", function () {
    const links = document.getElementsByClassName("js-content-promotion-link");

    for (let i = 0; i < links.length; i++) {
        links[i].addEventListener("click", handleContentPromotionClick);
    }
});

function handleContentPromotionClick() {
    const trackingValue = this.getAttribute("data-tracking-value");

    if (!trackingValue) {
        return;
    }

    const body = new URLSearchParams({ TrackingValue: trackingValue });

    // The click navigates away, so the request has to outlive the page. sendBeacon is built for
    // exactly that; keepalive on fetch is the fallback where it is unavailable.
    if (navigator.sendBeacon) {
        navigator.sendBeacon("/contentpromotionclick", body);
        return;
    }

    fetch("/contentpromotionclick", {
        method: "POST",
        body: body,
        keepalive: true
    }).catch(error => console.log(error));
}
