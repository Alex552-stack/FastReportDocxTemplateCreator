using System;
using System.Collections.Generic;
using Xceed.Words.NET;

namespace ConsoleApp1
{
    public class DocxPopulator
    {
        public static void PopulateDocxTemplate(string templatePath, string outputPath, List<Person> people)
        {
            using (var doc = DocX.Load(templatePath))
            {
                var table = doc.Tables[0]; // Assumes the first table is the target
                for (int i = 0; i < people.Count; i++)
                {
                    var row = table.InsertRow();
                    row.Cells[0].Paragraphs[0].Append(people[i].Name);
                    row.Cells[1].Paragraphs[0].Append(people[i].Age.ToString());
                }
                doc.SaveAs(outputPath);
            }
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}

