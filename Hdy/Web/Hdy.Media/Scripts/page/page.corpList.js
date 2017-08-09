(function (angular, doc) {
    'use strict'

    angular.module('merchantListApp', ['ui.bootstrap']).controller('MerchantListCtrl', function ($scope, $http) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.merchant = {};
        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/corpList/getList',
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
                    common.alert(resp.Message || '获取数据失败！');
                }
            }).error(function () {
                common.alert('出现错误或者网络异常！');
                $scope.totalItems = 0;
            });
        }

        var ui = {
            detailModel: $('#detailModal'),
            checkModal: $('#checkModal')
        }, getMerchant = function (id) {
            for (var i = 0; i < $scope.List.length; i++) {
                if ($scope.List[i].ID === id) {
                    $scope.merchant = $scope.List[i];
                    break;
                }
            }
        };
        $scope.detailModal = function (id) {
            getMerchant(id);
            var p = $scope.merchant.Province, c = $scope.merchant.City, r = $scope.merchant.Region;
            $scope.merchant.AddressDisplay = exAreaCode(p)
                + exAreaCode(p, c)
                + exAreaCode(p, c, r)
                + $scope.merchant.Address;

            if ($scope.merchant.BusinessLicensePath) {
                $http.get('/corpList/GetBusinessLicense?filePath=' + $scope.merchant.BusinessLicensePath).success(function (result) {
                    if (result.Success) {
                        $('#bLContainer').empty().html('<img src=' + result.Data + ' width="100"/>');
                    }
                    ui.detailModel.modal('show');
                });
            }
            else {
                ui.detailModel.modal('show');
            }
        }

        $scope.RecordStates = 2;
        $scope.check = function (id) {
            getMerchant(id);
            ui.checkModal.modal('show');
        };
        var ajaxBtn;
        $('#btnCheck').ajaxClick({
            loadingText: "正在保存...",
            formSelector: '#checkForm',
            onBefore: function () {
                var r = $('#RefusalReasons');
                if (!r.parents('div.form-group').hasClass('ng-hide') && r.val() === '') {
                    var errClass = 'input-validation-error';
                    r.addClass(errClass);
                    r.keypress(function () {
                        r.removeClass(errClass);
                    });
                    return false;
                }
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    common.alert("保存成功");
                    ui.checkModal.modal('hide');
                }
                else {
                    $ajaxBtn.reset();
                    common.alert("保存失败：" + result.Message)
                }
                ajaxBtn = $ajaxBtn;
            }
        });

        $scope.radioClick = function (e) {
            $scope.RecordStates = e.target.value;
        };
        ui.detailModel.on('hidden.bs.modal', function () {
            $scope.merchant = null;
        });
        ui.checkModal.on('hidden.bs.modal', function () {
            $scope.merchant = null;
            ajaxBtn && ajaxBtn.reset();
        });

        var urlParams = common.getUrlParameToJSON();
        if (urlParams.isCancel) {
            $scope.getList();
        }
    }).filter('areaFilter', function () {
        return exAreaCode;
    });
})(window.angular, document);
