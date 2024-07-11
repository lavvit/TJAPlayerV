using System.Text;

namespace SeaDrop
{
    public class Text
    {
        public static List<string> Read(string path)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return list;

            using (StreamReader sr = new StreamReader(path, GetEncoding(path)))
            {
                list.AddRange(sr.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }
            return list;
        }
        public static void Save(List<string> list, string path, string encode = "utf-8", bool append = false)
        {
            using (StreamWriter sw = new StreamWriter(path, append, Encoding.GetEncoding(encode)))
            {
                foreach (var line in list)
                {
                    sw.WriteLine(line);
                }
            }
        }


        /// <summary>
        /// エンコードを読み込みます。
        /// </summary>
        /// <param name="path">ファイル名</param>
        public static Encoding GetEncoding(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // memo: Shift-JISを扱うためのおまじない
            var encode = GetJpEncoding(path);
            if (encode == null) encode = Encoding.GetEncoding("shift_jis");
            return encode;
        }
        #region Encoding
        public static Encoding? GetJpEncoding(string file, long maxSize = 50 * 1024)//ファイルパス、最大読み取りバイト数
        {
            try
            {
                if (!File.Exists(file))//ファイルが存在しない場合
                {
                    return null;
                }
                else if (new FileInfo(file).Length == 0)//ファイルサイズが0の場合
                {
                    return null;
                }
                else//ファイルが存在しファイルサイズが0でない場合
                {
                    //バイナリ読み込み
                    byte[]? bytes = null;
                    bool readAll = false;
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        long size = fs.Length;

                        if (size <= maxSize)
                        {
                            bytes = new byte[size];
                            fs.Read(bytes, 0, (int)size);
                            readAll = true;
                        }
                        else
                        {
                            bytes = new byte[maxSize];
                            fs.Read(bytes, 0, (int)maxSize);
                        }
                    }

                    //判定
                    return GetJpEncoding(bytes, readAll);
                }
            }
            catch
            {
                return null;
            }
        }
        public static Encoding? GetJpEncoding(byte[] bytes, bool readAll = false)
        {
            //BOM判定
            var enc = checkBOM(bytes);
            if (enc != null) return enc;

            //簡易ISO-2022-JP判定
            if (checkISO_2022_JP(bytes)) return Encoding.GetEncoding(50220);//iso-2022-jp

            //簡易文字コード判定(再変換確認)
            Encoding enc_sjis = Encoding.GetEncoding(932);//ShiftJIS
            Encoding enc_euc = Encoding.GetEncoding(51932);//EUC-JP
            Encoding enc_utf8_check = new UTF8Encoding(false, false);//utf8
            Encoding enc_utf8 = new UTF8Encoding(false, true);//utf8

            int sjis = checkReconversion(bytes, enc_sjis);
            int euc = checkReconversion(bytes, enc_euc);
            int utf8 = checkReconversion(bytes, enc_utf8_check);

            //末尾以外は同一の場合は同一とみなす
            if (utf8 >= bytes.Length - 3 && !readAll) utf8 = -1;
            if (sjis >= bytes.Length - 1 && !readAll) sjis = -1;
            if (euc >= bytes.Length - 1 && !readAll) euc = -1;

            //同一のものが1つもない場合
            if (sjis >= 0 && utf8 >= 0 && euc >= 0) return null;

            //再変換で同一のものが1個だけの場合
            if (sjis < 0 && utf8 >= 0 && euc >= 0) return enc_sjis;
            if (utf8 < 0 && sjis >= 0 && euc >= 0) return enc_utf8;
            if (euc < 0 && utf8 >= 0 && sjis >= 0) return enc_euc;

            //同一のものが複数ある場合は日本語らしさ判定
            double like_sjis = likeJapanese_ShiftJIS(bytes);
            double like_euc = likeJapanese_EUC_JP(bytes);
            double like_utf8 = likeJapanese_UTF8(bytes);

            if (utf8 < 0 && sjis < 0 && euc < 0)
            {
                if (like_utf8 >= like_sjis && like_utf8 >= like_euc) return enc_utf8;
                if (like_sjis >= like_euc) return enc_sjis;
                return enc_euc;
            }
            else if (utf8 < 0 && sjis < 0)
            {
                return like_utf8 >= like_sjis ? enc_utf8 : enc_sjis;
            }
            else if (utf8 < 0 && euc < 0)
            {
                return like_utf8 >= like_euc ? enc_utf8 : enc_euc;
            }
            else if (euc < 0 && sjis < 0)
            {
                return like_sjis >= like_euc ? enc_sjis : enc_euc;
            }

            return null;
        }

