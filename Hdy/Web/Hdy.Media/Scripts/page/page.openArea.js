; +(function (angular, doc) {
    'use strict'

    angular.module('openAreaApp', ['ui.bootstrap']).controller('OpenAreaCtrl', function ($scope, $http) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.area = {};
        $scope.getList = function () {
            $http({
                method: 'POST',
                url: '/openarea/getlist',
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
                }
            }).error(function (resp) {
                common.alert('出现错误或者网络异常！');
                $scope.List = [];
                $scope.totalItems = 0;
            });
        }

        var ui = {
            areaModal: $('#areaModal')
        };

        $scope.areaModal = function () {
            if (arguments.length != 0) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].ID == arguments[0]) {
                        $scope.area.ID = $scope.List[i].ID;
                        $scope.area.Province = $scope.List[i].Province;
                        $scope.area.City = $scope.List[i].City;
                        $scope.area.Region = $scope.List[i].Region;
                        $scope.area.Name = $scope.List[i].Name;
                        $scope.area.Remark = $scope.List[i].Remark;
                        break;
                    }
                }
            }
            ui.areaModal.modal('show');
        }

        $scope.delete = function (id, e) {
            var target = $(e.target);

            if (!target.data('delete')) {
                target.text('确定要删除吗');
                target.data('delete', true).css({ 'color': 'red' });
                return;
            }

            common.alert("正在删除...", function () {
                $http.post('/openarea/delete', { id: id })
                    .success(function (resp) {
                        if (resp.Success) {
                            common.alert('删除成功！');
                            $scope.getList();
                        } else {
                            common.alert(resp.Message);
                            target.text('删除');
                            target.data('delete', false).css({ 'color': '' });
                        }
                    }).error(function (resp) {
                        common.alert('出现错误或者网络异常！');
                        $scope.List = [];
                        $scope.totalItems = 0;
                    });
            });
        };
        var ajaxBtn;
        $('#btnCheck').ajaxClick({
            formSelector: '#areaForm',
            onBefore: function (self) {
                self.$form.attr('action', '/openarea/' + ($scope.area.ID ? 'update' : 'create'));
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    common.alert("保存成功");
                    ui.areaModal.modal('hide');
                }
                else {
                    common.alert("保存失败：" + result.Message)
                }
                $ajaxBtn.reset();
            }
        });
        ui.areaModal.on('hide.bs.modal', function () {
            $scope.area = {};
        });
    })
         .filter('areaFilter', function () {
             return exAreaCode;
         });
})(window.angular, document);