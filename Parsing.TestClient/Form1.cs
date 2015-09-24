using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Parsing.TestClient
{
    public partial class Form1 : Form
    {
        private readonly string _filePath;
        private readonly Parser _parser;
        private Builder _builder;
        private IDictionary<string, string> _values;

        public Form1()
        {
            _parser = new Parser();
            _builder = new Builder();
            InitializeComponent();

            _filePath = @"c:\temp\parsing.xml";

            LoadSetting();
            Parse();
        }

        private void LoadSetting()
        {
            if (File.Exists(_filePath))
            {
                var xDocument = XDocument.Load(_filePath, LoadOptions.PreserveWhitespace);

                data.Text = xDocument.Root.Element("data").Value.Replace("\n", Environment.NewLine);
                template.Text = xDocument.Root.Element("template").Value;
            }
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            XDocument xDocument = new XDocument(
                new XElement("root",
                    new XElement("data", data.Text),
                    new XElement("template", template.Text)));

            xDocument.Save(_filePath, SaveOptions.DisableFormatting);
        }

        private void data_TextChanged(object sender, EventArgs e)
        {
            _values = new Dictionary<string, string>();

            foreach (var kvp in data.Text
                .Split('\n')
                .Select(x => new KeyValuePair<string, string>(x.Split(' ')[0], x.Split(' ')[1])))
            {
                _values.Add(kvp);
            }
        }

        private void template_TextChanged(object sender, EventArgs e)
        {
            Parse();
        }

        private void Parse()
        {
            try
            {
                var node = _parser.Parse(template.Text);
                var func = _builder.Build(node);

                WriteNode(node);
                
                result.Text += Environment.NewLine + Environment.NewLine + func(_values);
            }
            catch (Exception e)
            {
                result.Text = e.Message;
            }
        }

        private void WriteNode(Node node)
        {
            result.Text = "";
            WriteNode(node, 0);
        }

        private void WriteNode(Node node, int indent)
        {
            foreach (Node child in node.Children)
            {
                result.Text += Environment.NewLine +
                    new string(' ', indent * 4) +
                    child.NodeType +
                    (child.Text == "" ? "" : ":" + child.Text);

                WriteNode(child, indent + 1);
            }
        }

    }
}
