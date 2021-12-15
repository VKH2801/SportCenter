﻿using Microsoft.Win32;
using SportCenter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SportCenter.ViewModel
{
    public class MainViewModel : BaseViewModel
        
    {
        //List fields which was booked
        private ObservableCollection<bookingInfo> _Listbooking;
        public ObservableCollection<bookingInfo> Listbooking { get => _Listbooking; set { _Listbooking = value; OnPropertyChanged(); } }

        //List goods in storage
        private ObservableCollection<good> _Listgood;
        public ObservableCollection<good> Listgood { get => _Listgood; set { _Listgood = value; OnPropertyChanged(); } }

        //List goods which was ordered
        private ObservableCollection<buyingInfo> _Listbuying;
        public ObservableCollection<buyingInfo> Listbuying { get => _Listbuying; set { _Listbuying = value; OnPropertyChanged(); } }

        private ObservableCollection<buyingInfo> _Listorder;
        public ObservableCollection<buyingInfo> Listorder { get => _Listorder; set { _Listorder = value; OnPropertyChanged(); } }

        private ObservableCollection<BaseCustomerInfo> _ListCustomerInfo;
        public ObservableCollection<BaseCustomerInfo> ListCustomerInfo { get => _ListCustomerInfo; set { _ListCustomerInfo = value; OnPropertyChanged(); } }

        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand _ShowWindowCommand_FB { get; set; }
        public ICommand ShowWindowCommand_BK { get; set; }
        public ICommand ShowWindowCommand_VL { get; set; }
        public ICommand ShowFootballFieldCommand { get; set; }
        public ICommand ShowVolleyballFieldCommand { get; set; }
        public ICommand ShowBasketballFieldCommand { get; set; }
        public ICommand LogoutCommand { get; set; }


        //Storage VM

        private string imageFileName;

        private int _idgood;
        public int idgood { get => _idgood; set { _idgood = value; OnPropertyChanged(); } }

        private string _namegood;
        public string namegood { get => _namegood; set { _namegood = value; OnPropertyChanged(); } }

        private decimal? _pricegood;
        public decimal? pricegood { get => _pricegood; set { _pricegood = value; OnPropertyChanged(); } }

        private string _unitgood;
        public string unitgood { get => _unitgood; set { _unitgood = value; OnPropertyChanged(); } }
        
      

       
        
        public ICommand OpenBillReportWindow { get; set; }

        

        // Good VM
        public ICommand addCommand { get; set; } 
        public ICommand editCommand { get; set; }
        public ICommand deleteCommand { get; set; }
       
        public ICommand SelectImageCommand { get; set; }

        private good _SelectedItem;
        public good SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    idgood = SelectedItem.id;
                    namegood = SelectedItem.name;
                    pricegood = SelectedItem.price;
                    unitgood = SelectedItem.unit;
                }

            }
        }


        //Order VM
     

        private decimal? _total = 0;
        public decimal? total { get => _total; set { _total = value; OnPropertyChanged(); } }

        
        public ICommand AddbuyingCommand => new RelayCommand<object>(CanExecuted, AddExecuted);
        public ICommand DelbuyingCommand => new RelayCommand<object>(CanExecuted, DelExecuted);
        public ICommand PlusCommand => new RelayCommand<object>(Plus_CanExecuted, Plus_Executed);      
        public ICommand MinusCommand => new RelayCommand<object>(Minus_CanExecuted, Minus_Executed);
        public ICommand ClearAllCommand { get; set; }
        public ICommand OrderCommand { get; set; }

        private int _Selectedidbooking;
        public int Selectedidbooking { get => _Selectedidbooking; set { _Selectedidbooking = value; OnPropertyChanged(); } }

        private bookingInfo _FieldSelectedItem;
        public bookingInfo FieldSelectedItem
        {
            get => _FieldSelectedItem;
            set
            {
                _FieldSelectedItem = value;
                OnPropertyChanged();
                if (FieldSelectedItem != null)
                {
                    Selectedidbooking = FieldSelectedItem.id;
                }

            }
        }

        public MainViewModel()
        {
            _Listbooking = new ObservableCollection<bookingInfo>(DataProvider.Ins.DB.bookingInfoes);
            _Listbuying = new ObservableCollection<buyingInfo>(DataProvider.Ins.DB.buyingInfoes);
            _Listgood = new ObservableCollection<good>(DataProvider.Ins.DB.goods);
            _Listorder = new ObservableCollection<buyingInfo>();
            _ListCustomerInfo = new ObservableCollection<BaseCustomerInfo>();

            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                Isloaded = true;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                p.Show();
                LoadStorageData();
                LoadListCustomerInfo();
                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginViewModel;

                if (loginVM.IsLogin)
                {

                }
                else
                {
                    p.Close();
                }

            }); 
            _ShowWindowCommand_FB = new RelayCommand<object>((parameter) => true, (parameter) => _ShowWindowFuntion_FB());
            ShowWindowCommand_BK = new RelayCommand<object>((parameter) => true, (parameter) => ShowWindowFuntion_BK());
            ShowWindowCommand_VL = new RelayCommand<object>((parameter) => true, (parameter) => ShowWindowFuntion_VL());
            ShowFootballFieldCommand = new RelayCommand<object>((parameter) => true, (parameter) => ShowFootballFieldFunction());
            ShowVolleyballFieldCommand = new RelayCommand<object>((parameter) => true, (parameter) => ShowVolleyballFieldFunction());
            ShowBasketballFieldCommand = new RelayCommand<object>((parameter) => true, (parameter) => ShowBasketballFieldFuction());
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => ChooseImage(parameter));
            OpenBillReportWindow = new RelayCommand<object>((parameter) => true, (parameter) => f_Open_Bill_Report());
            LogoutCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {

                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
                if (loginWindow.DataContext == null)
                    return;
                var loginVM = loginWindow.DataContext as LoginViewModel;

                if (loginVM.IsLogin)
                {
                    p.Show();
                    LoadStorageData();
                    LoadListCustomerInfo();
                }
                else
                {
                    p.Close();
                }
            });

            
        

        

 
            
            // Add goods
            addCommand = new RelayCommand<object>((parameter) =>
            {
                if (string.IsNullOrEmpty(namegood))
                {
                    return false;
                }

                var nameList = DataProvider.Ins.DB.goods.Where(p => p.name == namegood);
                if (nameList == null || nameList.Count() != 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }, (parameter) =>
            {
                Listgood = new ObservableCollection<good>(DataProvider.Ins.DB.goods);
                byte[] imgByteArr;
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                var good = new good() { name = namegood, id = idgood, price = pricegood,unit=unitgood,imageFile=imgByteArr };
                DataProvider.Ins.DB.goods.Add(good);
                DataProvider.Ins.DB.SaveChanges();
                Listgood.Add(good);

            });

            //Edit goods
            editCommand = new RelayCommand<object>((parameter) =>
            {

                if (string.IsNullOrEmpty(namegood)/*||SelectedItem==null*/)
                    return false;
                var nameList = DataProvider.Ins.DB.goods.Where(p => p.id == idgood);
                if (nameList != null && nameList.Count() != 0)
                    return true;
                return false;
            }, (parameter) =>
            {
                MessageBoxResult result = MessageBox.Show("Xác nhận sửa hàng hóa?", "Thông báo", MessageBoxButton.YesNo);
                Listgood = new ObservableCollection<good>(DataProvider.Ins.DB.goods);
                if (result == MessageBoxResult.Yes)
                {
                    byte[] imgByteArr;
                    imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
                    var good = DataProvider.Ins.DB.goods.Where(x => x.id == SelectedItem.id).SingleOrDefault();
                    good.name = namegood;
                    good.price = pricegood;
                    good.unit = unitgood;
                    good.imageFile = imgByteArr;

                    DataProvider.Ins.DB.SaveChanges();
                }

            });

            //Delete goods
            deleteCommand = new RelayCommand<object>((parameter) => true,
            (parameter) =>
            {
                MessageBoxResult result = MessageBox.Show("Xác nhận xóa hàng hóa?", "Thông báo", MessageBoxButton.YesNo);

                Listgood = new ObservableCollection<good>(DataProvider.Ins.DB.goods);
                if (result == MessageBoxResult.Yes)
                {
                    var good = DataProvider.Ins.DB.goods.Where(x => x.id == SelectedItem.id).SingleOrDefault();
                    DataProvider.Ins.DB.goods.Remove(good);
                    DataProvider.Ins.DB.SaveChanges();
                    Listgood.Remove(good);
                }
            });

            OrderCommand = new RelayCommand<object>((parameter) => true,
            (parameter) =>
            {
                if(FieldSelectedItem==null)
                {
                    MessageBox.Show("Vui lòng chọn sân!!");
                }
                else { 
                MessageBoxResult result = MessageBox.Show("Xác nhận đặt hàng?", "Thông báo", MessageBoxButton.YesNo);


                    if (result == MessageBoxResult.Yes)
                    {

                        foreach (var item in Listorder)
                        {
                            //buyingInfo buying = new buyingInfo {idGood=item.idGood,quantity=item.quantity,idBookingInfo=Selectedidbooking,};
                            buyingInfo buying = new buyingInfo();
                            buying.idGood = item.good.id;
                            buying.quantity = item.quantity;
                            buying.orderprice = item.orderprice;
                            buying.idBookingInfo = Selectedidbooking;

                            DataProvider.Ins.DB.buyingInfoes.Add(buying);
                            DataProvider.Ins.DB.SaveChanges();
                        }


                        Listorder.Clear();
                    }
                }
            });
            ClearAllCommand = new RelayCommand<object>((parameter) => true,
            (parameter) =>
            {
                Listorder.Clear();
                total = 0;
            });
        }
        
        private void LoadListCustomerInfo()
        {
            
            var temp_bookingInfo = DataProvider.Ins.DB.bookingInfoes;
            var temp_billInfo = DataProvider.Ins.DB.bills;
            ObservableCollection<BaseCustomerInfo> temp_listCusInfo = new ObservableCollection<BaseCustomerInfo>();
            if(temp_billInfo == null)
            {
                return;
            }
            //Adding Customer info in to ListCustomerInfo
            foreach (var item_bill in temp_billInfo) 
            {
                var temp_Cusinfo = new BaseCustomerInfo();
                foreach(var item_booking in temp_bookingInfo)
                {
                    if (item_booking.id == item_bill.idBookingInfo)
                    {
                        temp_Cusinfo.Baseinfo_CusName = item_booking.Customer_name;
                        temp_Cusinfo.Baseinfo_CusPhoneNum = item_booking.Customer_PhoneNum.ToString();
                        temp_Cusinfo.Baseinfo_SumBillAmount = 1;
                        temp_Cusinfo.Baseinfo_SumCusMoneyAmount = Decimal.ToInt32(item_bill.totalmoney.Value);
                        temp_Cusinfo.Baseinfo_TypeCus = "Lever1";
                        temp_listCusInfo.Add(temp_Cusinfo);
                    }
                }
            }
            // Bill count 
            
            List<BaseCustomerInfo> temp_list1 = new List<BaseCustomerInfo>();
            List<BaseCustomerInfo> temp_list2 = new List<BaseCustomerInfo>();
            List<BaseCustomerInfo> temp_list3 = new List<BaseCustomerInfo>();

            temp_list2 = temp_listCusInfo.ToList();
            temp_list1 = temp_listCusInfo.ToList();
            
            for(int i = 0; i < temp_list1.Count(); i++)
            {
                int total1 = temp_list1[i].Baseinfo_SumCusMoneyAmount;
                int billnum = 1;
                for(int j = i; j < temp_list2.Count(); j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    else
                    {
                        if (temp_list1[i].Baseinfo_CusName == temp_list2[j].Baseinfo_CusName && temp_list1[i].Baseinfo_CusPhoneNum == temp_list2[j].Baseinfo_CusPhoneNum)
                        {
                            total1 += temp_list2[j].Baseinfo_SumCusMoneyAmount;
                            billnum++;
                            temp_list2.RemoveAt(j);
                        }
                    }
                }
                BaseCustomerInfo adding = new BaseCustomerInfo();
                adding = temp_list1[i];
                adding.Baseinfo_SumCusMoneyAmount = total1;
                adding.Baseinfo_SumBillAmount = billnum;
                temp_list3.Add(adding);
            }
            foreach(var item in temp_list2)
            {
                _ListCustomerInfo.Add(item);
            }
            
            
            
            //Setting STT, member lv
            foreach(var item in _ListCustomerInfo.ToList())
            {
                if (item.Baseinfo_SumCusMoneyAmount >= 1000000)
                {
                    item.Baseinfo_TypeCus = "Level 2";
                }
                if (item.Baseinfo_SumCusMoneyAmount >= 3000000)
                {
                    item.Baseinfo_TypeCus = "Level 3";
                }
                if (item.Baseinfo_SumCusMoneyAmount >= 5000000)
                {
                    item.Baseinfo_TypeCus = "VIP";
                }
            }
            for(int i = 0;i < _ListCustomerInfo.ToList().Count(); i++)
            {
                _ListCustomerInfo[i].STT = i + 1;
            }
        }

        private void Update_ListCustomerInfo()
        {
            if(_ListCustomerInfo == null)
            {
                return;
            }
            foreach(var item in _ListCustomerInfo.ToList())
            {
                _ListCustomerInfo.Remove(item);
            }
        }


        private void f_Open_Bill_Report()
        {
            Bill_Report rp = new Bill_Report();
            rp.Show();
        }

        private void ChooseImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Chọn ảnh";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                if (parameter.Children.Count > 1)
                {
                    parameter.Children.Remove(parameter.Children[0]);
                    parameter.Children.Remove(parameter.Children[1]);
                }
            }
        }



        //Order
        private bool CanExecuted(object sender)
        {
            return true;
            
        }

        private void AddExecuted(object sender)
        {
            
                
                good good = sender as good;
            buyingInfo buys = new buyingInfo();
            buys.good = new good();
            buys.good = good;
            
            buys.idGood = good.id;
            foreach(var item in Listgood)
            {
                if(item.id==buys.idGood)
                {
                    buys.orderprice = item.price;
                }
            }
            buys.quantity = 1;
            Listorder.Add(buys);

            total = Calc() ;




        }
        private void DelExecuted(object sender)
        {
            
            if (sender is buyingInfo)
            {
                Listorder.Remove(sender as buyingInfo);
                total = Calc();
            }
        }
        public decimal? Calc()
        {
            return Listorder.Sum(p => p.quantity * p.good.price);
        }

        private void Minus_Executed(object sender)
        {
            buyingInfo order = sender as buyingInfo;
            BaseGood buys = new BaseGood();
            buys.g_basebuying = new buyingInfo();
            buys.g_basegood = new good();
            buys.g_basebuying = order;
            order.quantity = buys.g_basebuying.quantity--;
            order.good.price = buys.g_basegood.price * buys.g_basebuying.quantity;


            total = Calc();
        }
        private void Plus_Executed(object sender)
        {
           
            buyingInfo order = sender as buyingInfo;
            BaseGood buys = new BaseGood();
            buys.g_basebuying = new buyingInfo();
            buys.g_basegood = new good();
            buys.g_basebuying = order;
            order.quantity++;
            foreach (var item in Listgood)
            {
                if(item.id==order.idGood)
                {
                    order.orderprice = order.quantity * item.price;
                }

            }
            
            
            
            total = Calc();
        }

        private bool Plus_CanExecuted(object obj)
        {
            return true;
        }

        private bool Minus_CanExecuted(object obj)
        {
            bool ret = false;
            if (obj is buyingInfo)
            {
                buyingInfo order = obj as buyingInfo;
                if (order.quantity > 1)
                {
                    ret = true;
                }
            }
            return ret;
        }
        internal void LoadStorageData()
        {

            Listgood = new ObservableCollection<good>();
            var listgood = DataProvider.Ins.DB.goods;
            foreach (var item in listgood)
            {
                good Storage = new good();

                Storage = item;

                Listgood.Add(Storage);
            }

        }

       

        private void ShowWindowFuntion_VL()
        {
            Volleyball_Court_Bill Volleyball_Bill = new Volleyball_Court_Bill();
            Volleyball_Bill.ShowDialog();
            Update_ListCustomerInfo();
            LoadListCustomerInfo();
        }

        private void ShowWindowFuntion_BK()
        {
            Basketball_Field_Bill basketball_bill = new Basketball_Field_Bill();
            basketball_bill.ShowDialog();
            Update_ListCustomerInfo();
            LoadListCustomerInfo();
        }

        public void _ShowWindowFuntion_FB()
        {
            Football_Field_Bill football_bill = new Football_Field_Bill();
            football_bill.ShowDialog();
            Update_ListCustomerInfo();
            LoadListCustomerInfo();
        }
        public void ShowFootballFieldFunction()
        {
            SoccerField soccerField = new SoccerField();
            soccerField.ShowDialog();
        }
        public void ShowVolleyballFieldFunction()
        {
            VolleyballField volleyballField = new VolleyballField();
            volleyballField.ShowDialog();
        }
        public void ShowBasketballFieldFuction()
        {
            BasketballField basketballField = new BasketballField();
            basketballField.ShowDialog();
        }


      

      
        
    }


   
}