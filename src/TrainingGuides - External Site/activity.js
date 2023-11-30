//Fetches the codename of the marketing consent
function getConsentCodeName(){
    return fetch("https://The-URL-of-your-Xperience-site.com/")
        .then(response => response.json())
        .then(consentJson => JSON.stringify(consentJson).replace(/"+/g, ''));
}

// Click handler that creates a consent agreement for the current contact
function trackingConsentAgree(consentName) {
    kxt('consentagree', {
        codeName: consentName,
        callback: () => {
            // Enables tracking for any subsequent logging scripts
            kxt('updateconsent', {
                allow_tracking: true,
                allow_datainput: true
            });
        },
        onerror: t => console.log(t)
    });
}

// Click handler that revokes the tracking consent agreement for the current contact
function trackingConsentRevoke(consentName) {
    kxt('consentrevoke', {
        codeName: consentName,
        callback: () => {
            // Disables tracking for any subsequent logging scripts
            kxt('updateconsent', {
                allow_tracking: false,
                allow_datainput: false
            });
        },
        onerror: t => console.log(t)
    });
}

//Click handler that logs a link click.
function logLinkClick() {
    kxt('click', {
        label: this.getAttribute("alt"),
        onerror: t => console.log(t)
    });
}

//Click handler that logs a file download activity
function logDownload() {
    kxt('customactivity', {
        type: 'filedownload',
        value: this.getAttribute('alt') + ', ' + window.location.pathname,
        title: 'File download',
        onerror: t => console.log(t)
    });
}

//When the document loads
document.addEventListener('DOMContentLoaded', function () {
    // Disables all tracking by default
    kxt('consentdefault', {
        allow_tracking: false,
        allow_datainput: false,
        onerror: t => console.log(t)
    });

    getConsentCodeName()
        .then(consentName => {
            // Retrieves and displays the consent text
            kxt('consentdata', {
                codeName: consentName,
                languageName: 'en',
                callback: consentData => {
                    document.getElementById('lblConsentText').innerHTML = consentData.shortText;
                },
                onerror: t => console.log(t)
            });

            // Enables tracking if the current contact has agreed with the consent
            kxt('consentcontactstatus', {
                codeName: consentName,
                callback: consentStatus => {
                    if (consentStatus.isAgreed) {
                        kxt('updateconsent', {
                            allow_tracking: true,
                            allow_datainput: true
                        });
                    }
                },
                onerror: t => console.log(t)
            });

            // Logs a page visit activity (if tracking is enabled for the current contact)
            kxt('pagevisit', {
                onerror: t => console.log(t)
            });

            //Registers click event handlers for consent functions
            const consentAgreeButton = document.getElementById("btnConsentAgree");
            consentAgreeButton.addEventListener("click", function () {
                trackingConsentAgree(consentName);  
            });

            const consentRevokeButton = document.getElementById("btnConsentRevoke");
            consentRevokeButton.addEventListener("click", function () {
                trackingConsentRevoke(consentName);
            });
        });

    const links = document.getElementsByTagName("a");
    //Registers click event handlers for download and standard links
    for (let i = 0; i < links.length; i++) {
        if (links[i].hasAttribute("download")) {
            links[i].addEventListener("click", logDownload);
        }
        else{
            links[i].addEventListener("click", logLinkClick);
        }
    }
});