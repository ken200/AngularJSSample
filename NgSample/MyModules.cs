using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.ModelBinding;

namespace NgSample
{
    public class MyModule : NancyModule
    {
        public MyModule()
        {
            Get["/"] = _ =>
            {
                return View["index"];
            };
        }
    }

    public class ApiModule : NancyModule
    {
        public ApiModule()
            : base("/api")
        {
            Get["items"] = _ =>
            {
                string q = Request.Query.q;

                if (q == "error")
                    throw new Exception("エラーが発生しました。");

                if (q.Length > 5)
                {
                    return Response.AsJson<ApiResult<IEnumerable<Item>>>(
                        new ApiResult<IEnumerable<Item>>()
                        {
                            Error = true,
                            Message = "パラメーターが大きすぎます。",
                            Detail = Enumerable.Empty<Item>()
                        }
                        , HttpStatusCode.BadRequest);
                }
                else
                {
                    return Response.AsJson<ApiResult<IEnumerable<Item>>>(
                        new ApiResult<IEnumerable<Item>>()
                        {
                            Detail = q.Length % 2 == 0 ? new[] {
                                new Item(){Name = "アイテム１", Price = 100},
                                new Item(){Name = "アイテム２", Price = 200},
                                new Item(){Name = "アイテム３", Price = 300},
                                new Item(){Name = "アイテム４", Price = 400},
                                new Item(){Name = "アイテム５", Price = 500},
                                new Item(){Name = "アイテム６", Price = 600},
                                new Item(){Name = "アイテム７", Price = 700},
                                new Item(){Name = "アイテム８", Price = 800},
                                new Item(){Name = "アイテム９", Price = 900},
                                new Item(){Name = "アイテム１０", Price = 1000}
                            } : Enumerable.Empty<Item>()
                        });
                }
            };

            Post["items"] = _ => 
            {
                var newItem = this.Bind<Item>();

                if(newItem.Price >= 200 && newItem.Price <= 299)
                {
                    return Response.AsJson<ApiResult>(new ApiResult(){Error = true, Message = "200円代の商品は登録できません。"}, HttpStatusCode.BadRequest);
                }

                return Response.AsJson<ApiResult>(new ApiResult());
            };

            Delete["items"] = _ =>
            {
                //todo: DeleteMethodの場合はバインドできない？？？
                var newItem = this.Bind<Item>();
                return Response.AsJson<ApiResult>(new ApiResult());
            };
        }

        public class ApiResult
        {
            public bool Error { get; set; }
            public string Message { get; set; }

            public ApiResult()
            {
                Error = false;
                Message = "";
            }
        }

        public class ApiResult<T> : ApiResult
        {
            public T Detail { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }
            public int Price { get; set; }
        }
    }
}