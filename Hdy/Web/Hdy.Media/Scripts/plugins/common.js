/* ==============================================================
 *公用扩展函数
 * create 2015-2-9
 * @author miaoxin，qq409001887
 *============================================================== */
/**
 * 格式化字符串
 * @returns {String} 返回格式后的字符
 */
String.format = function () {
    if (arguments.length === 0)
        return null;
    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
};

/**
 * 删除字符空格,会与angular冲突,使用的时候要注意
 * @returns {String} 
 */
//String.prototype.trim = function () {
//  return this.replace(/\s/ig, '');
//};

/**
 * 判断字符串是以什么开始
 * @returns {String} 
 */
String.prototype.startWidth = function (testStr) {
    if (this === null || this === "" || this.length === 0 || testStr.length > this.length)
        return false;
    if (this.substr(0, testStr.length) === testStr)
        return true;
    else
        return false;
    return true;
};
/**
 * 判断字符串是以什么结尾
 * @returns {String} 
 */
String.prototype.endWith = function (testStr) {
    if (testStr === null || testStr === "" || this.length === 0 || testStr.length > this.length)
        return false;
    if (this.substring(this.length - testStr.length) === testStr)
        return true;
    else
        return false;
    return true;
};
/**
 * 删除数组对象
 * @returns {String} 
 */
Array.prototype.remove = function (item) {
    for (var i = this.length; i--;) {
        if (this[i] === item) {
            this.splice(i, 1);
        }
    }
};
String.prototype.replaceAll = function (target, replacement) {
    return this.split(target).join(replacement);
};

String.prototype.isUrl = function () {
    var urlregex = new RegExp("^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$");
    return urlregex.test(this.toString());
}

Date.prototype.getBaseDate = function () {
    return new Date(this.getFullYear(), this.getMonth(), this.getDate());
};

Date.prototype.addDays = function (day) {
    if (!isNaN(day)) {
        var dayToAdd = parseInt(day);
        var base = new Date();
        base.setTime(this.getTime());
        base.setDate(base.getDate() + day);
        return base;
    } else {
        throw new Error("day should be an interger");
    }
};

Date.prototype.max = function (a, b) {
    if (a.getTime() > b.getTime()) {
        return a;
    } else {
        return b;
    }
};

/* ==============================================================
 * 公用类函数和相pageIndex关参数配置,不依赖任何框架，纯js编写
 * @author miaoxin，qq409001887
 * ============================================================= */
