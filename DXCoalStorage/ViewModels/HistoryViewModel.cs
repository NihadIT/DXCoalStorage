using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DXAppWPF.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace DXCoalStorage.ViewModels
{
    [POCOViewModel]
    public class HistoryViewModel : ViewModelBase
    {
        StockContext _context;
        public HistoryViewModel()
        {
            if (IsInDesignMode)
            {
                HistoryTable = new ObservableCollection<History>();
            }
            else
            {
                _context = new StockContext();
                _context.History.Load();
                HistoryTable = _context.History.Local;
            }
        }

        [Command]
        public void RemoveHistory()
        {
            _context.History.RemoveRange(_context.History);
        }

        public static HistoryViewModel Create()
        {
            return ViewModelSource.Create(() => new HistoryViewModel());
        }
        public ObservableCollection<History> HistoryTable
        {
            get => GetValue<ObservableCollection<History>>();
            private set => SetValue(value);
        }
        public string Caption
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
