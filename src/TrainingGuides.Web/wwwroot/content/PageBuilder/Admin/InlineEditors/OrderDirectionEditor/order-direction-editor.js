(function () {
    window.kentico.pageBuilder.registerInlineEditor("order-direction-editor", {
        init: function (options) {
            var editor = options.editor;
            var radios = editor.querySelectorAll("input[name='Order']");

            radios.forEach(input => {
                input.addEventListener("change", function () {
                    if (input.value != options.propertyValue) {
                        var event = new CustomEvent("updateProperty", {
                            detail: {
                                value: input.value,
                                name: options.propertyName,
                            }
                        });
                        editor.dispatchEvent(event);
                    }
                });
            });
        }
    });
})();
