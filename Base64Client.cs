using System.Diagnostics;
using System.Text;

namespace OCRAPI_W511.Base64
{
    public class Base64Client
    {
        public string Base64 { get; set; }
        public byte[] Bytes { get; set; }

        public Base64Client(string base64)
        {
            this.Base64 = base64;
            this.Bytes = Convert.FromBase64String(base64);
        }

        //
        // base64か確認
        //
        public static void Isbase64String(string base64String)
        {
            // nullか空白ならエラーを投げる
            if (string.IsNullOrEmpty(base64String)) throw new ArgumentNullException();

            // base64か判定。bse64でないならエラーを投げる。
            Span<byte> buffer = new Span<byte>(new byte[base64String.Length]);
            if (!Convert.TryFromBase64String(base64String, buffer, out _)) throw new Exception();           

        }
    }
}