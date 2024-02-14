(function (pageBuilder) {
    var richTextEditor = pageBuilder.richTextEditor = pageBuilder.richTextEditor || {};
    var configurations = richTextEditor.configurations = richTextEditor.configurations || {};
    // Overrides the "default" configuration of the Rich text widget, assuming you have not specified a different configuration
    // The below configuration adds the h5 and h6 values to the paragraphFormat and removes the code value. Also sets the toolbar to be visible without selecting any text.
    configurations["default"] = {
        toolbarVisibleWithoutSelection: true,
        paragraphFormat: {
            N: "Normal",
            H1: "Headline 1",
            H2: "Headline 2",
            H3: "Headline 3",
            H4: "Headline 4",
            H5: "Headline 5",
            H6: "Headline 6",
        },
    };

    // Defines a new configuration for a simple toolbar with only formatting
    // options and disables the inline design of the toolbar
    configurations["simple"] = {
        toolbarButtons: ['paragraphFormat', '|', 'bold', 'italic', 'underline', '|', 'align', 'formatOL', 'formatUL'],
        paragraphFormatSelection: true,
        toolbarInline: false
    };
})(window.kentico.pageBuilder);