; + function (doc, window, undefined) {
    "use strict";

    var Com = function () { };
    // 全局配置
    Com.config = {
        rootPath: '',//当前站点的根目录路径
        absPath: location.host, //当前站点的绝对路径
        homeUrl: '', //当前站点的首页地址
        cdnUrl: ''//资源分发地址，如css，js，img等静态文件单独放在一个服务器上
    }

    /*
    *把图片转成数据流实现客户端预览功能，兼容ie，firefox，chrome
    *@param {String} obj img 对象
    *@param {String} file Input File 对象
    *@param {String} callback 操作完成后的回调
    *@returns {String} 返回图片路径
    */
    Com.previewImage = function (obj, file, callback) {
        if (window.navigator.userAgent.indexOf("MSIE") >= 1) {
            obj.select();
            var path = document.selection.createRange().text;
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

    /*
    *判断是否是IE
    *@returns {Boolean} 
    */
    Com.isIE = function () {
        if (!!window.ActiveXObject || "ActiveXObject" in window)
            return true;
        else
            return false;
    };

    /*
    *创建Ajax请求数据包
    *@param {Object} data 数据对象
    *@param {String} type 请求类型，默认为POST
    *@param {Bool} async 是否异步
    *@returns {Object} Ajax参数对象
    */
    Com.createAjaxOpt = function (data, type, async) {
        return {
            dataType: 'json',
            async: async || true,
            type: type || 'POST',
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(data),
            processData: true
        };
    };

    /**
    *在指定html节点插入元素
    *@param {HtmlObject} ele 节点对象
    *@param {String} where 可选择beforebegin,afterbegin,beforeend,afterend
    *@param {String} html 是否异步
    *@returns {Null} 无返回值
    */
    Com.insertHTML = function (ele, where, html) {
        if (!ele) {
            return false;
        }
        where = where.toLowerCase();

        if (ele.insertAdjacentHTML) { //IE
            ele.insertAdjacentHTML(where, html);
        } else {
            var range = ele.ownerDocument.createRange(),
                frag = null;
            switch (where) {
                case "beforebegin":
                    range.setStartBefore(el);
                    frag = range.createContextualFragment(html);
                    ele.parentNode.insertBefore(frag, el);
                    return ele.previousSibling;
                case "afterbegin":
                    if (ele.firstChild) {
                        range.setStartBefore(ele.firstChild);
                        frag = range.createContextualFragment(html);
                        ele.insertBefore(frag, ele.firstChild);
                    } else {
                        ele.innerHTML = html;
                    }
                    return ele.firstChild;
                case "beforeend":
                    if (ele.lastChild) {
                        range.setStartAfter(ele.lastChild);
                        frag = range.createContextualFragment(html);
                        ele.appendChild(frag);
                    } else {
                        ele.innerHTML = html;
                    }
                    return ele.lastChild;
                case "afterend":
                    range.setStartAfter(el);
                    frag = range.createContextualFragment(html);
                    ele.parentNode.insertBefore(frag, ele.nextSibling);
                    return ele.nextSibling;
            }
        }
    }

    //事件代理
    Com.proxy = function (func, obj) {
        if (typeof (func) !== "function")
            return;

        // If obj is empty or another set another object 
        if (!obj) obj = this;

        return function () {
            func.apply(obj, arguments);
        }
    }

    /**
     * 弹出提示
     * @param {Object} 置参数
     * @param {String} msg 提示文本
     * @param {Int} timeout 关闭时间
     * @param {Function} onShow 弹出时触发事件
     * @param {Function} onHidden 关闭时触发事件
     * @param {Function} callback 弹出时触发事件
     * @returns {alertObj} 
     */
    Com.alert = function () {
        var options = {
            timeOut: 2800,
            autoClose: true
        };

        if (typeof (arguments[0]) === 'string') {
            options.msg = arguments[0];
        } else if (typeof (arguments[0]) === 'object') {
            options = arguments[0];
        }

        if (arguments.length > 1) {
            if (typeof arguments[1] === 'function') options.callback = arguments[1];
            if (typeof arguments[1] === 'number') options.timeOut = arguments[1];
            if (typeof arguments[1] === 'boolean') options.autoClose = arguments[1];
            if (typeof arguments[1] === 'object') {
                options.msg = arguments[1].msg || options.msg,
                options.timeOut = arguments[1].timeOut || 1500,
                options.onHidden = arguments[1].onHidden,//在关闭完之后执行
                options.callback = arguments[1].callback
            };
        }

        var motify = doc.getElementById('motify'), _self = this;

        this.msgDiv = null;
        this.close = function (timeout) {
            setTimeout(function () {
                motify.style.display = 'none';
                typeof options.onHidden === 'function' && options.onHidden(_self);
            }, timeout || options.timeOut);
        };

        this.show = function () {
            motify.style.display = 'block';
            //如果有回调，就在回调内部处理关闭提示，没有回调则在定时器内关闭
            if (typeof options.callback === 'function') {
                options.callback(_self);
            }
            else {
                options.autoClose && this.close();
            }
        };
        this.setMsg = function (msg) {
            _self.msgDiv.innerHTML = msg;
        };

        if (motify === null) {
            motify = doc.createElement('div');
            motify.id = 'motify';
            motify.style.cssText = 'display:none;position: fixed; top: 35%; left: 50%; width: 220px;margin: 0 0 0 -110px; z-index: 9999;' +
                'background: rgba(0,0,0,0.8); color: #fff; font-size: 14px; line-height: 1.5em; border-radius: 6px;';
            this.msgDiv = doc.createElement('div');
            this.msgDiv.style.cssText = ' padding: 10px 10px;text-align: center;word-wrap: break-word;';
            motify.appendChild(this.msgDiv);
            doc.body.appendChild(motify);
        } else {
            this.msgDiv = motify.firstChild;
        }

        //如果有参数，则应用参数
        if (arguments.length > 0) {
            this.setMsg(options.msg);
            this.show();
        }

        return this;
    }

    // 获取url的所有参数，以json对象返回
    Com.getUrlParameToJSON = function () {
        var pairs = location.search.slice(1).split('&');
        var result = {};
        pairs.forEach(function (pair) {
            pair = pair.split('=');
            result[pair[0]] = decodeURIComponent(pair[1] || '');
        });
        return result;
    }

    //使用html5api获取地理位置
    Com.getPosition = function (success, fail) {
        var coords = sessionStorage.getItem("position");
        if (coords !== null) {
            typeof success === 'function' && success(JSON.parse(coords));
        } else {
            var loading = doc.getElementById("loading");
            var container = doc.getElementByI("container");
            container.append($loading.removeClass("hidden"));
            loading.querySelector("span").innerText = "正在获取地理位置...";

            var config = {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 30000
            };
            navigator.geolocation.getCurrentPosition(function (position) {
                //获得经度纬度 
                sessionStorage.setItem("position", JSON.stringify(position.coords));
                typeof success === 'function' && success(position.coord);
            }, function (error) {
                typeof fail === 'function' && fail(error);
            }, config);
        }
    }

    Com.timestamp = function () {
        return Date.parse(new Date()) / 1000;
    }

    //判断是pc还是移动设备
    Com.isMobile = function () {
        var a = navigator.userAgent || navigator.vendor || window.opera;
        return (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) ||
            /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)))
    }

    /**
      *验证手机号码的有效性
      * @param {String} num 手机号码
      * @returns {Int} 3是电信，2是联通，1移动，0是无效手机号码 
      */
    Com.chkMobileNum = function (num) {
        var _emp = /^\s*|\s*$/g;
        num = num.replace(_emp, "");
        var _d = /^1[3578][01379]\d{8}$/g;//电信
        var _l = /^1[34578][01256]\d{8}$/g;//联通
        var _y = /^(134[012345678]\d{7}|1[34578][012356789]\d{8})$/g;//移动

        if (_d.test(num) || _l.test(num) || _y.test(num)) {
            return true;
        } else {
            return false;
        }
    }
    /**
     *验证只能输入有效的金额数字
     * @param {String} e 文本对象
     * @returns {Float} 
     */
    Com.chkMoney = function (e) {
        var obj = e.target;
        obj.value = obj.value.replace(/[^\d.]/g, ""); //先把非数字的都替换掉，，除了数字和. 
        obj.value = obj.value.replace(/^\./g, ""); //必须保证第一个为数字而不是. 
        obj.value = obj.value.replace(/\.{2,}/g, "."); //保证只有出现一个.而没有多个. 
        obj.value = obj.value.replace(".", "$#$").replace(/\./g, "").replace("$#$", "."); //保证.只出现一次，而不能出
    }

    /**
     *收藏
     * @returns {NULL} 无返回值 
     */
    Com.addFavorite = function () {
        var url = window.location;
        var title = document.title;
        var ua = navigator.userAgent.toLowerCase();
        if (ua.indexOf("360se") > -1) {
            alert("由于360浏览器功能限制，请按 Ctrl+D 手动收藏！");
        }
        else if (ua.indexOf("msie 8") > -1) {
            window.external.AddToFavoritesBar(url, title); //IE8
        }
        else if (document.all) {
            try {
                window.external.addFavorite(url, title);
            } catch (e) {
                alert('您的浏览器不支持,请按 Ctrl+D 手动收藏!');
            }
        }
        else if (window.sidebar) {
            window.sidebar.addPanel(title, url, "");
        }
        else {
            alert('您的浏览器不支持,请按 Ctrl+D 手动收藏!');
        }
    }
    /**
    *选择器，获取元素
    * @param {String} selector 选择符
    * @returns {Object} 
    */
    Com.get = function (selector) {
        var htmlObj = doc.getElementById(selector);
        return htmlObj || doc.querySelector(selector);
    }
    /**
    * 在有上拉或者下拉刷新的时候,设置页码
    * @param {Int} index 页码
    * @returns {Int} 页码值
    */
    Com.pageIndex = function (index) {
        var pageIndex = 0;
        if (index >= 0) {
            pageIndex = index;
        } else {
            pageIndex = document.body.getAttribute('data-pageindex');
            if (pageIndex === null || pageIndex === undefined) {
                pageIndex = 1;
            } else {
                pageIndex = parseInt(pageIndex) + 1;
            }
        }
        document.body.setAttribute('data-pageIndex', pageIndex);
        return pageIndex;
    };
    /**
     *把表单转换成json数据，需要jquery支持
     * @param {String} formSelector 
     * @returns {Json} params 
     */
    Com.parseFormToJson = function (formSelector) {
        var params = {};
        if (jQuery && typeof jQuery === 'function') {
            $(formSelector).serializeArray().map(function (item) {
                if (params[item.name]) {
                    if (typeof (params[item.name]) === "string") {
                        params[item.name] = [params[item.name]];
                    }
                    params[item.name].push(item.value);
                } else if (item.value !== '') {
                    params[item.name] = item.value;
                }
            });

        }
        else {
            alert(' Com.parseFormToJson 需要Jquery支持')
        }
        return params;
    }
    /**
     *获取元素的坐标
     * @param {HtmlElement} el 手机号码
     * @returns {Object} x，y
     */
    Com.getCoordinate = function (el) {
        var xPos = 0;
        var yPos = 0;

        while (el) {
            if (el.tagName === "BODY") {
                var xScroll = el.scrollLeft || document.documentElement.scrollLeft;
                var yScroll = el.scrollTop || document.documentElement.scrollTop;

                xPos += (el.offsetLeft - xScroll + el.clientLeft);
                yPos += (el.offsetTop - yScroll + el.clientTop);
            } else {
                xPos += (el.offsetLeft - el.scrollLeft + el.clientLeft);
                yPos += (el.offsetTop - el.scrollTop + el.clientTop);
            }

            el = el.offsetParent;
        }
        return {
            x: xPos,
            y: yPos
        };
    };
    Com.setValErrorMsg = function (form, id, msg) {
        common.get(id).classList.add('input-validation-error');
        var errorSpan = common.get('span[data-valmsg-for="' + id + '"]');
        if (errorSpan) {
            errorSpan.classList.add('field-validation-error');
            errorSpan.innerHTML = '<span id="' + id + '-error" class="">' + msg + '</span>';
        }
    };
    Com.clearValErrorMsg = function (id) {
        common.get(id).classList.remove('input-validation-error');
        var errorSpan = common.get('span[data-valmsg-for="' + id + '"]');
        if (errorSpan) errorSpan.innerHTML = '';
    };
    window.common = Com;
}(document, window, undefined);

//
/* ==============================================================
 *页面预处理内容
 * ============================================================= */
; +(function (doc, win) {
    $(function () {
        var errClass = 'input-validation-error';
        $('.modal').on('hidden.bs.modal', function () {
            var $this = $(this);

            $this.removeData('bs.modal');
            $this.find('.' + errClass).removeClass(errClass);
            $this.find('.field-validation-error span').text('');

            //$this.find('[data-ajaxclick]').removeClass('disabled').html('保 存');
            //$this.find('input[type=text], input[type=password], input[type=number], input[type=email], textarea,select').each(function () {
            //    var $item = $(this);
            //    var clear = $item.data('clear'), def = $item.data('default');

            //    if (clear != "False") {
            //        if (this.localName === 'select') {
            //            this.selectIndex = 0;
            //        }
            //        else {
            //            $item.val(def || '');
            //        }
            //    }
            //});
        });
    });
})(document, window);