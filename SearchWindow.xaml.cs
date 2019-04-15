using System.Windows;
using System.Windows.Input;

namespace RKSI.EduPractice
{
    public partial class SearchWindow : Window
    {
        public SearchEventArgs eventArgs { get; }
        private MainWindow wnd;
        public SearchWindow(SearchEventArgs eArgs, MainWindow wnd)
        {
            InitializeComponent();
            eventArgs = eArgs;
            this.wnd = wnd;

            foreach (var i in eArgs.SearchColumns)
            {
                cbSearchCols.Items.Add(i);
            }
            cbSearchCols.SelectedIndex = 0;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            StartSearch();
        }

        private void TbSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Return") StartSearch();
        }

        private void StartSearch()
        {
            if (string.IsNullOrWhiteSpace(tbSearchString.Text) && string.IsNullOrEmpty(tbSearchString.Text))
            {
                MessageBox.Show("Поле поиска не может быть пустым!");
            }
            else
            {
                eventArgs.SearchString = tbSearchString.Text;
                eventArgs.SelectedCol = cbSearchCols.Items[cbSearchCols.SelectedIndex].ToString();
                wnd.SelectByFilter(eventArgs);
            }
        }
    }
}
