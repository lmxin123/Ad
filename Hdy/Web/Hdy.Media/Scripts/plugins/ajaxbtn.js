/* ==============================================================
 * ajaxClick  为提升用户体验，在Ajax请求时，增加开始请求，请求完成提示，设置按钮状态功能。
 * 兼容RequireJS方式调用和直接把文件引用到页面中使用
 * @version 1.0.0 (create 2015-01-08 )
 * @author miaoxin，qq409001887
 *------------------------------------------------------------------------------------------------------------------
 * 1.ajaxClick 更新了before，completed事件名称
 * 2.修改了表单的取值方式
 * 3.新加了jquery.form表单依赖
 *  @version 1.0.1 (create 2015-04-08 )
 * @author miaoxin，qq409001887
 *-------------------------------------------------------------------------------------------------------------------
 * 1. 兼融jquery和mui框架
 * 2.事件before，completed名称改为以on开头
 * 3.增加init函数，让结构更清晰
 * @version 1.0.2 (create 2015-04-26 )
 * @author miaoxin，qq409001887
 *============================================================== */
; +(function ($, doc, window) {
    'use strict';
    /*
     * Ajax请求按钮
     * @param {HtmlObject} ele  jquery 选择器表达式,
     * @param {Object} opt 配置参数
     */
    var AjaxClick = function (ele, opt) {
        var defaults = {
            loadingCss: '',//
            loadingText: '正在保存...',
            loadingSize: '18px',
            loading: '#loading img',
            formSelector: 'form',
            params: '',
            url: '',
            //事件
            onBefore: null,
            onCompleted: null
        },
         _self = this,
         //初始化
           init = function (ele) {
               _self.ajaxBtn = ele;
               _self.spanIcon = _self.ajaxBtn.querySelector("i,span");
               _self.options.defaultHtml = _self.ajaxBtn.innerHTML;

               if (_self.options.loadingCss === '') {
                   _self.options.loadingCss = "background: url(" + doc.querySelector(_self.options.loading).src +
                       ") no-repeat 0 50%;" +
                       "background-size:" + _self.options.loadingSize + ";" +
                       "height: " + _self.options.loadingSize + ";" +
                       "display: inline-block ;" +
                       "padding-left:" + _self.options.loadingSize + ";"
               };

               if (_self.options.formSelector !== '') {
                   _self.$form = $(_self.options.formSelector);
               }
               if (typeof jQuery === 'function')
                   $(_self.ajaxBtn).on('click', common.proxy(_self.begin, _self));
               else
                   _self.ajaxBtn.addEventListener('tap', common.proxy(_self.begin, _self));
           },
          
           //ajax完成之后
           complete = function (resp) {
               var result = JSON.parse(resp.responseText);
               if (_self.options.onCompleted) {
                   _self.options.onCompleted(result, _self, resp);
               }
               else if (result.Success) {
                   _self.options.onSuccess(result, _self);
               }
               else {
                   _self.options.onFaild(result, _self);
               }
           },
            //成功状态
           success = function (result) {
               common.alert((result.Message && result.Message !== 'success') || "保存成功！");
               _self.reset();
           },
            //失败状态
           faild = function (msg) {
               common.alert(msg.Message || "保存失败，请稍后再试！");
               _self.reset();
           },
            //出错
           error = function (resp) {
               common.alert(resp.responseText);
               _self.reset();
           };

        defaults.onSuccess = success;
        defaults.onFaild = faild;
        defaults.onErrorde = error;
        //开始执行
        this.begin = function (e) {
            var before = this.options.onBefore,
                res = false,
                bResult;

            //如果在提交之前有逻辑，则先执行
            if (typeof before === 'function') {
                bResult = before(_self);
                switch (typeof (bResult)) {
                    case 'boolean':
                        res = bResult;
                        break;
                    case 'object':
                        res = true;
                        this.options.params = bResult;
                        break;
                    default:
                        res = true;
                        break;
                }
            } else {
                res = true;
            }

            if (res) {
                //如果有表单，则需要先验证
                if (this.$form && this.$form.length > 0 && $.isFunction(this.$form.valid) && !this.$form.valid())
                    return;

                this.setBusy();
                var formData = new FormData(_self.$form[0]);
                if (_self.options.params) {
                    for (var p in _self.options.params) {
                        var val = _self.options.params[p];
                        if (typeof (val) !== "function") {
                            formData.append(p, val);
                        }
                    }
                }
                var ajaxOptions = {
                    data: formData,
                    type: 'POST',
                    contentType: false,
                    processData: false,
                    url: _self.options.url || _self.$form.attr('action'),
                    complete: complete,
                    error: _self.options.onError
                };
                $.ajax(ajaxOptions);
            }
        };
        //合并参数
        this.options = $.extend({}, defaults, opt);
        init(ele);

        /*
       * 设置按钮为加载状态
       */
        this.setBusy = function () {
            var span = document.createElement('span');
            span.style.cssText = this.options.loadingCss;
            span.innerText = this.options.loadingText;
            //这里使用两个样式名称，因为可能用了bootstrap也可能用了mui
            this.ajaxBtn.classList.add('disabled');
            this.ajaxBtn.classList.add('mui-disabled');

            this.ajaxBtn.innerHTML = '';
            this.ajaxBtn.appendChild(span);
        };
        /*
        * 获取加载提示按钮
        */
        this.getLoading = function () {
            return this.ajaxBtn.children[0];
        };
        /*
        * 重置按钮为加载提示文本
        *@param {String} txt 文本
        *@param {Int} resetTimeout 超时时间
        */
        this.setLoadingText = function (txt, resetTimeout, hideBusy) {
            this.getLoading().innerText = txt;

            if (resetTimeout) {
                setTimeout(function () {
                    _selt.reset();
                }, resetTimeout);
            }

            if (hideBusy) {
                this.getLoading().style.background = null;
            }
        },
        /*
         * 重置按钮为初始状态
         */
        this.reset = function () {
            this.ajaxBtn.classList.remove('disabled');
            this.ajaxBtn.classList.remove('mui-disabled');//手机端使用
            this.ajaxBtn.innerHTML = this.options.defaultHtml;
        }
    }

    window.AjaxClick = AjaxClick;

    /*
    * 为提升用户体验，在Ajax请求时，增加开始请求，请求完成提示，设置按钮状态功能。
    * @param {Object} option 配置参数，有 loadingCss：String，loadingText：String，url：String，params: Object，onBefore：Function，onCompleted：Function
    */
    $.fn.ajaxClick = function (option) {
        return this.each(function () {
            var data = this.dataset.ajaxclick;
            var options = typeof option === 'object' && option;

            if (!data) {
                this.dataset.ajaxclick = new AjaxClick(this, options);
            }
            return this.dataset.ajaxclick;
        });
    }
})($, document, window);