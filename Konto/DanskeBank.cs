﻿using System;
using System.Globalization;

// Beware danske bank is using decimal point, hence culrura varial en-US
namespace Converter
{
    public class DanskeBank
    {
        static Logger logger;
        String[] lines;
        int numberOfSupoerPortRecords;

        public DanskeBank(String[] lines, Logger l)
        {
            this.lines = lines;
            logger = l;
        }

        public String splitTransactioNumber(String t)
        {
            String lastPart = t.Split(' ')[t.Split(' ').Length - 1];
            String[] datePart = lastPart.Split('-');
            String dotPart = lastPart.Split('.')[lastPart.Split('.').Length - 1];

            if (datePart.Length < 2)
            {
                return t;
            }

            if (dotPart.Length < 1)
            {
                return t;
            }

            return datePart[0] + datePart[1] + datePart[2].Substring(0, 2) + dotPart;
        }

        public int Process(ref string emailBody, ref bool debugLevel, ref bool success, string fileName)
        {
            numberOfSupoerPortRecords = 0;

            logger.Write("    Danske Bank format");

            int kontoAfstemninger = 0;

            if (lines.Length > 2)
            {
                for (int k = 1; k < lines.Length - 1; k++)
                {
                    string[] fields = lines[k].Split((char)31);
                    if (debugLevel)
                    {
                        logger.WriteFields(fields);
                    }

                    kontoAfstemninger++;
                    ImpRecord impRecord = new ImpRecord(logger);

                    if (fields.Length < 5)
                    {
                        emailBody += Environment.NewLine + "Danske bank record " + kontoAfstemninger + " has too few fields";
                        logger.Write("      Record too few fields");
                    }
                    else
                    {
                        impRecord.setAmount(fields[4]);
                        impRecord.setAccountNumber(fields[2], false, 14);

                        numberOfSupoerPortRecords++;
                        impRecord.writeKonto(fileName);
                    }
                }
                if (kontoAfstemninger > 0) logger.Write("      Konto afstemninger : " + kontoAfstemninger);
            }
            else
            {
                if (lines.Length == 2 && lines[1].IndexOf("TAIL") == 0)
                {
                    logger.Write("      Filen indeholder ingen rcords");
                }
                else
                {
                    success = false;
                    logger.Write("      Ukendt fil format");
                }
            }

            return numberOfSupoerPortRecords;
        }
    }
}
