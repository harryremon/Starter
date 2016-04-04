using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using StarterLibrary;

namespace Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
                PrintHelp(HelpType.All);
            switch (args.FirstOrDefault().ToLower())
            {
                #region Help
                case "help":
                case "h":
                case "?":
                case "/?":
                    PrintHelp(HelpType.All);
                    break;
                #endregion

                #region List
                case "l":
                case "list":
                    var programs = GetApplications.GetAll();
                    foreach (var programTitle in programs.Select(program => program.Title).OrderBy(title => title))
                    {
                        Console.WriteLine(programTitle);
                    }
                    break;
                #endregion

                #region Start
                case "start":
                    if (args.Count() < 2)
                        PrintHelp(HelpType.Start);
                    else
                    {
                        var programList = GetApplications.GetAll();
                        var param = args[1].ToLower();
                        var result = programList.Where(program => program.Title.ToLower().Contains(param)).ToList();
                        if (!result.Any())
                            Console.WriteLine("Application Not Found");
                        else if (result.Count() == 1)
                        {
                            var executePath = result.FirstOrDefault().ExecutePath;
                            var process = new Process { StartInfo = { FileName = executePath } };
                            process.Start();
                        }
                        else if (result.Count() > 1)
                        {
                            Console.WriteLine("Found Multiple Entries, Please choose which application to start.");
                            var index = 0;
                            foreach (var program in result)
                            {
                                Console.WriteLine($"{index}) {program.Title}");
                                index++;
                            }
                            var choosenIndexString = Console.ReadLine();
                            int choosenIndex;
                            var converted = int.TryParse(choosenIndexString, out choosenIndex);
                            if (!converted)
                                Console.WriteLine("Wrong Parameter");
                            else
                            {
                                if (choosenIndex < 0 || choosenIndex > index)
                                    Console.WriteLine("Wrong Parameter");
                                else
                                {
                                    var executePath = result[choosenIndex].ExecutePath;
                                    var process = new Process { StartInfo = { FileName = executePath } };
                                    process.Start();
                                }
                            }
                        }
                    }
                    break;
                    #endregion
            }

            #if DEBUG
            Console.ReadKey();
            #endif
        }

        static void PrintHelp(HelpType type)
        {
            switch (type)
            {
                case HelpType.All:
                    Console.WriteLine("Help Goes Here");
                    break;
                case HelpType.Start:
                    Console.WriteLine("Start Help Goes Here");
                    break;
            }
        }

        enum HelpType
        {
            All,
            Start
        }
    }
}
