var ngApp = angular.module("ngapp", []);

ngApp.controller("mainCtrl", ["$scope", "$http", function ($scope, $http) {
    var alertError = function (message) {
        alert(["エラーが発生しました", message].join(" : "));
    };

    /**
    * 検索の実行
    */
    $scope.doSearch = function () {
        $http.get(["/api/items?q=", $scope.query].join("")).success(function (data, status) {
            if (data.error) {
                //この処理パスを通るのはステータスコードが200だが、Error==falseの場合のみ。
                alertError(data.message);
            } else {
                $scope.items = data.detail;
            }
        }).error(function (error) {
            //サーバー側で例外発生(5XX)またはステータスコードが4XXの場合にエラーとなる。
            //5XXの場合はerrorには例外補足しなかった場合だと、FontFoundページのhtml文字列が戻る。(要考慮)
            if (_.isObject(error))
                alertError(error.message);
            else
                console.log(error);
        });
    };

    /**
    * アイテムの追加
    */
    $scope.doAdd = function () {
        var name = $scope.newItemName;
        var price = $scope.newItemPrice;
        $http.post("/api/items", { name: name, price: price })
        .success(function (data, status) {
            $scope.items.push({ name: name, price: price });
        }).error(function (error) {
            if (_.isObject(error)) {
                alertError(error.message);
            } else {
                console.log(error);
            }
        });
    };

    /**
    * アイテムの削除
    */
    $scope.deleteItem = function (idx) {
        var tgt = $scope.items[idx];
        $http.delete("/api/items", { name: tgt.name, price: tgt.price }).success(function (data, status) {
            $scope.items.splice(idx, 1);
        }).error(function (error) {
            alertError(error.message);
        });
    }
}]);
