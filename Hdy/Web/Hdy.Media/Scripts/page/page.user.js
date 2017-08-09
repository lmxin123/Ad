; +(function (angular, doc) {
    'use strict'

    angular.module('userApp', ['ui.bootstrap']).controller('UserCtrl', function ($scope, $http) {
        var ui = {
            userModal: $('#userModal'),
            userForm: $('#userForm')
        },
        errClass = 'input-validation-error';

        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.user = {};
        $scope.List = [];
        $scope.getList = function () {
            $http({
                url: '/user/getList',
                method: 'POST',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    data = new FormData(doc.getElementById('queryForm'));
                    data.append('pageIndex', $scope.currentPage);
                    data.append('pageSize', $scope.pageSize);
                    return data;
                }
            }).success(function (resp) {
                if (resp.Success) {
                    resp.Data.length == 0 && common.alert('未查到数据！');
                    $scope.List = resp.Data;
                    $scope.totalItems = resp.TotalCount;
                } else {
                    common.alert(resp.Message);
                    $scope.List = [];
                    $scope.totalItems = 0;
                }
            }).error(function (resp) {
                common.alert('出现错误或者网络异常！');
                $scope.List = [];
                $scope.totalItems = 0;
            });
        };
        $scope.operate = function (action, id, state, e, txt) {
            var target = $(e.target);
            if (!target.data('confirm')) {
                target.text('确定要' + txt + '吗');
                target.data('confirm', true).css({ 'color': 'red' });
                return;
            }

            common.alert('正在' + txt + '...', function () {
                $http.post('/user/' + action, { id: id, state: state })
                    .success(function (resp) {
                        if (resp.Success) {
                            common.alert(txt + '成功！');
                            $scope.getList();
                        } else {
                            common.alert(resp.Message);

                            target.text(txt);
                            target.data('confirm', false).css({ 'color': '' });
                        }
                    }).error(function () {
                        common.alert('出现错误或者网络异常！');
                    });
            });
        };

        $scope.userModal = function (id) {
            if (id) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].Id === id) {
                        $scope.user.Id = $scope.List[i].Id;
                        $scope.user.RoleId = $scope.List[i].RoleId;
                        $scope.user.UserName = $scope.List[i].UserName;
                        $scope.user.Password = $scope.List[i].Password;
                        $scope.user.PhoneNumber = $scope.List[i].PhoneNumber;
                        $scope.user.Email = $scope.List[i].Email;
                        $scope.user.Remark = $scope.List[i].Remark;
                        
                        ui.userModal.find('#Id').val($scope.user.Id);
                        ui.userModal.find('#RoleId').val($scope.user.RoleId);
                        break;
                    }
                }
            }
            else {
                $scope.user = {};
            }
            ui.userModal.modal('show');
        };

        $('#btnCreate').ajaxClick({
            formSelector: '#userForm',
            onBefore: function (self) {
                self.$form.attr('action', '/user/' + ($scope.user.Id ? 'update' : 'create'));
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    common.alert("保存成功");
                    ui.userModal.modal('hide');
                }
                else {
                    common.setValErrorMsg($ajaxBtn.$form, 'UserName', result.Message);
                }
                $ajaxBtn.reset();
            }
        });

        ui.userModal.on('hide.bs.modal', function () {
            $scope.user = {};
        });

        $scope.getList();

    }).filter('userStateFilter', function () {
        return function (state) {
            switch (state) {
                case 0:
                    return '正常';
                case 1:
                    return '锁定';
                default:
                    return '删除'
            }
        };
    });
})(window.angular, document);