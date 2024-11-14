function RedirectOnAuthenticationSuccess(result) {
    if (result.success) {
        window.location.href = result.redirectUrl;
    }
}