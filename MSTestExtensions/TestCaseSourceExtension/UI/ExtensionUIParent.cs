using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace TestCaseSourceExtension.UI
{
    public class ExtensionUIParent : TestTypeExtensionUI
    {
        public override Icon Icon
        {
            get { return ExtensionResources.FolderIcon; }
        }
        public override UserControl ResultExtensionViewer
        {
            get
            {
                MessageBox.Show("Parent.UI");
                return new ParameterizedTestResultViewer();
            }
        }
    }
}