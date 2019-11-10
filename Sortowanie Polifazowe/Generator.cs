using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sortowanie_Polifazowe
{
    class Generator
    {
        DiskFunctionality df;
        Random random;
        public Generator(DiskFunctionality diskFunctionality)
        {
            random = new Random();
            df = diskFunctionality;
        }

        public void Generate(int amount, int max)
        {
            int record;
            df.OpenSaveStream("MainFile.txt");
            for (int i = 0; i < amount; i++)
            {
                record = random.Next(1, max);
                df.SaveRecord(record, "MainFile.txt");
            }
            df.FlushRecordBuffer("MainFile.txt");
            df.CloseStream("MainFile.txt");
            df.ZeroWriteOp();
        }


        public int GenerateFromKeyboard()
        {
            Console.WriteLine("Write numbers. 0 to exit");
            int nrOfRecords = 0;
            df.OpenSaveStream("MainFile.txt");
            int record;
            while (true)
            {
                record = Int32.Parse(Console.ReadLine());
                if (record == 0)
                    break;
                nrOfRecords++;
                df.SaveRecord(record, "MainFile.txt");
            }
            df.FlushRecordBuffer("MainFile.txt");
            df.CloseStream("MainFile.txt");
            df.ZeroWriteOp();
            return nrOfRecords;
        }


        public int GenerateFromFile()
        {
            int nrOfRecords = 0;
            df.OpenSaveStream("MainFile.txt");
            int record;
            Console.WriteLine("Set path to read from");
            string path = Console.ReadLine();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    record = Int32.Parse(sr.ReadLine());
                    if (record < 1)
                        break;
                    nrOfRecords++;
                    df.SaveRecord(record, "MainFile.txt");
                }
            }
            df.FlushRecordBuffer("MainFile.txt");
            df.CloseStream("MainFile.txt");
            df.ZeroWriteOp();
            return nrOfRecords;
        }

        ~Generator()
        {
            random = null;
        }
    }
}
