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


                template.Text = xDocument.Root.Element("template").Value;
                List<Dictionary<string, string>> items = xDocument
                    .Root
                    .Element("items")
                    .Elements("item")
                    .Select(x => x.Attributes().ToDictionary(k => k.Name.LocalName, v => v.Value))
                    .ToList();

                var names = items
                    .SelectMany(x => x.Keys)
                    .Distinct()
                    .ToList();

                foreach (var name in names)
                {
                    this.items.Columns.Add(name, 75);
                }

                var totalWidth = this.items.Columns.Cast<ColumnHeader>().Sum(x => x.Width);

                var width = this.items.Width = totalWidth;

                this.items.Columns.Add("Output", width);

                foreach (var item in items)
                {
                    ListItem listViewItem = new ListItem(item);

                    bool first = true;

                    foreach (string name in names)
                    {
                        string value = "";

                        if (item.ContainsKey(name))
                        {
                            value = item[name];
                        }

                        if (first)
                        {
                            listViewItem.Text = value;
                        }
                        else
                        {
                            listViewItem.SubItems.Add(value);
                        }
                        
                        first = false;
                    }

                    listViewItem.SubItems.Add("");

                    this.items.Items.Add(listViewItem);
                }
            }
        }

        private class ListItem : ListViewItem
        {
            public Dictionary<string, string> Attributes { get; set; }

            public ListItem(Dictionary<string, string> attributes)
            {
                Attributes = attributes;
            }
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            XDocument xDocument = new XDocument(
                new XElement("root",
                    //new XElement("data", data.Text),
                    new XElement("template", template.Text)));

            xDocument.Save(_filePath, SaveOptions.DisableFormatting);
        }

        private void template_TextChanged(object sender, EventArgs e)
        {
            Parse();
        }

        private void Parse()
        {
            expressionTree.Text = "";

            try
            {
                var node = _parser.Parse(template.Text);

                try
                {
                    var func = _builder.Build(node);

                    foreach (ListItem listItem in items.Items)
                    {
                        listItem
                            .SubItems
                            .Cast<ListViewItem.ListViewSubItem>()
                            .Last()
                            .Text = func(listItem.Attributes);
                    }
                }
                catch (Exception e)
                {
                    expressionTree.Text += Environment.NewLine + e.Message;
                }

                WriteNode(node);
            }
            catch (Exception e)
            {
                expressionTree.Text = e.Message;
            }
        }

        private void WriteNode(Node node)
        {
            WriteNode(node, 0);
        }

        private void WriteNode(Node node, int indent)
        {
            foreach (Node child in node.Children)
            {
                expressionTree.Text += Environment.NewLine +
                    new string(' ', indent * 4) +
                    child.NodeType +
                    (child.Text == "" ? "" : ":" + child.Text);

                WriteNode(child, indent + 1);
            }
        }

    }
}