        //BOM判定
        private static Encoding? checkBOM(byte[] bytes)
        {
            if (bytes.Length >= 2)
            {
                if (bytes[0] == 0xfe && bytes[1] == 0xff)//UTF-16BE
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (bytes[0] == 0xff && bytes[1] == 0xfe)//UTF-16LE
                {
                    return Encoding.Unicode;
                }
            }
            if (bytes.Length >= 3)
            {
                if (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)//UTF-8
                {
                    return new UTF8Encoding(true, true);
                }
                /*else if (bytes[0] == 0x2b && bytes[1] == 0x2f && bytes[2] == 0x76)//UTF-7
                {
                    return Encoding.UTF7;
                }*/
            }
            if (bytes.Length >= 4)
            {
                if (bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xfe && bytes[3] == 0xff)//UTF-32BE
                {
                    return new UTF32Encoding(true, true);
                }
                if (bytes[0] == 0xff && bytes[1] == 0xfe && bytes[2] == 0x00 && bytes[3] == 0x00)//UTF-32LE
                {
                    return new UTF32Encoding(false, true);
                }
            }

            return null;
        }

        //簡易ISO-2022-JP判定
        private static bool checkISO_2022_JP(byte[] bytes)
        {
            string str = BitConverter.ToString(bytes);

            if (str.Contains("1B-24-40")
            || str.Contains("1B-24-42")
            || str.Contains("1B-26-40-1B-24-42")
            || str.Contains("1B-24-28-44")
            || str.Contains("1B-24-28-4F")
            || str.Contains("1B-24-28-51")
            || str.Contains("1B-24-28-50")
            || str.Contains("1B-28-4A")
            || str.Contains("1B-28-49")
            || str.Contains("1B-28-42")
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //簡易文字コード判定
        //バイトを文字に変換し再度バイト変換したとき同一かどうか
        private static int checkReconversion(byte[] bytes, Encoding enc)
        {
            try
            {
                //文字列に変換
                string str = enc.GetString(bytes);

                //バイトに再変換
                byte[] rebytes = enc.GetBytes(str);

                if (BitConverter.ToString(bytes) == BitConverter.ToString(rebytes))
                {
                    return -1;//同一
                }
                else
                {
                    int len = bytes.Length <= rebytes.Length ? rebytes.Length : bytes.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (bytes[i] != rebytes[i])
                        {
                            return i == 0 ? 0 : i - 1;//一致バイト数
                        }
                    }
                }
            }
            catch
            {
                ;
            }

            return 0;
        }

        //簡易日本語らしさ判定
        //日本語の文章と仮定したときShiftJISらしいか
        private static double likeJapanese_ShiftJIS(byte[] bytes)
        {
            int counter = 0;
            bool judgeSecondByte = false; //次回の判定がShiftJISの2バイト目の判定かどうか

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (!judgeSecondByte)
                {
                    if ((b == 0x0D  //CR
                    || b == 0x0A //LF
                    || b == 0x09 //tab
                    || (0x20 <= b && b <= 0x7E)))//半角カナ除く1バイト
                    {
                        counter++;
                    }
                    else if ((0x81 <= b && b <= 0x9F) || (0xE0 <= b && b <= 0xFC))//ShiftJISの2バイト文字の1バイト目の場合
                    {
                        //2バイト目の判定を行う
                        judgeSecondByte = true;
                    }
                    else if (0xA1 <= b && b <= 0xDF)//ShiftJISの1バイト文字の場合(半角カナ)
                    {
                        ;
                    }
                    else if (0x00 <= b && b <= 0x7F)//ShiftJISの1バイト文字の場合
                    {
                        ;
                    }
                    else
                    {
                        //ShiftJISでない
                        return 0;
                    }
                }
                else
                {
                    if ((0x40 <= b && b <= 0x7E) || (0x80 <= b && b <= 0xFC)) //ShiftJISの2バイト文字の2バイト目の場合
                    {
                        counter += 2;
                        judgeSecondByte = false;
                    }
                    else
                    {
                        //ShiftJISでない
                        return 0;
                    }
                }
            }

            return (double)counter / (double)bytes.Length;
        }

