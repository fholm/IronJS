(function (window, undefined) {

    // Define a local copy of jQuery
    var jQuery = function (selector, context) {
        // The jQuery object is actually just the init constructor 'enhanced'
        return new jQuery.fn.init(selector, context);
    },

    // Map over jQuery in case of overwrite
	_jQuery = window.jQuery,

    // Map over the $ in case of overwrite
	_$ = window.$,

    // Use the correct document accordingly with window argument (sandbox)
	document = window.document,

    // A central reference to the root jQuery(document)
	rootjQuery;
});