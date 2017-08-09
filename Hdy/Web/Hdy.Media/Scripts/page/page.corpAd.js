(function (angular, doc) {
    'use strict'

    angular.module('adApp', ['ui.bootstrap'])
        .controller('AdCtrl', function ($scope, $http) {
            $scope.pageSize = 20;
            $scope.totalItems = 0;
            $scope.currentPage = 1;
            $scope.List = [];
            $scope.DetailList = [];
            $scope.merchantId = '';
            $scope.getList = function (e) {
                $http({
                    method: 'POST',
                    url: '/corpAd/GetList',
                    headers: { 'Content-Type': undefined },
                    transformRequest: function (data) {
                        data = new FormData(common.get('queryForm'));
                        data.append('pageIndex', $scope.currentPage);
                        data.append('pageSize', $scope.pageSize);
                        return data;
                    }
                }).success(function (resp) {
                    if (resp.Success) {
                        resp.Data.length === 0 && e && common.alert('未查到数据！');
                        $scope.List = resp.Data;
                        $scope.totalItems = resp.TotalCount;
                        setPagination(resp.TotalCount);
                    } else {
                        common.alert(resp.Message || '获取数据失败！');
                    }
                    setTimeout(function () {
                        $("[data-toggle='popover']").popover({
                            html: true,
                            delay: { "hide": 200 }
                        });
                    }, 10);
                }).error(function () {
                    common.alert('出现错误或者网络异常！');
                    $scope.totalItems = 0;
                });
            }
            $scope.getDetailList = function (adId) {
                $http.post('/corpAd/getDetailList', { adID: adId }).success(function (resp) {
                    if (resp.Success) {
                        $scope.DetailList = resp.Data;
                        ui.detailModel.modal('show');
                    } else {
                        common.alert(resp.Message || '获取数据失败！');
                    }
                }).error(function () {
                    common.alert('出现错误或者网络异常！');
                });
            };
            $scope.delete = function (id, e) {
                var target = $(e.target);

                if (!target.data('delete')) {
                    target.text('确定要删除吗');
                    target.data('delete', true).css({ 'color': 'red' });
                    return;
                }
                common.alert("正在删除...", function () {
                    $http.post('/corpAd/delete', { id: id })
                        .success(function (resp) {
                            if (resp.Success) {
                                common.alert('删除成功！');
                                $scope.getList();
                            } else {
                                common.alert(resp.Message);
                                target.text('删除');
                                target.data('delete', false).css({ 'color': '' });
                            }
                        }).error(function () {
                            common.alert('出现错误或者网络异常！');
                            $scope.List = [];
                            $scope.totalItems = 0;
                        });
                });
            };
            var ui = {
                detailModel: $('#detailModal')
            },
            setPagination = function (total, size) {
                $('ul[uib-pagination]').attr('total-items', total).attr('max-size', 2);
            };

            $scope.detailModal = function (id) {
                $scope.getDetailList(id);
            }

            ui.detailModel.on('hidden.bs.modal', function () {
                $scope.detailList = null;
            });

            var urlParams = common.getUrlParameToJSON();
            if (urlParams.isCancel) {
                $scope.getList();
            }
        })
        .filter('areaFilter', function () {
            return exAreaCode;
        })
        .filter('playTimeFilter', function () {
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

    $(function () {

    });
})(window.angular, document);
