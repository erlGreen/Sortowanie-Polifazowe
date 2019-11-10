using System;

namespace Sortowanie_Polifazowe
{
    class Program
    {
        static void Main(string[] args)
        {
            int DISK_PAGE_SIZE = 4096;//MULTITUDE OF 4
            string mainFileName = "MainFile.txt";
            string tape1Name = "tape1.txt";
            string tape2Name = "tape2.txt";
            string tape3Name = "tape3.txt";
            int amount;
            int max;
            bool showSum;
            int option;
            bool show;
            RunInfo ri;
            DiskFunctionality df = new DiskFunctionality(DISK_PAGE_SIZE, mainFileName, tape1Name, tape2Name, tape3Name);
            Generator generator = new Generator(df);



            Console.WriteLine("1 - Generate automatically");
            Console.WriteLine("2 - Generate from keyboard");
            Console.WriteLine("3 - Generate from file");
            option = Int32.Parse(Console.ReadLine());
            if (option == 1)
            {
                Console.WriteLine("Set amount of numbers");
                amount = Int32.Parse(Console.ReadLine());
                if (amount < 1)
                {
                    Console.WriteLine("Amount of numbers must be grater than 0");
                    return;
                }
                Console.WriteLine("Set maximum value of a number");
                max = Int32.Parse(Console.ReadLine());
                if (max < 1)
                {
                    Console.WriteLine("Maximum number must be grater than 0");
                    return;
                }
                generator.Generate(amount, max);
            }
            else if (option == 2)
            {
                amount = generator.GenerateFromKeyboard();
                if (amount < 1)
                {
                    Console.WriteLine("Amount of numbers must be grater than 0");
                    return;
                }
            }
            else if (option == 3)
            {
                amount = generator.GenerateFromFile();
                if (amount < 1)
                {
                    Console.WriteLine("Amount of numbers must be grater than 0");
                    return;
                }
            }
            Console.WriteLine("Show file after every phase of sorting? y/n");
            string phaseAsnwear = Console.ReadLine();
            if (phaseAsnwear == "y")
                show = true;
            else
                show = false;
            Console.WriteLine("Show sum of divisors next to each number? y/n");
            string showSumAnswear = Console.ReadLine();
            if (showSumAnswear == "y")
                showSum = true;
            else
                showSum = false;
            Console.WriteLine("File before sorting:");
            df.OpenReadStream(mainFileName);
            df.ShowFileContent(mainFileName, showSum);
            df.CloseStream(mainFileName);
            df.OpenReadStream(mainFileName);
            Console.WriteLine("Number of runs in file: " + df.GetNumberOfRuns(mainFileName));
            df.CloseStream(mainFileName);


            ri = DivideBetweenTapes(df, mainFileName, tape1Name, tape2Name);

            FibonacciMerge(df, ri, tape1Name, tape2Name, tape3Name, show, showSum);
            int r = df.GetNumberOfReadOperations();
            int w = df.GetNumberOfWriteOperations();
            int sum = r + w;
            Console.WriteLine("Number of disk operations: " + sum);
        }

