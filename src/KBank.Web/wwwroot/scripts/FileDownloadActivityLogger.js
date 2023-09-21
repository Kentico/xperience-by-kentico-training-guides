window.onload = function () {
    const links = document.getElementsByTagName("a");

    for (let i = 0; i < links.length; i++) {
        if (links[i].hasAttribute("download")) {
            links[i].addEventListener("click", handleClick);
        }
    }
}

function handleClick(event) {
    kxt('customactivity', {
        type: 'filedownload',
        value: window.location.pathname,
        title: 'File downloaded - ' + this.getAttribute("alt")
    });
}