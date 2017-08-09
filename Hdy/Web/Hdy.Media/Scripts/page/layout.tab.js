+(function () {
    'use strit'

    var Tab = function (ele, option) {
        var self = this, $container = $(option.container);

        ele.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            //合并参数
            self.options = {
                title: this.innerText,
                id: e.target.pathname.replaceAll('/', ''),
                url: this.href + '?t=' + common.timestamp(),
                close: true
            };
            self.open();
        });

        this.open = function () {
            $container.find(".active").removeClass("active");
            var id = "tab_" + self.options.id;

            if (!$("#" + id)[0]) {
                mainHeight = document.body.scrollHeight - 130 + 'px';
                title = '<li role="presentation" id="tab_' + id + '"><a href="#' + id + '" aria-controls="' + id + '" role="tab" data-toggle="tab">' + self.options.title;
                if (self.options.close) {
                    title += ' <i class="tabclose" tabclose="' + id + '"><span class="glyphicon glyphicon-remove"></span></i>';
                }
                title += '</a></li>';
                if (self.options.content) {
                    content = '<div role="tabpanel" class="tab-pane" id="' + id + '">' + self.options.content + '</div>';
                } else { //没有内容，使用IFRAME打开链接
                    //     obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
                    // obj.style.width = obj.contentWindow.document.body.scrollWidth + 'px';
                    content = '<div role="tabpanel" height="100%" width="100%" class="tab-pane" id="' + id + '"><iframe style="position:absolute;" src="' + self.options.url + '" width="100%" height="' + mainHeight +
                        '" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="auto" allowtransparency="yes"></iframe></div>';
                }
                //加入TABS
                $container.find(".nav-tabs").append(title);
                $container.find(".tab-content").append(content);

                $container.find('i[tabclose=' + id + ']').click(function () {
                    self.close($(this).attr("tabclose"));
                });
            }
            //激活TAB
            $container.find("#tab_" + id).addClass('active');
            $container.find("#" + id).addClass("active");
        };

        this.close = function (id) {
            //如果关闭的是当前激活的TAB，激活他的前一个TAB
            if ($("li.active").attr('id') === "tab_" + id) {
                $("#tab_" + id).prev().addClass('active');
                $("#" + id).prev().addClass('active');
            }
            //关闭TAB
            $("#tab_" + id).remove();
            $("#" + id).remove();
        };
    };

    $.fn.tab = function (option) {
        return this.each(function () {
            var data = this.dataset.tab;
            var options = typeof option === 'object' && option;

            if (!data) {
                this.dataset.tab = new Tab(this, options);
            }
            return this.dataset.tab;
        });
    }

    $(function () {
        $('#dashboard-menu a').filter(function () {
            return this.href !== 'javascript:;' && this.href !== '#';
        }).tab({ container: '#masterContent' });
        
        $('#masterContent').height($(document).height() - 20);
    });
})($, document)