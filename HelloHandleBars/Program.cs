using System;
using System.IO;
using HandlebarsDotNet;

namespace HelloHandleBars
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var classReader = File.OpenText(@"Templates\Class.hbs"))
            using (var importReader = File.OpenText(@"Templates\Partials\Import.hbs"))
            using (var propertyReader = File.OpenText(@"Templates\Partials\Property.hbs"))
            using (var classWriter = File.CreateText("Person.cs"))
            {
                // Register tab helper
                Handlebars.RegisterHelper("spaces", (writer, context, parameters) =>
                {
                    var spaces = string.Empty;
                    if (parameters.Length > 0
                        && parameters[0] is string param
                        && int.TryParse(param, out int count))
                    {
                        for (int i = 0; i < count; i++)
                            spaces += " ";
                    }
                    writer.Write(spaces);
                });

                // Register import partial
                var importTemplate = Handlebars.Compile(importReader);
                Handlebars.RegisterTemplate("import", importTemplate);

                // Register property partial
                var propertyTemplate = Handlebars.Compile(propertyReader);
                Handlebars.RegisterTemplate("property", propertyTemplate);

                var data = new
                {
                    @namespace = "HelloHandleBars",
                    @class = "Person",
                    imports = new[]
                    {
                        new { import = "System" },
                        new { import = "System.Collections.Generic" }
                    },
                    properties = new[]
                    {
                        new { type = "string", name = "Name" },
                        new { type = "int", name = "Age" }
                    },
                };

                var classTemplate = Handlebars.Compile(classReader);
                classTemplate(Console.Out, data);
                Console.WriteLine();
                classTemplate(classWriter, data);
            }
        }
    }
}
