using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RKSI.EduPractice
{
    public partial class MainWindow : Window
    {
        #region Declarations

        private DatabaseContext db;
        private NpgsqlConnection connection;
        private List<Citizen> users;
        private int IdOfEditableUser;
        public string connectionString { get; private set; } =
            "Server=127.0.0.1; Port=5432; User Id=postgres; Password=1234; Database=Users";
        private bool ConnectedToDb = false;

        #endregion

        #region PreparingsBeforeStart

        public MainWindow()
        {
            InitializeComponent();
            users = new List<Citizen>();

            //db = new DatabaseContext("Server=127.0.0.1; Port=5432; User Id=postgres; Password=1234; Database=Users");
            db = new DatabaseContext(connectionString);
            connection = db.Connection;

            try
            {
                LoadData();

                foreach (var i in users)
                {
                    FillCitizenFromOtherTables(i);
                }

                LoadDataFromLocalList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            gbEdit.Visibility = Visibility.Hidden;
            foreach (UIElement tab in tabControlMain.Items)
            {
                Grid tabGrid = (tab as TabItem).Content as Grid;
                foreach (UIElement i in tabGrid.Children)
                {
                    if (i is DatePicker dp) dp.SelectedDate = DateTime.Now;
                }
            }
        }

        private void OnFormLoaded(object sender, RoutedEventArgs e)
        {
            Width = dgUsers.ActualWidth + 100;
            if (!ConnectedToDb)
            {
                ErrorConnectingWindow ecw = new ErrorConnectingWindow(this);
                ecw.Show();
            }
        }

        #endregion

        #region DataActions

        public void ReconnectWithNewParams(string connString)
        {
            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                db = new DatabaseContext(connString);
                connection = db.Connection;
                connection.Open();

                LoadData();

                foreach (var i in users)
                {
                    FillCitizenFromOtherTables(i);
                }

                LoadDataFromLocalList();
                ConnectedToDb = true;
            }
            finally
            {
                connection.Close();
            }
        }

        private void LoadData()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                ConnectedToDb = true;
                NpgsqlCommand selectCtz = new NpgsqlCommand("SELECT * FROM users.citizen", connection);
                NpgsqlDataReader reader = selectCtz.ExecuteReader();

                CreateCitizenFromCitizenTable(reader, true);

            }
            catch (PostgresException) { MessageBox.Show("Ошибка подключения к базе"); }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }

        public void SelectByFilter(SearchEventArgs e)
        {
            IEnumerable<Citizen> selected;
            try
            {
                switch (e.SelectedCol)
                {
                    case "Id":
                        selected = from i in users where i.Id == Convert.ToUInt32(e.SearchString) select i;
                        break;
                    case "Шифр":
                        selected = from i in users where i.Cypher == Convert.ToUInt32(e.SearchString) select i;
                        break;
                    case "Фамилия":
                        selected = from i in users where i.Surname == e.SearchString select i;
                        break;
                    case "ИНН":
                        selected = from i in users where i.Inn == Convert.ToUInt32(e.SearchString) select i;
                        break;
                    case "Место выдачи документа":
                        selected = from i in users where i.DocumentWhereIssued == e.SearchString select i;
                        break;
                    default:
                        MessageBox.Show("Пока что не поддерживается");
                        selected = null;
                        break;
                }
                if (selected != null)
                {
                    if (selected.Count() > 0)
                    {
                        dgUsers.Items.Clear();
                        foreach (var i in selected)
                        {
                            dgUsers.Items.Add(i);
                        }
                    }
                    else MessageBox.Show("Поиск не дал результатов", "Пусто", MessageBoxButton.OK, MessageBoxImage.None);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Неправильный формат ввода");
            }
        }

        public void LoadDataFromLocalList()
        {
            dgUsers.Items.Clear();
            users.Sort();
            foreach (var i in users)
            {
                dgUsers.Items.Add(i);
            }
        }

        #endregion

        #region DataFills

        private Citizen CreateCitizenFromCitizenTable(NpgsqlDataReader reader, bool addToList = false)
        {
            Citizen ctz = null;
            while (reader.Read())
            {
                ctz = new Citizen
                {
                    Id = Convert.ToUInt32(reader[0]),
                    PId = Convert.ToInt32(reader[1]),
                    DId = Convert.ToInt32(reader[2]),
                    Name = reader[3].ToString(),
                    Surname = reader[4].ToString(),
                    Patronym = reader[5].ToString()
                };
                if (addToList) users.Add(ctz);
            }
            return ctz;
        }

        private async void FillCitizenFromOtherTables(Citizen ctz)
        {
            if (ConnectedToDb)
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                NpgsqlCommand select = new NpgsqlCommand("SELECT * FROM users.document WHERE \"Id\"=" + ctz.DId, connection);
                NpgsqlDataReader readerDoc = select.ExecuteReader();
                while (await readerDoc.ReadAsync())
                {
                    ctz.DocumentName = readerDoc[0].ToString();
                    ctz.DocumentSerial = Convert.ToUInt32(readerDoc[2]);
                    ctz.DocumentWhereIssued = readerDoc[3].ToString();
                    ctz.DocumentDateIssued = DateTime.Parse(readerDoc[4].ToString());
                }
                connection.Close();

                connection.Open();
                NpgsqlCommand selectPers = new NpgsqlCommand("SELECT * FROM users.person WHERE \"Id\"=" + ctz.PId, connection);
                NpgsqlDataReader readerPers = selectPers.ExecuteReader();
                while (await readerPers.ReadAsync())
                {
                    ctz.Cypher = Convert.ToUInt32(readerPers[1]);
                    ctz.Inn = Convert.ToUInt32(readerPers[2]);
                    ctz.Type = readerPers[3].ToString();
                    ctz.Date = DateTime.Parse(readerPers[4].ToString());
                }
                connection.Close();
            }
        }

        #endregion

        #region EditRecordActions

        private void BtnFindToEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!IsStringEmpty(tbIdToFind.Text))
            {
                IdOfEditableUser = Convert.ToInt32(tbIdToFind.Text);

                IEnumerable<Citizen> foundusers = from i in users where i.Id == IdOfEditableUser select i;
                foreach (var editableUser in foundusers)
                {
                    tbEditName.Text = editableUser.Name;
                    tbEditSurname.Text = editableUser.Surname;
                    tbEditPatronym.Text = editableUser.Patronym;
                    tbEditCypher.Text = editableUser.Cypher.ToString();
                    tbEditInn.Text = editableUser.Inn.ToString();
                    tbEditType.Text = editableUser.Type;
                    tbEditWhereIssued.Text = editableUser.DocumentWhereIssued;
                    tbEditDocName.Text = editableUser.DocumentName;
                    tbEditSerial.Text = editableUser.DocumentSerial.ToString();
                    dpEditRegister.SelectedDate = editableUser.Date;
                    dpEditDocIssued.SelectedDate = editableUser.DocumentDateIssued;
                }
                gbEdit.Visibility = Visibility.Visible;
            }
            else MessageBox.Show("Пустое поле ID", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void BtnSaveAfterEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTextBoxes(EditGrid.Children))
            {
                IEnumerable<Citizen> foundusers = from i in users where i.Id == IdOfEditableUser select i;
                var editableUser = foundusers.First();
                editableUser.Name = tbEditName.Text;
                editableUser.Surname = tbEditSurname.Text;
                editableUser.Patronym = tbEditPatronym.Text;
                editableUser.Cypher = Convert.ToUInt32(tbEditCypher.Text);
                editableUser.Inn = Convert.ToUInt32(tbEditInn.Text);
                editableUser.Type = tbEditType.Text;
                editableUser.DocumentWhereIssued = tbEditWhereIssued.Text;
                editableUser.DocumentName = tbEditDocName.Text;
                editableUser.DocumentSerial = Convert.ToUInt32(tbEditSerial.Text);
                editableUser.Date = dpEditRegister.SelectedDate.Value;
                editableUser.DocumentDateIssued = dpEditDocIssued.SelectedDate.Value;
                if (ConnectedToDb)
                {
                    if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                    NpgsqlCommand update = new NpgsqlCommand($"UPDATE users.citizen " +
                        $"SET \"FirstName\"='{editableUser.Name}'," +
                        $"\"LastName\"='{editableUser.Surname}', " +
                        $"\"Patronym\"='{editableUser.Patronym}'" +
                        $" WHERE \"Id\"={IdOfEditableUser};" +

                        $"UPDATE users.person " +
                        $"SET \"Cypher\"={editableUser.Cypher}," +
                        $"\"Inn\"={editableUser.Inn}, " +
                        $"\"Type\"='{editableUser.Type}'," +
                        $"\"Date\"='{editableUser.ShortDate}'" +
                        $" WHERE \"Id\"={users[IdOfEditableUser].PId};" +

                        $"UPDATE users.document " +
                        $"SET \"Name\"='{editableUser.DocumentName}'," +
                        $"\"Serial\"={editableUser.DocumentSerial}, " +
                        $"\"WhereIssued\"='{editableUser.DocumentWhereIssued}'," +
                        $"\"DateIssued\"='{editableUser.DocumentShortDate}'" +
                        $" WHERE \"Id\"={users[IdOfEditableUser].DId};", connection);
                    await update.ExecuteNonQueryAsync();
                    connection.Close();
                }
                LoadDataFromLocalList();

                MessageBox.Show("Данные обновлены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                if (cbMoveToEditedRecord.IsChecked == true)
                {
                    tabItemShowRecords.Focus();
                    dgUsers.SelectedItem = editableUser;
                }
                gbEdit.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region AddRecordActions

        private async void BtnAddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTextBoxes(AddGrid.Children))
            {
                IdEventArgs args = Citizen.GetFreeId(users);
                if (ConnectedToDb)
                {
                    if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                    NpgsqlCommand insert = new NpgsqlCommand($"INSERT INTO users.document VALUES " +
                        $"('{tbAddDocName.Text}', {args.AvailableDocId}, {tbAddSerial.Text}, " +
                        $"'{tbAddWhereIssued.Text}', '{dpAddDocIssued.SelectedDate.Value.ToShortDateString()}');" +

                        $"INSERT INTO users.person VALUES " +
                        $"({args.AvailablePersId}, {tbAddCypher.Text}, {tbAddInn.Text}, " +
                        $"'{tbAddType.Text}', '{dpAddRegister.SelectedDate.Value.ToShortDateString()}');" +

                        $"INSERT INTO users.citizen VALUES " +
                        $"({args.AvailableCtzId}, {args.AvailablePersId}, {args.AvailableDocId}," +
                        $"'{tbAddName.Text}', '{tbAddSurname.Text}', '{tbAddPatronym.Text}');", connection);
                    await insert.ExecuteNonQueryAsync();
                    connection.Close();
                }
                Citizen ctz = new Citizen
                {
                    Id = (uint)args.AvailableCtzId,
                    PId = args.AvailablePersId,
                    DId = args.AvailableDocId,
                    Name = tbAddName.Text,
                    Surname = tbAddSurname.Text,
                    Patronym = tbAddPatronym.Text,
                    DocumentName = tbAddDocName.Text,
                    DocumentSerial = uint.Parse(tbAddSerial.Text),
                    DocumentWhereIssued = tbAddWhereIssued.Text,
                    DocumentDateIssued = dpAddDocIssued.SelectedDate.Value,
                    Cypher = uint.Parse(tbAddCypher.Text),
                    Date = dpAddRegister.SelectedDate.Value,
                    Inn = uint.Parse(tbAddInn.Text),
                    Type = tbAddType.Text
                };
                users.Add(ctz);
                LoadDataFromLocalList();
                if (cbMoveToNewRecord.IsChecked == true)
                {
                    tabControlMain.Focus();
                    dgUsers.SelectedItem = ctz;
                }
            }
        }

        #endregion

        #region DeleteRecordActions

        private void TbIdToDelete_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsStringEmpty(tbIdToDelete.Text))
            {
                if (uint.TryParse(tbIdToDelete.Text, out uint id))
                {
                    try
                    {
                        var selectedUsers = from i in users where i.Id == id select i;
                        Citizen user = selectedUsers.First();
                        lblFullnameToDelete.Content = user.FullName;
                        btnDelete.IsEnabled = true;
                    }
                    catch (InvalidOperationException)
                    {
                        lblFullnameToDelete.Content = string.Empty;
                        btnDelete.IsEnabled = false;
                    }
                }
            }
            else
            {
                lblFullnameToDelete.Content = string.Empty;
                btnDelete.IsEnabled = false;
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTextBoxes(DeleteGrid.Children))
            {
                var selectedUsers = from i in users where i.Id == uint.Parse(tbIdToDelete.Text) select i;
                Citizen user = selectedUsers.First();
                if (ConnectedToDb)
                {
                    NpgsqlCommand delete = new NpgsqlCommand($"DELETE FROM users.citizen WHERE \"Id\"={user.Id};" +
                        $"DELETE FROM users.document WHERE \"Id\"={user.DId};" +
                        $"DELETE FROM users.person WHERE \"Id\"={user.PId};", connection);
                    if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
                    await delete.ExecuteNonQueryAsync();
                    connection.Close();
                }
                users.Remove(user);
                LoadDataFromLocalList();
            }
        }

        #endregion

        #region ValidatingMethods

        public bool ValidateTextBoxes(UIElementCollection collection)
        {
            foreach (UIElement i in collection)
            {
                if (i is TextBox tb)
                {
                    if (IsStringEmpty(tb.Text))
                    {
                        tb.Focus();
                        tb.Style = Resources["RedBorder"] as Style;
                        MessageBox.Show("Не заполнено поле!");
                        return false;
                    }
                    else
                    {
                        tb.Style = Resources["DefaultBorder"] as Style;
                    }
                }
            }
            return true;
        }

        private bool IsStringEmpty(string expr)
        {
            if (string.IsNullOrEmpty(expr) || string.IsNullOrWhiteSpace(expr))
                return true;
            else
                return false;
        }

        #endregion

        #region FormLogic

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchEventArgs args = new SearchEventArgs();
            SearchWindow sw = new SearchWindow(args, this);
            sw.Show();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            users.Clear();
            LoadData();
            dgUsers.Items.Clear();
            foreach (var i in users)
            {
                FillCitizenFromOtherTables(i);
                dgUsers.Items.Add(i);
            }
        }

        private void BtnRefreshFromLocal(object sender, RoutedEventArgs e)
        {
            LoadDataFromLocalList();
        }


        #endregion

        #region LocalDBwithJSON

        private void BtnSaveLocalDb_Click(object sender, RoutedEventArgs e)
        {
            LocalUsersDb ldb = new LocalUsersDb();
            SaveFileDialog sfd = new SaveFileDialog();
            try
            {
                sfd.Filter = "All files (*.*)|*.*";
                sfd.ShowDialog();
                ldb.SaveLocalDb(users, sfd.FileName);
                MessageBox.Show($"Сохранено {users.Count} записей");
            }
            catch (ArgumentException) { }
        }

        private void BtnLoadLocalDb_Click(object sender, RoutedEventArgs e)
        {
            LocalUsersDb ldb = new LocalUsersDb();
            OpenFileDialog ofd = new OpenFileDialog();
            try
            {
                ofd.Filter = "Database Context (*.dbcnt)|*.dbcnt|All files (*.*)|*.*";
                ofd.ShowDialog();
                users = ldb.LoadLocalDb(ofd.FileName);
                MessageBox.Show($"Загружено {users.Count} записей");
            }
            catch (ArgumentException) { }
        }

        private void BtnClearLocalDb(object sender, RoutedEventArgs e)
        {
            users.Clear();
            dgUsers.Items.Clear();
        }

        #endregion
    }
}