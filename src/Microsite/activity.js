//Fetches the codename of the marketing consent
async function getConsentCodeName(){
    let consentJson;
    const res = await fetch("https://the-domain-of-your-xperience-site.com/consent/marketing/");

    consentJson = await res.json();

    return JSON.stringify(consentJson).replace(/"+/g, '');
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
        }
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
        }
    });
}

//Click handler that logs a link click.
function logLinkClick() {
    kxt('click', {
        label: this.getAttribute("alt")
    });
}

//Click handler that logs a file download activity
function logDownload() {
    kxt('customactivity', {
        type: 'filedownload',
        value: this.getAttribute('alt') + ', '  + window.location.pathname,
        title: 'File download'
    });
}

//When the document loads
document.addEventListener('DOMContentLoaded', function () {
    // Disables all tracking by default
    kxt('consentdefault', {
        allow_tracking: false,
        allow_datainput: false
    });
    getConsentCodeName().then((consentName) => {
        // Retrieves and displays the consent text
        kxt('consentdata', {
            codeName: consentName,
            cultureCode: 'en-US',
            callback: consentData => {
                document.getElementById('lblConsentText').innerHTML = consentData.shortText;
            }
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
            }
        });

        // Logs a page visit activity (if tracking is enabled for the current contact)
        kxt('pagevisit');

        //Registers click event handlers for consent functions
        const consentAgreeButton = document.getElementById("btnConsentAgree");
        consentAgreeButton.addEventListener("click", function () {
            trackingConsentAgree(consentName);
        })
        const consentRevokeButton = document.getElementById("btnConsentRevoke");
        consentRevokeButton.addEventListener("click", function () {
            trackingConsentRevoke(consentName);
        })
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