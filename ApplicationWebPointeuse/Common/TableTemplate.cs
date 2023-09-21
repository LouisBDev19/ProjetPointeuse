using APIPointeuse.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Reflection;
using System.Text;
using static NuGet.Client.ManagedCodeConventions;

namespace ApplicationWebPointeuse.Common
{
    public class TableTemplate
    {
        public string Create<T>(IEnumerable<T> arrayData, string tableTitle, string tableId, Dictionary<string, string> properties, string mainId)
        {
            var tableBuilder = new StringBuilder();

            tableBuilder.Append($@"
                <h2>{tableTitle} :</h2>
                <table id=""{tableId}Table"" class=""display"">
                    <thead>
                        <tr>"
            );

            foreach (var column in properties.Values)
            {
                tableBuilder.Append($@"<th>{column}</th>"
                );
            }
            tableBuilder.Append($@"<th><input type='checkbox' id='mainCheckbox-{tableId}'></th>"
            );

            tableBuilder.Append(@"
                        </tr>
                    </thead>
                    <tbody>"
            );

            foreach (var data in arrayData)
            {
                tableBuilder.Append("<tr>");

                foreach (var propertyName in properties.Keys)
                {
                    var propertyValue = GetNestedPropertyValue(data, propertyName);

                    if (propertyValue is DateTime dateTime)
                    {
                        propertyValue = dateTime.ToShortDateString();
                    }

                    tableBuilder.Append($@"<td>{propertyValue}</td>");
                }

                var itemId = typeof(T).GetProperty(mainId);
                var checkboxItemId = itemId.GetValue(data);

                tableBuilder.Append($@"<td><input type='checkbox' id='subCheckbox-{checkboxItemId}-{tableId}'></td>");
                tableBuilder.Append(" </tr>");
            }

            tableBuilder.Append(@"
                    </tbody>
                </table>"
            );

            return tableBuilder.ToString();
        }

        private object GetNestedPropertyValue(object obj, string propertyName)
        {
            var propertyNames = propertyName.Split('.');
            var property = obj.GetType().GetProperty(propertyNames[0]);
            var propertyValue = property?.GetValue(obj);

            if (propertyNames.Length > 1 && propertyValue != null)
            {
                return GetNestedPropertyValue(propertyValue, string.Join(".", propertyNames.Skip(1)));
            }

            return propertyValue;
        }
    }
}
