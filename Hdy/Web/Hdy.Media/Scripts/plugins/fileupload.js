; +(function ($, doc, win) {
    "use strict";
    var File = function (ele, options) {
        var _self = this,
            FILE_SIZE_UNIT = 1048576,
         defaults = {
             imgMaxSize: 2,//两M
             vadioMaxSize: 50,//50M,
             maxCount: 5,
             container: '',
             errorClass: 'input-validation-error',
             imgFilter: /^(image\/bmp|image\/gif|image\/jpeg|image\/png|image\/tiff)$/i,
             videoFilter: /^(video\/mpvg|video\/mpg|video\/avi|video\/wmv|video\/flv|video\/mp4)$/i,
             //事件
             callback: null
         },
         errMsg = '{0}文件大小超出要求！上传文件大小不能超过{1} M',
         validResult = [],
        init = function (ele) {
            _self.$file = $(ele);
            if (_self.options.container !== '')
                _self.container = common.get(_self.options.container);
            else
                _self.container = _self.$file.parent();

            _self.$file.change(function () {
                _self.validate(this.files);
            });
        },

       validImg = function (file) {
           if (file.size > _self.options.imgMaxSize * FILE_SIZE_UNIT) {
               validResult.push(String.format(errMsg, file.name, _self.options.imgMaxSize));
               return false;
           }
           else {
               var img = doc.createElement('img');
               img.width = 100;

               var multiple = _self.$file.attr('multiple');

               if (typeof multiple !== typeof undefined && multiple !== false) {
                   _self.container.append(img);
               }
               else {
                   var existImg = _self.container.querySelector('img');
                   if (existImg)
                       img = existImg;
                   else
                       _self.container.append(img);
               }

               _self.previewImg(img, file);
               return true;
           }
       },

       validVideo = function (file) {
           if (file.size > _self.options.vadioMaxSize * FILE_SIZE_UNIT) {
               validResult.push(String.format(errMsg, file.name, _self.options.vadioMaxSize));
               return false;
           }
           else {
               //视频预览
               return true;
           }
       };

        //合并参数
        this.options = $.extend({}, defaults, options);

        init(ele);

        this.validate = function (files) {
            validResult = [];
            if (files.length > this.options.maxCount) {
                validResult.push('上传文件数量超出限制！');
            } else {
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    if (this.options.imgFilter.test(file.type)) {//图片
                        if (!validImg(file)) files[i].value = null;
                    } else if (this.options.videoFilter.test(file.type)) {//视频
                        if (!validVideo(file)) files[i].value = null;
                    }
                    else {
                        validResult.push(file.name + '文件格式不支持上传！');
                        _self.$file[i]
                        files[i].value = null;
                    }
                }
            }
            if (!this.getValidResult()) {
                common.alert(this.getValidMsg(), 5000);
                _self.$file.addClass(this.options.errorClass);
            }
        };

        this.previewImg = function (obj, file, callback) {
            if (win.navigator.userAgent.indexOf("MSIE") >= 1) {
                obj.select();
                var path = doc.selection.createRange().text;
                obj.removeAttribute("src");
                obj.setAttribute("src", obj.value);
                obj.style.filter =
                    "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + path + "', sizingMethod='scale');";
                callback && typeof (callback) === "function" && callback(obj.value);
            } else {
                var reader = new FileReader();
                reader.onload = function (e) {
                    obj.setAttribute("src", e.target.result);
                    callback && typeof (callback) === "function" && callback(e.target.result);
                }
                reader.readAsDataURL(file);
            }
        };

        this.getValidResult = function () {
            return validResult.length === 0;
        };
        this.getValidMsg = function () {
            return validResult.join('<br>');
        }
    };

    /*
   * 文件上传插件，自动处理格式验证，大小验证等
   */
    $.fn.file = function (option, fun) {
        return this.each(function () {
            var data = this.dataset.file;
            var options = typeof option === 'object' && option;
            var file;
            if (!data) {
                file = new File(this, options);
                this.dataset.file = file;
                //把对象提供出外部访问
                if (fun) {
                    fun(file);
                }
            }

            return this.dataset.file;
        });
    }
})($, document, window);