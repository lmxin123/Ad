(function (angular, doc) {
    'use strict'

    angular.module('adApp', ['ui.bootstrap']).controller('AdCtrl', function ($scope, $http, $timeout) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.Ad = {};
        $scope.DetailList = {};
        $scope.checkState = 0;
        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/adList/GetList',
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

                    setTimeout(function () {
                        $("[data-toggle='popover']").popover({
                            html: true,
                            delay: { "hide": 200 }
                        });
                    }, 10);

                } else {
                    common.alert(resp.Message || '获取数据失败！');
                }
            }).error(function () {
                common.alert('出现错误或者网络异常');
                $scope.totalItems = 0;
            });
        }
        $scope.getDetailList = function (adId) {
            $http.post('/adList/GetDetailList', { adID: adId }).success(function (resp) {
                if (resp.Success) {
                    $scope.DetailList = resp.Data;
                    ui.detailModel.modal('show');
                } else {
                    common.alert(resp.Message);
                }
            }).error(function (resp) {
                common.alert(resp.Message);
            });
        };

        var ui = {
            detailModel: $('#detailModal'),
            checkModal: $('#checkModal'),
            rContainer: $('#rContainer'),
            refusalReasonsContainer: $('#refusalReasonsContainer'),
            footerContainer: $('#footerContainer')
        },
            getAd = function (id) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].ID === id) {
                        $scope.Ad = $scope.List[i];
                        break;
                    }
                }
            },
            setUI = function () {
                if ($scope.Ad.RecordState === 3) {
                    ui.refusalReasonsContainer.show();
                }
                else {
                    ui.refusalReasonsContainer.hide();
                }
                if ($scope.Ad.RecordState === 1) {
                    ui.footerContainer.show();
                }
                else {
                    ui.footerContainer.hide();
                }
            };

        $scope.detailModal = function (id) {
            $scope.getDetailList(id);
        }

        ui.detailModel.on('hidden.bs.modal', function () {
            $scope.DetailList = null;
        });

        $scope.RecordStates = 2;
        $scope.check = function (id) {
            getAd(id);
            setUI();
            $scope.getDetailList(id);
        };
        var ajaxBtn = new AjaxClick(common.get('btnCheck'), {
            loadingText: "正在保存...",
            formSelector: '#checkForm',
            onBefore: function () {
                var r = $('#txtRefusal');
                if (!ui.rContainer.hasClass('hidden') && r.val() === '') {
                    var errClass = 'input-validation-error';
                    r.addClass(errClass);
                    r.keypress(function () {
                        r.removeClass(errClass);
                    });
                    return false;
                }
                return {
                    id: $('[name="ID"]').val(),
                    reasons: r.val(),
                    checkState: $('[name="checkState"]:checked').val()
                }
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    $scope.Ad.RecordState = 2;
                    setUI();
                    $('#txtRefusal').val('');
                    common.alert("保存成功");
                    ui.detailModel.modal('hide');
                }
                else {
                    $ajaxBtn.reset();
                    common.alert("保存失败：" + result.Message)
                }
                ajaxBtn = $ajaxBtn;
            }
        });
        $scope.checkClick = function () {
            ajaxBtn.begin();
        }

        $scope.radioClick = function (e) {
            if (e.target.value === "2") {
                ui.rContainer.addClass('hidden');
            }
            else {
                ui.rContainer.removeClass('hidden');
            }
        };
        ui.detailModel.on('hidden.bs.modal', function () {
            $scope.Ad = null;
            $scope.DetailList = null;
            ajaxBtn && ajaxBtn.reset();
        });
        ui.checkModal.on('hidden.bs.modal', function () {
            $scope.Ad = null;
        });

        var urlParams = common.getUrlParameToJSON();
        if (urlParams.isCancel) {
            $scope.getList();
        }
    }).filter('areaFilter', function () {
        return exAreaCode;
    }).filter('playTimeFilter', function () {
        return function () {
            var t = arguments[0], index = t.indexOf('，'), result;
            index = index === -1 ? t.indexOf(',') : index;
            if (index === -1)
                result = t;
            else
                result = t.substring(0, index) + '...';
            return result;
        }
    });
})(window.angular, document);
