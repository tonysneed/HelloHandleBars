using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;

namespace HelloHandleBars
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var classReader = File.OpenText(@"Templates\Class.hbs"))
            using (var ctorReader = File.OpenText(@"Templates\Partials\Constructors.hbs"))
            using (var importReader = File.OpenText(@"Templates\Partials\Imports.hbs"))
            using (var propertyReader = File.OpenText(@"Templates\Partials\Properties.hbs"))
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

                // Register ctor partial
                var ctorTemplate = Handlebars.Compile(ctorReader);
                Handlebars.RegisterTemplate("constructors", ctorTemplate);

                // Register import partial
                var importTemplate = Handlebars.Compile(importReader);
                Handlebars.RegisterTemplate("imports", importTemplate);

                // Register property partial
                var propertyTemplate = Handlebars.Compile(propertyReader);
                Handlebars.RegisterTemplate("properties", propertyTemplate);

                //var data = new
                //{
                //    @namespace = "HelloHandleBars",
                //    @class = "Person",
                //    imports = new[]
                //    {
                //        new { import = "System" },
                //        new { import = "System.Collections.Generic" }
                //    },
                //    properties = new[]
                //    {
                //        new { type = "string", name = "Name" },
                //        new { type = "int", name = "Age" }
                //    },
                //};

                //var data = new Dictionary<string, object>
                //{
                //    {"namespace", "HelloHandleBars"},
                //    {"class", "Person"},
                //    { "imports", new[]
                //        {
                //            new { import = "System" },
                //            new { import = "System.Collections.Generic" }
                //        }
                //    },
                //    { "properties", new[]
                //        {
                //            new { type = "string", name = "Name" },
                //            new { type = "int", name = "Age" }
                //        }
                //    },
                //};

                var imports = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> { {"import", "System"} },
                    new Dictionary<string, object> { {"import", "System.Collections.Generic" } },
                };

                var constructors = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "ctor-class", "Person"},
                        { "ctor-params", ""},
                        { "ctor-content", @"Console.WriteLine(""ctor""); "},
                    },
                    new Dictionary<string, object>
                    {
                        { "ctor-class", "Person"},
                        { "ctor-params", "string name"},
                        { "ctor-content", "Name = name;"},
                    },
                };

                var properties = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        { "type", "string"},
                        { "name", "Name"},
                    },
                    new Dictionary<string, object>
                    {
                        { "type", "int"},
                        { "name", "Age"},
                    },
                };

                var data = new Dictionary<string, object>
                {
                    { "namespace", "HelloHandleBars" },
                    { "class", "Person" },
                    { "imports", imports },
                    { "constructors", constructors },
                    { "properties", properties },
                };

                var classTemplate = Handlebars.Compile(classReader);
                classTemplate(Console.Out, data);
                Console.WriteLine();
                //classTemplate(classWriter, data);
            }
        }
    }
}
