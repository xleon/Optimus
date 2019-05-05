using System;
using System.IO;
using NUnit.Framework;
using Optimus.Helpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class JsonFileTests
    {
        [Test]
        public void Object_can_be_written_and_read()
        {
            var instance = new MyClass
            {
                Number = 458,
                Text = "hello world",
                Date = DateTime.Now.AddDays(-1)
            };

            const string path = "test.json";
            JsonFile.Write(instance, path);
            var copy = JsonFile.Read<MyClass>(path);
            
            copy.Number.ShouldBe(instance.Number);
            copy.Text.ShouldBe(instance.Text);
            copy.Date.ShouldBe(instance.Date);
            
            File.Delete(path);
        }
    }

    internal class MyClass
    {
        public int Number { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}