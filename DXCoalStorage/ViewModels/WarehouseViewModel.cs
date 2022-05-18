using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using DXAppWPF.Models;
using DXCoalStorage.Data;
using DXCoalStorage.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DXCoalStorage.ViewModels
{
    [POCOViewModel]
    public class WarehouseViewModel : ViewModelBase
    {
        StockContext context;
        protected IWindowService WindowService {
            get { return this.GetService<IWindowService>(); } }

        protected WarehouseViewModel()
        {
            if (IsInDesignMode)
            {
                Table = new ObservableCollection<StockTable>();
            }
            else
            {
                context = new StockContext();
                context.Warehouses.Load();
                context.Sites.Load();

                TableItems();
            }
        }
        public static WarehouseViewModel Create()
        {
            return ViewModelSource.Create(() => new WarehouseViewModel());
        }

        // Передача данных строки в дочернее окно
        [Command]
        public void RowDoubleClick(RowClickArgs args)
        {
            var selectResult =
                Table.Where(x => x.WarehouseNumber ==
                ((StockTable)args.Item).WarehouseNumber);

            var result = new ObservableCollection<StockTable>(selectResult);

            UpdateWindowViewModel = new UpdateViewModel() {_context = context,
                TableRow = result, Caption = "Изменить"
            };
            WindowService.Show(UpdateWindowViewModel);
        }

        [Command]
        public void ShowHistory()
        {
            var selectResult = Table.GroupBy(s => s.WarehouseNumber)
                   .Select(grp => grp.First())
                   .ToList();

            var detailList = new ObservableCollection<StockTable>(selectResult);

            HistoryWindowViewModel = new HistoryViewModel()
            { Caption = "История" };

            WindowService.Show(HistoryWindowViewModel);
        }

        // Заполнение данными 
        private void TableItems()
        {
            List<StockTable> result = new List<StockTable>();

            var table = from warehouse in context.Warehouses
                      join sites in context.Sites
                      on warehouse.Site_Id equals sites.Id
                      select new
                      {
                          WarehouseNumber = warehouse.WarehouseNumber,
                          PicketNumber = warehouse.PicketNumber,
                          SiteNumber = sites.SiteNumber,
                          Cargo = sites.Cargo
                      };

            foreach (var p in table)
            {
                result.Add(new StockTable
                {
                    WarehouseNumber = p.WarehouseNumber,
                    PicketNumber = p.PicketNumber,
                    SiteNumber = p.SiteNumber,
                    Cargo= p.Cargo
                });
            }
            TableQuantity(result);
            Table = new ObservableCollection<StockTable>(result);
        }

        // Группировка данных таблицы
        private void TableQuantity(List<StockTable> table)
        {
            var quantity = table
            .GroupBy(l => l.WarehouseNumber)
            .Select(cl => new StockTable
            {
                WarehouseNumber = cl.FirstOrDefault().WarehouseNumber,
                PicketNumber = cl.Select(x => x.PicketNumber).Count(),
                SiteNumber = cl.Select(x => x.SiteNumber).Count().ToString(),
                Cargo = cl.Select(c => c.Cargo).Distinct().Sum()
            }).ToList();

            TQuantity = new ObservableCollection<StockTable>(quantity);
        }
      
        public HistoryViewModel HistoryWindowViewModel
        {
            get { return GetValue<HistoryViewModel>(); }
            set { SetValue(value); }
        }
        public UpdateViewModel UpdateWindowViewModel
        {
            get { return GetValue<UpdateViewModel>(); }
            set { SetValue(value); }
        }

        public ObservableCollection<StockTable> Table
        {
            get => GetValue<ObservableCollection<StockTable>>();
            private set => SetValue(value);
        }
        public ObservableCollection<StockTable> TQuantity
        {
            get => GetValue<ObservableCollection<StockTable>>();
            private set => SetValue(value);
        }
    }
    public class TestDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HistoryViewModel { get; set; }
        public DataTemplate UpdateViewModel { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is HistoryViewModel)
                return HistoryViewModel;
            if (item is UpdateViewModel)
                return UpdateViewModel;
            return base.SelectTemplate(item, container);
        }
    }
}
