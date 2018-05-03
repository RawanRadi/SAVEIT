/**
 * Setup a App namespace to prevent JS conflicts.
 */
var app = {

    Posts: function () {
        var URL = "/Scripts/jquery-1.10.2.min.js";
        $.getScript(URL);
        /**
         * This method contains the list of functions that needs to be loaded
         * when the "Posts" object is instantiated.
         *
         */
        this.init = function () {
            this.get_all_items_pagination();
        };

        /**
         * Load front-end items pagination.
         */
        this.get_all_items_pagination = function () {

            _this = this;

            /* Check if our hidden form input is not empty, meaning it's not the first time viewing the page. */
            if ($('form.post-list input').val()) {
                /* Submit hidden form input value to load previous page number */
                data = JSON.parse($('form.post-list input').val());
                _this.ajax_get_all_items_pagination(data.page);
            } else {
                /* Load first page */
                _this.ajax_get_all_items_pagination(1 );
            }

            $('body').on('click', '.pagination-nav li.active', function () {
                var page = $(this).attr('p');
                _this.ajax_get_all_items_pagination(page);
            });
            

            /* Search */
            $('body').on('click', '.post_search_submit', function () {
                _this.ajax_get_all_items_pagination(1);
            });
            var error = $("#Required");

            /* Search when Enter Key is triggered */
            $(".post_search_text").keydown(function (e) {
                //if (e.keyCode == 13) {
               
                _this.ajax_get_all_items_pagination(1);
                //}
            });

            /* Pagination Clicks   */
            $('body').on('click', '.pagination-nav li.active', function () {
                var page = $(this).attr('p');
                _this.ajax_get_all_items_pagination(page );
            });



        };

        /**
         * AJAX front-end items pagination.
         */
        this.ajax_get_all_items_pagination = function (page, searchby) {

            if ($(".pagination-container").length > 0 && $('.Supervisors-view-all').length > 0) {
                $(".pagination-container").html('<img src="/Content/Images/loading.gif" class="ml-tb" />');

                var post_data = {
                    page: page,
                    search: $('.post_search_text').val(),
                    searchby: searchby,
                    max: $('.post_max').val()
                };

                $('form.post-list input').val(JSON.stringify(post_data));

                var data = {
                    action: 'get-all-Supervisors',
                    data: JSON.parse($('form.post-list input').val())
                };

                $.ajax({
                    // Test is name of controller , Index is the method in the view
                    url: "/AreaOfInterest/Index",
                    type: "POST",
                    data: data,
                    success: function (response) {
                        response = JSON.parse(response);

                        if ($(".pagination-container").html(response.content)) {
                            $('.pagination-nav').html(response.navigation);
                            $('.table-post-list th').each(function () {
                                /* Append the button indicator */
                                $(this).find('span.glyphicon').remove();
                                if ($(this).hasClass('active')) {
                                    
                                }
                            });
                        }
                    }
                });
            }
        };
    }
};

/**
 * When the document has been loaded...
 *
 */

jQuery(document).ready(function () {

    posts = new app.Posts(); /* Instantiate the Posts Class */
    posts.init(); /* Load Posts class methods */
    
});