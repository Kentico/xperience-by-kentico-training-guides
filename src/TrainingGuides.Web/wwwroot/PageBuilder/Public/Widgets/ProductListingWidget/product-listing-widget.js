(function() {
    'use strict';

    // Wait for DOM to be ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    function init() {
        const applyFiltersBtn = document.getElementById('apply-filters');
        const clearFiltersBtn = document.getElementById('clear-filters');
        const filterSelects = document.querySelectorAll('.product-filter');

        if (applyFiltersBtn) {
            applyFiltersBtn.addEventListener('click', applyFilters);
        }

        if (clearFiltersBtn) {
            clearFiltersBtn.addEventListener('click', clearFilters);
        }

        // Load filters from query string on page load
        loadFiltersFromQueryString();
    }

    function applyFilters() {
        const filterSelects = document.querySelectorAll('.product-filter');
        const params = new URLSearchParams();

        filterSelects.forEach(select => {
            const selectedValues = Array.from(select.selectedOptions)
                .map(option => option.value)
                .filter(value => value !== ''); // Exclude empty "All" option

            if (selectedValues.length > 0) {
                // Use the select's id as the query parameter name
                params.set(select.id, selectedValues.join(','));
            }
        });

        // Update URL with query parameters
        const newUrl = params.toString() 
            ? `${window.location.pathname}?${params.toString()}`
            : window.location.pathname;
        
        window.location.href = newUrl;
    }

    function clearFilters() {
        const filterSelects = document.querySelectorAll('.product-filter');
        
        filterSelects.forEach(select => {
            // Clear all selections
            Array.from(select.options).forEach(option => {
                option.selected = false;
            });
        });

        // Remove query parameters and reload
        window.location.href = window.location.pathname;
    }

    function loadFiltersFromQueryString() {
        const params = new URLSearchParams(window.location.search);
        const filterSelects = document.querySelectorAll('.product-filter');

        filterSelects.forEach(select => {
            const paramValue = params.get(select.id);
            if (paramValue) {
                const values = paramValue.split(',');
                Array.from(select.options).forEach(option => {
                    if (values.includes(option.value)) {
                        option.selected = true;
                    }
                });
            }
        });
    }
})();
