using System.ComponentModel;
using static PivotGridDemo.ProductSales;

namespace PivotGridDemo
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Data = ProductSales.GetSalesData();
        }
        private ProductSalesCollection _data;
        private bool _isBusy;
        public ProductSalesCollection Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(nameof(Data));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
