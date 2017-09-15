; +(function (angular, doc) {
    'use strict'

    angular.module('deviceApp', ['ui.bootstrap']).controller('DeviceCtrl', function ($scope, $http) {
        $scope.pageSize = 20;
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.device = {};
        $scope.List = [];
        $scope.getList = function () {
            $http({
                url: '/device/getList',
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

        $scope.deviceModal = function () {
            if (arguments.length !== 0) {
                for (var i = 0; i < $scope.List.length; i++) {
                    if ($scope.List[i].ID === arguments[0]) {
                        $scope.device.ID = arguments[0];
                        $scope.device.Name = $scope.List[i].Name;
                        $scope.device.Address = $scope.List[i].Address;
                        $scope.device.DeviceCode = $scope.List[i].DeviceCode;
                        $scope.device.Province = $scope.List[i].Province;
                        $scope.device.City = $scope.List[i].City;
                        $scope.device.Region = $scope.List[i].Region;
                        $scope.device.Contact = $scope.List[i].Contact;
                        $scope.device.Phone = $scope.List[i].Phone;
                        $scope.device.Email = $scope.List[i].Email;
                        $scope.device.Count = $scope.List[i].Count;
                        $scope.device.IsOnlineDesplay = $scope.List[i].IsOnlineDesplay;
                        $scope.device.CreateDateDisplay = $scope.List[i].CreateDateDisplay;
                        $scope.device.Operator = $scope.List[i].Operator;
                        $scope.device.Remark = $scope.List[i].Remark;
                        ui.deviceModal.modal('show');
                        break;
                    }
                }
            }
            ui.deviceModal.modal('show');
            //else {
            //    $http.get('/device/getDeviceCode').success(function (resp) {
            //        if (resp.Success) {
            //            $scope.device.DeviceCode = resp.Data;
            //            ui.deviceModal.modal('show');
            //        } else {
            //            common.alert(resp.Message);
            //        }
            //    }).error(function (resp) {
            //        common.alert(resp.Message || "出错了");
            //        $scope.List = [];
            //        $scope.totalItems = 0;
            //    });
            //}
        };
        $scope.delete = function (id, e) {
            var target = $(e.target);

            if (!target.data('delete')) {
                target.text('确定要删除吗');
                target.data('delete', true).css({ 'color': 'red' });
                return;
            }
            common.alert("正在删除...", function () {
                $http.post('/device/delete', { id: id })
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
        $('#btnSaveDevice').ajaxClick({
            formSelector: '#deviceForm',
            onBefore: function (self) {
                self.$form.attr('action', '/device/' + ($scope.device.ID ? 'update' : 'create'));
            },
            onCompleted: function (result, $ajaxBtn) {
                if (result.Success) {
                    $scope.getList();
                    common.alert("保存成功");
                    ui.deviceModal.modal('hide');
                }
                else {
                    common.alert("保存失败：" + result.Message)
                }
                $ajaxBtn.reset();
            }
        });
        var ui = {
            deviceModal: $('#deviceModal'),
            deviceForm: $('#deviceForm')
        };
        ui.deviceModal.on('hide.bs.modal', function () {
            $scope.device = {};
        });
    }).filter('areaFilter', function () {
        return exAreaCode;
    });
})(window.angular, document);