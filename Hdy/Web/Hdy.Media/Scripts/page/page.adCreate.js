+(function ($, doc) {
    'use strict';

    $(function () {
        var errClass = 'input-validation-error',
        openAreaContainer = $('#openAreaContainer'),
         playTimesContainer = $('#playTimesContainer'),
         fileContainer = $('#fileContainer'),
         content = $('#Content'),
         btnStartUpload = $('#btnStartUpload'),
         btnCreateAd = $('#btnCreateAd'),
         urlParams = common.getUrlParameToJSON(),
         aId = $('#ID').val(),
         openAreaValues = $('#OpenAreaValues').val(),
         playTimeValues = $('#PlayTimeValues').val(),
         form = $('#createAdForm'),
         submitFlag = false,
         initOpenArea = function () {
             var areas = [];
             if (openAreaValues === '') return;
             areas = JSON.parse(openAreaValues);
             if (areas.length === 0) return;

             if (openAreaContainer.has('input')) {
                 openAreaContainer.find('input').each(function (index, item) {
                     $.each(areas, function (i, area) {
                         if (area.Value === item.value) {
                             item.checked = area.Checked;
                         }
                     });
                 });
             }
         },
         initPlayTime = function () {
             var playTime = [];
             if (playTimeValues === '') return;
             playTime = JSON.parse(playTimeValues);
             if (playTime.length === 0) return;

             if (playTimesContainer.has('input')) {
                 playTimesContainer.find('input').each(function (index, item) {
                     $.each(playTime, function (i, time) {
                         if (time.Value === item.value) {
                             item.checked = time.Checked;
                         }
                     });
                 });
             }
         },
         setRedirectUrls = function () {
             var redirectUrls = [], validate = true;

             $('tbody.files input[type="text"]').each(function () {
                 var input = $(this);
                 var url = input.val();
				 if(url!==''){
                 if (url.isUrl()) {
                     redirectUrls.push({
                         Name: input.attr('name'),
                         RedirectUrl: input.val()
                     });
                     common.clearValErrorMsg(input.attr('name'));
                 }
                 else {
                     validate = false;
                     common.setValErrorMsg(null, input.attr('name'), '跳转地址无效');
                 }
				 }
             });
             $('#RedirectUrls').val(JSON.stringify(redirectUrls));
             return validate;
         };

        setTimeout(function () {
            initOpenArea();
            initPlayTime();
        }, 400);

        $.get('/adList/GetFileList', { adId: aId }, function (result) {
            if (result.Data && result.Data.length > 0) {
                form.fileupload('option', 'done').call(form, $.Event('done'), { result: { files: result.Data } });
            }
        });
        $.get('/adList/GetPlayTimesSelectorPartial', function (html) {
            $('#playTimesContainer').html(html);
        }, 'html');

        var operationFileCount = 0, uploadCount = 0;
        var jqXHR = form.fileupload({
            dataType: 'json',
            maxNumberOfFiles: 5,
            maxFileSize: 209715200,
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png|mp4|mpeg|mpg|avi|wmv|flv)$/i
        }).bind('fileuploadchange', function (e, data) {
            operationFileCount += data.files ? data.files.length : data.context.length;
        }).bind('fileuploadfail', function (e, data) {
            operationFileCount -= data.files ? data.files.length : data.context.length;
        }).bind('fileuploadcompleted', function (e, data, result) {
            if (submitFlag) {
                uploadCount += data.result.files.length;
                if (operationFileCount === uploadCount) {
                    uploadCount = 0;
                    ajaxBtn.setLoadingText('保存成功', null, true);
                    if (!urlParams.aId) {
                        common.alert('内容保存成功，3秒之后将刷新界面！', 10000);
                        setTimeout(function () {
                            location.reload();
                        }, 3000);
                    }
                }
            }
        });

        $('#Region').change(function () {
            initOperArea();
        });

        var dateOptions = {
            minView: "month",
            format: "yyyy-mm-dd",
            language: 'zh-CN',
            autoclose: true
        };
        var endDate = $('#EndPlayDate').datetimepicker(dateOptions);

        $('#BeginPlayDate')
            .datetimepicker(dateOptions)
            .on('changeDate', function (ev) {
                endDate.datetimepicker('show');
            });

        btnStartUpload.click(function () {
            fileContainer.find('.files .start').trigger('click');
        });

        fileContainer.click(function () {
            content.removeClass(errClass);
            this.classList.remove(errClass);
        });

        var ajaxBtn;
        btnCreateAd.ajaxClick({
            formSelector: '#createAdForm',
            loadingText: "正在上传文件...",
            onBefore: function ($ajaxBtn) {
                ajaxBtn = $ajaxBtn;
                $ajaxBtn.$form.valid();

                var tbody = $('.files'), errorMsg = [];

                if (tbody.find('tr').length === 0) {
                    fileContainer.addClass(errClass);
                    errorMsg.push('请选择内容文件！');
                }
                else if (tbody.find(':disabled').length > 0 && fileContainer.find('.error').text() !== '') {
                    fileContainer.find('.error').each(function () {
                        var $this = $(this);
                        if ($this.text().trim().length) {
                            $this.parents('tr').addClass(errClass)
                        }
                    });
                    errorMsg.push('请取消无效文件！');
                }
                else {
                    fileContainer.find('.error').each(function () {
                        var $this = $(this);
                        if ($this.text().trim().length) {
                            $this.parents('tr').removeClass(errClass)
                        }
                    });
                }

                var openAreaChks = $('[name="OpenArea"]:checked'),
                    playTimeChks = $('[name="PlayTime"]:checked'),
                 openAreas = [], playTimes = [];

                if (openAreaChks.length === 0) {
                    openAreaContainer.addClass(errClass);
                    errorMsg.push('请选择播放区域！');
                }
                else {
                    openAreaChks.each(function (index, item) {
                        openAreas.push({
                            Value: item.value,
                            Checked: 'Checked',
                            Name: $.trim($(item).parent().text())
                        });
                    });
                    openAreaContainer.removeClass(errClass);
                }

                if (playTimeChks.length === 0) {
                    playTimesContainer.addClass(errClass);
                    errorMsg.push('请选择播放时段！');
                }
                else {
                    playTimeChks.each(function (index, item) {
                        playTimes.push({
                            Value: item.value,
                            Checked: 'Checked',
                            Name: $.trim($(item).parent().text())
                        });
                    });
                    playTimesContainer.removeClass(errClass);
                }

                setRedirectUrls() || errorMsg.push('跳转地址无效！');

                if (errorMsg.length > 0) {
                    common.alert(errorMsg.join('</br>'), 5000);
                    return false;
                }

                return {
                    OpenAreas: JSON.stringify(openAreas),
                    PlayTimes: JSON.stringify(playTimes)
                };
            },
            onSuccess: function (result, $ajaxBtn) {
                if (operationFileCount > 0) {
                    common.alert('文件正在上传，请勿关闭窗口或者刷新界面！', 3000);
                    // form.attr('data-url', String.format('/ad/Upload?mId={0}&aId={1}', urlParams.mId, result.Data));
                    form.fileupload('option', 'url', String.format('/adList/Upload?mId={0}&aId={1}', urlParams.mId, result.Data));
                    setRedirectUrls();
                    btnStartUpload.trigger('click');
                    submitFlag = true;
                }
                else {
                    ajaxBtn.setLoadingText('保存成功', null, true);
                }
            }
        });

    });
})($, document);
