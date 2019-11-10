using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Sortowanie_Polifazowe
{
    class DiskFunctionality
    {
        byte[] mainFileBuff;
        int mainFileBuffSize = 0;
        int mainFileBuffOffset = 0;
        byte[] tape1Buff;
        int tape1BuffSize = 0;
        int tape1BuffOffset = 0;
        byte[] tape2Buff;
        int tape2BuffSize = 0;
        int tape2BuffOffset = 0;
        byte[] tape3Buff;
        int tape3BuffSize = 0;
        int tape3BuffOffset = 0;
        int DISK_PAGE_SIZE;
        byte[] intToByteArray;
        string main, tape1, tape2, tape3;
        byte[] tempBuff;
        int TEMP_BUFF_INDEX = 4;

        int nrOfReadOp = 0, nrOfWriteOp = 0;
        FileStream mainStream, tape1Stream, tape2Stream, tape3Stream;

        public DiskFunctionality(int disk_page_size, string main, string tape1, string tape2, string tape3)
        {
            DISK_PAGE_SIZE = disk_page_size;
            mainFileBuff = new byte[DISK_PAGE_SIZE];
            tape1Buff = new byte[DISK_PAGE_SIZE];
            tape2Buff = new byte[DISK_PAGE_SIZE];
            tape3Buff = new byte[DISK_PAGE_SIZE];
            tempBuff = new byte[DISK_PAGE_SIZE];
            if (File.Exists("MainFile.txt"))
                File.Delete("MainFile.txt");
            if (File.Exists("tape1.txt"))
                File.Delete("tape1.txt");
            if (File.Exists("tape2.txt"))
                File.Delete("tape2.txt");
            if (File.Exists("tape3.txt"))
                File.Delete("tape3.txt");
            this.main = main;
            this.tape1 = tape1;
            this.tape2 = tape2;
            this.tape3 = tape3;
        }

        public void ZeroWriteOp()
        {
            nrOfWriteOp = 0;
        }
        public void OpenReadStream(string path)
        {
            if (path == main)
            {
                mainStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                mainFileBuffOffset = 0;
                mainFileBuffSize = 0;
            }
            else if (path == tape1)
            {
                tape1Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                tape1BuffOffset = 0;
                tape1BuffSize = 0;
            }
            else if (path == tape2)
            {
                tape2Stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                tape2BuffOffset = 0;
                tape2BuffSize = 0;
            }
            else if (path == tape3)
            {
                tape3Stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
                tape3BuffOffset = 0;
                tape3BuffSize = 0;
            }
        }

        public void OpenSaveStream(string path)
        {
            if (path == main)
            {
                mainStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                mainFileBuffOffset = 0;
                mainFileBuffSize = DISK_PAGE_SIZE;
            }
            else if (path == tape1)
            {
                tape1Stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                tape1BuffOffset = 0;
                tape1BuffSize = DISK_PAGE_SIZE;
            }
            else if (path == tape2)
            {
                tape2Stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                tape2BuffOffset = 0;
                tape2BuffSize = DISK_PAGE_SIZE;
            }
            else if (path == tape3)
            {
                tape3Stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                tape3BuffOffset = 0;
                tape3BuffSize = DISK_PAGE_SIZE;
            }
        }

        public void CloseStream(string path)
        {
            if (path == main)
                mainStream.Close();
            else if (path == tape1)
                tape1Stream.Close();
            else if (path == tape2)
                tape2Stream.Close();
            else if (path == tape3)
                tape3Stream.Close();
        }


        public int ReadRecord(string fileName)
        {
            int record;
            int bytesRead;
            if (fileName == main)
            {
                if (mainFileBuffOffset == mainFileBuffSize)
                {
                    bytesRead = mainStream.Read(mainFileBuff, 0, mainFileBuff.Length);
                    mainFileBuffSize = bytesRead;
                    mainFileBuffOffset = 0;
                    if (bytesRead == 0)
                        return -1;
                    nrOfReadOp++;
                }
                record = BitConverter.ToInt32(mainFileBuff, mainFileBuffOffset);
                mainFileBuffOffset += 4;
                return record;
            }
            else if (fileName == tape1)
            {
                if (tape1BuffOffset == tape1BuffSize)
                {
                    bytesRead = tape1Stream.Read(tape1Buff, 0, tape1Buff.Length);
                    tape1BuffSize = bytesRead;
                    tape1BuffOffset = 0;
                    if (bytesRead == 0)
                        return -1;
                    nrOfReadOp++;
                }
                record = BitConverter.ToInt32(tape1Buff, tape1BuffOffset);
                tape1BuffOffset += 4;
                return record;
            }
            else if (fileName == tape2)
            {
                if (tape2BuffOffset == tape2BuffSize)
                {
                    bytesRead = tape2Stream.Read(tape2Buff, 0, tape2Buff.Length);
                    tape2BuffSize = bytesRead;
                    tape2BuffOffset = 0;
                    if (bytesRead == 0)
                        return -1;
                    nrOfReadOp++;
                }
                record = BitConverter.ToInt32(tape2Buff, tape2BuffOffset);
                tape2BuffOffset += 4;
                return record;
            }
            else
            {
                if (tape3BuffOffset == tape3BuffSize)
                {
                    bytesRead = tape3Stream.Read(tape3Buff, 0, tape3Buff.Length);
                    tape3BuffSize = bytesRead;
                    tape3BuffOffset = 0;
                    if (bytesRead == 0)
                        return -1;
                    nrOfReadOp++;
                }
                record = BitConverter.ToInt32(tape3Buff, tape3BuffOffset);
                tape3BuffOffset += 4;
                return record;
            }
        }

        public void SaveRecord(int record, string fileName)
        {
            if (fileName == main)
            {
                if (mainFileBuffOffset == DISK_PAGE_SIZE)
                {
                    mainStream.Write(mainFileBuff, 0, mainFileBuff.Length);
                    nrOfWriteOp++;
                    mainFileBuffOffset = 0;
                }
                intToByteArray = BitConverter.GetBytes(record);
                for (int i = 0; i < 4; i++)
                {
                    mainFileBuff[mainFileBuffOffset] = intToByteArray[i];
                    mainFileBuffOffset++;
                }
            }
            else if (fileName == tape1)
            {
                if (tape1BuffOffset == DISK_PAGE_SIZE)
                {
                    tape1Stream.Write(tape1Buff, 0, tape1Buff.Length);
                    nrOfWriteOp++;
                    tape1BuffOffset = 0;
                }
                intToByteArray = BitConverter.GetBytes(record);
                for (int i = 0; i < 4; i++)
                {
                    tape1Buff[tape1BuffOffset] = intToByteArray[i];
                    tape1BuffOffset++;
                }
            }
            else if (fileName == tape2)
            {
                if (tape2BuffOffset == DISK_PAGE_SIZE)
                {
                    tape2Stream.Write(tape2Buff, 0, tape2Buff.Length);
                    nrOfWriteOp++;
                    tape2BuffOffset = 0;
                }
                intToByteArray = BitConverter.GetBytes(record);
                for (int i = 0; i < 4; i++)
                {
                    tape2Buff[tape2BuffOffset] = intToByteArray[i];
                    tape2BuffOffset++;
                }
            }
            else
            {
                if (tape3BuffOffset == DISK_PAGE_SIZE)
                {
                    tape3Stream.Write(tape3Buff, 0, tape3Buff.Length);
                    nrOfWriteOp++;
                    tape3BuffOffset = 0;
                }
                intToByteArray = BitConverter.GetBytes(record);
                for (int i = 0; i < 4; i++)
                {
                    tape3Buff[tape3BuffOffset] = intToByteArray[i];
                    tape3BuffOffset++;
                }
            }
        }

        public void FlushRecordBuffer(string fileName)
        {
            if (fileName == main)
            {
                if (mainFileBuffOffset != 0)
                {
                    mainStream.Write(mainFileBuff, 0, mainFileBuffOffset);
                    nrOfWriteOp++;
                }
            }
            else if (fileName == tape1)
            {
                if (tape1BuffOffset != 0)
                {
                    tape1Stream.Write(tape1Buff, 0, tape1BuffOffset);
                    nrOfWriteOp++;
                }
            }
            else if (fileName == tape2)
            {
                if (tape2BuffOffset != 0)
                {
                    tape2Stream.Write(tape2Buff, 0, tape2BuffOffset);
                    nrOfWriteOp++;
                }
            }
            else
            {
                if (tape3BuffOffset != 0)
                {
                    tape3Stream.Write(tape3Buff, 0, tape3BuffOffset);
                    nrOfWriteOp++;
                }
            }
        }


        public void ShowFileContent(string fileName, bool show)
        {
            int readOp = nrOfReadOp;
            long position;
            int record, buffSize, buffOffset;
            if (fileName == main)
            {
                position = mainStream.Position;
                buffSize = mainFileBuffSize;
                buffOffset = tape1BuffOffset;
                copy(0, TEMP_BUFF_INDEX, buffOffset, buffSize);
            }
            else if (fileName == tape1)
            {
                position = tape1Stream.Position;
                buffSize = tape1BuffSize;
                buffOffset = tape1BuffOffset;
                copy(1, TEMP_BUFF_INDEX, buffOffset, buffSize);
            }
            else if (fileName == tape2)
            {
                position = tape2Stream.Position;
                buffSize = tape2BuffSize;
                buffOffset = tape2BuffOffset;
                copy(2, TEMP_BUFF_INDEX, buffOffset, buffSize);
            }
            else
            {
                position = tape3Stream.Position;
                buffSize = tape3BuffSize;
                buffOffset = tape3BuffOffset;
                copy(3, TEMP_BUFF_INDEX, buffOffset, buffSize);
            }
            while (true)
            {
                record = ReadRecord(fileName);
                if (record == -1)
                    break;
                else if (show == true)
                    Console.WriteLine(record + "    Sum of divosors: " + Program.GetSumOfDivisors(record));
                else
                    Console.WriteLine(record);
            }
            if (fileName == main)
            {
                mainStream.Position = position;
                mainFileBuffOffset = buffOffset;
                mainFileBuffSize = buffSize;
                copy(TEMP_BUFF_INDEX, 0, buffOffset, buffSize);
            }
            else if (fileName == tape1)
            {
                tape1Stream.Position = position;
                tape1BuffOffset = buffOffset;
                tape1BuffSize = buffSize;
                copy(TEMP_BUFF_INDEX, 1, buffOffset, buffSize);
            }
            else if (fileName == tape2)
            {
                tape2Stream.Position = position;
                tape2BuffOffset = buffOffset;
                tape2BuffSize = buffSize;
                copy(TEMP_BUFF_INDEX, 2, buffOffset, buffSize);
            }
            else
            {
                tape3Stream.Position = position;
                tape3BuffOffset = buffOffset;
                tape3BuffSize = buffSize;
                copy(TEMP_BUFF_INDEX, 3, buffOffset, buffSize);
            }
            nrOfReadOp = readOp;
        }


        public void copy(int s, int d, int offset, int size)
        {
            byte[] source;
            byte[] destination;
            switch (s)
            {
                case 0:
                    source = mainFileBuff;
                    break;
                case 1:
                    source = tape1Buff;
                    break;
                case 2:
                    source = tape2Buff;
                    break;
                case 3:
                    source = tape3Buff;
                    break;
                default:
                    source = tempBuff;
                    break;
            }
            switch (d)
            {
                case 0:
                    destination = mainFileBuff;
                    break;
                case 1:
                    destination = tape1Buff;
                    break;
                case 2:
                    destination = tape2Buff;
                    break;
                case 3:
                    destination = tape3Buff;
                    break;
                default:
                    destination = tempBuff;
                    break;
            }
            for (int i = offset; i < size; i++)
                destination[i] = source[i];
        }


        public void ReturnRecord(string fileName)
        {
            if (fileName == main)
                mainFileBuffOffset -= 4;
            else if (fileName == tape1)
                tape1BuffOffset -= 4;
            else if (fileName == tape2)
                tape2BuffOffset -= 4;
            else if (fileName == tape3)
                tape3BuffOffset -= 4;
        }

        public int GetNumberOfReadOperations()
        {
            return nrOfReadOp;
        }

        public int GetNumberOfWriteOperations()
        {
            return nrOfWriteOp;
        }


        public int GetNumberOfRuns(string path)
        {
            int readOp = nrOfReadOp;
            int numberOfRuns = 1;
            long position;
            if (path == main)
                position = mainStream.Position;
            else if (path == tape1)
                position = tape1Stream.Position;
            else if (path == tape2)
                position = tape2Stream.Position;
            else
                position = tape3Stream.Position;
            int previousRecord = -1, record = ReadRecord(path);
            while (record != -1)
            {
                if (!Program.Compare(previousRecord, record))
                    numberOfRuns++;
                previousRecord = record;
                record = ReadRecord(path);
            }
            if (path == main)
            {
                mainStream.Position = position;
                mainFileBuffOffset = mainFileBuffSize;
            }
            else if (path == tape1)
            {
                tape1Stream.Position = position;
                tape1BuffOffset = tape1BuffSize;
            }
            else if (path == tape2)
            {
                tape2Stream.Position = position;
                tape2BuffOffset = tape2BuffSize;
            }
            else
            {
                tape3Stream.Position = position;
                tape2BuffOffset = tape2BuffSize;
            }
            nrOfReadOp = readOp;
            return numberOfRuns;
        }

        ~DiskFunctionality()
        {
            mainFileBuff = null;
            tape1Buff = null;
            tape2Buff = null;
        }
    }
}
