﻿using System;
using System.Linq;
using NUnit.Framework;
using V3.Parsing.Core;

namespace V3.DomainDef.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Test()
        {
            Parser parser = new Parser();

            var domainDef = @"domain CrossFitJST
entity App.Entity
      Id byte
    , Name string(50)
    , Plural string(52)

entity App.MenuItem
      Id short
    , ParentId MenuItem null
    , Url string(50)
    , Text string(30)
    , DisplayIndex short

entity App.TaskType enum
      Id byte readonly
    , Name string(50) unique
    , SelectedId Selected
    [1, 'Add', 15]
    [2, 'Edit', 2]
    [3, 'Delete', 14]
    [4, 'Activate', 14]
    [5, 'Deactivate', 14]
    [6, 'MoveUp', 2]
    [7, 'MoveDown', 2]
    [255, 'Custom', 14]

entity App.Selected enum
      Id byte readonly
    , Name string(20)
    [0, 'Unspecified']
    [1, 'Zero = 1']
    [2, 'One = 2']
    [4, 'Two = 4']
    [8, 'More = 8']
    [12, 'TwoOrMore']
    [14, 'OneOrMore']
    [15, 'Any']

entity App.Task
      Id byte readonly
    , EntityId Entity
    , Name string(50) unique
    , TaskTypeId TaskType
    , Text string(50) unique
    , SelectedIds byte null

entity App.Page
      Id short
    , Name string(50) unique
    , Text string(50)

entity App.Form
      Id short
    , EntityId Entity
    , PageId Page
    , Name string(50) unique
    , Text string(50)

entity App.Field
      Id short
    , FormId Form
    , Name string(50)
    , Text string(50)
    , DisplayOrder short

entity App.Grid
      Id short
    , EntityId Entity
    , PageId Page
    , Name string(50) unique
    , Text string(50)

entity App.Column
      Id short
    , GridId Grid
    , Name string(50)
    , Text string(50)
    , SortOrder short
    , DisplayOrder short

entity Logging.LogItemType enum
      Id byte readonly
    , Name string(50) unique
    [1, 'Debug']
    [2, 'Info']
    [3, 'Warning']
    [4, 'Error']
    [5, 'Exception']

entity Logging.LogItem
      Id int
    , LogItemTypeId LogItemType
    , Message string
    , StackTrace string
    , DateTime datetime(s)
    , AccountId Account null
    procs
      GetIds
    , Insert
    , DeleteMany
    tasks
      Delete

entity Security.Role
      Id byte auto
    , Code string(4, 20) unique
    , Name string(4, 50) unique
    procs
      SelectMany
    , Select (Id)
    , Select (Code)
    , Select (Name)

entity Security.RoleMenuItem
      Id int
    , RoleId Role
    , MenuItemId MenuItem
    procs
      Insert
    , DeleteMany

entity Security.RoleTask
      Id short auto
    , RoleId Role
    , MenuItemId MenuItem
    procs
      Insert

entity Security.Account
      Id int
    , Name string(5, 30) unique
    , Forenames string(2, 50)
    , Surname string(2, 30)
    , PreferredName string(2, 30)
    , Email string(100) null unique
    , Password string(8, 20)
    , PasswordHash string(1000) null
    , Salt string(200) null
    , ChangePassword bool
    , ResetKey string(100) null
    , IsActive bool
    procs
      GetIds
    , Insert
    , Update
    , Select (Id)
    , Select (Name)
    , Select (Email)
    , SelectMany
    , DeleteMany
    , Activate
    , Deactivate
    tasks
      Add
    , Edit
    , Delete
    , Activate
    , Deactivate
    , EmailPasswordReset 'Send Reset Password Email' oneOrMore

entity AccountRole
      Id short auto
    , AccountId Account
    , RoleId Role
    indexes
      unique (AccountId, RoleId)
    procs
      SelectMany (AccountId)
    , Insert
    , DeleteMany";

            var node = parser.Parse(domainDef);

            var nodeString = NodeToString(node);

            var domain = new Domain(node);

            var actual = domain.ToString();

            Assert.That(actual, Is.EqualTo(domainDef));
        }

        private string NodeToString(Node<NodeType> node)
        {
            string ret = "";

            NodeToString(node, ref ret, 0);

            return ret;
        }

        private void NodeToString(Node<NodeType> node, ref string ret, int indent)
        {
            string str = node.Nodes.Any(x => x.Nodes.Count > 0)
                ? Environment.NewLine + new string(' ', indent * 2)
                : " ";

            str = Environment.NewLine + new string(' ', indent * 2);

            ret += str + node.NodeType + (node.Text == "" ? "" : " : " + node.Text);

            foreach (Node<NodeType> child in node.Nodes)
            {
                NodeToString(child, ref ret, indent + 1);
            }
        }
    }

}
