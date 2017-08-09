(function ($, doc) {
    'use strict'

    $(function () {
        $('#file').file({
            container: '#fileContainer'
        });
        $("#btnSubmit").ajaxClick({
            formSelector: '#createMerForm',
            loadingText: "正在保存..."
        });
    });
})($, document);
