using System.ComponentModel;
using System.Activities;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Shortcuts
{
    public class LogMessage : CodeActivity
    { 
        [Category("Input/Output")]
        [RequiredArgument]
        [Description("Takes in and outputs a string variable")]
        public InOutArgument<string> Message { get; set; }

        [Category("Input")]
        [Description("String to be added to the next line of the log message")]
        public InArgument<string> Add { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var message = Message.Get(context);
            var add = Add.Get(context);
            var dummy = message + add + System.Environment.NewLine;
            Message.Set(context, dummy);
        }
    }

    public class RemoveDatatableEmptyRows : CodeActivity
    {
        [Category("Input/Output")]
        [RequiredArgument]
        [Description("Input and output datatable")]
        public InOutArgument<DataTable> Input { get; set; }

        private DataTable _table;

        protected override void Execute(CodeActivityContext context)
        {
            _table = Input.Get(context);
            _table = _table.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is System.DBNull ||
                         string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
            Input.Set(context, _table);
        }
    }

    public class ConvertDatatableToString : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Input datatable")]
        public InArgument<DataTable> Input { get; set; }

        [Category("Output")]
        [RequiredArgument]
        [Description("Output string")]
        public OutArgument<string> Result { get; set; }
        private DataTable _table;

        protected override void Execute(CodeActivityContext context)
        {
            _table = Input.Get(context);
            string res = string.Join(System.Environment.NewLine, _table.Rows.OfType<DataRow>().Select(x => string.Join(" ; ", x.ItemArray)));
            Result.Set(context, res);
        }
    }

    public class SortDatatable : CodeActivity
    {
        public enum ddEnum
        {
            Ascending,
            Descending
        }

        [Category("Input")]
        [RequiredArgument]
        [Description("Input datatable")]
        public InArgument<DataTable> Input { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Input column name (string)")]
        public InArgument<string> ColumnName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Input order")]
        public ddEnum InputOrder { get; set; }

        [Category("Output")]
        [RequiredArgument]
        [Description("Output Datatable")]
        public OutArgument<DataTable> Result { get; set; }
        private DataTable _table;
        private string order;

        protected override void Execute(CodeActivityContext context)
        {
            if (InputOrder == ddEnum.Ascending)
            {
                order = " ASC";
            }
            else
            {
                order = " DESC";
            }
            _table = Input.Get(context);
            var name = ColumnName.Get(context);
            DataView view = _table.DefaultView;
            view.Sort = name.ToString() + order;
            DataTable res = view.ToTable();
            Result.Set(context, res);
        }
    }

    public class RemoveDatatableRowByValue : CodeActivity
    {
        
        [Category("Input/Output")]
        [RequiredArgument]
        [Description("Input and output datatable")]
        public InOutArgument<DataTable> Input { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Input column name (string)")]
        public InArgument<string> ColumnName { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("Input value to be removed (string)")]
        public InArgument<string> Value { get; set; }

        private DataTable _table;

        protected override void Execute(CodeActivityContext context)
        {
            _table = Input.Get(context);
            var check = ColumnName.Get(context);
            var value = Value.Get(context);
            List<DataRow> rowsToDelete = new List<DataRow>();
            foreach (DataRow row in _table.Rows)
            {
                if (value.Contains(row[check].ToString()))
                {
                    rowsToDelete.Add(row);
                }
            }

            foreach (DataRow row in rowsToDelete)
            {
                _table.Rows.Remove(row);
            }
            _table.AcceptChanges();
            Input.Set(context, _table);
        }
    }

    public class ConvertArrayToList : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Input array")]
        public InArgument<Array> Input { get; set; }

        [Category("Output")]
        [RequiredArgument]
        [Description("Output list")]
        public OutArgument<List<Object>> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var newArray = Input.Get(context);
            List<object> res = newArray.Cast<Object>().ToList();
            Result.Set(context, res);
        }
    }

    public class ConvertListToArray : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        [Description("Input list")]
        public InArgument<List<Object>> Input { get; set; }

        [Category("Output")]
        [RequiredArgument]
        [Description("Output array")]
        public OutArgument<Array> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var newList = Input.Get(context);
            var res = newList.ToArray();
            Result.Set(context, res);
        }
    }
}
