﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2016 Jerry Lee
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using Excel;
using QuickUnity.Core.Miscs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace QuickUnityEditor.Data
{
    /// <summary>
    /// Handle the process of data tables import.
    /// </summary>
    public static class DataImport
    {
        /// <summary>
        /// Struct DataTableRowInfo.
        /// </summary>
        private struct DataTableRowInfo
        {
            /// <summary>
            /// The field name.
            /// </summary>
            public string fieldName;

            /// <summary>
            /// The type string.
            /// </summary>
            public string type;

            /// <summary>
            /// The comments.
            /// </summary>
            public string comments;

            /// <summary>
            /// Initializes a new instance of the <see cref="DataTableRowInfo"/> struct.
            /// </summary>
            /// <param name="fieldName">Name of the field.</param>
            /// <param name="dataType">Type of the data.</param>
            /// <param name="comments">The comments.</param>
            public DataTableRowInfo(string fieldName, string type, string comments)
            {
                this.fieldName = fieldName;
                this.type = type;
                this.comments = comments;
            }
        }

        /// <summary>
        /// The specifiers collection of script template.
        /// </summary>
        private static class ScriptTemplateSpecifiers
        {
            /// <summary>
            /// The specifier of namespace.
            /// </summary>
            public const string NamespaceSpecifier = "#NAMESPACE#";

            /// <summary>
            /// The specifier of script name.
            /// </summary>
            public const string ScriptNameSpecifier = "#SCRIPTNAME#";

            /// <summary>
            /// The specifier of fields.
            /// </summary>
            public const string FieldsSpecifier = "#FIELDS#";
        }

        /// <summary>
        /// The messages collection of dialog.
        /// </summary>
        private static class DialogMessages
        {
            /// <summary>
            /// The message content of dataTableRowScriptsStorageLocation is null or empty.
            /// </summary>
            public const string DataTableRowScriptsStorageLocationNullMessage = "Please set the storage location of DataTableRow scripts first: [QuickUnity/DataTable/Preferences...]";

            /// <summary>
            /// The message of the content of template is null or empty.
            /// </summary>
            public const string CanNotFindTplFileMessage = "Can not find the template for DataTableRow scripts. Please make sure you already import it into the project!";
        }

        /// <summary>
        /// The file name of DataTableRow script template.
        /// </summary>
        private const string DataTableRowScriptTemplateFileName = "NewDataTableRowScript";

        /// <summary>
        /// The extension of excel file.
        /// </summary>
        private const string ExcelFileExtension = ".xls";

        /// <summary>
        /// The extension of script file.
        /// </summary>
        private const string ScriptFileExtension = ".cs";

        /// <summary>
        /// The search patterns of excel files.
        /// </summary>
        private static readonly string s_excelFileSearchPatterns = string.Format("*{0}", ExcelFileExtension);

        /// <summary>
        /// Imports data.
        /// </summary>
        [MenuItem("QuickUnity/DataTable/Import Data...", false, 100)]
        [MenuItem("Assets/Import Data...", false, 20)]
        public static void Import()
        {
            if (CheckPreferencesData())
            {
                string filesFolderPath = EditorUtility.OpenFolderPanel("Import Data...", "", "");

                if (!string.IsNullOrEmpty(filesFolderPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(filesFolderPath);
                    FileInfo[] fileInfos = dirInfo.GetFiles(s_excelFileSearchPatterns, SearchOption.AllDirectories);
                    string tplText = GetTplText();
                    string namespaceString = GetNamespace();

                    for (int i = 0, length = fileInfos.Length; i < length; ++i)
                    {
                        FileInfo fileInfo = fileInfos[i];

                        if (fileInfo != null)
                        {
                            string filePath = fileInfo.FullName;
                            string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                            string fileExtension = Path.GetExtension(fileInfo.Name).ToLower();

                            try
                            {
                                FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                                IExcelDataReader excelReader = null;

                                if (fileExtension == ExcelFileExtension)
                                {
                                    // '97-2003 format; *.xls
                                    excelReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                                }
                                else
                                {
                                    // 2007 format; *.xlsx
                                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                                }

                                if (excelReader != null)
                                {
                                    DataSet result = excelReader.AsDataSet();
                                    DataTable table = result.Tables[0];
                                    List<DataTableRowInfo> rowInfos = GenerateDataTableRowInfos(table);
                                    GenerateDataTableRowScript((string)tplText.Clone(), namespaceString, fileName, rowInfos);
                                    fileStream.Close();
                                    fileStream = null;
                                }
                            }
                            catch (Exception exception)
                            {
                                DebugLogger.LogException(exception);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the preferences data.
        /// </summary>
        /// <returns><c>true</c> if preferences data is ready, <c>false</c> otherwise.</returns>
        private static bool CheckPreferencesData()
        {
            bool success = true;

            // Load preferences data or create default preferences data.
            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (!preferencesData)
            {
                preferencesData = DataTablePreferencesWindow.CreateDefaultPreferencesData();
                DataTablePreferencesWindow.SavePreferenceData(preferencesData);
            }

            // Check preferences data is safe.
            if (string.IsNullOrEmpty(preferencesData.dataTableRowScriptsStorageLocation))
            {
                QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.DataTableRowScriptsStorageLocationNullMessage, () =>
                {
                    success = false;
                    DataTablePreferencesWindow.ShowEditorWindow();
                });
            }

            return success;
        }

        /// <summary>
        /// Gets the text content of template.
        /// </summary>
        /// <returns>System.String. The text content of template.</returns>
        private static string GetTplText()
        {
            string[] assetPaths = Utilities.EditorUtility.GetAssetPath(DataTableRowScriptTemplateFileName, "t:TextAsset");
            string tplPath = null;
            string tplText = null;

            if (assetPaths != null && assetPaths.Length > 0)
            {
                tplPath = assetPaths[0];
            }

            if (!string.IsNullOrEmpty(tplPath))
            {
                TextAsset tplAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(tplPath);

                if (tplAsset)
                {
                    tplText = tplAsset.text;
                }
            }
            else
            {
                QuickUnityEditorApplication.DisplaySimpleDialog("", DialogMessages.CanNotFindTplFileMessage);
            }

            return tplText;
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <returns>System.String. The namespace of scripts.</returns>
        private static string GetNamespace()
        {
            string namespaceString = string.Empty;

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                if (preferencesData.autoGenerateScriptsNamespace)
                {
                    // Generate namespace automatically.
                    namespaceString = GenerateNamespace(preferencesData);
                }
                else
                {
                    namespaceString = preferencesData.dataTableRowScriptsNamespace;
                }
            }

            return namespaceString;
        }

        /// <summary>
        /// Generates the namespace.
        /// </summary>
        /// <param name="preferencesData">The preferences data.</param>
        /// <returns>System.String. The generated namespace string.</returns>
        private static string GenerateNamespace(DataTablePreferences preferencesData)
        {
            string namespaceString = string.Empty;

            if (preferencesData)
            {
                string path = preferencesData.dataTableRowScriptsStorageLocation;
                path = path.Replace(Path.AltDirectorySeparatorChar, '.');
                int index = path.IndexOf(QuickUnityEditorApplication.ScriptsFolderName);

                if (index != -1)
                {
                    if (index + QuickUnityEditorApplication.ScriptsFolderName.Length + 1 <= path.Length)
                    {
                        namespaceString = path.Substring(index + QuickUnityEditorApplication.ScriptsFolderName.Length + 1);
                    }
                }
                else
                {
                    index = path.IndexOf(QuickUnityEditorApplication.AssetsFolderName);

                    if (index != -1)
                    {
                        if (index + QuickUnityEditorApplication.AssetsFolderName.Length + 1 <= path.Length)
                        {
                            namespaceString = path.Substring(index + QuickUnityEditorApplication.AssetsFolderName.Length + 1);
                        }
                    }
                    else
                    {
                        namespaceString = path.Replace(":", "");
                    }
                }
            }

            return namespaceString;
        }

        /// <summary>
        /// Generates the list of DataTableRowInfo.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>List&lt;DataTableRowInfo&gt; The list of DataTableRowInfo.</returns>
        private static List<DataTableRowInfo> GenerateDataTableRowInfos(DataTable dataTable)
        {
            DataColumnCollection columns = dataTable.Columns;
            DataRowCollection rows = dataTable.Rows;
            int columnCount = columns.Count;
            List<DataTableRowInfo> infos = new List<DataTableRowInfo>();

            for (int i = 0; i < columnCount; i++)
            {
                string fieldName = rows[0][i].ToString().Trim();
                string type = rows[1][i].ToString().Trim();
                string comments = rows[2][i].ToString().Trim();

                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(type))
                {
                    DataTableRowInfo info = new DataTableRowInfo(fieldName, type, FormatCommentsString(comments));
                    infos.Add(info);
                }
            }

            return infos;
        }

        /// <summary>
        /// Generates the script of DataTableRow.
        /// </summary>
        /// <param name="tplText">The script template text.</param>
        /// <param name="namesapceString">The string of namesapce.</param>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="rowInfos">The list of DataTableRowInfo.</param>
        private static void GenerateDataTableRowScript(string tplText, string namesapceString, string scriptName, List<DataTableRowInfo> rowInfos)
        {
            tplText = tplText.Replace(ScriptTemplateSpecifiers.NamespaceSpecifier, namesapceString);
            tplText = tplText.Replace(ScriptTemplateSpecifiers.ScriptNameSpecifier, scriptName);
            tplText = tplText.Replace(ScriptTemplateSpecifiers.FieldsSpecifier, GenerateScriptFieldsString(rowInfos));

            DataTablePreferences preferencesData = DataTablePreferencesWindow.LoadPreferencesData();

            if (preferencesData)
            {
                string scriptFilePath = Path.Combine(preferencesData.dataTableRowScriptsStorageLocation, scriptName + ScriptFileExtension);
                UnityEngine.Object scriptAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scriptFilePath);

                if (scriptAsset)
                {
                    EditorUtility.SetDirty(scriptAsset);
                }

                try
                {
                    FileStream fileStream = File.Open(scriptFilePath, FileMode.Create, FileAccess.Write);
                    StreamWriter writer = new StreamWriter(fileStream, new UTF8Encoding(true));
                    writer.Write(tplText);
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception exception)
                {
                    DebugLogger.LogException(exception);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                Utilities.EditorUtility.ClearConsole();
            }
        }

        /// <summary>
        /// Generates the script fields string.
        /// </summary>
        /// <param name="rowInfos">The row infos.</param>
        /// <returns>System.String The fields string.</returns>
        private static string GenerateScriptFieldsString(List<DataTableRowInfo> rowInfos)
        {
            string fieldsString = string.Empty;

            if (rowInfos != null && rowInfos.Count > 0)
            {
                for (int i = 0, length = rowInfos.Count; i < length; ++i)
                {
                    DataTableRowInfo rowInfo = rowInfos[i];
                    fieldsString += string.Format("\t\t/// <summary>{0}\t\t/// {1}{2}\t\t/// </summary>{3}\t\tpublic {4} {5};",
                        Environment.NewLine,
                        rowInfo.comments,
                        Environment.NewLine,
                        Environment.NewLine,
                        rowInfo.type,
                        rowInfo.fieldName);

                    if (i < length - 1)
                        fieldsString += Environment.NewLine + Environment.NewLine;
                }
            }

            return fieldsString;
        }

        /// <summary>
        /// Format the comments string.
        /// </summary>
        /// <param name="comments">The comments.</param>
        /// <returns>The formatted comments string.</returns>
        private static string FormatCommentsString(string comments)
        {
            if (!string.IsNullOrEmpty(comments))
            {
                const string pattern = @"\r*\n";
                Regex rgx = new Regex(pattern);
                return rgx.Replace(comments, Environment.NewLine + "\t\t/// ");
            }

            return comments;
        }
    }
}