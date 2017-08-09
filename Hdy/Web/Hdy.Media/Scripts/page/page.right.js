; +(function (angular, doc) {
    'use strict'

    angular.module('rightApp', []).controller('RightCtrl', function ($scope, $http) {
        $scope.rights = [];
        $scope.getRight = function () {
            $http({
                url: '/right/get',
                method: 'POST',
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    data = new FormData(common.get('queryForm'));
                    return data;
                }
            }).success(function (resp) {
                if (resp.Success) {
                    resp.Data.length === 0 && common.alert('未查到数据！');
                    $scope.rights = resp.Data;
                } else {
                  //  common.alert(resp.Message);
                    $scope.rights = [];
                }
            }).error(function () {
                common.alert("网络异常或者请求出错了！");
                $scope.rights = [];
            });
        };

        $('#roleId').change(function () {
            //if (this.value === '') {
            //    $scope.rights = [];
            //    return;
            //}

            $scope.getRight();
        });
        $scope.change = function (i, j, checked) {
            var target = $('input[data-role][data-index="[' + i + ',' + j + ']"');
            var role = target.data('role');
            if (role === 'parent') {
                for (j = 0; j < $scope.rights[i].SubRightList.length; j++) {
                    setRight(i, j, checked);
                }
            }
            else if (role === 'sub') {
                setRight(i, j, checked);
            }
            $scope.actionChange(i);
        };
        $scope.actionChange = function (a) {
            var length = $scope.rights[a].SubRightList.length, count = length;

            for (var i = 0; i < length ; i++) {
                if (($scope.rights[a].SubRightList[i].Select !== null && $scope.rights[a].SubRightList[i].Select) ||
                   ($scope.rights[a].SubRightList[i].Create !== null && $scope.rights[a].SubRightList[i].Create) ||
                   ($scope.rights[a].SubRightList[i].Update !== null && $scope.rights[a].SubRightList[i].Update) ||
                   ($scope.rights[a].SubRightList[i].Delete !== null && $scope.rights[a].SubRightList[i].Delete) ||
                   ($scope.rights[a].SubRightList[i].Auditing !== null && $scope.rights[a].SubRightList[i].Auditing)) {
                    $scope.rights[a].All = $scope.rights[a].SubRightList[i].All = true;
                }
                else {
                    $scope.rights[a].SubRightList[i].All = false;
                    count--;
                }
            }
            if (length !== 0)
                $scope.rights[a].All = count !== 0;

            save();
        };

        var setRight = function (i, j, chk) {
            if ($scope.rights[i].SubRightList[j].Select !== null)
                $scope.rights[i].SubRightList[j].Select = chk;
            if ($scope.rights[i].SubRightList[j].Create !== null)
                $scope.rights[i].SubRightList[j].Create = chk;
            if ($scope.rights[i].SubRightList[j].Update !== null)
                $scope.rights[i].SubRightList[j].Update = chk;
            if ($scope.rights[i].SubRightList[j].Delete !== null)
                $scope.rights[i].SubRightList[j].Delete = chk;
            if ($scope.rights[i].SubRightList[j].Auditing !== null)
                $scope.rights[i].SubRightList[j].Auditing = chk;
        };
        var save = function () {
            $http.post('/right/create',
                {
                    roleId: $('#roleId').val(),
                    roleJsonStr: JSON.stringify($scope.rights)
                }).success(function (resp) {
                    common.alert(resp.Success ? '数据己保存！' : '保存失败：' + resp.Message);
                }).error(function (resp) {
                    common.alert("网络异常！");
                });
        };
    });
})(window.angular, document);