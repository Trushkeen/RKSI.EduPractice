namespace RKSI.EduPractice
{
    public class SearchEventArgs
    {
        public string[] SearchColumns { get; set; } = { "Id", "Фамилия", "Шифр", "ИНН", "Место выдачи документа" };
        public string SearchString { get; set; }
        public string SelectedCol { get; set; }

        public SearchEventArgs() { }

        public SearchEventArgs(string[] columns)
        {
            SearchColumns = columns;
        }
    }
}
