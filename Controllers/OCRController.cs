using Microsoft.AspNetCore.Mvc; 
using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using System.IO;
using Google.Apis.Auth.OAuth2;
using System.Text;
using OCRAPI_W511.Model;
using OCRAPI_W511.Base64;
using OCRAPI_W511.PostProcess;
using OCRAPI_W511.ResponseClass;
using System.Diagnostics;
using static OCRAPI_W511.ResponseClass.ResponseBody;

namespace OCRAPI_W511.Controllers

{
    [ApiController]
    public class OCRController : ControllerBase
    {
      [HttpPost]
      [Route("api/v1/BridgeStone/WheelNumberDetection")]
        public IActionResult Post([FromBody] Param param)
        {
            try
            {
                // 入力値の確認
                // null,empty,base64でないときエラーを投げる
                //Base64Client.Isbase64String(param.uploadedFile);
                //StreamReader sr = new StreamReader("test.txt");

                //string text = sr.ReadToEnd();
                //base64を扱うインスタンス生成

                //RASを切らないと実行できないため注意

                Base64Client base64image = new(param.uploadedFile);
                
                // VisionAPIに投げる
                Image image = Image.FromBytes(base64image.Bytes);
                // ImageAnnotatorClientのインスタンス生成時にjsonキーの確認
                ImageAnnotatorClient client = ImageAnnotatorClient.Create();
                IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

                //TODO
                //文字を認識できなかった場合、textAnnotations自体が空?
                //textAnnotations[0]がエラーになった

                // BSルールに適合しているかなどのチェック
                Rb_Body rb_body = new Rb_Body(textAnnotations[0].Description);                
                PostProceser.Run(rb_body);
                
                // OCR結果のテキストをJSON形式に変換
                ResponseBody responsebody = new ResponseBody(rb_body);

                return new Response(200, JsonConvert.SerializeObject(responsebody));                    
            }
            catch (AnnotateImageException e)
            {
                // VisionAPIのエラー
                // エラーログを吐き出す
                //AnnotateImageResponse response = e.Response;
                //string errorMessage = e.Response.Error.Message;
                //Debug.WriteLine(errorMessage);
                return StatusCode(500); 

            }
            catch (ArgumentNullException e)
            {
                // POSTリクエストのuploadFileがnullの場合
                return StatusCode(400);
            }
            catch (Exception e)
            {
                //　上記以外のエラー
                return StatusCode(400);
            }
        }
    };
}