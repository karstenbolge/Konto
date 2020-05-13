using System;
using System.Collections.Generic;
using System.Globalization;

namespace Converter
{
    class BankData
    {
        static Logger logger;
        String[] lines;
        int numberOfSupoerPortRecords;
        FondCode fondCode;
        List<string> annuls;

        private bool isAnul(string anul)
        {
            return annuls.Find(x => x.CompareTo(anul) == 0) != null;
        }

        public BankData(String[] lines, ref FondCode fondCode, Logger l)
        {
            this.lines = lines;
            this.fondCode = fondCode;
            logger = l;
            annuls = new List<string>();
        }

        public int Process(ref string emailBody, ref bool debugLevel, ref bool success, string fileName, string folder)
        {
            numberOfSupoerPortRecords = 0;

            logger.Write("    Jyske Bank format");

            if (lines.Length > 1)
            {
                int kontoAfsteminger = 0;

                for (int k = 1; k < lines.Length; k++)
                {
                    string[] fields = lines[k].Split(';');
                    if (debugLevel)
                    {
                        logger.WriteFields(fields);
                    }

                    kontoAfsteminger++;
                    ImpRecord impRecord = new ImpRecord(logger);

                    impRecord.setAmount(fields[6].Replace("\"", string.Empty).Trim());
                    // take last 14 digits
                    impRecord.setAccountNumber(fields[2].Replace("\"", string.Empty).Trim(), false, 14);

                    numberOfSupoerPortRecords++;
                    impRecord.writeKonto(fileName);
                }

                if (kontoAfsteminger > 0) logger.Write("      Konto Afsteminger : " + kontoAfsteminger);
            }

            return numberOfSupoerPortRecords;
        }
    }
}
