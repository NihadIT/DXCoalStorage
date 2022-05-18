using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using DXAppWPF.Models;
using DXCoalStorage.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DXCoalStorage.ViewModels
{
    [POCOViewModel]
    public class UpdateViewModel : ViewModelBase
    {
        public StockContext _context;
        
        Warehouse wItem;
        Sites sItem;
        List<History> hItem;
        
        [Command]
        public void Loaded()
        {
           wItem = new Warehouse();
           sItem = new Sites();
           hItem =  new List<History>();
        }

        [Command]
        public void MouseDown(object sender)
        {
            GridControl gridControl = sender as GridControl;
           
            // Получить значение ячейки
            gridControl.BeginSelection();
            StockTable text = new StockTable(); 
            foreach (int rowHandle in gridControl.GetSelectedRowHandles())
            {
                int row = gridControl.GetRowHandleByVisibleIndex(rowHandle);
                var cellValue = gridControl.GetRow(rowHandle);
                text = (StockTable)cellValue;
            }
            gridControl.EndSelection();

            wItem.Site_Id = _context.Sites
                .Where(n => n.SiteNumber == text.SiteNumber)
                .Select(s => s.Id).FirstOrDefault();
            wItem.WarehouseNumber = text.WarehouseNumber;
            wItem.PicketNumber = text.PicketNumber;
        }

        [Command]
        public void ValidateRow(RowValidationArgs args)
        {
            var item = (StockTable)args.Item;
            args.Result = GetValidationErrorInfo((StockTable)args.Item);
            List<string> reps = new List<string>();
            int counter = 1;

            // Обновление данных таблицы
            using (var context = new StockContext())
            {
                var dbSites = context.Sites.SingleOrDefault(b => b.Id == wItem.Site_Id);
                if (dbSites != null && args.Result == null)
                {
                    foreach (var w in context.Warehouses)
                    {
                        foreach (var t in TableRow)
                        {
                            if (w.PicketNumber == t.PicketNumber)
                            {
                                wItem.Site_Id = context.Sites
                                    .Where(s => s.SiteNumber == t.SiteNumber)
                                    .Select(s => s.Id).FirstOrDefault();

                                // Если площади нет, создаем
                                if (wItem.Site_Id == 0)
                                {
                                    sItem.SiteNumber = t.SiteNumber;
                                    sItem.Cargo = t.Cargo;
                                    if (!reps.Contains(sItem.SiteNumber))
                                    {
                                        context.Sites.Add(sItem);
                                        w.Site_Id = context.Sites
                                            .OrderByDescending(o => o.Id)
                                            .Select(s => s.Id).First() + counter;

                                        reps.Add(sItem.SiteNumber);
                                        counter++;
                                    }
                                }
                                else 
                                { 
                                    w.Site_Id = wItem.Site_Id;
                                }
                            }
                        }
                    }
                    EntryHistory(context);
                    context.SaveChanges();
                }
            }
        }

        public void EntryHistory(StockContext context)
        {
            foreach(var s in hItem.Distinct())
            {
                context.History.Add(s);
            }
        }

        // Обработка исключений
        public ValidationErrorInfo GetValidationErrorInfo(StockTable table)
        {
            int[] splitSite = table.SiteNumber.Split('-').Select(x => int.Parse(x)).ToArray();
         
            if (table.SiteNumber != null)
            {
                foreach (int site in splitSite)
                {
                    // Сравнение площади и пикетов 
                    if (TableRow
                        .SingleOrDefault(x => x.PicketNumber == site) != null)
                    {
                        if(splitSite.Length == 1 && table.PicketNumber < site || table.PicketNumber > site)
                            return new ValidationErrorInfo("Площадь не соответствует пикету!");
                        if (splitSite.Length > 1 && splitSite[1] < table.PicketNumber)
                            return new ValidationErrorInfo("Площадь не соответствует пикету!");
                        else if(splitSite.Length > 1) 
                        { ChangeSites(splitSite, table); }
                    }
                    else
                        return new ValidationErrorInfo("Площадь за диапазоном!");
                }
            }
            return null;
        }

        private void ChangeSites(int[] splitSite, StockTable table)
        {
            History h = new History();

           TableRow.Where(x => x.PicketNumber >= splitSite[0] && x.PicketNumber <= splitSite[1])
                .ToList().ForEach(y => 
                {
                    h.Site_was = y.SiteNumber;
                    h.Сargo_was = y.Cargo;

                    y.SiteNumber = table.SiteNumber;
                    y.Cargo = table.Cargo;

                    h.Site_became = table.SiteNumber;
                    h.Сargo_became = table.Cargo;
                    h.DateTime = DateTime.Now;
                    hItem.Add(h);
                });

            TableRow.Where(x =>  x.PicketNumber >= splitSite[0] && x.PicketNumber > splitSite[1] )
                .ToList().ForEach(y => 
                {
                    y.SiteNumber = y.PicketNumber.ToString();
                });
        }

        public ObservableCollection<StockTable> TableRow 
        {
            get => GetValue<ObservableCollection<StockTable>>();
            set => SetValue(value);
        }

        public string Caption
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
