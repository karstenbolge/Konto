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
            return annuls.Find(x => x.CompareTo( anul) == 0) != null;
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
                int ub = 0;
                int kr = 0;
                int ks = 0;
                int ksAnuls = 0;

                for (int k = 1; k < lines.Length; k++)
                {
                    string[] fields = lines[k].Split(';');

                    if (fields[0].CompareTo("KS") == 0)
                    {
                        if (fields[5].CompareTo("95") == 0)
                        {
                            annuls.Add(fields[6]);
                        }
                    }
                }

                for (int k = 1; k < lines.Length; k++)
                {
                    string[] fields = lines[k].Split(';');
                    if (debugLevel)
                    {
                        logger.WriteFields(fields);
                    }

                    if (fields[0].CompareTo("KS") == 0)
                    {
                        ImpRecord impRecord = new ImpRecord(logger);

                        // impRecord.setTransactionNumber(fields[4]); use SuperPorts
                        impRecord.setAmount(fields[14]);
                        // take last 14 digits
                        impRecord.setAccountNumber(fields[2], false, 14);
                    }
                }

                if (ub > 0) logger.Write("      Udbytte : " + ub);
                if (kr > 0) logger.Write("      Kupon : " + kr);
                if (ks > 0) logger.Write("      Køb Salg : " + ks);
                if (ksAnuls > 0) logger.Write("      Køb Salg, annuleret : " + ksAnuls);
            }

            return numberOfSupoerPortRecords;
        }
    }
}
