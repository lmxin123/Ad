; +(function (angular, doc) {
    'use strict'

    angular.module('roleApp', ['ui.bootstrap']).controller('RoleCtrl', function ($scope, $http) {
        $scope.role = {};
        $scope.List = [];
        $scope.getList = function () {
            $http({
                url: '/role/getList',
                method: 'GET',
            }).success(function (resp) {
                if (resp.Success) {
                    resp.Data.length == 0 && common.alert('未查到数据！');
                    $scope.List = resp.Data;
                } else {
                    common.alert(resp.Message);
                }
            }).error(function (resp) {
                common.alert('出现错误或者网络异常！');
                $scope.List = [];
            });
        };
        var ui = {
            roleModal: $('#roleModal'),
            roleForm: $('#roleForm')
        };

        $scope.roleModal = function () {
            if (arguments.length != 0) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].Id == arguments[0]) {
                        $scope.role.Id = $scope.List[i].Id;
                        $scope.role.Name = $scope.List[i].Name;
                        $scope.role.Remark = $scope.List[i].Remark;

                        ui.roleModal.find('#Id').val($scope.role.Id);
                        break;
                    }
                }
            }
            ui.roleModal.modal('show');
        };

        $scope.delete = function (id, e) {
            var target = $(e.target);

            if (!target.data('delete')) {
                target.text('确定要删除吗');
                target.data('delete', true).css({ 'color': 'red' });
                return;
            }

            common.alert("正在删除...", function () {
                $http.post('/role/delete', { id: id })
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
                    });
            });
        };
        $('#btnCreate').ajaxClick({
            formSelector: '#roleForm',
            onBefore: function (self) {
                self.$form.attr('action', '/role/' + ($scope.role.Id ? 'update' : 'create'));
            },
            onSuccess: function (resp, $ajaxBtn) {
                $ajaxBtn.reset();
                common.alert('保存成功！');
                ui.roleModal.modal('hide');
                $scope.getList();
            },
            onFaild: function (resp, $ajaxBtn) {
                $ajaxBtn.reset();
                common.setValErrorMsg($ajaxBtn.$form, 'Name', resp.Message);
            }
        });
        ui.roleModal.on('hidden.bs.modal', function () {
            $scope.role = {};
        });

        $scope.getList();
    });
})(window.angular, document);