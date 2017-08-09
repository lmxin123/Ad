(function (angular, doc) {
    'use strict'

    angular.module('adApp', ['ui.bootstrap']).controller('AdCtrl', function ($scope, $http) {
        $scope.maxSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.DetailList = {};
        $scope.merchantId = '';
        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/ad/GetList',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    var data = new FormData(doc.getElementById('queryForm'));
                    data.append('pageIndex', $scope.currentPage);
                    data.append('pageSize', $scope.maxSize);
                    return data;
                }
            }).success(function (resp) {
                if (resp.Success) {
                    $scope.List = resp.Data;
                    $scope.totalItems = resp.TotalCount;
                } else {
                    common.alert(resp.Message || '获取数据失败！');
                }
            }).error(function (resp) {
                common.alert(resp.Message || '获取数据出错，请稍后再试！');
                $scope.totalItems = 0;
            });
        }
        $scope.getDetailList = function (adId) {
            $http.post('/ad/GetDetailList', { adID: adId }).success(function (resp) {
                if (resp.Success) {
                    $scope.DetailList = resp.Data;
                    ui.detailModel.modal('show');
                } else {
                    common.alert(resp.Message || '获取数据失败！');
                }
            }).error(function (resp) {
                common.alert(resp.Message || '获取数据出错，请稍后再试！');
            });
        };

        var ui = {
            detailModel: $('#detailModal')
        };

        $scope.detailModal = function (id) {
            $scope.getDetailList(id);
        }

        ui.detailModel.on('hidden.bs.modal', function () {
            $scope.detailList = null;
        });

    })
        .filter('areaFilter', function () {
            return exAreaCode;
        });
})(window.angular, document);
