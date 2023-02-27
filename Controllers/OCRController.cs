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
                // ���͒l�̊m�F
                // null,empty,base64�łȂ��Ƃ��G���[�𓊂���
                //Base64Client.Isbase64String(param.uploadedFile);
                //StreamReader sr = new StreamReader("test.txt");

                //string text = sr.ReadToEnd();
                //base64�������C���X�^���X����

                //RAS��؂�Ȃ��Ǝ��s�ł��Ȃ����ߒ���

                Base64Client base64image = new(param.uploadedFile);
                
                // VisionAPI�ɓ�����
                Image image = Image.FromBytes(base64image.Bytes);
                // ImageAnnotatorClient�̃C���X�^���X��������json�L�[�̊m�F
                ImageAnnotatorClient client = ImageAnnotatorClient.Create();
                IReadOnlyList<EntityAnnotation> textAnnotations = client.DetectText(image);

                //TODO
                //������F���ł��Ȃ������ꍇ�AtextAnnotations���̂���?
                //textAnnotations[0]���G���[�ɂȂ���

                // BS���[���ɓK�����Ă��邩�Ȃǂ̃`�F�b�N
                Rb_Body rb_body = new Rb_Body(textAnnotations[0].Description);                
                PostProceser.Run(rb_body);
                
                // OCR���ʂ̃e�L�X�g��JSON�`���ɕϊ�
                ResponseBody responsebody = new ResponseBody(rb_body);

                return new Response(200, JsonConvert.SerializeObject(responsebody));                    
            }
            catch (AnnotateImageException e)
            {
                // VisionAPI�̃G���[
                // �G���[���O��f���o��
                //AnnotateImageResponse response = e.Response;
                //string errorMessage = e.Response.Error.Message;
                //Debug.WriteLine(errorMessage);
                return StatusCode(500); 

            }
            catch (ArgumentNullException e)
            {
                // POST���N�G�X�g��uploadFile��null�̏ꍇ
                return StatusCode(400);
            }
            catch (Exception e)
            {
                //�@��L�ȊO�̃G���[
                return StatusCode(400);
            }
        }
    };
}