using System;
using System.Windows;
using Npgsql;

namespace RKSI.EduPractice
{
    public partial class ErrorConnectingWindow : Window
    {
        public string connString { get; private set; }
        public bool Completed { get; private set; }
        private MainWindow mainWindow;
        public ErrorConnectingWindow(MainWindow wndw)
        {
            InitializeComponent();
            mainWindow = wndw;
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            connString = $"Server={tbServerIp.Text}; " +
                $"Port={tbServerPort.Text}; " +
                $"User Id={tbServerUser.Text}; " +
                $"Password={tbServerUserPassword.Text}; " +
                $"Database={tbServerUsableDb.Text}";
            NpgsqlConnection conn = new NpgsqlConnection(connString);

            try { conn.Open(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                mainWindow.ReconnectWithNewParams(connString);
                this.Visibility = Visibility.Hidden;
            }
        }

        private void BtnInsertDefaultPort_Click(object sender, RoutedEventArgs e)
        {
            tbServerPort.Text = "5432";
        }

        private void BtnUseWithoutConnection_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
