//下边是几个IE里边不支持的ES6方法，自己写一下。

//filter
if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun) {
        if (this === void 0 || this === null)
            throw new TypeError();

        var t = Object(this);
        var len = t.length >>> 0;
        if (typeof fun !== "function")
            throw new TypeError();

        var res = [];
        var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i];
                if (fun.call(thisArg, val, i, t))
                    res.push(val);
            }
        }
        return res;
    };
}
//max
if (!Array.prototype.max) {
    Array.prototype.max = function () {
        var m = this[0];
        for (var i = 1; i < this.length; i++) {
            if (this[i] > m)
                m = this[i];
        }
        return m;
    }
}
//sum
if (!Array.prototype.sum) {
    Array.prototype.sum = function () {
        var m = 0;
        for (var i = 0; i < this.length; i++) {
            m += this[i];
        }
        return m;
    }
}
//find
if (!Array.prototype.find) {
    Array.prototype.find = function (fun) {
        var rst = this.filter(fun);
        if (rst.length > 0)
            return rst[0];
        else
            return null;
    }
}
//pushRange
if (!Array.prototype.pushRange) {
    Array.prototype.pushRange = function (array) {
        for (var i = 0; i < array.length; i++) {
            this.push(array[i]);
        }
    }
}
//contains
if (!Array.prototype.contains) {
    Array.prototype.contains = function (value) {
        var rst = this.filter(function (x) { return x == value; });
        if (rst.length > 0)
            return rst[0];
        else
            return null;
    }
}
//map
if (!Array.prototype.map) {
    Array.prototype.map = function (callback, thisArg) {
        var T, A, k;
        if (this == null) {
            throw new TypeError(" this is null or not defined");
        }
        var O = Object(this);
        var len = O.length >>> 0;
        if (typeof callback !== "function") {
            throw new TypeError(callback + " is not a function");
        }
        if (thisArg) {
            T = thisArg;
        }
        A = new Array(len);
        k = 0;
        while (k < len) {
            var kValue, mappedValue;
            if (k in O) {
                kValue = O[k];
                mappedValue = callback.call(T, kValue, k, O);
                A[k] = mappedValue;
            }
            k++;
        }
        return A;
    };
}

function alertj(s) {
    alert(JSON.stringify(s));
}
Array.prototype.contains = function (needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}
function SaveCollapseToLocalStorage() {
    localStorage.setItem("CollapseStatus", "1");
}
function ClearCollapseToLocalStorage() {
    localStorage.removeItem("CollapseStatus");
}
function GetCollapseFromLocalStorage() {
    return localStorage.getItem("CollapseStatus") == 1;
}
function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串 
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
function Request(name) {
    var Request = GetRequest();
    var value = Request[name];
    return value;
}


function CommonAjaxPost(url, data, funcSuc, dataType) {
    $("#DivCover").show();
    $.post(url, data, function (data) {
        var datatype = typeof data;
        if (datatype == "string")
            data = JSON.parse(data);
        if (data.status == "302") {
            //location = data.url;
            app.ShowLogin();
        } else {
            funcSuc(data);
        }
        $("#DivCover").hide();
    }, dataType).error(function () { $("#DivCover").hide(); });
}
function JsonAjaxPost(url, data, funcSuc) {
    $("#DivCover").show();
    $.ajax({
        contentType: 'application/json',
        type: 'POST',
        url: url,
        dataType: "json",
        data: JSON.stringify(data),
        success: function (data) {
            if (data.status == "302") {
                //location = data.url;
                if (app) {
                    app.ShowLogin();
                    app.submitting = false;
                }
            } else {
                funcSuc(data);
            }
            $("#DivCover").hide();
        }
    }).error(function () { $("#DivCover").hide(); });
}