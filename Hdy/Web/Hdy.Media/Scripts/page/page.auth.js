; +(function ($, doc) {
    'use strict'
    $(function () {
        $('#btnLogin').ajaxClick({
            formSelector: '#loginForm',
            loadingText: "正在登录...",
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $ajaxBtn.setLoadingText("登录成功，正在跳转...");
                    location.href = result.Data;
                }
                else {
                    common.alert("登录失败：" + result.Message);
                    $ajaxBtn.reset();
                }
            }
        });

        $('#btnResetPwd').ajaxClick({
            formSelector: '#resetPwdForm',
            loadingText: "正在保存...",
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    common.alert('密码修改成功，请牢记！');
                    var model = $('#resetPwdModal');
                    model.on('hidden.bs.modal', function () {
                        $('#OldPassword').val('');
                        $('#Password').val('');
                        $('#ConfirmPassword').val('');
                    });

                    model.modal('hide');
                }
                else {
                    common.alert("保存失败：" + result.Message);
                }
                $ajaxBtn.reset();
            }
        });
    });
})($, document)