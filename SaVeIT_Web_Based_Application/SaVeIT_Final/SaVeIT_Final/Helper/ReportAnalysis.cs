
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SaVeIT_Final.Models;
using SaVeIT_Final.ViewModel;

namespace SaVeIT_Final.Helpers
{



    public class ReportAnalysis
    {

        public void Analysis(byte[] pdf, ProjectVM report)
        {


            string line = "";
            bool stuFound = false, supervisorFound = false;
            int byIndex = 0, supervisorIndex = 0;
            PdfReader reader = new PdfReader(pdf);
            //report.Studuents = new List<Student>();
            report.Users = new List<User>();

            //Extract project information from 1st page.
            string txt = PdfTextExtractor.GetTextFromPage(reader, 1, new LocationTextExtractionStrategy());
            //Extract Abstract within page range from 2 to 10
            Abstract(reader, report);

            //split the extracted text to get line by line
            string[] words = txt.Split('\n');
            for (int j = 0, len = words.Length; j < len; j++)
            {
                line = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
            }

            for (int j = 1, len = words.Length; j < len; j++)
            {
                words[j] = words[j].Trim();
                if (words[j].Equals("by", StringComparison.OrdinalIgnoreCase) ||
                    words[j].Equals("by:", StringComparison.OrdinalIgnoreCase) ||
                    words[j].Equals("by :", StringComparison.OrdinalIgnoreCase))
                {
                    // check and return true if student list exist
                    stuFound = true;
                    // return the index of start list
                    byIndex = j;
                    break;
                }
            }

            if (stuFound == true)
            {
                for (int j = byIndex + 1, len = words.Length; j < len; j++)
                {
                    words[j] = words[j].Trim();
                    //exit from the loop if students list terminated
                    if (words[j].StartsWith("supervised", StringComparison.OrdinalIgnoreCase))
                    {
                        supervisorFound = true;
                        supervisorIndex = j + 1;
                        j = len + 1;
                        break;
                    }
                    // > 10 just check if the line contains id and name
                    string expr = String.Join(" ", words[j].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    string[] studentInfoLine = expr.Split(' ');
                    for (int i = 0; i < studentInfoLine.Length; i++)
                    {
                        if (studentInfoLine[i].Length > 2 && studentInfoLine[i].Any(char.IsDigit))
                        {
                            string temp = "";
                            bool emailExist = false; int id = 0; string email = ""; string name = "";
                            foreach (string info in studentInfoLine)
                            {
                                emailExist = false;
                                email = "";
                                temp = studentInfoLine[studentInfoLine.Length - 1];
                                if (IsValidEmail(info))
                                {
                                    emailExist = true;
                                    email = info;
                                }

                                if (emailExist == false && Regex.IsMatch(info, @"[0123456789]") == true)
                                {
                                    bool t = Int32.TryParse(info, out id);

                                }
                                if (id == 0 && email == "")
                                {
                                    name += info + " ";
                                }
                            }
                            //Student stu = new Student();
                            User stu = new User();
                            stu.userId = id + "";
                            stu.userName = name;
                            stu.userEmail = email;
                            stu.userRole = 3;
                            report.Users.Add(stu);

                        }
                    }
                }

            }

            int SupervisorId = 0;
            int indexOfSupName = 0;
            if (supervisorFound == true)
            {
                for (int j = supervisorIndex, len = words.Length; j < len; j++)
                {
                    words[j] = words[j].Trim();

                    if (words[j].StartsWith("dr", StringComparison.OrdinalIgnoreCase))
                    {
                        indexOfSupName = 2;
                        if (words[j].StartsWith("dr.", StringComparison.OrdinalIgnoreCase))
                        {
                            indexOfSupName = 3;


                        }

                        /* char[] arr = words[j].ToCharArray();
                         arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))));
                         words[j] = new string(arr);*/
                        string expr = String.Join(" ", words[j].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                        string[] supervisorLine = expr.Split(' ');

                        if (Regex.IsMatch(supervisorLine[supervisorLine.Length - 1], @"[0123456789]") == true)
                        {

                            Int32.TryParse(supervisorLine[supervisorLine.Length - 1], out SupervisorId);
                            words[j] = words[j].Substring(indexOfSupName);
                            words[j] = words[j].Substring(0, words[j].IndexOf((SupervisorId + ""))).Trim(); ;
                            report.supervisor = new User();

                            report.supervisor.userName = words[j];
                            report.supervisor.userId = (SupervisorId + "");
                            report.supervisor.userRole = 2;
                            j = len + 1;
                            break;
                        }
                        else
                        {
                            report.supervisor = new User();
                            report.supervisor.userName = words[j].Substring(indexOfSupName).Trim();
                            report.supervisor.userId = SupervisorId + "";
                            j = len + 1;
                            break;
                        }


                    }
                }
            }
            else
            {
                report.supervisor = new User();
                report.supervisor.userName = "failed to extract it";
                report.supervisor.userId = (0 + "");

            }
        }
        public void Abstract(PdfReader reader, ProjectVM report)
        {
            string line;
            string txt = "";
            //Extract text from pdf pages.
            for (int i = 1; i < 10; i++)
            {
                txt += PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());

            }
            //split the extracted text to get line by line
            string[] words = txt.Split('\n');
            string extractedAbstract = "";
            int count = 0;
            for (int j = 0, len = words.Length; j < len; j++)
            {
                line = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
            }
            for (int j = 0, len = words.Length; j < len; j++)
            {
                if (Contains(words[j].Trim(), "Abstract", StringComparison.OrdinalIgnoreCase))
                {
                    // extractedAbstract += j;

                    for (int i = j + 1; i < len; i++)
                    {

                        if (Contains(words[i], "TABLE OF CONTENTS", StringComparison.OrdinalIgnoreCase))
                        {
                            j = len - 1;
                            count++;
                            break;
                        }
                        extractedAbstract += words[i];

                    }
                }

            }
            report.SPAbstract = extractedAbstract + "\n ";
            //  report.Abstract = extractedAbstract + "\n ";
        }
        public static string GetStringBetween(string token, string first, string second)
        {
            if (!token.Contains(first)) return "";

            var afterFirst = token.Split(new[] { first }, StringSplitOptions.None)[1];

            if (!afterFirst.Contains(second)) return "";

            var result = afterFirst.Split(new[] { second }, StringSplitOptions.None)[0];

            return result;
        }
        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
   && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }





    }

}