using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BTheDJ
{
    class BMSUtil
    {
        public static int ExtHexToInt(string hex) // 10진수 -> 36진수 변환기
        {
            String sample = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int r = 0;
            for (int i = 0; i < hex.Length; i++)
            {
                r *= 36; // 시작은 0이기 때문에 곱해질 일이 없다.
                for (int j = 0; j < sample.Length; j++)
                {
                    if (hex.Substring(i, 1).CompareTo(sample.Substring(j, 1)) == 0)
                    {
                        r += j;
                        continue;
                    }
                }
            }
            return r;
        }

        public static string IntToExtHex(int Int) // 36진수 -> 10진수 변환기
        {
            // 검증된 진수 변환기. 정확도 100%
            string[] sample = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string str = "";
            // 둘 째 자리 수 계산
            int a = Int / 36;
            if (a == 0)
                // 35 이하 인 경우
                str += "0";
            else if (a > 0)
                // 36 이상 인 경우
                str += SingleIntToExtHex(a);
            // 한 자리 수 계산
            a = Int % 36;
            str += SingleIntToExtHex(a);
            return str;
        }

        static string SingleIntToExtHex(int Int)
        {
            // 한 자리 수 정수를 36진수로 바꿔주는 메소드.
            string[] sample = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string str = "";
            int a = Int % 36;
            // 10 이상인 경우
            if (a > 9)
                // 샘플 인덱스는 0부터 시작이기 때문
                str += sample[(a - 9) - 1];
            else
                // 그 외 1~9
                str += a.ToString();
            return str;
        }
    }
}
