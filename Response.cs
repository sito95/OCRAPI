using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace OCRAPI_W511.ResponseClass
{
    public class Response : ContentResult
    {

        public Response(int code)
        {
            ContentType = "application/json";
            StatusCode = code;
        }

        public Response(int code, string content) : this(code)
        {
            Content = content;
        }

    }

    // OCR成功時のレスポンスbody
    public class ResponseBody
    {
        public string? rb_header = null;
        public Rb_Body rb_body;

        public ResponseBody(Rb_Body rb_body)
        {
            this.rb_body = rb_body;
        }
    }

    public class Rb_Body
    {
        public string wheel_id;
        //　いったん保留
        public List<string> msgs;

        public Rb_Body(string str)
        {
            wheel_id = str;
            msgs = new List<string>();
            msgs.Append(" ");
        }
    }
}


