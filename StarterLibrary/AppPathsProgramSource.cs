﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;

namespace StarterLibrary
{
    [Serializable]
    [Browsable(false)]
    public class AppPathsProgramSource : AbstractProgramSource
    {
        public AppPathsProgramSource()
        {
            BonusPoints = -10;
        }

        public AppPathsProgramSource(ProgramSource source) : this()
        {
            BonusPoints = source.BonusPoints;
        }

        public override List<Program> LoadPrograms()
        {
            var list = new List<Program>();
            ReadAppPaths(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths", list);
            ReadAppPaths(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\App Paths", list); //TODO: need test more on 64-bit
            return list;
        }

        private void ReadAppPaths(string rootpath, List<Program> list)
        {
            using (var root = Registry.LocalMachine.OpenSubKey(rootpath))
            {
                if (root == null) return;
                foreach (var item in root.GetSubKeyNames())
                {
                    try
                    {
                        using (var key = root.OpenSubKey(item))
                        {
                            string path = key.GetValue("") as string;
                            if (string.IsNullOrEmpty(path)) continue;

                            // fix path like this ""\"C:\\folder\\executable.exe\"""
                            const int begin = 0;
                            int end = path.Length - 1;
                            const char quotationMark = '"';
                            if (path[begin] == quotationMark && path[end] == quotationMark)
                            {
                                path = path.Substring(begin + 1, path.Length - 2);
                            }

                            if (!File.Exists(path)) continue;
                            var entry = CreateEntry(path);
                            entry.ExecuteName = item;
                            list.Add(entry);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        public override string ToString()
        {
            return typeof(AppPathsProgramSource).Name;
        }
    }
}