        public static RunInfo DivideBetweenTapes(DiskFunctionality df, string main, string tape1, string tape2)
        {
            int record = -1, previousRecord;
            int whichTape = 1, tape1Runs = 0, tape2Runs = 0, whichFibonacci = 0;
            int fibbNumber = GetFibonacciNumber(whichFibonacci);
            df.OpenReadStream(main);
            df.OpenSaveStream(tape1);
            df.OpenSaveStream(tape2);
            while (true)
            {
                previousRecord = record;
                record = df.ReadRecord(main);
                if (record == -1)
                    break;
                if (whichTape == 1)
                {
                    if (Compare(previousRecord, record))
                        df.SaveRecord(record, tape1);
                    else
                    {
                        tape1Runs++;
                        if (tape1Runs == fibbNumber)
                        {
                            df.SaveRecord(record, tape2);
                            whichTape = 2;
                            whichFibonacci++;
                            fibbNumber = GetFibonacciNumber(whichFibonacci);
                        }
                        else
                            df.SaveRecord(record, tape1);
                    }
                }
                else
                {
                    if (Compare(previousRecord, record))
                        df.SaveRecord(record, tape2);
                    else
                    {
                        tape2Runs++;
                        if (tape2Runs == fibbNumber)
                        {
                            df.SaveRecord(record, tape1);
                            whichTape = 1;
                            whichFibonacci++;
                            fibbNumber = GetFibonacciNumber(whichFibonacci);
                        }
                        else
                            df.SaveRecord(record, tape2);
                    }
                }
            }
            if (whichTape == 1)// dummy runs
                tape1Runs = GetFibonacciNumber(whichFibonacci);
            else
                tape2Runs = GetFibonacciNumber(whichFibonacci);
            df.FlushRecordBuffer(tape1);
            df.FlushRecordBuffer(tape2);
            df.CloseStream(main);
            df.CloseStream(tape1);
            df.CloseStream(tape2);
            Console.WriteLine("Divide finished. Sorting...");
            return new RunInfo(tape1Runs, tape2Runs);
        }

        public static void FibonacciMerge(DiskFunctionality df, RunInfo ri, string tape1, string tape2, string tape3, bool show, bool showDiv)
        {
            int phases = 0;
            int input1Runs = ri.GetRuns(tape1), input2Runs = ri.GetRuns(tape2);
            string input1 = tape1, input2 = tape2, output = tape3;
            Console.WriteLine("Phase " + phases + " finished. Tape content:");
            if (show)
            {
                df.OpenReadStream(input1);
                df.OpenReadStream(input2);
                Console.WriteLine("\n" + input1 + " content:");
                df.ShowFileContent(input1, showDiv);
                Console.WriteLine("\n" + input2 + " content:");
                df.ShowFileContent(input2, showDiv);
                df.CloseStream(input1);
                df.CloseStream(input2);
            }
            df.OpenReadStream(input1);
            df.OpenReadStream(input2);
            df.OpenSaveStream(output);
            while (true)
            {
                if (input1Runs > input2Runs)
                {
                    for (int i = 0; i < input2Runs; i++)
                        MergeRuns(df, input1, input2, output);
                    input1Runs -= input2Runs;
                    df.FlushRecordBuffer(output);
                    df.CloseStream(output);
                    df.CloseStream(input2);
                    phases++;
                    Console.WriteLine("Phase " + phases + " finished. Tape content:");
                    if (show)
                    {
                        df.OpenReadStream(output);
                        Console.WriteLine("\n" + input1 + " content:");
                        df.ShowFileContent(input1, showDiv);
                        Console.WriteLine("\n" + output + " content:");
                        df.ShowFileContent(output, showDiv);
                        df.CloseStream(output);
                    }



                    swap(ref input2, ref output);
                    df.OpenSaveStream(output);
                    df.OpenReadStream(input2);
                }
                else if (input1Runs < input2Runs)
                {
                    for (int i = 0; i < input1Runs; i++)
                        MergeRuns(df, input1, input2, output);
                    input2Runs -= input1Runs;
                    df.FlushRecordBuffer(output);
                    df.CloseStream(output);
                    df.CloseStream(input1);
                    phases++;

                    Console.WriteLine("Phase " + phases + " finished. Tape content:");
                    if (show)
                    {
                        df.OpenReadStream(output);
                        Console.WriteLine("\n" + input2 + " content:");
                        df.ShowFileContent(input2, showDiv);
                        Console.WriteLine("\n" + output + " content:");
                        df.ShowFileContent(output, showDiv);
                        df.CloseStream(output);
                    }



                    swap(ref input1, ref output);
                    df.OpenSaveStream(output);
                    df.OpenReadStream(input1);
                }
                else if (input1Runs == 1 && input2Runs == 1)
                {
                    MergeRuns(df, input1, input2, output);
                    df.CloseStream(input1);
                    df.CloseStream(input2);
                    df.FlushRecordBuffer(output);
                    df.CloseStream(output);
                    phases++;
                    Console.WriteLine("\n" + "File after sorting:");
                    df.OpenReadStream(output);
                    df.ShowFileContent(output, true);
                    df.CloseStream(output);
                    Console.WriteLine("Number of phases: " + phases);
                    return;
                }
            }
        }


