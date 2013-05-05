using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace TestCaseSourceExtension.UI
{
    public partial class ParameterizedTestResultViewer : UserControl, ITestTypeExtensionResultViewer
    {
        public ParameterizedTestResultViewer()
        {
            InitializeComponent();

            resultsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            resultsGrid.RowHeadersVisible = false;
            resultsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            resultsGrid.Dock = DockStyle.Bottom;
            MessageBox.Show("Extension UI ctor");
        }

        public void Initialize(TestResult result)
        {
            var innards = (result as ITestResultExtension).ExtensionResult as string;
            ResultContent.Text = innards;
            //using (var reader = new CsvReader(new StringReader(innards)))
            //{
            //    reader.ReadHeaderRecord();
            //    foreach (var testCaseResult in reader.DataRecords)
            //    {
            //        resultsGrid.Rows.Add(testCaseResult.Values);
            //    }
            //}
        }
    }
}
