using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.CodeMonkey.Entities
{
    public interface IApiResultModel
    {
        bool result { get; }
        string message { get; }
        int code { get; }
        string time { get; }
    }
    public abstract class ApiResultModelBase : IApiResultModel
    {
        public string requestid { get; set; } = Guid.NewGuid().ToString();
        private string _businessKeyId;
        public string businessKeyId
        {
            get
            {
                if (!string.IsNullOrEmpty(_businessKeyId))
                    return _businessKeyId;
                return null;
            }
            set
            {
                _businessKeyId = value;
            }
        }
        public bool result { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public string time { get; set; }
        public string enumMsgType { get; set; }
    }
    public class ApiResultModelNoData : ApiResultModelBase { }
    public class ApiResultModel<T> : ApiResultModelBase
    {
        public T data { get; set; }
    }
    public class ApiListResultModel<T> : ApiResultModelBase
    {
        public List<T> listData { get; set; }
    }
    public class ApiPageListResultModel<T> : ApiListResultModel<T>
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int total { get; set; }
    }

    public class ApiPageResultModel<T> : ApiResultModel<T>
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int total { get; set; }
    }

    public class ApiListResultModel : ApiListResultModel<object>
    {
        public static ApiListResultModel<T> FromData<T>(List<T> data, int code = 1)
        {
            ApiListResultModel<T> apiResultModel = new ApiListResultModel<T>()
            {
                result = true,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                listData = data,
                code = code,
                message = "成功",
                enumMsgType = "SUCCESS"
            };
            return apiResultModel;
        }
        public static ApiPageListResultModel<T> FromPageData<T>(List<T> data, int total, int pageIndex, int pageSize, int code = 1)
        {
            ApiPageListResultModel<T> apiPageListResultModel = new ApiPageListResultModel<T>()
            {
                result = true,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                listData = data,
                code = code,
                page = pageIndex,
                pageSize = pageSize,
                total = total,
                totalPage = (total - 1) / pageSize + 1
            };
            return apiPageListResultModel;
        }
    }
    public class ApiResultModel : ApiResultModel<object>
    {

        public static ApiResultModel<T> FromData<T>(T data, int code = 1)
        {
            ApiResultModel<T> apiResultModel = new ApiResultModel<T>()
            {
                result = true,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                data = data,
                code = 1,
                message = "成功",
                enumMsgType = "SUCCESS"
            };
            return apiResultModel;
        }


        public static ApiResultModel FromError(string message, int code = 0)
        {
            ApiResultModel apiResultModel = new ApiResultModel()
            {
                result = false,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                message = message,
                code = code,
                enumMsgType = "FAIL"
            };
            return apiResultModel;
        }


        public static ApiPageResultModel<T> FromPageData<T>(T data, int total, int pageIndex, int pageSize, int code = 1)
        {
            ApiPageResultModel<T> apiPageListResultModel = new ApiPageResultModel<T>()
            {
                result = true,
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                data = data,
                code = code,
                page = pageIndex,
                pageSize = pageSize,
                total = total,
                totalPage = (total - 1) / pageSize + 1
            };
            return apiPageListResultModel;
        }
    }
}
