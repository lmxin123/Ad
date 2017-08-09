(function ($, doc) {
    angular.module('schedulingApp', ['ui.bootstrap']).controller('SchedulingCtrl', function ($scope, $http) {
        //variable
        var ui = {
            month: $('#PlayMonth'),
            modal: $('#schedulingModal'),
            copyModal: $('#copyModal'),
            province: $('#schedulingForm #Province'),
            region: $('#schedulingForm #Region'),
            openAreaContainer: $('#openAreaContainer'),
            playTimesContainer: $('#playTimesContainer'),
            adContainer: $('#adContainter'),
            schedulingForm: $('#schedulingForm'),
            beginPlayDate: $('#BeginPlayDate'),
            endPlayDate: $('#EndPlayDate'),
            adFilterForm: $('#adFilterFrom'),
            deviceID: $('#DeviceID'),
            sId: $('#ID'),
            btnDelete: $('#btnDelete'),
            now: new Date()
        },
        errClass = 'input-validation-error',
        key = 0,
        arrPos = new Array(),
        down = false,
        inArea = false,
        mX, mY, tdX, tdY, tdoX, tdoY, tdRows = [], checkAidList = [],
        //function
        getDaysInMonth = function () {
            $scope.DateList = [];
            $scope.month = ui.month.val();
            var items = $scope.month.split('-'),
             date = new Date(parseInt(items[0]), parseInt(items[1]), 0),
             days = date.getDate(),
             weeks = new Array("周日", "周一", "周二", "周三", "周四", "周五", "周六");
            for (var i = 1; i <= days ; i++) {
                var tmpDate = new Date(parseInt(items[0]), parseInt(items[1]) - 1, i);//javascript 的月份是从0开始的
                $scope.DateList.push({
                    week: weeks[tmpDate.getDay()].replace('周', ''),
                    date: { day: i, date: $scope.month + '-' + (i < 10 ? '0' + i.toString() : i) }
                })
            }
        },
        setPlayTimes = function () {
            if ($scope.scheduling.PlayTimes) {
                var playTimes = JSON.parse($scope.scheduling.PlayTimes);
                $.each(ui.playTimesContainer.find('input'), function (index, item) {
                    $.each(playTimes, function (i, time) {
                        if (item.value === time.Value) {
                            item.checked = true;
                            return false;
                        }
                        else {
                            item.checked = false;
                        }
                    });
                });
            }
        },
        showModal = function (target) {
            if (!validSelectedTd()) {
                common.alert('您选择的日期范围包含己排期的日期，请选择未排期的日期范围！', { timeOut: 5000 });
                return;
            }
            var $target = $(target), tr = $target.parent(), firstTd = tr.find('td:first'), selectedTds = [];

            tr.find('td.selected').each(function (index, item) {
                if ($(item).data('sid') === '')
                    selectedTds.push(item);
            });

            for (var i = 0; i < $scope.List.length; i++) {
                for (var j = 0; j < $scope.List[i].Schedulings.length; j++) {
                    if ($scope.List[i].Schedulings[j].ID === $target.data('sid')) {
                        $scope.scheduling = $scope.List[i].Schedulings[j];
                        saveDelAdIDs($scope.scheduling.Details);
                        break;
                    }
                }
            }

            ui.deviceID.val(firstTd.data('deviceid'));
            ui.sId.val($target.data('sid'));
            ui.province.area({
                cityClass: '#City',
                districtClass: '#Region',
                inputCode: '#Code',
                inputText: '#Text',
                defaultProvince: $scope.scheduling.Province || '44',
                defaultCity: $scope.scheduling.City || '03',
                defaultRegion: $scope.scheduling.Region || null
            });

            $scope.scheduling.ID = $target.data('sid');
            $scope.scheduling.DeviceID = firstTd.data('deviceid');
            $scope.scheduling.DeviceName = firstTd.text();
            $scope.scheduling.Loop = true;
            $scope.scheduling.BeginPlayDate = $scope.scheduling.BeginPlayDate || $(selectedTds[0]).data('date');
            $scope.scheduling.EndPlayDate = $scope.scheduling.EndPlayDate || $(selectedTds[selectedTds.length - 1]).data('date');
            //2017-04-28 要求修改己过期排期
            $scope.expired = false;// Date.parse($scope.scheduling.BeginPlayDate) - ui.now <= 0 ? true : false;
            updateDate($scope.scheduling.BeginPlayDate, $scope.scheduling.EndPlayDate);
            $scope.getAdList();

            ui.modal.modal('show');
        },
        saveDelAdIDs = function (detailIds) {
            var ids = [];
            $.each(detailIds, function (index, item) {
                ids.push(item.AdvertisementID);
            });
            $scope.DelAdIDs = ids;
        },
        updateDate = function (bgDate, edDate) {
            ui.beginPlayDate.datetimepicker('update', bgDate);
            ui.endPlayDate.datetimepicker('setStartDate', bgDate).datetimepicker('update', edDate);
        },
        validSelectedTd = function () {
            var i = 0, j = 0;
            $.each(tdRows, function (index, item) {
                item.data('sid') !== '' && i++;
                item.data('sid') === '' && j++;
            });
            return !(i > 0 && j > 0);
        },
        initForm = function (aid) {
            for (var i = 0; i < $scope.AdList.length; i++) {
                if ($scope.AdList[i].ID === aid) {
                    $scope.scheduling.PlayCount = $scope.AdList[i].PlayCount;
                    $scope.scheduling.PlayTimes = $scope.AdList[i].PlayTimes;
                    setPlayTimes();
                    break;
                }
            }
        },
        dpOptions = {
            minView: "month",
            format: "yyyy-mm-dd",
            language: 'zh-CN',
            autoclose: true,
            startDate: ui.now
        };

        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;

        $scope.adPageSize = 5;
        $scope.adTotalItems = 0;
        $scope.adCurrentPage = 1;

        $scope.month = ui.month.val();
        $scope.List = [];
        $scope.DateList = [];
        $scope.AdList = [];
        $scope.scheduling = {};
        $scope.DelAdIDs = [];
        $scope.DeviceListLeft = [];
        $scope.DeviceListRight = [];
        $scope.SchedulingList = [];
        $scope.CancelCopyIds = [];

        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/adScheduling/getList',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    data = new FormData(doc.getElementById('queryForm'));
                    data.append('pageIndex', $scope.currentPage);
                    data.append('pageSize', $scope.pageSize);
                    return data;
                }
            }).success(function (resp) {
                if (resp.Success) {
                    resp.Data.length === 0 && common.alert('未查到数据！');
                    $scope.List = resp.Data;
                    $scope.totalItems = resp.TotalCount;
                } else {
                    common.alert(resp.Message);
                    $scope.List = [];
                    $scope.totalItems = 0;
                }
            }).error(function () {
                common.alert('出现错误或者网络异常');
                $scope.List = [];
                $scope.totalItems = 0;
            });
        }
        $scope.getAdList = function () {
            $http({
                method: 'POST',
                url: '/adScheduling/getAdList',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    data = new FormData(doc.getElementById('adFilterFrom'));
                    data.append('pageIndex', $scope.adCurrentPage);
                    data.append('pageSize', $scope.adPageSize);
                    return data;
                }
            }).success(function (resp) {
                if (resp.Success) {
                    $scope.AdList = resp.Data;
                    $scope.adTotalItems = resp.TotalCount;
                    //$.each(checkAidList, function (index, item) {
                    //    $scope.check(item,true);
                    //});
                } else {
                    common.alert(resp.Message);
                    $scope.AdList = [];
                    $scope.adTotalItems = 0;
                }
            }).error(function () {
                common.alert('出现错误或者网络异常！');
                $scope.AdList = [];
                $scope.adTotalItems = 0;
            });
        };
        $scope.getDeviceLeft = function (deviceId, queryTxt, callback) {
            $http
                .post('/adScheduling/getHasSchedulingDevices', { queryText: queryTxt || $scope.queryTxtLeft, deviceId: deviceId })
                .success(function (resp) {
                    if (callback) {
                        callback(resp);
                    }
                    else if (resp.Success) {
                        $scope.DeviceListLeft = resp.Data;
                    }
                    else {
                        common.alert(resp.Message);
                        $scope.DeviceListLeft = [];
                    }
                })
                .error(function () {
                    common.alert('出现错误或者网络异常！');
                    $scope.DeviceListLeft = [];
                });
        };
        $scope.getDeviceRight = function () {
            $http
                .post('/adScheduling/getDeviceList', { queryText: $scope.queryTxtRight, filterDeviceId: $scope.deviceIdLeft })
                .success(function (resp) {
                    if (resp.Success) {
                        $scope.DeviceListRight = resp.Data;
                    }
                    else {
                        common.alert(resp.Message);
                        $scope.DeviceListRight = [];
                    }
                })
                .error(function () {
                    common.alert('出现错误或者网络异常！');
                    $scope.DeviceListRight = [];
                });
        };
        $scope.getScheduling = function (deviceId, e) {
            var target = $(e.target);
            target.prev().click();

            if (target.attr('aria-expanded') === 'true') return;

            $http.post('/adScheduling/getListByDeviceId', { deviceId: deviceId })
            .success(function (resp) {
                if (resp.Success) {
                    $scope.SchedulingList = resp.Data;
                    setTimeout(function () {
                        var content = $('#' + deviceId).find('div.popover-content')
                        var tmpl = content.clone().removeClass('hidden').html();
                        $("[data-toggle='popover']").popover({
                            trigger: 'hover',
                            html: true,
                            delay: { "hide": 200 },
                            content: tmpl
                        });
                    }, 10);
                }
                else {
                    commom.alert(resp.Message || '出错了！');
                }
            })
            .error(function () {
                common.alert('出现错误或者网络异常！');
            });
        };
        $scope.copyTo = function () {
            var errMsgs = [],
                deviceIdLeft = $('input[name= "deviceLeft"]:checked').val(), sIds = [], rightDeviceIds = [];

            deviceIdLeft || errMsgs.push('请选择被复制终端！');

            var schedulings = $('#' + deviceIdLeft).find('input');
            if (schedulings.length > 0) {
                schedulings.filter(':checked').each(function () {
                    sIds.push(this.value);
                });
                if (sIds.length === 0) {
                    errMsgs.push('请选择终端排期！');
                }
            }

            $('[name="deviceRight"]:checked').each(function () {
                rightDeviceIds.push(this.value);
            });

            rightDeviceIds.length === 0 && errMsgs.push('请选择目标终端！');

            $.each(rightDeviceIds, function (index, item) {
                if (item === deviceIdLeft) {
                    errMsgs.push('不允许把终端的排期复制到终端本身！');
                    $('input[name="deviceRight"][value="' + deviceIdLeft + '"]').parent().addClass(errClass);
                }
            });

            if (errMsgs.length > 0) {
                common.alert(errMsgs.join('<br>'));
                return;
            }

            $http
            .post('/adScheduling/copyTo', {
                deviceIdLeft: deviceIdLeft,
                schedulingIds: sIds,
                deviceIds: rightDeviceIds
            })
            .success(function (resp) {
                if (resp.Success) {
                    $scope.CancelCopyIds = resp.Data;
                    $scope.getList();
                    common.alert('复制成功！');
                }
                else {
                    common.alert(resp.Message, 5000);
                }
            })
            .error(function () {
                common.alert('网络异常！');
            });

        };
        $scope.copyThisDevice = function (deviceId) {
            $scope.getDeviceLeft(deviceId, '', function (resp) {
                if (resp.Success) {
                    $scope.DeviceListLeft = resp.Data;
                    $('[data-target="#copyModal"]').click();
                }
                else {
                    common.alert('此终端无有效排期！');
                    $scope.DeviceListLeft = [];
                }
            });
        };
        $scope.cancelCopy = function () {
            $http
            .post('/adScheduling/cancelCopy', { schedulingIds: $scope.CancelCopyIds })
            .success(function (resp) {
                if (resp.Success) {
                    common.alert('撤销成功！');
                    $scope.CancelCopyIds = [];
                    $scope.getList();
                }
                else {
                    common.alert(resp.Message);
                }
            })
            .error(function () {
                common.alert('出现错误或者网络异常！');
            });
        }

        $scope.tdInit = function (d, schedulings) {
            var s = {
                selected: '',
                sid: null
            }, date = new Date(d);

            for (var i = 0; i < schedulings.length; i++) {
                var beginDate = new Date(schedulings[i].BeginPlayDate),
                  endDate = new Date(schedulings[i].EndPlayDate);

                if (date >= beginDate && date <= endDate) {
                    s.selected = 'selected';
                    s.sid = schedulings[i].ID;
                    break;
                }

            }
            if (!s.sid && date < new Date()) {
                s.selected = 'expire';
            }

            return s;
        }
        $scope.tdClick = function (e) {
            if (!e.target.classList.contains('expire')) {
                e.target.style.backgroundColor = 'dodgerblue';
            }
        };
        $scope.down = function (e) {
            if (!e.target.classList.contains('expire')) {
                var x = e.clientX, y = e.clientY;
                arrPos.push(Array(x, y));
                key = 1;
            }
        };
        $scope.move = function (e) {
            var x = e.clientX, y = e.clientY;
            if (arrPos.length > 0) {
                var $e = $(e.target);
                //mX = e.clientX;
                //mY = e.clientY;
                //var p = common.getCoordinate(e.target)
                //tdX = p.x;
                //tdY = p.y;
                //tdoX = arrPos[0][1] + e.target.offsetWidth;
                //tdoY = arrPos[0][1] + e.target.offsetHeight;

                if (y <= arrPos[0][1] + e.target.offsetWidth && y >= arrPos[0][1] - 10 && 1 === key && e.target.tagName === "TD") {
                    $e.addClass("selected");
                    var exist = false;
                    $.each(tdRows, function (i, item) {
                        if ($e.data('date') === item.data('date')) {
                            exist = true;
                            return;
                        }
                    });
                    if (!exist)
                        tdRows.push($e);
                }
            }
        };
        $scope.up = function (e) {
            if (!e.target.classList.contains('expire')) {
                arrPos = new Array();
                key = 0;
                showModal(e.target);
                $.each(tdRows, function (index, td) {
                    if (td.data('sid') === '')
                        td.removeClass('selected');
                });
                tdRows = [];
            }
        };
        $scope.setDetailCheck = function (adId) {
            var result = false;
            $.each($scope.scheduling.Details, function (index, item) {
                if (item.AdvertisementID === adId || $.inArray(adId, checkAidList) !== -1) {
                    result = true;
                    return;
                }
            });
            return result;
        };
        $scope.check = function (aid, e) {
            if (e.target.checked) {
                initForm(aid);
                if (checkAidList.length === 0 || $.inArray(aid, checkAidList) === -1)
                    checkAidList.push(aid);
            }
            else {
                $.each(checkAidList, function (i) {
                    if (checkAidList[i] === aid) {
                        checkAidList.splice(i, 1);
                        return false;
                    }
                });
            }
        };
        $scope.toolBarChange = function (month) {
            if (month !== 0) {
                var date = new Date($scope.month);
                ui.month.datetimepicker('update', new Date(date.setMonth(date.getMonth() + month)));
            }
            else {
                ui.month.datetimepicker('update', ui.now);
            }
            getDaysInMonth();
            $scope.getList();
        };

        ui.modal.on('show.bs.modal', function () {
            if (ui.playTimesContainer.find('input').length === 0) {
                $.get('/adScheduling/getPlayTimesSelectorPartial', function (html) {
                    ui.playTimesContainer.html(html);
                    setPlayTimes();
                }, 'html');
            }
            else {
                setPlayTimes();
            }
        });
        ui.modal.on('hidden.bs.modal', function () {
            $scope.scheduling = {};
            tdRows = [];
            ui.adContainer.removeClass(errClass);
        });
        ui.copyModal.on('hidden.bs.modal', function () {
            $scope.DeviceListLeft = [];
            $scope.DeviceListRight = [];
            $scope.SchedulingList = [];
            $scope.CancelCopyIds = [];
        });

        ui.month.datetimepicker({
            startView: "year",
            minView: "year",
            format: "yyyy-mm",
            language: 'zh-CN',
            autoclose: true,
            initialDate: ui.now.getFullYear() + '-' + ui.now.getMonth()
        }).on('change', function (ev) {
            getDaysInMonth();
            $scope.getList();
        });
        ui.beginPlayDate.datetimepicker(dpOptions).on('changeDate', function (ev) {
            ui.endPlayDate.datetimepicker('setStartDate', ev.date).datetimepicker('show');
        });
        ui.endPlayDate.datetimepicker(dpOptions);
        getDaysInMonth();

        $('#btnCreate').ajaxClick({
            formSelector: '#schedulingForm',
            loadingText: "正在保存...",
            onBefore: function () {
                var playTimeChks = $('[name="PlayTime"]:checked'),
                    adIdsChks = ui.adContainer.find('[name="adId"]:checked'),
                 openAreas = [], playTimes = [], adIds = [], delAdIDs = [], errMsg = [];

                if (playTimeChks.length === 0) {
                    ui.playTimesContainer.addClass(errClass);
                    errMsg.push('请选择播放时段！');
                }
                else {
                    playTimeChks.each(function (index, item) {
                        playTimes.push({
                            Value: item.value,
                            Checked: 'Checked',
                            Name: $.trim($(item).parent().text())
                        })
                    });
                }

                if (adIdsChks.length === 0) {
                    ui.adContainer.addClass(errClass);
                    errMsg.push('请选择播放内容！');

                }
                else {
                    adIdsChks.each(function (index, item) {
                        adIds.push(item.value);
                    });

                    if ($scope.DelAdIDs.length > 0) {

                        $.each($scope.DelAdIDs, function (aIndex, oId) {
                            var isExist = false;
                            $.each(adIds, function (bIndex, nId) {
                                if (oId === nId) {
                                    isExist = true;
                                    return false;
                                }
                            });
                            !isExist && delAdIDs.push(oId);
                        });
                    }
                }

                if (errMsg.length > 0) {
                    common.alert(errMsg.join('</br>'), 5000);
                    return false;
                }

                return {
                    OpenAreas: JSON.stringify(openAreas),
                    PlayTimes: JSON.stringify(playTimes),
                    AdIDs: adIds.join('|'),
                    DelAdIDs: delAdIDs.join('|'),
                };
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    common.alert('保存成功');
                    $scope.getList();
                    ui.modal.modal('hide');
                }
                else {
                    common.alert(result.Message || "保存失败，请稍后再试！");
                }
                $ajaxBtn.reset();
            }
        });

        ui.btnDelete.ajaxClick({
            url: '/adScheduling/delete',
            loadingText: "正在删除...",
            onBefore: function () {
                if (!ui.btnDelete.data('delete')) {
                    ui.btnDelete.text("确定要删除吗？");
                    ui.btnDelete.data('delete', true);
                    return false;
                } else {
                    return { id: $scope.scheduling.ID };
                }
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    common.alert('删除成功！');
                    ui.modal.modal('hide');
                    $scope.getList();
                }
                else {
                    common.alert(result.Message || "删除失败，请稍后再试！");
                }
                $ajaxBtn.reset();
            }
        });
    }).filter('areaFilter', function () {
        return exAreaCode;
    });
})($, document)
