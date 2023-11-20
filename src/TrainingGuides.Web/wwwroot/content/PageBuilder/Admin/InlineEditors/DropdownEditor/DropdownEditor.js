(function () {
    window.kentico.pageBuilder.registerInlineEditor("dropdown-editor", {
        init: function (options) {
            var editor = options.editor;
            var dropdown = editor.querySelector(".dropdown-selector");
            dropdown.addEventListener("change", function () {
                var event = new CustomEvent("updateProperty", {
                    detail: {
                        value: dropdown.value,
                        name: options.propertyName
                    }
                });
                editor.dispatchEvent(event);
            });
        }
    });
})();
