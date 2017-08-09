(function (angular, doc) {
    'use strict'

    angular.module('dlRecordApp', ['ui.bootstrap']).controller('DlRecordCtrl', function ($scope, $http) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/adDowload/getList',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    var data = new FormData(doc.getElementById('queryForm'));
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
                }
            }).error(function () {
                common.alert('出现错误或者网络异常');
                $scope.List = [];
                $scope.totalItems = 0;
            });
        }
    })
		 .filter('areaFilter', function () {
		     return exAreaCode;
		 });
})(window.angular, document);
