using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
    class Nykredit
    {
        static Logger logger;
        String[] lines;
        int numberOfSupoerPortRecords;

        public Nykredit(String[] lines, Logger l)
        {
            this.lines = lines;
            logger = l;
        }

        public int Process(ref string emailBody, ref bool debugLevel, ref bool success, string fileName, string folder)
        {
            numberOfSupoerPortRecords = 0;

            logger.Write("    Nykrdit format");

            // Removing qoutes
            if (lines[0].IndexOf("\"") == 0)
            {
                for (int k = 1; k < lines.Length; k++)
                {
                    lines[k] = lines[k].Substring(1, lines[k].Length - 2);
                }
            }

            int kontoAfstemning = 0;

            if (lines.Length > 2)
            {
                for (int k = 1; k < lines.Length - 1; k++)
                {
                    string[] fields = lines[k].Split(';');
                    if (debugLevel)
                    {
                        logger.WriteFields(fields);
                    }

                    if (lines[k].IndexOf("KB;") == 0)
                    {
                        kontoAfstemning++;
                        ImpRecord impRecord = new ImpRecord(logger);
                        
                        if (fields.Length < 6)
                        {
                            emailBody += Environment.NewLine + "Nykredit KB record " + kontoAfstemning + " has too few fields";
                            logger.Write("      KB record too few fields");
                        }
                        else
                        {
                            impRecord.setAmount(fields[4]);
                            impRecord.setAccountNumber(fields[1], false, 14);

                            numberOfSupoerPortRecords++;
                            impRecord.writeKonto(fileName);
                        }
                    }
                    else
                    {
                        success = false;
                        logger.Write("      Ukendt fil format, " + lines[k]);
                    }
                }

                if (kontoAfstemning > 0) logger.Write("      Konto afstemninger : " + kontoAfstemning);
            }
            else
            {
                if (lines.Length == 2 && lines[1].IndexOf("TAIL;") == 0)
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
