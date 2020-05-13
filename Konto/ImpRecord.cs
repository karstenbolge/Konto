using System;
using System.IO;

namespace Converter
{
    public class ImpRecord
    {
        const string FILE_NAME = "PfKonto.Imp";
        Logger logger;

        String accountNumber;//Konto nummer
        DecimalNumber amount;//Stk

        const String spaces = "                                            ";

        public ImpRecord(Logger l)
        {
            logger = l;

            accountNumber = string.Empty;
            amount = new DecimalNumber(12, 2, true, true, logger);//Stk
        }


        public void setAccountNumber(String s, bool removePaddingZero = false, int lastXdigits = 0)
        {
            s = s.TrimEnd(' ').TrimStart(' ');
            if (lastXdigits > 0 && s.Length > lastXdigits)
            {
                s = s.Substring(s.Length - lastXdigits);
            }

            if (removePaddingZero)
            {
                while(s.Length > 0 && s[0] == '0')
                {
                    s = s.Substring(1);
                }
            }

            if (s.Length > 14)
            {
                while(s[0] == '0' && s.Length > 14)
                {
                    s = s.Substring(1, s.Length - 1);
                }
                if (s.Length > 14)
                {
                    logger.Write("AccountNumber : " + s + " too long!");
                }
            }

            accountNumber = s.Substring(0, s.Length > 14 ? 14 : s.Length);
        }

        public String getAccountNumber()
        {
            return spaces.Substring(0, 14 - accountNumber.Length) + accountNumber;
        }

        public void blankAmount()
        {
            amount.setBlank();
        }

        public void setAmount(String s)
        {
            amount.setDecimalNumber(s);
        }

        public String getAmount()
        {
            return amount.getDecimalNumber();
        }

        public void writeKonto(String path)
        {
            using (StreamWriter w = File.AppendText(path + "\\" + FILE_NAME))
            {
                w.WriteLine("{0}{1}{2}{3}{4}{5}",
                    (char)0x1F,
                    (char)0x1F,
                    (char)0x1F,
                    getAccountNumber(),
                    (char)0x1F,
                    getAmount());
            }
        }

        public void writeHead(String path, String date)
        {
            using (StreamWriter w = File.AppendText(path + "\\" + FILE_NAME))
            {
                w.WriteLine("HEAD{0}{1}{2}{3}    ",
                    (char) 0x1F,
                    date.Substring(6, 4),
                    date.Substring(3, 2),
                    date.Substring(0, 2));
            }
        }

        public void writeTail(String path, int numberOfRecords)
        {
            using (StreamWriter w = File.AppendText(path + "\\" + FILE_NAME))
            {
                w.WriteLine("TAIL{0}{1}        ",
                    (char) 0x1F,
                    numberOfRecords.ToString("000000"));
            }
        }

        public void createEmptyFile(String path)
        {
            File.Create(path + "\\" + FILE_NAME).Dispose();
        }
    }
}
