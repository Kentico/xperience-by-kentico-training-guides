(function () {
    window.kentico.pageBuilder.registerInlineEditor("path-editor", {
        init: function (options) {
            var selectButton = options.editor.querySelector(".path-editor-select-button");
            var clearButton = options.editor.querySelector(".path-editor-clear-button");

            clearButton.addEventListener("click", function () {
                var event = new CustomEvent("updateProperty", {
                    detail: {
                        value: null,
                        name: options.propertyName
                    }
                });
                options.editor.dispatchEvent(event);
            });

            selectButton.addEventListener("click", function () {
                var dialogOptions = {
                    tabs: ["page"],
                    selectedItems: {
                        type: "page",
                        items: [{ value: options.propertyValue ? options.propertyValue.path : null }]
                    },
                    pageOptions: {
                        identifierMode: "path"
                    },

                    applyCallback: function (data) {
                        items = data.items;
                        if (items && items.length) {
                            var newItem = items[0];
                            var event = new CustomEvent("updateProperty", {
                                detail: {
                                    value: {
                                        path: newItem.nodeAliasPath,
                                        name: newItem.name,
                                    },
                                    name: options.propertyName
                                }
                            });

                            options.editor.dispatchEvent(event);

                            return {
                                closeDialog: true
                            };
                        }
                    }
                };

                window.kentico.modalDialog.contentSelector.open(dialogOptions);
            });
        }
    });
})();