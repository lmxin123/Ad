; +(function (angular, doc) {
    'use strict'

    angular.module('categoryApp', ['ui.bootstrap']).controller('CategoryCtrl', function ($scope, $http) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.category = {};
        $scope.List = [];
        $scope.getList = function () {
            $http({
                url: '/deviceCategory/getList',
                method: 'POST',
                data: {
                    pageSize: $scope.pageSize,
                    pageIndex: $scope.currentPage
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
        };
        $scope.getList();

        var ui = {
            categroyModal: $('#categoryModal')
        };

        $scope.categoryModal = function () {
            if (arguments.length != 0) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].ID == arguments[0]) {
                        $scope.category.ID = $scope.List[i].ID;
                        $scope.category.Name = $scope.List[i].Name;
                        $scope.category.Remark = $scope.List[i].Remark;
                        break;
                    }
                }
            }
            ui.categroyModal.modal('show');
        }

        $scope.delete = function (id, e) {
            var target = $(e.target);

            if (!target.data('delete')) {
                target.text('确定要删除吗');
                target.data('delete', true).css({ 'color': 'red' });
                return;
            }

            common.alert("正在删除...", function () {
                $http.post('/deviceCategory/delete', { id: id })
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

        $('#btnCheck').ajaxClick({
            formSelector: '#categoryForm',
            onBefore: function (self) {
                self.$form.attr('action', '/deviceCategory/' + ($scope.category.ID ? 'update' : 'create'));
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    common.alert("保存成功");
                    ui.categroyModal.modal('hide');
                }
                else {

                    common.alert("保存失败：" + result.Message)
                }
                $ajaxBtn.reset();
            }
        });
        ui.categroyModal.on('hide.bs.modal', function () {
            $scope.category = {};
        });
    });
})(window.angular, document);