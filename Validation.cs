using OCRAPI_W511.ResponseClass;
using System;
using System.Text.RegularExpressions;

namespace OCRAPI_W511.PostProcess
{
    public static class PostProceser
    {
        public static void Run(Rb_Body rb_Body)
        {
            // OCR結果を整形
            BSConvert converter = new BSConvert(rb_Body.wheel_id);
            rb_Body.wheel_id = converter.Run();

            // タイヤルールに合致しているか判定
            BSValidation validater = new BSValidation(rb_Body);
            validater.Run();

            // 画像に関するチェック
            // 画像のラプラシアン値ってなに
            // 画像のRGBを読み取るのC#だと大変そうなんだが実装するの?
            //ImageCheck checker = new ImageCheck(rb_Body);
            //checker.Run();
        }
    }

    public class BSConvert
    {
        public string id;
        public BSConvert(string id)
        {
            this.id = id;
        }

        public string Run()
        {
            RemoveSymbol();
            GetNine();
            ConvertToZeroOne();
            return this.id;
        }

        // 数字とアルファベット以外取り除く
        private void RemoveSymbol()
        {
            Regex regex = new Regex("[^a-zA-Z0-9]+");
            this.id = regex.Replace(this.id, "");
        }

        // 10文字以上なら9文字目までを返す
        // 9文字以下ならそのまま返す
        private void GetNine()
        {
            if (this.id.Length >= 10)
            {
                this.id.Substring(0, 9);
            }
        }

        // 大文字のOと小文字のoを数字の0に置き換える
        // 大文字のIと小文字のiを数字の1に置き換える
        private void ConvertToZeroOne()
        {
            this.id = Regex.Replace(this.id, "o", "0", RegexOptions.IgnoreCase);
            this.id = Regex.Replace(this.id, "i", "1", RegexOptions.IgnoreCase);
        }
    }


    public class BSValidation
    {
        public List<string> msgs;
        public string idString;
        public char[] idCharArray;

        public BSValidation(Rb_Body rb_Body)
        {
            this.msgs = rb_Body.msgs;
            this.idString = rb_Body.wheel_id;
            this.idCharArray = this.idString.ToCharArray();
        }

        public void Run()
        {
            // idStringが空または9文字でないなら即終了
            // この前の処理で10文字以上の場合は9文字に切り落としてる。
            // 問題になるのは8文字以下の場合。
            if (!IsEmpty() && CheckLen())
            {
                CheckHead();
                CheckDigit();
                CheckMonth();
                // 前の処理で記号は全て落としているので必ず英数字じゃない?
                CheckDigitOrAlphabet();
            }
        }

        // 空か確認
        private bool IsEmpty()
        {
            if (this.idString == "")
            {
                this.msgs.Add("空です");
                return true;
            }
            return false;
        }

        // 9文字か確認
        private bool CheckLen()
        {
            if (this.idString.Length == 9)
            { 
                return true;
            }

            this.msgs.Add("9文字ではない");
            return false;
        }

        // 1文字目がルールに適合しているか確認
        private void CheckHead()
        {
            if ("SFBAMKWXY".Contains(this.idCharArray[0])) return;
            this.msgs.Add("先頭がSFBAMKXYではない");
        }


        // 2,6,7,8,9文字目がルールに適合しているか確認
        private void CheckDigit()
        {

            if (!Char.IsDigit(idCharArray[1]))
            {
                this.msgs.Add(String.Format("2文字目が数字ではない"));
            }

            for (int i = 5; i < 9; i++)
            {
                if (char.IsDigit(idCharArray[i])) continue;
                this.msgs.Add(String.Format("{0}文字目が数字ではない", i + 1));
            }

        }

        // 3文字目がルールに適合しているか確認
        private void CheckMonth()
        {
            if ("SERYALNUMBJK".Contains(idCharArray[2])) return;
            this.msgs.Add("3文字がありえない月");
        }

        // 4,5文字目がルールに適合しているか確認
        private void CheckDigitOrAlphabet()
        {

            if (!(Char.IsLetter(idCharArray[3]) || Char.IsDigit(idCharArray[3])))
            {
                this.msgs.Add("4文字目が英数字ではない");
            }

            if (!(Char.IsLetter(idCharArray[4]) || Char.IsDigit(idCharArray[4])))
            {
                this.msgs.Add("5文字目が英数字ではない");
            }

        }

    }

    //public class ImageCheck
    //{
    //    public List<string> msgs;

    //    public ImageCheck(Rb_Body rb_Body)
    //    {
    //        this.msgs = rb_Body.msgs;
    //    }
    //}
}



