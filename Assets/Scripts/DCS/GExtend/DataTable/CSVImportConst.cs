using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.DataTable
{
    public class CSVImportConst
    {
        public static string ScriptableObjectOutputPath = "Assets/Resources/DataTable/";
        public static string CSVStorePath = "Assets/Editor/DataTable/CSV/";

        public static Dictionary<string, string> CSVUrlToFileName = new Dictionary<string, string>
        {
            //{ "https://docs.google.com/spreadsheets/d/1Ehw-Yz4rbhF8JIEDBMqjKcoM4bFbwFKjkW4Ng829wPQ/edit?usp=sharing", "sample.csv" },
        };
    }
}