        //日本語の文章と仮定したときUTF-8らしいか
        private static double likeJapanese_UTF8(byte[] bytes)
        {
            string str = BitConverter.ToString(bytes) + "-";
            int len = str.Length;

            //日本語らしいものを削除

            //制御文字
            str = str.Replace("0D-", "");//CR
            str = str.Replace("0A-", "");//LF
            str = str.Replace("09-", "");//tab

            //英数字記号
            for (byte b = 0x20; b <= 0x7E; b++)
            {
                str = str.Replace(string.Format("{0:X2}-", b), "");
            }

            //ひらがなカタカナ
            for (byte b1 = 0x81; b1 <= 0x83; b1++)
            {
                for (byte b2 = 0x80; b2 <= 0xBF; b2++)
                {
                    str = str.Replace(string.Format("E3-{0:X2}-{1:X2}-", b1, b2), "");
                }
            }

            //常用漢字
            for (byte b1 = 0x80; b1 <= 0xBF; b1++)
            {
                for (byte b2 = 0x80; b2 <= 0xBF; b2++)
                {
                    str = str.Replace(string.Format("E4-{0:X2}-{1:X2}-", b1, b2), "");
                    str = str.Replace(string.Format("E5-{0:X2}-{1:X2}-", b1, b2), "");
                    str = str.Replace(string.Format("E6-{0:X2}-{1:X2}-", b1, b2), "");
                    str = str.Replace(string.Format("E7-{0:X2}-{1:X2}-", b1, b2), "");
                    str = str.Replace(string.Format("E8-{0:X2}-{1:X2}-", b1, b2), "");
                    str = str.Replace(string.Format("E9-{0:X2}-{1:X2}-", b1, b2), "");
                }
            }

            return ((double)len - (double)str.Length) / (double)len;
        }

        //日本語の文章と仮定したときEUC-JPらしいか
        private static double likeJapanese_EUC_JP(byte[] bytes)
        {
            string str = BitConverter.ToString(bytes) + "-";
            int len = str.Length;

            //日本語らしいものを削除

            //制御文字
            str = str.Replace("0D-", "");//CR
            str = str.Replace("0A-", "");//LF
            str = str.Replace("09-", "");//tab

            //英数字記号
            for (byte b = 0x20; b <= 0x7E; b++)
            {
                str = str.Replace(string.Format("{0:X2}-", b), "");
            }

            //ひらがなカタカナ記号
            for (byte b1 = 0xA1; b1 <= 0xA5; b1++)
            {
                for (byte b2 = 0xA1; b2 <= 0xFE; b2++)
                {
                    str = str.Replace(string.Format("{0:X2}-{1:X2}-", b1, b2), "");
                }
            }

            //常用漢字
            for (byte b1 = 0xB0; b1 <= 0xEE; b1++)
            {
                for (byte b2 = 0xA1; b2 <= 0xFE; b2++)
                {
                    str = str.Replace(string.Format("{0:X2}-{1:X2}-", b1, b2), "");
                }
            }

            return ((double)len - (double)str.Length) / (double)len;
        }
        #endregion
    }
}