        public static void swap(ref string s1, ref string s2)
        {
            string temp = s1;
            s1 = s2;
            s2 = temp;
        }


        public static void MergeRuns(DiskFunctionality df, string in1, string in2, string outTape)  //merge 1 series from each input
        {
            int previous1Record = -1, record1 = df.ReadRecord(in1);
            int previous2Record = -1, record2 = df.ReadRecord(in2);

            while (true)
            {
                if (record1 != -1 && record2 != -1)
                {
                    if (Compare(record1, record2))
                    {
                        if (Compare(previous1Record, record1))
                        {
                            df.SaveRecord(record1, outTape);
                            previous1Record = record1;
                            record1 = df.ReadRecord(in1);
                        }
                        else
                        {
                            df.ReturnRecord(in1);
                            while (true)
                            {
                                if (Compare(previous2Record, record2))
                                {
                                    df.SaveRecord(record2, outTape);
                                    previous2Record = record2;
                                    record2 = df.ReadRecord(in2);
                                    if (record2 == -1)
                                        return;
                                }
                                else
                                {
                                    df.ReturnRecord(in2);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Compare(previous2Record, record2))
                        {
                            df.SaveRecord(record2, outTape);
                            previous2Record = record2;
                            record2 = df.ReadRecord(in2);
                        }
                        else
                        {
                            df.ReturnRecord(in2);
                            while (true)
                            {
                                if (Compare(previous1Record, record1))
                                {
                                    df.SaveRecord(record1, outTape);
                                    previous1Record = record1;
                                    record1 = df.ReadRecord(in1);
                                    if (record1 == -1)
                                        return;
                                }
                                else
                                {
                                    df.ReturnRecord(in1);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (record2 == -1)
                    {
                        while (true)
                        {
                            if (Compare(previous1Record, record1))
                            {
                                df.SaveRecord(record1, outTape);
                                previous1Record = record1;
                                record1 = df.ReadRecord(in1);
                                if (record1 == -1)
                                    return;
                            }
                            else
                            {
                                df.ReturnRecord(in1);
                                return;
                            }
                        }
                    }
                    else if (record1 == -1)
                    {
                        while (true)
                        {
                            if (Compare(previous2Record, record2))
                            {
                                df.SaveRecord(record2, outTape);
                                previous2Record = record2;
                                record2 = df.ReadRecord(in2);
                                if (record2 == -1)
                                    return;
                            }
                            else
                            {
                                df.ReturnRecord(in2);
                                return;
                            }
                        }
                    }
                    else return;
                }
            }
        }

        public static int GetFibonacciNumber(int a)
        {
            if (a == 0)
                return 1;
            else if (a == 1)
                return 1;
            else return GetFibonacciNumber(a - 1) + GetFibonacciNumber(a - 2);
        }


        public static bool Compare(int smaller, int bigger)
        {
            if (smaller == -1)
                return true;
            int s = GetSumOfDivisors(smaller);
            int b = GetSumOfDivisors(bigger);

            if (s <= b)
                return true;
            else
                return false;
        }

        public static int GetSumOfDivisors(int a)
        {
            int sum = 0;
            for (int i = 1; i <= Math.Sqrt(a); i++)
            {
                if (a % i == 0)
                {
                    if (a / i == i)
                        sum += i;
                    else
                    {
                        sum += i;
                        sum += a / i;
                    }
                }
            }
            return sum;
        }
    }
}
